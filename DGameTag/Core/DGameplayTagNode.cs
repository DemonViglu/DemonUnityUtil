using System.Collections.Generic;

namespace demonviglu.GameplayTag
{
    public class DGameplayTagNode
    {
        private string FullTagName;

        private string LeafTagName;

        private DGameplayTag Tag;

        public List<DGameplayTagNode> ChildrenTagNodes;

        public DGameplayTagNode ParentTagNode;

        public DGameplayTagNode(DGameplayTag tag)
        {
            Tag = tag;
            ChildrenTagNodes = new();
            FullTagName = tag.GetTagName();
            LeafTagName = tag.GetLeafName();
        }

        public DGameplayTag GetTag()
        {
            return Tag;
        }

        public string Debug()
        {
            string s = string.Empty;
            s = $"Tag Name : {Tag.GetTagName()}\n";
            foreach (var child in ChildrenTagNodes)
            {
                s += $"Has Child : {child.GetTag().GetTagName()}\n";
            }
            s += $"Parent : {ParentTagNode.Tag.GetTagName()}\n";
            return s;
        }
    }
}