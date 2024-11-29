using UnityEngine;
using demonviglu.bt;
public class WaitNode : ActionNode
{
    public float WaitTime = 0f;
    private float startTime;
    protected override void OnStart()
    {
        startTime = Time.time;
    }

    protected override void OnStop()
    {
        
    }

    protected override NodeState OnTick()
    {
        if(Time.time - startTime< WaitTime)
        {
            return NodeState.Running;
        }
        else
        {
            return NodeState.Success;
        }
    }

}
