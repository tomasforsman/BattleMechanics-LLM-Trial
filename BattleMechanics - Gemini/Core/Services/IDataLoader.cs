namespace BattleMechanic.Core.Services;

/// <summary>
/// Interface for a service that loads all game data from a source.
/// </summary>
public interface IDataLoader
{
    /// <summary>
    /// Loads all game data from the specified root directory.
    /// </summary>
    /// <param name="gameDataPath">The root path of the game data.</param>
    /// <returns>A tuple containing dictionaries of all loaded game objects.</returns>
    (Dictionary<string, Models.Character> characters,
        Dictionary<string, Models.Team> teams,
        Dictionary<string, Models.Ability> abilities) LoadAll(string gameDataPath);
}