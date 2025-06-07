--- Script for using a damaging ability.
-- It checks the ability's name to apply different damage values.
-- @param user The character using the ability.
-- @param target The character being targeted.
function use_ability(user, target)
    local damage = 0
    
    if user.last_used_ability.name == 'fireball' then
        -- Fireball does fixed magic damage
        damage = 12 
        print(user.name .. " hurls a fireball at " .. target.name .. "!")
    else -- Default 'attack'
        -- Basic attack uses the character's attack stat
        damage = user.attack
        print(user.name .. " attacks " .. target.name .. "!")
    end

    -- The take_damage function is a C# method exposed to Lua.
    target:take_damage(damage)
end