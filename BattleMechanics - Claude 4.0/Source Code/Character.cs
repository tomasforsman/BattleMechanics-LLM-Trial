namespace BattleMechanic
{
    public class Character
    {
        public string Name { get; set; } = "";
        public string Team { get; set; } = "";
        public int MaxHp { get; set; }
        public int CurrentHp { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Speed { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public List<string> Abilities { get; set; } = new List<string>();
        public string? AiScript { get; set; }
        
        // Additional properties for modding support
        public Dictionary<string, object> CustomProperties { get; set; } = new Dictionary<string, object>();
        
        public bool IsAlive => CurrentHp > 0;
        public bool IsPlayerControlled => string.IsNullOrEmpty(AiScript) || AiScript.ToLower() == "null";

        public void Initialize()
        {
            CurrentHp = MaxHp;
        }

        public void TakeDamage(int damage)
        {
            var actualDamage = Math.Max(1, damage - Defense);
            CurrentHp = Math.Max(0, CurrentHp - actualDamage);
            Console.WriteLine($"  {Name} takes {actualDamage} damage! (HP: {CurrentHp}/{MaxHp})");
            
            if (CurrentHp <= 0)
            {
                Console.WriteLine($"  {Name} has been defeated!");
            }
        }

        public void Heal(int amount)
        {
            var oldHp = CurrentHp;
            CurrentHp = Math.Min(MaxHp, CurrentHp + amount);
            var actualHealing = CurrentHp - oldHp;
            if (actualHealing > 0)
            {
                Console.WriteLine($"  {Name} heals for {actualHealing} HP! (HP: {CurrentHp}/{MaxHp})");
            }
        }

        public void SetPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public double DistanceTo(Character other)
        {
            return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
        }

        public override string ToString()
        {
            var status = IsAlive ? $"{CurrentHp}/{MaxHp} HP" : "DEFEATED";
            return $"{Name} ({X},{Y}) - {status}";
        }
    }
}