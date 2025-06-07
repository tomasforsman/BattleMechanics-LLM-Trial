using YamlDotNet.Serialization;

namespace BattleMechanics___GPT_4._1.CombatPrototype;

public class Character
{
    [YamlMember(Alias = "name")] public string Name { get; set; }
    [YamlMember(Alias = "team")] public string Team { get; set; }
    [YamlMember(Alias = "max_hp")] public int MaxHP { get; set; }
    [YamlMember(Alias = "attack")] public int Attack { get; set; }
    [YamlMember(Alias = "defense")] public int Defense { get; set; }
    [YamlMember(Alias = "speed")] public int Speed { get; set; }
    [YamlMember(Alias = "x")] public int X { get; set; }
    [YamlMember(Alias = "y")] public int Y { get; set; }
    [YamlMember(Alias = "abilities")] public List<string> Abilities { get; set; } = new();
    [YamlMember(Alias = "ai_script")] public string AIScript { get; set; }

    // Runtime properties
    [YamlIgnore] public int CurrentHP { get; set; }
    [YamlIgnore] public bool IsAlive => CurrentHP > 0;

    public void Init()
    {
        CurrentHP = MaxHP;
    }

    public void TakeDamage(int amount)
    {
        CurrentHP -= amount;
        if (CurrentHP < 0) CurrentHP = 0;
    }

    public void Heal(int amount)
    {
        CurrentHP += amount;
        if (CurrentHP > MaxHP) CurrentHP = MaxHP;
    }
}