using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using demonviglu.bt;
using UnityEditor.Experimental.GraphView;
using System;
public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;
    public demonviglu.bt.Node node;
    public UnityEditor.Experimental.GraphView.Port Input;
    public UnityEditor.Experimental.GraphView.Port Output;
    public NodeView(demonviglu.bt.Node node)
    {
        this.node = node;

        this.title = node.name;

        this.viewDataKey = node.Guid;
        style.left = node.Position.x;
        style.top = node.Position.y;

        CreateInputPort();
        CreateOutputPort();
    }

    private void CreateOutputPort()
    {
        if (node is ActionNode)
        {
            Input = InstantiatePort(UnityEditor.Experimental.GraphView.Orientation.Horizontal, UnityEditor.Experimental.GraphView.Direction.Input, UnityEditor.Experimental.GraphView.Port.Capacity.Single, typeof(bool));
        }
        else if (node is CompositeNode)
        {
            Input = InstantiatePort(UnityEditor.Experimental.GraphView.Orientation.Horizontal, UnityEditor.Experimental.GraphView.Direction.Input, UnityEditor.Experimental.GraphView.Port.Capacity.Single, typeof(bool));

        }
        else if (node is DecoratorNode)
        {
            Input = InstantiatePort(UnityEditor.Experimental.GraphView.Orientation.Horizontal, UnityEditor.Experimental.GraphView.Direction.Input, UnityEditor.Experimental.GraphView.Port.Capacity.Single, typeof(bool));
        }

        if(Input != null)
        {
            Input.portName = "";
            inputContainer.Add(Input);
        }
    }

    private void CreateInputPort()
    {
        if (node is ActionNode)
        {

        }
        else if (node is CompositeNode)
        {
            Output = InstantiatePort(UnityEditor.Experimental.GraphView.Orientation.Horizontal, UnityEditor.Experimental.GraphView.Direction.Output, UnityEditor.Experimental.GraphView.Port.Capacity.Multi, typeof(bool));

        }
        else if (node is DecoratorNode)
        {
            Output = InstantiatePort(UnityEditor.Experimental.GraphView.Orientation.Horizontal, UnityEditor.Experimental.GraphView.Direction.Output, UnityEditor.Experimental.GraphView.Port.Capacity.Single, typeof(bool));

        }
        else if(node is RootNode)
        {
            Output = InstantiatePort(UnityEditor.Experimental.GraphView.Orientation.Horizontal, UnityEditor.Experimental.GraphView.Direction.Output, UnityEditor.Experimental.GraphView.Port.Capacity.Single, typeof(bool));
        }

        if (Output != null)
        {
            Output.portName = "";
            outputContainer.Add(Output);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);

        node.Position.x = newPos.xMin;
        node.Position.y = newPos.yMin;
    }

    public override void OnSelected()
    {
        base.OnSelected();
        if(OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(this);
        }
    }
}
