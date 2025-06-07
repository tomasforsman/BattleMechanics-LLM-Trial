using BattleMechanic.Core.Models;
using MoonSharp.Interpreter;
using YamlDotNet.Serialization;

namespace BattleMechanic.Core.Models;

[MoonSharpUserData]
public class Character
{
    // --- Properties loaded from YAML ---
    public string Name { get; set; } = string.Empty;

    // *** FIX: Re-added the 'Team' property to match the YAML files. ***
    // The YAML files have a 'team' key, so this property is needed for deserialization.
    public string Team { get; set; } = string.Empty; 

    public int MaxHp { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Speed { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public List<string> Abilities { get; set; } = [];
    public string? AiScript { get; set; }

    // --- Runtime properties (not from YAML) ---
    [YamlIgnore]
    public int CurrentHp { get; private set; }
    
    [YamlIgnore]
    public bool IsDefending { get; private set; }
    
    [YamlIgnore]
    public string TeamId { get; set; } = string.Empty;

    [YamlIgnore]
    public Vector2 Position => new(X, Y);

    [YamlIgnore]
    public bool IsAlive => CurrentHp > 0;
    
    [YamlIgnore]
    public List<Ability> KnownAbilities { get; set; } = [];

    [YamlIgnore]
    public string FilePath { get; set; } = string.Empty;
    
    public void InitializeForBattle()
    {
        CurrentHp = MaxHp;
        IsDefending = false;
    }

    public void OnTurnStart()
    {
        IsDefending = false;
    }

    public void TakeDamage(int baseDamage)
    {
        int effectiveDefense = IsDefending ? Defense * 2 : Defense;
        int actualDamage = Math.Max(0, baseDamage - effectiveDefense);
        CurrentHp = Math.Max(0, CurrentHp - actualDamage);
        
        Console.WriteLine($"  -> {Name} takes {actualDamage} damage! ({baseDamage} base - {effectiveDefense} defense).");

        if (!IsAlive)
        {
            Console.WriteLine($"  -> {Name} has been defeated!");
        }
    }
    
    public void Heal(int amount)
    {
        int healAmount = Math.Max(0, amount);
        CurrentHp = Math.Min(MaxHp, CurrentHp + healAmount);
        Console.WriteLine($"  -> {Name} heals for {healAmount} HP, now at {CurrentHp}/{MaxHp} HP.");
    }
    
    public void Defend()
    {
        IsDefending = true;
        Console.WriteLine($"  -> {Name} takes a defensive stance!");
    }

    public override string ToString()
    {
        return $"{Name} [{CurrentHp}/{MaxHp} HP] @ {Position}";
    }
}