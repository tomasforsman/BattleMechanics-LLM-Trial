function use_ability(user, target)
    local magic_damage = 15
    local intelligence_bonus = math_max(0, (user.attack - 8))
    local total_damage = magic_damage + intelligence_bonus
    
    print(user.name .. " hurls a blazing fireball at " .. target.name .. "!")
    print("The fireball explodes in a burst of magical energy!")
    target:take_damage(total_damage)
end
