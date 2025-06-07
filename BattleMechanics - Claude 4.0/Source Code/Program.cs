namespace BattleMechanic
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== BattleMechanic Combat Engine ===\n");

            try
            {
                // Initialize the data loader and load all game data
                var dataLoader = new DataLoader("GameData");
                await dataLoader.LoadAllDataAsync();

                // Initialize the script engine
                var scriptEngine = new ScriptEngine("GameData/Scripts");
                
                // Initialize the combat manager
                var combatManager = new CombatManager(dataLoader, scriptEngine);
                
                // Load teams for battle
                var heroTeam = dataLoader.GetTeam("heroes");
                var enemyTeam = dataLoader.GetTeam("enemies");
                
                if (heroTeam == null || enemyTeam == null)
                {
                    Console.WriteLine("Error: Could not load required teams 'heroes' and 'enemies'");
                    return;
                }

                Console.WriteLine($"Battle: {heroTeam.Name} vs {enemyTeam.Name}\n");
                
                // Start the battle
                var winningTeam = await combatManager.StartBattleAsync(new[] { heroTeam, enemyTeam });
                
                Console.WriteLine($"\n=== BATTLE COMPLETE ===");
                Console.WriteLine($"Winner: {winningTeam.Name}!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}