--- Script for the 'defend' action.
-- @param user The character using the ability.
-- @param target The character being targeted (usually self, but context provides it).
function use_ability(user, target)
    -- The defend function is a C# method exposed to Lua.
    user:defend()
end