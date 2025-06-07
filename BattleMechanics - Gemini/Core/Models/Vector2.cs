namespace BattleMechanic.Core.Models;

/// <summary>
/// Represents a 2D integer-based coordinate.
/// </summary>
public struct Vector2
{
    public int X { get; set; }
    public int Y { get; set; }

    public Vector2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString() => $"({X}, {Y})";
}