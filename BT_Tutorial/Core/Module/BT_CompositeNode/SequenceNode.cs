using UnityEngine;
using demonviglu.bt;
public class SequenceNode : CompositeNode
{
    int CurrentChildIndex;
    protected override void OnStart()
    {
        CurrentChildIndex = 0;
    }

    protected override void OnStop()
    {

    }

    protected override NodeState OnTick()
    {
        var child = Childrens[CurrentChildIndex];
        switch (child.Tick())
        {
            case NodeState.Running:
                return NodeState.Running;
            case NodeState.Failure:
                return NodeState.Failure;
            case NodeState.Success:
                CurrentChildIndex++;
                break;
        }

        if(CurrentChildIndex == Childrens.Count)
        {
            return NodeState.Success;
        }
        else
        {
            return NodeState.Running;
        }
    }
}
