using MoonSharp.Interpreter;

namespace CombatPrototype;

public class ScriptEngine
{
    private readonly string scriptFolder;

    public ScriptEngine(string scriptFolder)
    {
        this.scriptFolder = scriptFolder;

        // Important line: Register Character class for MoonSharp
        UserData.RegisterType<Character>();
    }

    public void ExecuteAbility(string scriptName, Character user, Character target)
    {
        var script = new Script();

        script.Globals["user"] = UserData.Create(user);
        script.Globals["target"] = UserData.Create(target);

        script.DoFile(Path.Combine(scriptFolder, scriptName));
        script.Call(script.Globals["use_ability"], user, target);
    }
}