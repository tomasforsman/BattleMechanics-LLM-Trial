namespace BattleMechanic
{
    public class Action
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Script { get; set; } = "";
        
        // Additional properties for modding support
        public Dictionary<string, object> CustomProperties { get; set; } = new Dictionary<string, object>();
        
        public override string ToString()
        {
            return $"{Name}: {Description}4";
        }
    }
}