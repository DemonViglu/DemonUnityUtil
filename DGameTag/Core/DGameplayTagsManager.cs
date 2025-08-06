using System;
using System.Collections.Generic;


namespace demonviglu.GameplayTag
{
    public class DGameplayTagsManager : GameSubInstance
    {
        private static DGameplayTagsManager Instance;

        Dictionary<string, DGameplayTagNode> Mp;

        public List<DGameplayTagNode> TagNodes;

        public DGameplayTagsManager()
        {
            Mp = new();
            TagNodes = new();

            RegisterGameplayTag();
        }

        public static DGameplayTagsManager Get()
        {
            return GetInstance(typeof(DGameplayTagsManager)) as DGameplayTagsManager;
        }

        private void RegisterGameplayTag()
        {
            List<string> tags = new()
            {
                "Message.Debug.Log",
                "Message.Debug.CheckMessage"
            };

            string[] constructer;

            string curNodeTagName;
            string lastNodeTagName;

            foreach (string tagName in tags)
            {
                constructer = tagName.Split('.');

                curNodeTagName = string.Empty;
                lastNodeTagName = string.Empty;

                foreach (string partTagName in constructer)
                {

                    if(curNodeTagName == string.Empty)
                    {
                        curNodeTagName = partTagName;
                    }
                    else
                    {
                        curNodeTagName = curNodeTagName + '.' + partTagName;
                    }

                    //UnityEngine.Debug.Log(curNodeTagName+"||"+lastNodeTagName);

                    if (!Mp.ContainsKey(curNodeTagName))
                    {
                        DGameplayTag tag = new(curNodeTagName);
                        DGameplayTagNode node = new(tag);

                        Mp.Add(curNodeTagName, node);
                        TagNodes.Add(node);
                    }
                    else
                    {
                        lastNodeTagName = curNodeTagName;
                        continue;
                    }

                    if (Mp.ContainsKey(lastNodeTagName))
                    {
                        DGameplayTagNode parentNode = Mp[lastNodeTagName];
                        parentNode.ChildrenTagNodes.Add(Mp[curNodeTagName]);
                        Mp[curNodeTagName].ParentTagNode = parentNode;
                    }
                    else
                    {
                        Mp[curNodeTagName].ParentTagNode = new(new());
                    }

                    lastNodeTagName = curNodeTagName;
                }
            }
        }

        public static bool IsValidGameplayTagString(string tagString)
        {
            return (Get() as DGameplayTagsManager).Mp.ContainsKey(tagString);
        }

        internal void ExtractParentTags(DGameplayTag dGameplayTag, List<DGameplayTag> parentTags)
        {
            DGameplayTagNode ParentTagNode = Mp[dGameplayTag.GetTagName()].ParentTagNode;

            while (ParentTagNode.GetTag().IsValid())
            {
                parentTags.Add(ParentTagNode.GetTag());

                ParentTagNode = ParentTagNode.ParentTagNode;
            }

        }

        internal DGameplayTag RequestGameplayTagDirectParent(DGameplayTag dGameplayTag)
        {
            if (dGameplayTag.IsValid())
            {
                return Mp[dGameplayTag.GetTagName()].ParentTagNode.GetTag();
            }
            return dGameplayTag;
        }

        internal DGameplayTagContainer RequestGameplayTagParents(DGameplayTag dGameplayTag)
        {
            DGameplayTagContainer dGameplayTagContainer = new();

            DGameplayTagNode ParentTagNode = Mp[dGameplayTag.GetTagName()];

            while (ParentTagNode.GetTag().IsValid())
            {
                dGameplayTagContainer.AddTag(ParentTagNode.GetTag());

                ParentTagNode = ParentTagNode.ParentTagNode;
            }

            return dGameplayTagContainer;
        }
    }
}