using demonviglu.MissonSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class MissionFactoryView : GraphView
{
    public new class UxmlFactory : UxmlFactory<MissionFactoryView, GraphView.UxmlTraits> { }

    List<MissionNode> missionNodes = new();
    MissionNodeManager MissionNodeManager = new();

    public Action<MissionNodeView> OnNodeSelectd;
    public MissionFactoryView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets\\DemonUtil\\MissonUtil\\Editor\\DemonMissionEditor.uss");

        if (styleSheet != null)
        {
            styleSheets.Add(styleSheet);
        }
    }

    public void PopulateView()
    {
        missionNodes = MissionNodeManager.DeSerialNode();

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        missionNodes.ForEach(n =>
        {
            CreateNodeView(n);
            MissionNodeManager.RegisterMissionNode(n);
        });

        missionNodes.ForEach(n =>
        {
            MissionNodeView parentView = FindNodeView(n);
            foreach (var childID in n.Childrens)
            {
                MissionNode node = MissionNodeManager.GetMissionNode(childID);

                if (node.GUID != MissionNodeManager.Failure)
                {
                    MissionNodeView childView = FindNodeView(node);

                    AddElement(parentView.Output.ConnectTo(childView.Input));
                }
            }
        });
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(e =>
            {
                MissionNodeView node = e as MissionNodeView;
                if (node != null)
                {
                    MissionNodeManager.UnRegisterMissionNode(node.Node);
                }

                Edge edge = e as Edge;
                if (edge != null)
                {
                    MissionNodeView parentView = edge.output.node as MissionNodeView;
                    MissionNodeView childView = edge.input.node as MissionNodeView;
                    MissionNodeManager.UnLinkMissionNode(parentView.Node, childView.Node);
                }
            });
        }
        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(e =>
            {
                MissionNodeView parentView = e.output.node as MissionNodeView;
                MissionNodeView childView = e.input.node as MissionNodeView;
                MissionNodeManager.LinkMissionNode(parentView.Node, childView.Node);
            });
        }

        MissionNodeManager.SerialNode();

        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction($"Create a Node", (a) => CreateNode());
        evt.menu.AppendAction($"Save&Refresh", (a) => MissionNodeManager.SerialNode());
        evt.menu.AppendAction($"ClearState", (a) => MissionNodeManager.Reinit());
    }

    private void CreateNodeView(MissionNode node)
    {
        node.GUID = GUID.Generate().ToString();
        MissionNodeView nv = new(node);
        nv.OnNodeSelected = OnNodeSelectd;
        AddElement(nv);
    }

    MissionNodeView FindNodeView(MissionNode node)
    {
        return GetNodeByGuid(node.GUID) as MissionNodeView;
    }

    void CreateNode()
    {
        var node = new MissionNode();
        CreateNodeView(node);
        MissionNodeManager.RegisterMissionNode(node);
        MissionNodeManager.SerialNode();
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction && endPort.node != startPort.node
        ).ToList();
    }
}
