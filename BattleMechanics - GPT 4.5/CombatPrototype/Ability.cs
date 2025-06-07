using YamlDotNet.Serialization;

namespace CombatPrototype;

public class Ability
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; }

    [YamlMember(Alias = "description")]
    public string Description { get; set; }

    [YamlMember(Alias = "script")]
    public string Script { get; set; }
}