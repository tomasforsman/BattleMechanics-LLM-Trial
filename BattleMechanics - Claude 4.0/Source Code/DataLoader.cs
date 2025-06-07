using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BattleMechanic
{
    public class DataLoader
    {
        private readonly string _dataPath;
        private readonly IDeserializer _deserializer;
        private readonly Dictionary<string, Character> _characters = new Dictionary<string, Character>();
        private readonly Dictionary<string, Team> _teams = new Dictionary<string, Team>();
        private readonly Dictionary<string, Action> _actions = new Dictionary<string, Action>();

        public DataLoader(string dataPath)
        {
            _dataPath = dataPath;
            _deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
        }

        public async Task LoadAllDataAsync()
        {
            Console.WriteLine("Loading game data...");
            
            await LoadActionsAsync();
            await LoadCharactersAsync();
            await LoadTeamsAsync();
            
            Console.WriteLine($"Loaded: {_characters.Count} characters, {_teams.Count} teams, {_actions.Count} actions\n");
        }

        private async Task LoadActionsAsync()
        {
            var actionsPath = Path.Combine(_dataPath, "Actions");
            if (!Directory.Exists(actionsPath))
            {
                Console.WriteLine($"Warning: Actions directory '{actionsPath}' not found");
                return;
            }

            var actionFiles = Directory.GetFiles(actionsPath, "*.yaml", SearchOption.AllDirectories);
            foreach (var file in actionFiles)
            {
                try
                {
                    var content = await File.ReadAllTextAsync(file);
                    var action = _deserializer.Deserialize<Action>(content);
                    var actionName = Path.GetFileNameWithoutExtension(file);
                    
                    _actions[actionName] = action;
                    Console.WriteLine($"Loaded action: {action.Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading action file {file}: {ex.Message}");
                }
            }
        }

        private async Task LoadCharactersAsync()
        {
            var charactersPath = Path.Combine(_dataPath, "Characters");
            if (!Directory.Exists(charactersPath))
            {
                Console.WriteLine($"Warning: Characters directory '{charactersPath}' not found");
                return;
            }

            var characterFiles = Directory.GetFiles(charactersPath, "*.yaml", SearchOption.AllDirectories);
            foreach (var file in characterFiles)
            {
                try
                {
                    var content = await File.ReadAllTextAsync(file);
                    var character = _deserializer.Deserialize<Character>(content);
                    var characterName = Path.GetFileNameWithoutExtension(file);
                    
                    _characters[characterName] = character;
                    Console.WriteLine($"Loaded character: {character.Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading character file {file}: {ex.Message}");
                }
            }
        }

        private async Task LoadTeamsAsync()
        {
            var teamsPath = Path.Combine(_dataPath, "Teams");
            if (!Directory.Exists(teamsPath))
            {
                Console.WriteLine($"Warning: Teams directory '{teamsPath}' not found");
                return;
            }

            var teamFiles = Directory.GetFiles(teamsPath, "*.yaml", SearchOption.AllDirectories);
            foreach (var file in teamFiles)
            {
                try
                {
                    var content = await File.ReadAllTextAsync(file);
                    var team = _deserializer.Deserialize<Team>(content);
                    var teamName = Path.GetFileNameWithoutExtension(file);
                    
                    // Populate team with character instances
                    foreach (var memberName in team.Members)
                    {
                        if (_characters.ContainsKey(memberName))
                        {
                            // Create a copy of the character for this team
                            var character = CloneCharacter(_characters[memberName]);
                            team.AddCharacter(character);
                        }
                        else
                        {
                            Console.WriteLine($"Warning: Character '{memberName}' not found for team '{team.Name}'");
                        }
                    }
                    
                    _teams[teamName] = team;
                    Console.WriteLine($"Loaded team: {team.Name} with {team.Characters.Count} members");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading team file {file}: {ex.Message}");
                }
            }
        }

        private Character CloneCharacter(Character original)
        {
            return new Character
            {
                Name = original.Name,
                Team = original.Team,
                MaxHp = original.MaxHp,
                CurrentHp = original.MaxHp,
                Attack = original.Attack,
                Defense = original.Defense,
                Speed = original.Speed,
                X = original.X,
                Y = original.Y,
                Abilities = new List<string>(original.Abilities),
                AiScript = original.AiScript,
                CustomProperties = new Dictionary<string, object>(original.CustomProperties)
            };
        }

        public Character? GetCharacter(string name) => _characters.GetValueOrDefault(name);
        public Team? GetTeam(string name) => _teams.GetValueOrDefault(name);
        public Action? GetAction(string name) => _actions.GetValueOrDefault(name);
        
        public IEnumerable<Character> GetAllCharacters() => _characters.Values;
        public IEnumerable<Team> GetAllTeams() => _teams.Values;
        public IEnumerable<Action> GetAllActions() => _actions.Values;
    }
}