function use_ability(user, target)
    local base_damage = user.attack
    local special_multiplier = 1.8
    local damage = math_max(5, base_damage * special_multiplier)
    
    print(user.name .. " charges up for a devastating special attack!")
    print("A powerful strike that bypasses armor!")
    
    -- Direct damage, ignoring defense
    local old_hp = target.current_hp
    local new_hp = math_max(0, old_hp - damage)
    target.current_hp = new_hp
    
    print("  " .. target.name .. " takes " .. damage .. " piercing damage! (HP: " .. new_hp .. "/" .. target.max_hp .. ")")
    
    if new_hp <= 0 then
        print("  " .. target.name .. " has been defeated!"")
    end
end
