using BattleMechanic.Core.Models;
using BattleMechanic.Core.Services;

namespace BattleMechanic.Gameplay;

/// <summary>
/// Manages the state and turn-by-turn flow of a single combat encounter.
/// </summary>
public class CombatManager
{
    private readonly IScriptingEngine _scriptingEngine;
    private readonly IReadOnlyDictionary<string, Ability> _abilityBlueprints;
    private readonly IReadOnlyDictionary<string, Team> _teamBlueprints;
    private readonly List<Character> _combatants;
    private int _roundNumber;

    public CombatManager(List<Team> teamsToInclude,
                         IReadOnlyDictionary<string, Character> characterBlueprints,
                         IReadOnlyDictionary<string, Ability> abilityBlueprints,
                         IReadOnlyDictionary<string, Team> teamBlueprints,
                         IScriptingEngine scriptingEngine)
    {
        _scriptingEngine = scriptingEngine;
        _abilityBlueprints = abilityBlueprints;
        _teamBlueprints = teamBlueprints;
        _combatants = [];

        foreach (var team in teamsToInclude)
        {
            foreach (var charFileName in team.Members)
            {
                if (characterBlueprints.TryGetValue(charFileName, out var blueprint))
                {
                    var characterInstance = new Character
                    {
                        Name = blueprint.Name,
                        TeamId = team.Name, // This sets the TeamId to the display name, e.g., "Red Team"
                        MaxHp = blueprint.MaxHp,
                        Attack = blueprint.Attack,
                        Defense = blueprint.Defense,
                        Speed = blueprint.Speed,
                        X = blueprint.X,
                        Y = blueprint.Y,
                        Abilities = new List<string>(blueprint.Abilities),
                        AiScript = blueprint.AiScript
                    };

                    characterInstance.InitializeForBattle();
                    
                    foreach (var abilityName in characterInstance.Abilities)
                    {
                        if (_abilityBlueprints.TryGetValue(abilityName, out var ability))
                        {
                            characterInstance.KnownAbilities.Add(ability);
                        }
                    }
                    _combatants.Add(characterInstance);
                }
            }
        }
    }

    public void StartBattle()
    {
        BattleLogger.LogBattleStatus(_combatants);
        _roundNumber = 1;

        while (!IsBattleOver())
        {
            BattleLogger.LogRoundStart(_roundNumber);
            ExecuteRound();
            BattleLogger.LogBattleStatus(_combatants);
            _roundNumber++;
        }

        var winner = GetWinningTeam();
        if (winner != null)
        {
            BattleLogger.LogBattleEnd(winner);
        }
    }

    private void ExecuteRound()
    {
        var turnOrder = _combatants.Where(c => c.IsAlive).OrderByDescending(c => c.Speed).ToList();
        
        foreach (var activeCharacter in turnOrder)
        {
            if (!activeCharacter.IsAlive) continue;

            BattleLogger.LogTurnStart(activeCharacter);
            activeCharacter.OnTurnStart();
            ExecuteTurn(activeCharacter);
            BattleLogger.HorizontalRule();
        }
    }

    private void ExecuteTurn(Character actor)
    {
        var allies = _combatants.Where(c => c.TeamId == actor.TeamId && c.IsAlive).ToList();
        var enemies = _combatants.Where(c => c.TeamId != actor.TeamId && c.IsAlive).ToList();

        if (enemies.Count == 0) return;

        (Ability? chosenAbility, Character? chosenTarget) = (null, null);

        // *** FIX: Changed dictionary lookup to a safe LINQ query. ***
        // This finds the team blueprint by its 'Name' property instead of trying to use the name as a key.
        var controllingTeam = _teamBlueprints.Values.FirstOrDefault(t => t.Name.Equals(actor.TeamId, StringComparison.OrdinalIgnoreCase));
        
        if (controllingTeam == null)
        {
            // This is a safety check in case something goes wrong.
            Console.WriteLine($"[ERROR] Could not find team data for TeamId: {actor.TeamId}");
            return;
        }

        if (controllingTeam.IsPlayerControlled)
        {
            (chosenAbility, chosenTarget) = GetPlayerChoice(actor, enemies);
        }
        else if (!string.IsNullOrEmpty(actor.AiScript))
        {
            (chosenAbility, chosenTarget) = _scriptingEngine.ExecuteAiScript(actor.AiScript, actor, allies, enemies);
        }

        if (chosenAbility == null || chosenTarget == null)
        {
            Console.WriteLine(" -> AI decision failed or not provided. Using fallback: basic attack.");
            chosenAbility = actor.KnownAbilities.FirstOrDefault(a => a.Name.Equals("attack", StringComparison.OrdinalIgnoreCase))
                           ?? actor.KnownAbilities.FirstOrDefault();
            chosenTarget = enemies.MinBy(e => e.CurrentHp);
        }

        if (chosenAbility != null && chosenTarget != null)
        {
            BattleLogger.LogAction(actor, chosenAbility, chosenTarget);
            _scriptingEngine.ExecuteAbilityScript(chosenAbility, actor, chosenTarget);
        }
        else
        {
            Console.WriteLine($"{actor.Name} is unable to act.");
        }
    }
    
    private (Ability? chosenAbility, Character? chosenTarget) GetPlayerChoice(Character actor, List<Character> enemies)
    {
        Console.WriteLine("Player, choose an action:");
        for(int i = 0; i < actor.KnownAbilities.Count; i++)
        {
            Console.WriteLine($" [{i + 1}] {actor.KnownAbilities[i].Name} - {actor.KnownAbilities[i].Description}");
        }

        Ability? selectedAbility = null;
        while (selectedAbility == null)
        {
            Console.Write("Action choice > ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= actor.KnownAbilities.Count)
            {
                selectedAbility = actor.KnownAbilities[choice - 1];
            }
            else
            {
                Console.WriteLine("Invalid choice. Please enter a number from the list.");
            }
        }

        Console.WriteLine("Choose a target:");
        for (int i = 0; i < enemies.Count; i++)
        {
            Console.WriteLine($" [{i + 1}] {enemies[i].Name} [{enemies[i].CurrentHp}/{enemies[i].MaxHp} HP]");
        }

        Character? selectedTarget = null;
        while(selectedTarget == null)
        {
            Console.Write("Target choice > ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= enemies.Count)
            {
                selectedTarget = enemies[choice - 1];
            }
            else
            {
                Console.WriteLine("Invalid target. Please enter a number from the list.");
            }
        }

        return (selectedAbility, selectedTarget);
    }

    private bool IsBattleOver()
    {
        return _combatants.Where(c => c.IsAlive).Select(c => c.TeamId).Distinct().Count() <= 1;
    }

    private string? GetWinningTeam()
    {
        return _combatants.Where(c => c.IsAlive).Select(c => c.TeamId).FirstOrDefault();
    }
}