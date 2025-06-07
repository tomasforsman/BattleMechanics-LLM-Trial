using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter.Interop;

namespace BattleMechanics___GPT_4._1.CombatPrototype;

/// <summary>
/// Provides script execution for abilities and AI via MoonSharp/Lua.
/// </summary>
public class ScriptEngine : IAbilityExecutor
{
    private readonly Dictionary<string, string> _scriptCache = new();
    private readonly string _scriptsPath;
    private static bool _proxyRegistered = false;

    public ScriptEngine(string scriptsPath)
    {
        _scriptsPath = scriptsPath ?? throw new ArgumentNullException(nameof(scriptsPath));
        if (!Directory.Exists(_scriptsPath))
            throw new DirectoryNotFoundException($"Scripts directory not found: {_scriptsPath}");

        // Register proxy type only once for MoonSharp
        if (!_proxyRegistered)
        {
            UserData.RegisterType<CharacterLuaProxy>();
            _proxyRegistered = true;
        }
    }

    /// <summary>
    /// Executes a Lua script for an ability, passing user/target proxies.
    /// </summary>
    public void UseAbility(Character user, Character target, Ability ability)
    {
        if (ability == null) throw new ArgumentNullException(nameof(ability));
        var scriptFile = ability.Script;
        if (string.IsNullOrEmpty(scriptFile))
        {
            Console.WriteLine($"Ability '{ability.Name}' has no script assigned!");
            return;
        }

        var scriptPath = Path.Combine(_scriptsPath, scriptFile);

        try
        {
            var scriptCode = LoadScript(scriptPath);
            var script = new Script();

            var userProxy = new CharacterLuaProxy(user);
            var targetProxy = new CharacterLuaProxy(target);

            script.Globals["user"] = userProxy;
            script.Globals["target"] = targetProxy;

            script.DoString(scriptCode);

            var fn = script.Globals.Get("use_ability");
            if (fn.Type == DataType.Function)
                script.Call(fn, userProxy, targetProxy);
            else
                Console.WriteLine($"Script '{scriptFile}' is missing 'use_ability(user, target)' function.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in script '{scriptFile}': {ex.Message}");
        }
    }

    /// <summary>
    /// Loads and caches Lua script files by path.
    /// </summary>
    public string LoadScript(string scriptPath)
    {
        if (_scriptCache.TryGetValue(scriptPath, out var code))
            return code;
        if (!File.Exists(scriptPath))
            throw new FileNotFoundException($"Script file '{scriptPath}' not found.");
        code = File.ReadAllText(scriptPath);
        _scriptCache[scriptPath] = code;
        return code;
    }

    /// <summary>
    /// Lists all available Lua script files.
    /// </summary>
    public string[] ListScripts() =>
        Directory.GetFiles(_scriptsPath, "*.lua");

    /// <summary>
    /// Loads the AI script code for a given character, or null if not found.
    /// </summary>
    public string AIScriptForCharacter(string aiScriptFile)
    {
        if (string.IsNullOrWhiteSpace(aiScriptFile)) return null;
        var scriptPath = Path.Combine(_scriptsPath, aiScriptFile);
        return File.Exists(scriptPath) ? LoadScript(scriptPath) : null;
    }

    /// <summary>
    /// Runs a Lua AI script, returns chosen ability and target index (1-based).
    /// </summary>
    public (string ability, int? targetIdx) RunAIScript(string scriptCode, CharacterLuaProxy selfProxy, List<CharacterLuaProxy> enemies)
    {
        var script = new Script();

        script.Globals["self"] = selfProxy;

        // Provide enemies as a Lua array (1-based)
        var enemiesTbl = new Table(script);
        for (int i = 0; i < enemies.Count; i++)
            enemiesTbl[i + 1] = enemies[i];
        script.Globals["enemies"] = enemiesTbl;

        script.DoString(scriptCode);

        var fn = script.Globals.Get("choose_action");
        if (fn.Type != DataType.Function)
            throw new Exception("AI script missing 'choose_action(self, enemies)'");

        var result = script.Call(fn, selfProxy, enemiesTbl);
        if (result.Type != DataType.Table)
            throw new Exception("AI script 'choose_action' must return a table { ability = <string>, target = <int> }");

        var tbl = result.Table;
        var ability = tbl.Get("ability").CastToString();
        var target = tbl.Get("target");
        return (ability, target.IsNil() ? null : (int)target.Number);
    }

    /// <summary>
    /// Proxy object exposing character data and actions to Lua scripts.
    /// </summary>
    [MoonSharpUserData]
    public class CharacterLuaProxy
    {
        private readonly Character _character;
        public CharacterLuaProxy(Character c) => _character = c;

        public List<string> abilities => _character.Abilities;

        public string name => _character.Name;
        public int x => _character.X;
        public int y => _character.Y;
        public int hp => _character.CurrentHP;
        public int max_hp => _character.MaxHP;
        public bool is_alive => _character.IsAlive;
        public int attack => _character.Attack;
        public int defense => _character.Defense;
        public int speed => _character.Speed;

        public void take_damage(int amount) => _character.TakeDamage(amount);
        public void heal(int amount) => _character.Heal(amount);
    }
}