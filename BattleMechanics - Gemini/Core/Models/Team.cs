using YamlDotNet.Serialization;

namespace BattleMechanic.Core.Models;

public class Team
{
    public string Name { get; set; } = string.Empty;
    // 'is_player_controlled' in YAML now maps to 'IsPlayerControlled' in C#
    public bool IsPlayerControlled { get; set; } = false;

    // FIX: Renamed from 'MemberCharacterFileNames' to match YAML key 'members'
    public List<string> Members { get; set; } = [];

    [YamlIgnore]
    public string FilePath { get; set; } = string.Empty;
}