namespace BattleMechanic
{
    public class Team
    {
        public string Name { get; set; } = "";
        public List<string> Members { get; set; } = new List<string>();
        public List<Character> Characters { get; set; } = new List<Character>();
        
        // Additional properties for modding support
        public Dictionary<string, object> CustomProperties { get; set; } = new Dictionary<string, object>();

        public bool IsDefeated => Characters.All(c => !c.IsAlive);
        public int AliveCount => Characters.Count(c => c.IsAlive);
        
        public void AddCharacter(Character character)
        {
            Characters.Add(character);
            character.Team = Name;
        }

        public List<Character> GetAliveCharacters()
        {
            return Characters.Where(c => c.IsAlive).ToList();
        }

        public List<Character> GetEnemyTargets(List<Team> allTeams)
        {
            return allTeams
                .Where(t => t.Name != Name)
                .SelectMany(t => t.GetAliveCharacters())
                .ToList();
        }
    }
}