using BattleMechanic.Core.Models;
using BattleMechanic.Core.Services;
using BattleMechanic.Gameplay;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("--- BattleMechanic Prototype Initializing ---");

        string gameDataPath = Path.Combine(AppContext.BaseDirectory, "GameData");

        // 1. Instantiate Services
        IDataLoader dataLoader = new YamlDataLoader();
        IScriptingEngine scriptingEngine = new MoonSharpScriptingEngine(gameDataPath);

        try
        {
            // 2. Load all game data blueprints from files
            var (characterBlueprints, teamBlueprints, abilityBlueprints) = dataLoader.LoadAll(gameDataPath);
            
            // 3. Pre-flight checks
            if (teamBlueprints.Count < 2)
            {
                Console.WriteLine("[FATAL] Not enough teams defined to start a battle. At least 2 teams are required.");
                return;
            }

            if (characterBlueprints.Count == 0 || abilityBlueprints.Count == 0)
            {
                 Console.WriteLine("[FATAL] No characters or abilities were loaded. Cannot start battle.");
                return;
            }

            // 4. Setup and start the combat
            var teamsForBattle = teamBlueprints.Values.Take(2).ToList();
            Console.WriteLine($"\nStarting battle: '{teamsForBattle[0].Name}' vs '{teamsForBattle[1].Name}'");
            
            var combatManager = new CombatManager(
                teamsForBattle, 
                characterBlueprints, 
                abilityBlueprints, 
                teamBlueprints, 
                scriptingEngine);

            combatManager.StartBattle();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[FATAL ERROR] An unhandled exception occurred in the engine: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
        
        Console.WriteLine("\n--- Prototype Execution Finished ---");
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }
}