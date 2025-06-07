namespace BattleMechanics___GPT_4._1.CombatPrototype;

/// <summary>
///     Tracks characters and their positions.
/// </summary>
public class Battlefield
{
    public Battlefield(List<Character> characters)
    {
        Characters = characters;
        foreach (var c in Characters) c.Init();
    }

    public List<Character> Characters { get; }

    public Character GetAtPosition(int x, int y)
    {
        return Characters.FirstOrDefault(c => c.IsAlive && c.X == x && c.Y == y);
    }
}