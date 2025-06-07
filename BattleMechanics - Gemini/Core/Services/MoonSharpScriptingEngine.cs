using BattleMechanic.Core.Models;
using MoonSharp.Interpreter;

namespace BattleMechanic.Core.Services;

public class MoonSharpScriptingEngine : IScriptingEngine
{
    private readonly string _scriptsPath;

    public MoonSharpScriptingEngine(string gameDataPath)
    {
        _scriptsPath = Path.Combine(gameDataPath, "Scripts");
        UserData.RegisterType<Character>();
        UserData.RegisterType<Ability>();
        UserData.RegisterType<Vector2>();
    }

    private Script CreateConfiguredScript()
    {
        var script = new Script();
        script.Globals["print"] = (Action<object>)Console.WriteLine;
        return script;
    }

    public void ExecuteAbilityScript(Ability ability, Character user, Character target)
    {
        var scriptFile = Path.Combine(_scriptsPath, ability.Script);
        if (!File.Exists(scriptFile))
        {
            Console.WriteLine($"[SCRIPT ERROR] Script not found for ability '{ability.Name}': {ability.Script}");
            return;
        }

        try
        {
            var lua = CreateConfiguredScript();
            lua.DoFile(scriptFile);
            var function = lua.Globals["use_ability"];
            lua.Call(function, ability, user, target);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SCRIPT ERROR] Failed to execute script '{scriptFile}': {ex.Message}");
        }
    }

    // *** FIX: Ensure the implementation method name is ExecuteAiScript ***
    public (Ability? chosenAbility, Character? chosenTarget) ExecuteAiScript(string aiScriptFile, Character actor, List<Character> allies, List<Character> enemies)
    {
         var scriptFile = Path.Combine(_scriptsPath, aiScriptFile);
        if (!File.Exists(scriptFile))
        {
            Console.WriteLine($"[SCRIPT ERROR] AI Script not found: {aiScriptFile}");
            return (null, null);
        }

        try
        {
            var lua = CreateConfiguredScript();
            lua.DoFile(scriptFile);
            
            var function = lua.Globals["choose_action"];
            var result = lua.Call(function, actor, allies, enemies);

            if (result == null || result.Type != DataType.Table)
            {
                Console.WriteLine($"[SCRIPT WARNING] AI script '{aiScriptFile}' did not return a valid action table.");
                return (null, null);
            }

            var resultTable = result.Table;
            var chosenAbility = resultTable.Get("ability").UserData?.Object as Ability;
            var chosenTarget = resultTable.Get("target").UserData?.Object as Character;
            
            if (chosenAbility == null || chosenTarget == null)
            {
                Console.WriteLine($"[SCRIPT WARNING] AI script '{aiScriptFile}' returned an incomplete action.");
                return (null, null);
            }

            return (chosenAbility, chosenTarget);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SCRIPT ERROR] Failed to execute AI script '{scriptFile}': {ex.Message}");
            return (null, null);
        }
    }
}