using demonviglu.bt;
using UnityEngine;
public class DebugLogNode : ActionNode
{
    public string Message;
    protected override void OnStart()
    {
        Debug.Log($"{Message}");
    }

    protected override void OnStop()
    {
    }

    protected override NodeState OnTick()
    {
        return NodeState.Success;
    }
}
