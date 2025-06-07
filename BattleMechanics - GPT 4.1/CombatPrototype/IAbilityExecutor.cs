namespace BattleMechanics___GPT_4._1.CombatPrototype;

/// <summary>
///     Interface for executing an ability.
/// </summary>
public interface IAbilityExecutor
{
    void UseAbility(Character user, Character target, Ability ability);
}