using YamlDotNet.Serialization;
using CombatPrototype;

var basePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "GameData");

var deserializer = new DeserializerBuilder().Build();  // <-- Removed naming convention here

var charsPath = Path.Combine(basePath, "Characters");
var actionsPath = Path.Combine(basePath, "Actions");
var scriptsPath = Path.Combine(basePath, "Scripts");

List<Character> chars = Directory.GetFiles(charsPath, "*.yaml")
    .Select(f => deserializer.Deserialize<Character>(File.ReadAllText(f)))
    .ToList();

Dictionary<string, Ability> abilities = Directory.GetFiles(actionsPath, "*.yaml")
    .Select(f => deserializer.Deserialize<Ability>(File.ReadAllText(f)))
    .ToDictionary(a => a.Name, a => a);

var scriptEngine = new ScriptEngine(scriptsPath);
var combatManager = new CombatManager(chars, abilities, scriptEngine);
combatManager.RunCombat();