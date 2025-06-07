function use_ability(user, target)
  local dmg = user.Attack
  target:TakeDamage(dmg)
  print(user.Name .. " attacks, dealing " .. dmg .. " damage!")
end
