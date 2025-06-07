# BattleMechanic – Modular RPG Combat Engine (C#/.NET 9)

## Overview

BattleMechanic is a modular, **data-driven RPG combat prototype**.  
All game content is loaded from simple YAML and Lua files—no C# coding needed to add new characters, teams, or actions!

- **Teams, characters, and actions/abilities**: Defined by modder-editable YAML files in `/GameData/`.
- **Battle logic and AI**: Handled by Lua scripts in `/GameData/Scripts/`.
- **Grid-based**: Each character has a 2D (X,Y) position.
- **Console-only**: No graphics—everything is text output, perfect for extensibility.

## Folder Structure

```
/GameData/
/Characters/
knight.yaml
mage.yaml
/Teams/
red_team.yaml
blue_team.yaml
/Actions/
attack.yaml
fireball.yaml
/Scripts/
fireball.lua
ai_simple.lua
/CombatPrototype/
(C# source files)
```

## How to Run

1. **Install .NET 9** (Preview or latest if not GA).
2. Restore NuGet packages: `YamlDotNet` and `MoonSharp`.
3. Place all YAML and Lua files in `/GameData/` as above.
4. Run the project from `/CombatPrototype/`.
5. Observe the battle—modify YAML or scripts to add your own heroes, monsters, or crazy new actions!

## Modding Guide

### Add/Edit Characters

- Copy `/GameData/Characters/knight.yaml` as a template.
- Edit `name`, `team`, stats, position, abilities, or `ai_script` as you like.

```yaml
name: Knight
team: Red
max_hp: 30
attack: 8
defense: 4
speed: 5
x: 0
y: 1
abilities:
  - attack
  - special_strike
ai_script: ai_simple.lua
```

Add/Edit Teams
List character names as members.

```yaml
name: Red
members:
  - Knight
  - Mage
```

Add/Edit Abilities
Each ability points to a Lua script.

yaml
Copy
Edit
name: Fireball
description: "Hurls a fireball at the target"
script: fireball.lua
Lua Scripting API
For Abilities:
Called as: use_ability(user, target)

User/target have:

user.name, user.x, user.y, user.hp, user.max_hp

user.attack, user.defense, user.speed

Methods: user:take_damage(amount), user:heal(amount)

For AI Scripts:
Called as: choose_action(self, enemies)

Return: { ability = "attack", target = 1 } (target index 1-based)

enemies is a Lua array of character objects

Print to console: use print(...) in Lua.
Error Handling
The engine prints clear errors for missing fields, broken scripts, or YAML mistakes.

See console output for help.

Extending the Engine
Add support for JSON/TOML: create new loader classes using IContentLoader.

Add a new scripting engine: implement IAbilityExecutor.

Add new object types: just add new YAML/Lua files!

Sample Data
(See /GameData/ for working YAML and Lua samples.)

License
MIT. Use, fork, or mod for any project or application.

Contact
Pull requests, questions, or suggestions?
Open an issue on GitHub or contact the project owner.

yaml
Copy
Edit
