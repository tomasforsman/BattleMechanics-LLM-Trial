using BattleMechanic.Core.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BattleMechanic.Core.Services;

/// <summary>
/// Implementation of IDataLoader that loads and validates game data from YAML files.
/// </summary>
public class YamlDataLoader : IDataLoader
{
    private readonly IDeserializer _deserializer;

    public YamlDataLoader()
    {
        // *** FIX: Switched to UnderscoredNamingConvention. ***
        // This tells the deserializer to automatically map snake_case from YAML (e.g., max_hp)
        // to PascalCase in C# (e.g., MaxHp). This is the correct fix for the errors.
        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();
    }

    public (Dictionary<string, Character> characters, Dictionary<string, Team> teams, Dictionary<string, Ability> abilities) LoadAll(string gameDataPath)
    {
        Console.WriteLine($"\nLoading game data from: {gameDataPath}");

        var characters = LoadDirectory<Character>(Path.Combine(gameDataPath, "Characters"), (c, path) => c.FilePath = path);
        var teams = LoadDirectory<Team>(Path.Combine(gameDataPath, "Teams"), (t, path) => t.FilePath = path);
        var abilities = LoadDirectory<Ability>(Path.Combine(gameDataPath, "Abilities"), (a, path) => a.FilePath = path);
        
        ValidateData(characters, teams, abilities);
        
        return (characters, teams, abilities);
    }

    private Dictionary<string, T> LoadDirectory<T>(string path, Action<T, string> setPathAction) where T : class
    {
        var data = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"[WARNING] Data directory not found, skipping: {path}");
            return data;
        }

        foreach (var file in Directory.GetFiles(path, "*.yaml", SearchOption.AllDirectories))
        {
            var key = Path.GetFileNameWithoutExtension(file);
            try
            {
                var item = _deserializer.Deserialize<T>(File.ReadAllText(file));
                if (item != null)
                {
                    setPathAction(item, file);
                    data[key] = item;
                }
                else
                {
                     Console.WriteLine($"[ERROR] Failed to deserialize '{file}'. The file appears to be empty or invalid.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load or parse '{file}': {ex.Message}");
            }
        }
        Console.WriteLine($" -> Loaded {data.Count} assets from {Path.GetFileName(path)}.");
        return data;
    }
    
    private void ValidateData(Dictionary<string, Character> characters, Dictionary<string, Team> teams, Dictionary<string, Ability> abilities)
    {
        Console.WriteLine("\n--- Validating Game Data ---");
        bool hasErrors = false;

        // Validate character abilities (using corrected property name 'Abilities')
        foreach (var (key, character) in characters)
        {
            foreach(var abilityName in character.Abilities)
            {
                if (!abilities.ContainsKey(abilityName))
                {
                    Console.WriteLine($"[VALIDATION ERROR] Character '{character.Name}' (in {key}.yaml) references unknown ability '{abilityName}'.");
                    hasErrors = true;
                }
            }
             if (!string.IsNullOrEmpty(character.AiScript) && !File.Exists(Path.Combine(AppContext.BaseDirectory, "GameData/Scripts", character.AiScript)))
            {
                Console.WriteLine($"[VALIDATION ERROR] Character '{character.Name}' (in {key}.yaml) references missing AI script '{character.AiScript}'.");
                hasErrors = true;
            }
        }

        // Validate team members (using corrected property name 'Members')
        foreach (var (key, team) in teams)
        {
            foreach (var memberName in team.Members)
            {
                if (!characters.ContainsKey(memberName))
                {
                    Console.WriteLine($"[VALIDATION ERROR] Team '{team.Name}' (in {key}.yaml) references unknown character '{memberName}'.");
                    hasErrors = true;
                }
            }
        }

        if (!hasErrors) {
            Console.WriteLine("All data references are valid.");
        }
        Console.WriteLine("--- Validation Complete ---\n");
    }
}