using BattleMechanic.Core.Models;

namespace BattleMechanic.Gameplay;

/// <summary>
/// Handles formatted console output for battle events.
/// </summary>
public static class BattleLogger
{
    public static void LogRoundStart(int roundNumber)
    {
        Console.WriteLine("\n" + new string('=', 40));
        Console.WriteLine($"          ROUND {roundNumber} - FIGHT!");
        Console.WriteLine(new string('=', 40));
    }

    public static void LogTurnStart(Character character)
    {
        Console.WriteLine($"\n--- {character.Name}'s Turn (Team: {character.TeamId}) ---");
    }

    public static void LogAction(Character user, Ability action, Character target)
    {
        Console.WriteLine($"{user.Name} uses '{action.Name}' on {target.Name}.");
    }

    public static void LogBattleStatus(List<Character> allCharacters)
    {
        Console.WriteLine("\n-- Battle Status --");
        var teams = allCharacters.GroupBy(c => c.TeamId).ToList();
        foreach (var team in teams)
        {
            Console.WriteLine($"  Team: {team.Key}");
            foreach (var member in team)
            {
                string status = member.IsAlive ? $"{member.CurrentHp}/{member.MaxHp} HP" : "DEFEATED";
                Console.WriteLine($"    - {member.Name,-10} | {status,-15} | Pos: {member.Position}");
            }
        }
    }

    public static void LogBattleEnd(string winningTeam)
    {
        Console.WriteLine("\n" + new string('*', 40));
        Console.WriteLine($"* BATTLE OVER            *");
        Console.WriteLine($"* Team '{winningTeam}' is victorious!      *");
        Console.WriteLine(new string('*', 40));
    }
    
    public static void HorizontalRule()
    {
        Console.WriteLine(new string('-', 40));
    }
}