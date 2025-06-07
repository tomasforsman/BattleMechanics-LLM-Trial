using YamlDotNet.Serialization;

namespace CombatPrototype;

public class Character
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; }

    [YamlMember(Alias = "team")]
    public string Team { get; set; }

    [YamlMember(Alias = "max_hp")]
    public int MaxHp { get; set; }

    [YamlIgnore]
    public int CurrentHp { get; set; }

    [YamlMember(Alias = "attack")]
    public int Attack { get; set; }

    [YamlMember(Alias = "defense")]
    public int Defense { get; set; }

    [YamlMember(Alias = "speed")]
    public int Speed { get; set; }

    [YamlMember(Alias = "x")]
    public int X { get; set; }

    [YamlMember(Alias = "y")]
    public int Y { get; set; }

    [YamlMember(Alias = "abilities")]
    public List<string> Abilities { get; set; }

    [YamlMember(Alias = "ai_script")]
    public string AiScript { get; set; }

    public void TakeDamage(int dmg)
    {
        CurrentHp -= dmg;
        Console.WriteLine($"{Name} takes {dmg} damage. Current HP: {CurrentHp}/{MaxHp}");
    }

    public bool IsAlive => CurrentHp > 0;

    public void ResetHp() => CurrentHp = MaxHp;
}