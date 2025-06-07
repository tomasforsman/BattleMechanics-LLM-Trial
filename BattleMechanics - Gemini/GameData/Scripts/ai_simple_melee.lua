--- Chooses an action for an AI-controlled character.
-- @param actor The character performing the action.
-- @param allies A table of the actor's allies.
-- @param enemies A table of the actor's enemies.
-- @return A table with `ability` and `target` keys.
function choose_action(actor, allies, enemies)
    print(actor.name .. " is thinking...")

    local target = nil
    local lowest_hp = 9999

    -- Find the enemy with the lowest HP
    for i, enemy in ipairs(enemies) do
        if enemy.current_hp < lowest_hp then
            lowest_hp = enemy.current_hp
            target = enemy
        end
    end
    
    -- Prefer fireball if available, otherwise use first ability (attack)
    local ability_to_use = nil
    for i, ability in ipairs(actor.known_abilities) do
        if ability.name == 'fireball' then
            ability_to_use = ability
            break
        end
    end

    if ability_to_use == nil then
        ability_to_use = actor.known_abilities[1]
    end

    if target ~= nil and ability_to_use ~= nil then
        print(actor.name .. " decides to use " .. ability_to_use.name .. " on " .. target.name)
        return { ability = ability_to_use, target = target }
    else
        -- Fallback if no target or ability found
        return nil
    end
end