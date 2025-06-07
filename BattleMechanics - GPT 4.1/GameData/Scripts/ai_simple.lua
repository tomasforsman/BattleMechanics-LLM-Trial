function choose_action(self, enemies)
  -- Always use first ability on the first enemy
  return { ability = self.abilities and self.abilities[1] or "attack", target = 1 }
end
