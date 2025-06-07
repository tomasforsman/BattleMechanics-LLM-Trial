function use_ability(user, target)
    -- For simplicity, heal the user slightly to represent defensive stance
    local defense_bonus = 3
    print(user.name .. " takes a defensive stance!")
    user:heal(defense_bonus)
end
