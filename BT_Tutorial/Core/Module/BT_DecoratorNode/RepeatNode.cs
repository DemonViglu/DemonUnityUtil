using demonviglu.bt;
public class RepeatNode : DecoratorNode
{
    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {

    }

    protected override NodeState OnTick()
    {
        Child.Tick();
        return NodeState.Running;
    }
}
