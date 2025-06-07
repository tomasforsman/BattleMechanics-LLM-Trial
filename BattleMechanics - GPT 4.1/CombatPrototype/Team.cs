using YamlDotNet.Serialization;

namespace BattleMechanics___GPT_4._1.CombatPrototype;

public class Team
{
    [YamlMember(Alias = "name")] public string Name { get; set; }
    [YamlMember(Alias = "members")] public List<string> Members { get; set; } = new();

    // Runtime population after loading
    [YamlIgnore] public List<Character> Characters { get; set; } = new();
}