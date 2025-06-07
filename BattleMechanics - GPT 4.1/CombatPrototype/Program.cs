namespace BattleMechanics___GPT_4._1.CombatPrototype;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("=== BattleMechanic RPG Combat Engine ===");
        try
        {
            // Load all game data
            var gameDataPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "GameData");
            if (!Directory.Exists(gameDataPath))
            {
                Console.WriteLine($"GameData folder not found at {gameDataPath}");
                return;
            }

            var dataLoader = new DataLoader(gameDataPath);
            dataLoader.LoadAll();

            // Set up battlefield and teams
            var battlefield = new Battlefield(dataLoader.Characters.Values.ToList());

            var combatManager = new CombatManager(
                battlefield,
                dataLoader.Teams.Values.ToList(),
                dataLoader.Abilities,
                dataLoader.ScriptEngine
            );

            combatManager.RunBattle();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fatal error: " + ex.Message);
            Console.WriteLine(ex.StackTrace);
        }

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }
}