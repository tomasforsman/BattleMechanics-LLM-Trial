# BattleMechanic Prototype

Welcome to the BattleMechanic prototype, a modular, data-driven combat engine for a text-based RPG. This document explains how the engine works, how to run it, and how to add your own content.

## 1. Project Overview

The core philosophy of this engine is **moddability**. All game content—characters, abilities, and teams—is defined in simple `YAML` data files. The logic for complex abilities and AI behavior is defined in `Lua` script files. This separation allows non-programmers to easily create and modify game content without ever touching the C# source code.

### Core Features
- **Data-Driven:** All content is loaded from `/GameData`. No hardcoded characters or stats.
- **Scriptable Logic:** `Lua` scripts control ability effects and AI decisions.
- **Modular Architecture:** The system is built with interfaces for data loading (`IDataLoader`) and scripting (`IScriptingEngine`), making it possible to support JSON files or a different scripting language in the future.
- **Team-Based Combat:** The combat loop supports any number of teams and is not restricted to a simple "player vs. enemy" setup.
- **Player Input:** Teams can be flagged as `is_player_controlled` to allow a human to make decisions during combat.

## 2. How to Run

1.  **Prerequisites:** You need the [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) installed.
2.  **Build:** Open a terminal in the root project directory and run `dotnet build`.
3.  **Run:** After a successful build, run `dotnet run`.

The application will automatically load all data from the `/GameData` directory, validate it, and start a battle between the first two teams it finds (e.g., "Blue Team" vs. "Red Team").

## 3. Folder & File Structure

The project is organized into two main parts: the C# source code and the `GameData` content.

/
├── BattleMechanic/ (C# Project Files)
│   ├── Core/         # Core models and service interfaces
│   ├── Gameplay/     # Combat loop and game flow logic
│   └── Program.cs    # Main application entry point
│
├── GameData/ (All Moddable Content)
│   ├── Abilities/    # YAML files for each ability
│   ├── Characters/   # YAML files for each character type
│   ├── Teams/        # YAML files defining team rosters
│   └── Scripts/      # LUA scripts for abilities and AI
│
└── README.md (This file)


## 4. How to Mod the Game

Modding is done by adding or editing files in the `GameData` directory.

### Adding a New Character

1.  Create a new `.yaml` file in `/GameData/Characters/` (e.g., `paladin.yaml`).
2.  Define its properties. All fields are required except `ai_script`.

    ```yaml
    name: Paladin      # In-game name
    max_hp: 40         # Maximum health
    attack: 7          # Base physical attack power
    defense: 6         # Base damage reduction
    speed: 3           # Determines turn order (higher is faster)
    x: 0               # Grid position (for future use)
    y: 0               # Grid position (for future use)
    abilities:         # List of ability file names (without .yaml)
      - attack
      - heal_light
    ai_script: ai_simple_melee.lua # Optional: Lua script for AI logic
    ```

### Adding a New Ability

1.  Create a new `.yaml` file in `/GameData/Abilities/` (e.g., `heal_light.yaml`).
2.  Define its script. This script will be executed when the ability is used.

    ```yaml
    name: Heal Light
    description: "A faint light that restores a small amount of health."
    script: heal_light.lua # The corresponding Lua script in /GameData/Scripts/
    ```

3.  Create the corresponding Lua script in `/GameData/Scripts/` (e.g., `heal_light.lua`).

### Adding a New Team

1.  Create a new `.yaml` file in `/GameData/Teams/` (e.g., `green_team.yaml`).
2.  List the character file names (without `.yaml`) that belong to this team.
3.  Set `is_player_controlled` to `true` if you want to control this team's actions.

    ```yaml
    name: Green Team
    is_player_controlled: false
    members:
      - paladin
      - goblin
    ```

## 5. Lua Scripting API

Your Lua scripts have access to a powerful API to interact with the game engine.

### Ability Scripts (`use_ability`)

Every ability script must define a global function: `use_ability(user, target)`.

-   `user`: The character object using the ability.
-   `target`: The character object being targeted.

**Available C# Methods on Character Objects:**

You can call these methods directly on the `user` and `target` objects in Lua.

-   `target:take_damage(amount)`: Inflicts damage on the target. The engine automatically calculates defense.
-   `user:heal(amount)`: Heals the user (or any target) by a given amount.
    -t-   `user:defend()`: Puts the user in a defending state, doubling their defense until their next turn.

**Available Properties on Character Objects:**

You can read these properties from any character object.

-   `name` (string)
-   `current_hp` (number)
-   `max_hp` (number)
-   `attack` (number)
-   `defense` (number)
-   `speed` (number)
-   `position` (Vector2 object with .x and .y)
-   `team_id` (string)
-   `known_abilities` (table of ability objects)

**Example (`heal_light.lua`):**

```lua
-- /Scripts/heal_light.lua
function use_ability(user, target)
    local heal_power = 10
    print(user.name .. " calls upon a holy light!")
    
    -- The target of a heal is usually the user themselves.
    user:heal(heal_power) 
end
AI Scripts (choose_action)
An AI script must define a global function: choose_action(actor, allies, enemies).

actor: The AI character whose turn it is.
allies: A Lua table of all living allied characters.
enemies: A Lua table of all living enemy characters.
The function must return a table containing two keys: ability (the ability object to use) and target (the character object to target).

Example (ai_healer.lua):

Lua

-- /Scripts/ai_healer.lua
function choose_action(actor, allies, enemies)
    -- If an ally is below 50% health, heal them.
    for i, ally in ipairs(allies) do
        if ally.current_hp < (ally.max_hp / 2) then
            -- Find the 'heal' ability
            for j, ability in ipairs(actor.known_abilities) do
                if ability.name == 'Heal Light' then
                    return { ability = ability, target = ally }
                end
            end
        end
    end

    -- Otherwise, attack the weakest enemy.
    local weakest_enemy = enemies[1]
    for i, enemy in ipairs(enemies) do
        if enemy.current_hp < weakest_enemy.current_hp then
            weakest_enemy = enemy
        end
    end

    return { ability = actor.known_abilities[1], target = weakest_enemy }
end
```

6. Future Extensibility
The architecture was designed for growth:

New Data Formats: To support JSON, one could create a JsonDataLoader class that implements the IDataLoader interface. The Program.cs file would then just need to be told to instantiate that class instead of YamlDataLoader.
New Scripting Languages: To support JavaScript, one could create a JintScriptingEngine that implements the IScriptingEngine interface, leaving the core combat logic untouched.
New Game Mechanics: Add new properties to the character YAML files (e.g., mana: 100). The C# Character.cs class can be updated to include this field, and it will be automatically available for use in Lua scripts.