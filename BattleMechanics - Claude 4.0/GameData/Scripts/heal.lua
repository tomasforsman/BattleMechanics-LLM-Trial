function use_ability(user, target)
    local heal_amount = 12 + math_random(0, 6)
    
    if target.team == user.team then
        print(user.name .. " casts a healing spell on " .. target.name)
        print("Warm light surrounds " .. target.name .. " as wounds close!")
        target:heal(heal_amount)
    else
        print(user.name .. " tries to heal " .. target.name .. " but they're enemies!")
        print("The healing magic fails to take effect on a hostile target.")
    end
end
