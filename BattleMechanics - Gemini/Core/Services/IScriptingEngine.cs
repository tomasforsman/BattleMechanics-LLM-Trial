using BattleMechanic.Core.Models;

namespace BattleMechanic.Core.Services;

public interface IScriptingEngine
{
    void ExecuteAbilityScript(Ability ability, Character user, Character target);

    (Ability? chosenAbility, Character? chosenTarget) ExecuteAiScript(
        string aiScriptFile, 
        Character actor, 
        List<Character> allies, 
        List<Character> enemies);
}