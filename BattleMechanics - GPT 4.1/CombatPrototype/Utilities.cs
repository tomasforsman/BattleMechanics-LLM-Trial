namespace BattleMechanics___GPT_4._1.CombatPrototype;

public static class Utilities
{
    /// <summary>
    /// (Read a string input from the console with a prompt.
    /// </summary>
    /// <param name="prompt"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int ReadInt(string prompt, int min, int max)
    {
        int result;
        do
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (int.TryParse(input, out result) && result >= min && result <= max)
                return result;
            Console.WriteLine($"Please enter a number between {min} and {max}.");
        } while (true);
    }
}