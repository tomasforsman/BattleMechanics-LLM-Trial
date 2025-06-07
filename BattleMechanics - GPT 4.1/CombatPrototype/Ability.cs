using YamlDotNet.Serialization;

namespace BattleMechanics___GPT_4._1.CombatPrototype;

public class Ability
{
    [YamlMember(Alias = "name")] public string Name { get; set; }
    [YamlMember(Alias = "description")] public string Description { get; set; }
    [YamlMember(Alias = "script")] public string Script { get; set; }
}