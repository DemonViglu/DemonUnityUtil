using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;

namespace demonviglu.GameplayTag
{

    public class DGameplayTagContainer
    {
        public List<DGameplayTag> GameplayTags;

        //As for the unreal engine code. there will be same parenttag in the parentags list.
        //So you can't just return the list without make tag unique.
        List<DGameplayTag> ParentTags;

        public DGameplayTagContainer()
        {
            GameplayTags = new();
            ParentTags = new();
        }

        //If the tag is in the scope
        public bool HasTag(DGameplayTag tag)
        {
            if (!tag.IsValid()) return false;

            return GameplayTags.Contains(tag) || ParentTags.Contains(tag);
        }

        public bool HasTagExact(DGameplayTag tag)
        {
            if (!tag.IsValid()) return false;

            return GameplayTags.Contains(tag);
        }

        public bool IsValid()
        {
            return GameplayTags.Count > 0;
        }

        public DGameplayTagContainer GetGameplayTagParents()
        {

            DGameplayTagContainer ResultContainer = new();
            ResultContainer.GameplayTags = new(GameplayTags);

            // Add parent tags to explicit tags, the rest got copied over already
            foreach (var Tag in ParentTags)
	{
                if(!ResultContainer.GameplayTags.Contains(Tag))ResultContainer.GameplayTags.Add(Tag);
            }

            return ResultContainer;
        }

        public void AddTag(DGameplayTag tag)
        {
            if (tag.IsValid())
            {
                if (!GameplayTags.Contains(tag))
                {
                    GameplayTags.Add(tag);
                }
                DGameplayTagsManager.Get().ExtractParentTags(tag, ParentTags);
            }
        }

        public void AppendTags(DGameplayTagContainer Other)
        {
            if (Other.GameplayTags.Count() == 0)
            {
                return;
            }

            int OldTagNum = GameplayTags.Count();
            // Add other container's tags to our own
            foreach (var OtherTag in Other.GameplayTags)
	{
                int SearchIndex = 0;
                while (true)
                {
                    if (SearchIndex >= OldTagNum)
                    {
                        // Stop searching once we've looked at all existing tags, this is faster when appending large containers
                        GameplayTags.Add(OtherTag);
                        break;
                    }
                    else if (GameplayTags[SearchIndex] == OtherTag)
                    {
                        // Matching tag found, stop searching
                        break;
                    }

                    SearchIndex++;
                }
            }

            OldTagNum = ParentTags.Count();
            foreach (var OtherTag in Other.ParentTags)
	{
                int SearchIndex = 0;
                while (true)
                {
                    if (SearchIndex >= OldTagNum)
                    {
                        ParentTags.Add(OtherTag);
                        break;
                    }
                    else if (ParentTags[SearchIndex] == OtherTag)
                    {
                        break;
                    }

                    SearchIndex++;
                }
            }
        }

        public bool RemoveTag(DGameplayTag TagToRemove, bool bDeferParentTags = false)
        {
            bool NumChanged = GameplayTags.Remove(TagToRemove);

            if (NumChanged)
            {
                if (!bDeferParentTags)
                {
                    // Have to recompute parent table from scratch because there could be duplicates providing the same parent tag
                    FillParentTags();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Fill the list of parenttags
        /// </summary>
        public void FillParentTags()
        {
            ParentTags.Clear();

            if (GameplayTags.Count() > 0)
            {
                var TagManager = DGameplayTagsManager.Get();

                foreach (var Tag in GameplayTags)
		{
                    TagManager.ExtractParentTags(Tag, ParentTags);
                }
            }
        }
    }
}
