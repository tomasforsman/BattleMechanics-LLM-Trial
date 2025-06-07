namespace CombatPrototype;

public class CombatManager
{
    private readonly List<Character> characters;
    private readonly Dictionary<string, Ability> abilities;
    private readonly ScriptEngine scriptEngine;

    public CombatManager(List<Character> chars, Dictionary<string, Ability> abs, ScriptEngine scriptEng)
    {
        characters = chars;
        abilities = abs;
        scriptEngine = scriptEng;
        characters.ForEach(c => c.ResetHp());
    }

    public void RunCombat()
    {
        int round = 1;
        while (characters.Where(c => c.IsAlive).Select(c => c.Team).Distinct().Count() > 1)
        {
            Console.WriteLine($"--- Round {round++} ---");
            var orderedChars = characters.Where(c => c.IsAlive).OrderByDescending(c => c.Speed).ToList();
            foreach (var character in orderedChars)
            {
                if (!character.IsAlive) continue;

                var enemies = characters.Where(c => c.Team != character.Team && c.IsAlive).ToList();
                if (!enemies.Any()) break;

                var abilityName = character.Abilities.First();
                var ability = abilities[abilityName];

                var target = enemies.First();

                Console.WriteLine($"{character.Name} uses {ability.Name} on {target.Name}");
                scriptEngine.ExecuteAbility(ability.Script, character, target);

                if (!target.IsAlive)
                    Console.WriteLine($"{target.Name} has been defeated!");
            }
        }

        var winningTeam = characters.First(c => c.IsAlive).Team;
        Console.WriteLine($"Team {winningTeam} wins!");
    }
}