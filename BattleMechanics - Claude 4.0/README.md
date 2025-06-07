# BattleMechanics - Modular RPG Combat Engine

## Overview

BattleMechanics is a flexible, data-driven RPG combat prototype built in C# .NET 9. It features a modular architecture that supports:

- **YAML-based configuration** for all game content (characters, teams, actions)
- **Lua scripting** for custom abilities and AI behaviors
- **Grid-based positioning** system
- **Turn-based combat** with speed-based initiative
- **Extensible architecture** for future enhancements

The system is designed to be easily moddable by non-programmers through simple YAML files and Lua scripts.

## Quick Start

### Prerequisites
- .NET 9.0 SDK
- Windows, macOS, or Linux

### Running the Game

1. Clone or download the project
2. Open terminal/command prompt in the project directory
3. Run: dotnet restore
4. Run: dotnet run

The game will automatically load all content from the GameData folder and start a battle between Heroes and Enemies teams.

## Project Structure

`
BattleMechanics/
├── GameData/                    # All mod content goes here
│   ├── Characters/              # Character definitions
│   │   ├── knight.yaml
│   │   ├── mage.yaml
│   │   ├── archer.yaml
│   │   ├── orc.yaml
│   │   └── goblin.yaml
│   ├── Teams/                   # Team compositions
│   │   ├── heroes.yaml
│   │   └── enemies.yaml
│   ├── Actions/                 # Ability definitions
│   │   ├── attack.yaml
│   │   ├── defend.yaml
│   │   ├── fireball.yaml
│   │   ├── heal.yaml
│   │   └── special_strike.yaml
│   └── Scripts/                 # Lua behavior scripts
│       ├── attack.lua
│       ├── defend.lua
│       ├── fireball.lua
│       ├── heal.lua
│       ├── special_strike.lua
│       ├── ai_simple.lua
│       └── ai_aggressive.lua
├── Source Code/                 # C# engine files
│   ├── Program.cs
│   ├── Character.cs
│   ├── Team.cs
│   ├── Action.cs
│   ├── CombatManager.cs
│   ├── ScriptEngine.cs
│   └── DataLoader.cs
├── BattleMechanics.csproj        # Project file
└── README.md                    # This file
`

## Game Mechanics

### Combat Flow

1. **Initialization**: All characters start with full HP
2. **Turn Order**: Characters act in descending order of Speed stat
3. **Player Input**: Heroes require player input for actions and targets
4. **AI Actions**: Enemy characters use AI scripts or simple fallback AI
5. **Action Execution**: Lua scripts handle ability effects
6. **Victory Condition**: Last team standing wins

### Character Stats

- **HP (Health Points)**: Character's life force
- **Attack**: Base damage for physical abilities
- **Defense**: Reduces incoming damage
- **Speed**: Determines turn order
- **Position (X,Y)**: Grid coordinates (currently display-only)

## Modding Guide
[...truncated for brevity: use the content as in your prompt above...]
**Created by**: BattleMechanics Development Team  
**Engine Version**: 1.0.0  
**Target Framework**: .NET 9.0  
**Last Updated**: 2025
---
