function use_ability(user, target)
    local base_damage = user.attack
    local damage = math_max(1, base_damage + math_random(-2, 2))
    
    print(user.name .. " attacks " .. target.name .. " with their weapon!")
    target:take_damage(damage)
end
