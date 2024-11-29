using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace demonviglu.bt
{
    [CreateAssetMenu(fileName = "BT", menuName = "Demon/BT")]
    public class BehaviorTree : ScriptableObject
    {
        public Node RootNode;
        public NodeState treeState = NodeState.Running;
        public List<Node> nodes = new List<Node>();

        public NodeState Tick()
        {
            if(RootNode.State == NodeState.Running)
            {
                treeState = RootNode.Tick();
            }
            return treeState;
        }

        public Node CreateNode(System.Type type)
        {
            Node node =  ScriptableObject.CreateInstance(type) as Node;
            node.name  = type.Name;
            node.Guid = GUID.Generate().ToString();
            nodes.Add(node);

            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(Node node)
        {
            nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent,Node child)
        {
            DecoratorNode decorator = parent as DecoratorNode;
            if(decorator != null)
            {
                decorator.Child = child;
            }

            CompositeNode composite = parent as CompositeNode;
            if(composite != null)
            {
                composite.Childrens.Add(child);
            }

            RootNode root = parent as RootNode;
            if (root != null)
            {
                root.Child = (child);
            }
        }

        public void RemoveChild(Node parent,Node child)
        {
            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator != null)
            {
                decorator.Child = null;
            }

            CompositeNode composite = parent as CompositeNode;
            if (composite != null)
            {
                composite.Childrens.Remove(child);
            }

            RootNode root = parent as RootNode;
            if (root != null)
            {
                root.Child = null;
            }
        }

        public List<Node> GetChildren(Node parent)
        {
            if (parent is DecoratorNode && (parent as DecoratorNode).Child !=null)
            {
                return new List<Node> { (parent as DecoratorNode).Child };
            }

            else if(parent is CompositeNode)
            {
                return (parent as CompositeNode).Childrens;
            }

            else if(parent is RootNode && (parent as RootNode).Child != null)
            {
                return new List<Node> { (parent as RootNode).Child };
            }

            else
            {
                return new();
            }
        }
    }

}