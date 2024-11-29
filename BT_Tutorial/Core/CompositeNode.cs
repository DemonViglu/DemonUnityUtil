using System.Collections.Generic;
using UnityEngine;
namespace demonviglu.bt
{
    public abstract class CompositeNode : Node
    {
        public List<Node> Childrens = new List<Node>();
    }
}