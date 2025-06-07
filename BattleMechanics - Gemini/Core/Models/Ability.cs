using YamlDotNet.Serialization;
using MoonSharp.Interpreter;

namespace BattleMechanic.Core.Models;

[MoonSharpUserData]
public class Ability
{
    // No [YamlMember] attributes needed now; the naming convention handles it.
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Script { get; set; } = string.Empty;

    [YamlIgnore]
    public string FilePath { get; set; } = string.Empty;
}