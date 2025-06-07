This is the prompt used:

You are a game developer. This is a project that will help you get a job if it impresses the client. Your job is to create all the files for a fully working prototype of this project. Each file is important, the design and architecture of the project is important, the code clarity is important. A lot is on the line, it's important to impress and not leave anything unfinished.

I am here to copy and paste each file for you into a .Net 9 console project.
---

# **Project Brief: BattleMechanic** 

## **Project Summary**

Create a **prototype combat engine** for a modular, text-based RPG in C#, designed for future extensibility and modding.
The system must support **data-driven game objects via YAML files** and allow **modders to define behavior/logic using Lua scripts**.
This is a backend/logic prototype—no graphics or UI are needed beyond text output. The architecture must be clean, modular, and prepared for possible future support of additional serialization formats or scripting languages. The engine prototype should work as a mini game by itself. 

---

## **Goals**

* **Flexible combat loop:** Not hardcoded for “heroes vs enemies”; any number of teams and characters per battle.
* **Moddable:** All content (characters, teams, actions, etc.) defined as files, not hardcoded.
* **Scriptable:** Game logic and custom abilities/AI are defined using Lua scripts, loaded at runtime.
* **Spatial awareness:** Characters exist on a 2D grid (X,Y) even if this does not influence battle mechanics yet.
* **Extendable:** Structure and document code to allow additional serialization formats (e.g., JSON, TOML) and other scripting engines in the future.
* **Usable by non-coders:** Modders should be able to add or edit YAML/Lua files to create new content and behaviors.

---

## **Minimum Technical Requirements**

* **Language:** C#, .NET 7+ (console app)
* **Serialization:** YamlDotNet for YAML data loading
* **Scripting:** MoonSharp for Lua scripting (must be pure .NET, no native dependencies)
* **Text-based interaction:** Console output for status, actions, and events

---

## **Core Concepts & Data Model**

### **Characters**

* Defined by YAML file, one file per character.
* Key properties: `name`, `team`, `max_hp`, `attack`, `defense`, `speed`, `x`, `y`, `abilities` (list), `ai_script` (optional)
* YAML Example:

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
    - defend
    - special_strike
  ai_script: ai_simple.lua
  ```

### **Teams**

* Defined by YAML, one per team.
* Properties: `name`, `members` (list of character names or file references)
* Teams are referenced by `team` property in Character files for linking.

### **Actions/Abilities**

* Each action/ability is a YAML file.
* Key properties: `name`, `description`, `script` (Lua script filename), plus other fields as needed.
* Example:

  ```yaml
  name: Fireball
  description: "Hurls a fireball at the target"
  script: fireball.lua
  ```

### **Lua Scripts**

* Lua scripts reside in a `/Scripts/` directory.
* Used for abilities, AI, or any behavior that is not just basic damage.
* Example:

  ```lua
  -- fireball.lua
  function use_ability(user, target)
    local dmg = 12
    target:take_damage(dmg)
    print(user.name .. " hurls a fireball at " .. target.name .. " for " .. dmg .. " damage!")
  end
  ```

---

## **System Design & Requirements**

### **1. Data Loading**

* At startup, recursively load all YAML files from subfolders of `/GameData/` into in-memory objects.

  * `/Characters/`, `/Teams/`, `/Actions/`, `/Scripts/`
* Validate loaded data for missing references, incomplete fields, or errors.
* Provide clear console error messages if a data file is invalid or incomplete.

### **2. Object Model**

* Core C# classes: `Character`, `Team`, `Action` (or `Ability`), `Battlefield` (or similar for grid), `CombatManager`.
* Each object maps directly to its YAML structure.
* `Character` includes grid position (`x`, `y`), and a list of abilities by name or file reference.
* Keep `Team` simple for now, just a collection of characters.

### **3. Scripting**

* Integrate MoonSharp for Lua scripting.
* At runtime, load Lua scripts from `/GameData/Scripts/`.
* When an action or ability with a script is triggered, execute the script in the context of the relevant `user` (acting character) and `target` (targeted character), passing them as Lua-accessible objects.
* Provide an API in C# for Lua scripts to:

  * Get/set character properties (name, HP, position, etc.)
  * Call methods like `take_damage`, `heal`, etc.
  * Print messages to the console.
* **Example:**
  Calling the `use_ability(user, target)` Lua function from C# when an ability is triggered.

### **4. Turn Order & Combat Loop**

* Each battle loads a set of teams/characters from files.
* Initiative order for the round is determined by the `speed` property of each character.
* Characters take turns individually, based on descending speed.
* On their turn, a character selects an action (basic AI: always attack, or use their AI script if provided).
* The action is executed (either native or scripted).
* Print detailed status to the console after each turn.
* Battle continues until only one team has living characters.

### **5. Spatial Model**

* Each character has an `x` and `y` coordinate.
* These values are loaded, saved, and displayed, but (for now) do not affect available actions or turn logic.

### **6. Console Output**

* On each turn, print:

  * Turn number and acting character.
  * What action is chosen and its result.
  * HP/status of all characters (including positions).
* Print a message when a character is defeated.
* Print a summary and winning team at the end.

### **7. Moddability and Extensibility**

* All content and scripts must be loadable from files; **no hardcoded data**.
* Code must be written so new formats (JSON, TOML), new script engines (C#, JS), and new object types can be added with minimal refactoring.
* Use interfaces or abstract classes for pluggable data/script providers.
* Document the structure of YAML files, the loader mechanism, and how scripts interact with the C# API.

### **8. Error Handling**

* Fail gracefully and print errors if:

  * Data files are missing, malformed, or incomplete.
  * Scripts fail to load or execute.
  * Modder makes a syntax error in YAML or Lua.
* Where possible, explain the error and suggest how to fix it (for example, “Missing ‘max\_hp’ in knight.yaml”).

### **9. Project Structure Example**

```
/GameData/
    /Characters/
        knight.yaml
        mage.yaml
    /Teams/
        red_team.yaml
    /Actions/
        attack.yaml
        fireball.yaml
    /Scripts/
        fireball.lua
        ai_simple.lua
/CombatPrototype/
    Program.cs
    Character.cs
    Team.cs
    Action.cs
    CombatManager.cs
    ScriptEngine.cs
    (other engine/core files)
    README.md (docs for modders)
```

---

## **Deliverables**

* Working C# solution (.NET 9), runs as a console app. 
* Loads and validates content from `/GameData/` folders.
* Demonstrates a battle with at least two teams, several characters, and at least one ability scripted in Lua.
* Prints clear output of combat state and results.
* Includes sample YAML data and Lua scripts for easy modding.
* Clean, modular code with comments and simple docs for how to add new content.
* **README.md** explaining:

  * How the system works
  * Folder/file structure
  * How to add new characters, teams, abilities, or scripts
  * API available to Lua scripts
  * How to run/extend the system

---

## **Stretch Goals (if time allows, not required)**

* Allow team/character/ability files to specify custom fields for modders (e.g., flavor text).
* Simple status effect support (e.g., “poisoned”).
* Hot-reload data/scripts without restarting app.
* Provide a way to override battle win conditions from YAML or script.

---

## **Summary**

Build a **clean, modular, and data-driven RPG combat engine in C#**:

* **All content from YAML.**
* **Behavior/logic in Lua scripts (via MoonSharp).**
* **Grid positions for characters (X,Y).**
* **Console-based, no UI needed.**
* **Easy to extend for future formats and scripting engines.**
* **One 'Team' should use user input and it should be testable that different choices yield different results** 
* **Include a readme.md file for the project.** 

*The codebase must be easy for another developer or a modder to understand, extend, and maintain. The first priority is clarity, modularity, and safe handling of external data/scripts.*
---------
This is your chance to be great, let's impress the clients with a project that will make them awe your powers as a game developer. 
