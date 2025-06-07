namespace BattleMechanics___GPT_4._1.CombatPrototype;

/// <summary>
///     Handles the battle loop, turn order, and overall combat flow.
/// </summary>
public class CombatManager
{
    private readonly Dictionary<string, Ability> _abilities;
    private readonly Battlefield _battlefield;
    private readonly ScriptEngine _scriptEngine;
    private readonly List<Team> _teams;

    private int _turn = 1;

    public CombatManager(Battlefield battlefield, List<Team> teams, Dictionary<string, Ability> abilities,
        ScriptEngine scriptEngine)
    {
        _battlefield = battlefield;
        _teams = teams;
        _abilities = abilities;
        _scriptEngine = scriptEngine;
    }

    public void RunBattle()
    {
        Console.WriteLine("Starting battle!");

        PrintStatus();

        while (!IsBattleOver())
        {
            Console.WriteLine($"\n--- Turn {_turn} ---");

            // Determine initiative order (descending speed)
            var activeChars = _battlefield.Characters.Where(c => c.IsAlive).OrderByDescending(c => c.Speed).ToList();

            foreach (var actingChar in activeChars)
            {
                if (!actingChar.IsAlive) continue;

                var enemyChars = _battlefield.Characters.Where(c => c.IsAlive && c.Team != actingChar.Team).ToList();
                if (!enemyChars.Any()) break;

                // Determine action
                string chosenAbility = null;
                Character chosenTarget = null;

                if (!string.IsNullOrEmpty(actingChar.AIScript))
                {
                    try
                    {
                        var aiScript = _scriptEngine.AIScriptForCharacter(actingChar.AIScript);
                        var proxy = new ScriptEngine.CharacterLuaProxy(actingChar);
                        var enemyProxies = enemyChars.Select(e => new ScriptEngine.CharacterLuaProxy(e)).ToList();
                        var (abilityName, targetIdx) = _scriptEngine.RunAIScript(aiScript, proxy, enemyProxies);
                        chosenAbility = abilityName;
                        if (targetIdx.HasValue && targetIdx.Value > 0 && targetIdx.Value <= enemyChars.Count)
                            chosenTarget = enemyChars[targetIdx.Value - 1];
                        else
                            chosenTarget = enemyChars.FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in AI script for {actingChar.Name}: {ex.Message}");
                    }
                }
                else
                {
                    // --- PLAYER INPUT ---
                    Console.WriteLine($"\n{actingChar.Name} ({actingChar.Team}) - Your turn!");

                    // Abilities
                    for (int i = 0; i < actingChar.Abilities.Count; i++)
                    {
                        var abName = actingChar.Abilities[i];
                        Console.WriteLine($"  [{i+1}] {abName}");
                    }
                    int abIdx = Utilities.ReadInt($"Choose ability [1-{actingChar.Abilities.Count}]: ", 1, actingChar.Abilities.Count) - 1;
                    chosenAbility = actingChar.Abilities[abIdx];

                    // Targets
                    for (int i = 0; i < enemyChars.Count; i++)
                    {
                        var c = enemyChars[i];
                        Console.WriteLine($"  [{i+1}] {c.Name} ({c.Team}) HP:{c.CurrentHP}/{c.MaxHP} Pos:[{c.X},{c.Y}]");
                    }
                    int tgtIdx = Utilities.ReadInt($"Choose target [1-{enemyChars.Count}]: ", 1, enemyChars.Count) - 1;
                    chosenTarget = enemyChars[tgtIdx];
                }

                if (chosenAbility == null || chosenTarget == null)
                {
                    Console.WriteLine($"{actingChar.Name} does nothing!");
                    continue;
                }

                // Find ability
                if (!_abilities.TryGetValue(chosenAbility, out var ability))
                {
                    Console.WriteLine($"{actingChar.Name} tried to use unknown ability '{chosenAbility}'.");
                    continue;
                }

                Console.WriteLine(
                    $"{actingChar.Name} ({actingChar.Team}) at [{actingChar.X},{actingChar.Y}] uses '{ability.Name}' on {chosenTarget.Name} ({chosenTarget.Team}) at [{chosenTarget.X},{chosenTarget.Y}]");

                _scriptEngine.UseAbility(actingChar, chosenTarget, ability);

                // Print defeat
                if (!chosenTarget.IsAlive)
                    Console.WriteLine($"{chosenTarget.Name} is defeated!");

                if (IsBattleOver()) break;
            }

            PrintStatus();
            _turn++;
        }

        var winningTeams = _teams.Where(t => t.Characters.Any(c => c.IsAlive)).ToList();
        if (winningTeams.Count == 1)
            Console.WriteLine($"\n*** Team '{winningTeams[0].Name}' wins! ***");
        else
            Console.WriteLine("\n*** The battle ends in a draw! ***");
    }

    private void PrintStatus()
    {
        Console.WriteLine("\nStatus:");
        foreach (var t in _teams)
        {
            var aliveChars = t.Characters.Where(c => c.IsAlive).ToList();
            if (!aliveChars.Any()) continue;
            Console.WriteLine($"  [{t.Name}]");
            foreach (var c in aliveChars)
                Console.WriteLine(
                    $"    {c.Name} HP:{c.CurrentHP}/{c.MaxHP} Pos:[{c.X},{c.Y}] Abilities: {string.Join(",", c.Abilities)}");
        }
    }

    private bool IsBattleOver()
    {
        var aliveTeams = _teams.Where(t => t.Characters.Any(c => c.IsAlive)).ToList();
        return aliveTeams.Count <= 1;
    }
}