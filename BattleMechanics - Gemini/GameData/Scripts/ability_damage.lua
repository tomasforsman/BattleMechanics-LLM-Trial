--- Generic script for handling damaging abilities.
-- It intelligently switches behavior based on the ability's name.
-- @param ability The ability object being used.
-- @param user The character using the ability.
-- @param target The character being targeted.
function use_ability(ability, user, target)
    local damage = 0
    
    if ability.name == 'fireball' then
        -- Fireball is magical and does a fixed amount of damage.
        damage = 15 
        print(user.name .. " conjures and hurls a fireball at " .. target.name .. "!")
    else 
        -- Default case is a physical 'attack'.
        -- It uses the character's own 'attack' stat.
        damage = user.attack
        print(user.name .. " strikes at " .. target.name .. "!")
    end

    -- Call the C# method to apply the calculated damage.
    -- The C# side handles all the defense and health calculations.
    target:take_damage(damage)
end