using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using demonviglu.MissonSystem;
public class MissionNodeView : UnityEditor.Experimental.GraphView.Node
{
    public UnityEditor.Experimental.GraphView.Port Input;
    public UnityEditor.Experimental.GraphView.Port Output;

    public Action<MissionNodeView> OnNodeSelected;

    public MissionNode Node;

    public MissionNodeView(MissionNode node)
    {
        Node = node;

        this.title = node.ID.ToString();

        viewDataKey = node.GUID.ToString();

        style.left = node.Position.x;
        style.top = node.Position.y;

        Output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));

        Input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));

        Output.portName = "";
        Input.portName = "";

        inputContainer.Add(Input);
        outputContainer.Add(Output);
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);

        Node.Position.x = newPos.xMin;
        Node.Position.y = newPos.yMin;
    }

    public override void OnSelected()
    {
        base.OnSelected();
        if (OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(this);
        }
    }
}
