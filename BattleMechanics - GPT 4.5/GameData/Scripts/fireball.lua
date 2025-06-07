function use_ability(user, target)
  local dmg = user.Attack + 5
  target:TakeDamage(dmg)
  print(user.Name .. " casts Fireball, dealing " .. dmg .. " damage!")
end
