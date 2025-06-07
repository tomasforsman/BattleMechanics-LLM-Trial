namespace BattleMechanic
{
    public class CombatManager
    {
        private readonly DataLoader _dataLoader;
        private readonly ScriptEngine _scriptEngine;
        private List<Team> _teams = new List<Team>();
        private int _turnNumber = 0;

        public CombatManager(DataLoader dataLoader, ScriptEngine scriptEngine)
        {
            _dataLoader = dataLoader;
            _scriptEngine = scriptEngine;
        }

        public async Task<Team> StartBattleAsync(Team[] teams)
        {
            _teams = teams.ToList();
            _turnNumber = 0;

            // Initialize all characters
            foreach (var team in _teams)
            {
                foreach (var character in team.Characters)
                {
                    character.Initialize();
                }
            }

            PrintBattleStatus();

            // Combat loop
            while (_teams.Count(t => !t.IsDefeated) > 1)
            {
                _turnNumber++;
                Console.WriteLine($"\n--- Turn {_turnNumber} ---");

                var turnOrder = GetTurnOrder();
                
                foreach (var character in turnOrder)
                {
                    if (!character.IsAlive)
                        continue;

                    await ExecuteCharacterTurnAsync(character);
                    
                    // Check if battle is over after each action
                    if (_teams.Count(t => !t.IsDefeated) <= 1)
                        break;
                }

                PrintBattleStatus();
            }

            return _teams.First(t => !t.IsDefeated);
        }

        private List<Character> GetTurnOrder()
        {
            return _teams
                .SelectMany(t => t.GetAliveCharacters())
                .OrderByDescending(c => c.Speed)
                .ThenBy(c => Guid.NewGuid()) // Random tiebreaker
                .ToList();
        }

        private async Task ExecuteCharacterTurnAsync(Character character)
        {
            Console.WriteLine($"\n{character.Name}'s turn:");

            Action? chosenAction = null;
            Character? target = null;

            if (character.IsPlayerControlled)
            {
                // Player input
                var choice = GetPlayerChoice(character);
                chosenAction = choice.action;
                target = choice.target;
            }
            else
            {
                // AI decision
                var choice = await GetAIChoiceAsync(character);
                chosenAction = choice.action;
                target = choice.target;
            }

            if (chosenAction != null && target != null)
            {
                await ExecuteActionAsync(character, target, chosenAction);
            }
        }

        private (Action action, Character target) GetPlayerChoice(Character character)
        {
            // Show available actions
            Console.WriteLine("Available actions:");
            var availableActions = new List<Action>();
            
            for (int i = 0; i < character.Abilities.Count; i++)
            {
                var action = _dataLoader.GetAction(character.Abilities[i]);
                if (action != null)
                {
                    availableActions.Add(action);
                    Console.WriteLine($"{i + 1}. {action}");
                }
            }

            // Get action choice
            Action? chosenAction = null;
            while (chosenAction == null)
            {
                Console.Write("Choose action (number): ");
                if (int.TryParse(Console.ReadLine(), out int actionChoice) && 
                    actionChoice > 0 && actionChoice <= availableActions.Count)
                {
                    chosenAction = availableActions[actionChoice - 1];
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }
            }

            // Show available targets
            var possibleTargets = GetPossibleTargets(character);
            Console.WriteLine("Available targets:");
            for (int i = 0; i < possibleTargets.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {possibleTargets[i]}");
            }

            // Get target choice
            Character? target = null;
            while (target == null)
            {
                Console.Write("Choose target (number): ");
                if (int.TryParse(Console.ReadLine(), out int targetChoice) && 
                    targetChoice > 0 && targetChoice <= possibleTargets.Count)
                {
                    target = possibleTargets[targetChoice - 1];
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }
            }

            return (chosenAction, target);
        }

        private async Task<(Action action, Character target)> GetAIChoiceAsync(Character character)
        {
            // Try to use AI script first
            if (!string.IsNullOrEmpty(character.AiScript))
            {
                try
                {
                    var choice = await _scriptEngine.ExecuteAIScriptAsync(character.AiScript, character, _teams);
                    if (choice.action != null && choice.target != null)
                    {
                        return choice;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"AI script error for {character.Name}: {ex.Message}");
                }
            }

            // Fallback to simple AI
            var possibleTargets = GetPossibleTargets(character);
            var availableActions = character.Abilities
                .Select(a => _dataLoader.GetAction(a))
                .Where(a => a != null)
                .ToList();

            if (availableActions.Any() && possibleTargets.Any())
            {
                // Simple AI: prefer "attack" action, target lowest HP enemy
                var preferredAction = availableActions.FirstOrDefault(a => a!.Name.ToLower() == "attack") ?? availableActions.First();
                var preferredTarget = possibleTargets
                    .Where(t => t.Team != character.Team)
                    .OrderBy(t => t.CurrentHp)
                    .FirstOrDefault() ?? possibleTargets.First();

                return (preferredAction!, preferredTarget);
            }

            // Should not happen if data is valid
            throw new InvalidOperationException($"No valid action/target combination for {character.Name}");
        }

        private List<Character> GetPossibleTargets(Character actor)
        {
            // For now, all living characters are possible targets
            // This could be modified based on action type, range, etc.
            return _teams.SelectMany(t => t.GetAliveCharacters()).ToList();
        }

        private async Task ExecuteActionAsync(Character user, Character target, Action action)
        {
            Console.WriteLine($"{user.Name} uses {action.Name} on {target.Name}");

            try
            {
                await _scriptEngine.ExecuteActionScriptAsync(action.Script, user, target);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Script execution error: {ex.Message}");
                Console.WriteLine("Falling back to basic attack...");
                
                // Fallback to basic damage
                var damage = Math.Max(1, user.Attack);
                target.TakeDamage(damage);
            }
        }

        private void PrintBattleStatus()
        {
            Console.WriteLine("\n=== Battle Status ===");
            foreach (var team in _teams)
            {
                Console.WriteLine($"{team.Name} ({team.AliveCount}/{team.Characters.Count} alive):");
                foreach (var character in team.Characters)
                {
                    Console.WriteLine($"  {character}");
                }
            }
        }
    }
}