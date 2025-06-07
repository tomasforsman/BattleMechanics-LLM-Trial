using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BattleMechanics___GPT_4._1.CombatPrototype;

public class DataLoader
{
    private readonly string _basePath;

    public DataLoader(string basePath)
    {
        _basePath = basePath;
    }

    public Dictionary<string, Character> Characters { get; private set; } = new();
    public Dictionary<string, Team> Teams { get; private set; } = new();
    public Dictionary<string, Ability> Abilities { get; private set; } = new();
    public ScriptEngine ScriptEngine { get; private set; }

    public void LoadAll()
    {
        // Load YAML files
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        // Load Abilities first (needed by others)
        Abilities = LoadYamlFiles<Ability>(Path.Combine(_basePath, "Actions"))
            .ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);

        Characters = LoadYamlFiles<Character>(Path.Combine(_basePath, "Characters"))
            .ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

        Teams = LoadYamlFiles<Team>(Path.Combine(_basePath, "Teams"))
            .ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);

        // Assign Team references to Characters
        foreach (var team in Teams.Values)
        {
            team.Characters = new List<Character>();
            foreach (var charName in team.Members)
            {
                if (!Characters.TryGetValue(charName, out var ch))
                    throw new Exception($"Team '{team.Name}' references unknown character '{charName}'.");
                team.Characters.Add(ch);
            }
        }

        // Setup Script Engine
        var scriptsPath = Path.Combine(_basePath, "Scripts");
        ScriptEngine = new ScriptEngine(scriptsPath);

        // Validate abilities exist for each character
        foreach (var c in Characters.Values)
        {
            foreach (var ab in c.Abilities)
                if (!Abilities.ContainsKey(ab))
                    throw new Exception($"Character '{c.Name}' lists unknown ability '{ab}'.");
            c.Init();
        }
    }

    private List<T> LoadYamlFiles<T>(string dir)
    {
        var result = new List<T>();
        if (!Directory.Exists(dir))
            throw new DirectoryNotFoundException($"Expected folder '{dir}' not found.");
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
        foreach (var file in Directory.EnumerateFiles(dir, "*.yaml"))
            try
            {
                var text = File.ReadAllText(file);
                var obj = deserializer.Deserialize<T>(text);
                result.Add(obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading '{file}': {ex.Message}");
            }

        return result;
    }
}