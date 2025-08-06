using System;
using System.Collections.Generic;
using UnityEditor.Search;

namespace demonviglu.GameplayTag {

    public class DGameplayTag
    {
        private string TagName;

        private string InvalidTagName = "INVALID_TAG";

        public DGameplayTag()
        {
            TagName = InvalidTagName;
        }

        /// <summary>
        /// This is only for manager or itself
        /// </summary>
        /// <param name="tagName"></param>
        public DGameplayTag(string tagName)
        {
            TagName = (tagName == string.Empty )?InvalidTagName:tagName;
        }

        public static DGameplayTag TryGet(string tagName)
        {
            if (DGameplayTagsManager.IsValidGameplayTagString(tagName))
            {
                return new DGameplayTag(tagName);
            }
            else
            {
                return new();
            }
        }
        public bool IsValid()
        {
            return (TagName != InvalidTagName);
        }

        public bool MatchesTagExact(DGameplayTag tag)
        {
            if (!tag.IsValid()) return false;
            return tag.TagName == TagName;
        }

        public DGameplayTag RequestDirectParent()
        {
            return DGameplayTagsManager.Get().RequestGameplayTagDirectParent(this);
        }

        public DGameplayTagContainer GetGameplayTagParents()
        {
            return DGameplayTagsManager.Get().RequestGameplayTagParents(this);
        }

        public string GetLeafName()
        {
            if (TagName == InvalidTagName) return TagName;

            string[] tmp = TagName.Split('.');

            if (tmp.Length == 0)
            {
                return TagName;
            }
            else return tmp[^1];

        }

        public string GetTagName()
        {
            return TagName;
        }

        public override string ToString()
        {
            return TagName;
        }

        public override bool Equals(object obj)
        {
            return TagName == ((DGameplayTag)obj).TagName;
        }

        public override int GetHashCode()
        {
            return TagName.ToLowerInvariant().GetHashCode();
        }
    }
}