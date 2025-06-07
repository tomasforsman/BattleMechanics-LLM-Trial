function use_ability(user, target)
  local dmg = 12
  target:take_damage(dmg)
  print(user.name .. " hurls a fireball at " .. target.name .. " for " .. dmg .. " damage!")
end
