using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using demonviglu.bt;
public class RootNode : Node
{
    public Node Child;
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {

    }

    protected override NodeState OnTick()
    {
        if(Child != null )
        {
            return Child.Tick();
        }
        else
        {
            return NodeState.Failure;
        }
    }
}
