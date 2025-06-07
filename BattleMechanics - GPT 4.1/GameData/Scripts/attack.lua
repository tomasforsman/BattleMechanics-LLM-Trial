function use_ability(user, target)
  local dmg = user.attack - target.defense
  if dmg < 1 then dmg = 1 end
  target:take_damage(dmg)
  print(user.name .. " strikes " .. target.name .. " for " .. dmg .. " damage!")
end