using MoonSharp.Interpreter;

namespace BattleMechanic
{
    public class ScriptEngine
    {
        private readonly string _scriptsPath;
        private readonly Dictionary<string, string> _scriptCache = new Dictionary<string, string>();

        public ScriptEngine(string scriptsPath)
        {
            _scriptsPath = scriptsPath;
            LoadAllScripts();
        }

        private void LoadAllScripts()
        {
            if (!Directory.Exists(_scriptsPath))
            {
                Console.WriteLine($"Warning: Scripts directory '{_scriptsPath}' not found");
                return;
            }

            var scriptFiles = Directory.GetFiles(_scriptsPath, "*.lua", SearchOption.AllDirectories);
            foreach (var file in scriptFiles)
            {
                try
                {
                    var scriptName = Path.GetFileName(file);
                    var scriptContent = File.ReadAllText(file);
                    _scriptCache[scriptName] = scriptContent;
                    Console.WriteLine($"Loaded script: {scriptName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading script {file}: {ex.Message}");
                }
            }
        }

        public async Task ExecuteActionScriptAsync(string scriptName, Character user, Character target)
        {
            if (!_scriptCache.ContainsKey(scriptName))
            {
                throw new FileNotFoundException($"Script '{scriptName}' not found");
            }

            var script = new Script();
            
            // Register character API
            RegisterCharacterAPI(script, user, "user");
            RegisterCharacterAPI(script, target, "target");
            
            // Register utility functions
            script.Globals["print"] = (System.Action<string>)Console.WriteLine;
            script.Globals["math_max"] = (Func<double, double, double>)Math.Max;
            script.Globals["math_min"] = (Func<double, double, double>)Math.Min;
            script.Globals["math_random"] = (Func<double, double, double>)((min, max) => new Random().NextDouble() * (max - min) + min);

            try
            {
                script.DoString(_scriptCache[scriptName]);
                
                // Call the main function if it exists
                if (script.Globals["use_ability"] != null)
                {
                    await Task.Run(() => script.Call(script.Globals["use_ability"], user, target));
                }
                else if (script.Globals["execute"] != null)
                {
                    await Task.Run(() => script.Call(script.Globals["execute"], user, target));
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Script execution failed: {ex.Message}", ex);
            }
        }

        public async Task<(Action? action, Character? target)> ExecuteAIScriptAsync(string scriptName, Character character, List<Team> allTeams)
        {
            if (!_scriptCache.ContainsKey(scriptName))
            {
                return (null, null);
            }

            var script = new Script();
            
            // Register character API
            RegisterCharacterAPI(script, character, "self");
            
            // Register teams and characters
            var allCharacters = allTeams.SelectMany(t => t.GetAliveCharacters()).ToArray();
            script.Globals["all_characters"] = allCharacters;
            script.Globals["teams"] = allTeams.ToArray();
            
            // Register utility functions
            script.Globals["print"] = (System.Action<string>)Console.WriteLine;

            try
            {
                script.DoString(_scriptCache[scriptName]);
                
                if (script.Globals["choose_action"] != null)
                {
                    var result = await Task.Run(() => script.Call(script.Globals["choose_action"], character));
                    // Parse result and return action/target
                    // For now, return null to fall back to simple AI
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI script error: {ex.Message}");
            }

            return (null, null);
        }

        private void RegisterCharacterAPI(Script script, Character character, string variableName)
        {
            var characterTable = new Table(script);
            characterTable["name"] = character.Name;
            characterTable["team"] = character.Team;
            characterTable["max_hp"] = character.MaxHp;
            characterTable["current_hp"] = character.CurrentHp;
            characterTable["attack"] = character.Attack;
            characterTable["defense"] = character.Defense;
            characterTable["speed"] = character.Speed;
            characterTable["x"] = character.X;
            characterTable["y"] = character.Y;
            characterTable["is_alive"] = character.IsAlive;

            // Methods
            characterTable["take_damage"] = (System.Action<int>)character.TakeDamage;
            characterTable["heal"] = (System.Action<int>)character.Heal;
            characterTable["set_position"] = (System.Action<int, int>)character.SetPosition;
            
            script.Globals[variableName] = characterTable;
        }
    }
}