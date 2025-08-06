using System;
using System.Collections.Generic;
using demonviglu.GameplayTag;

namespace demonviglu.MessageSystem
{
    public class DMessageSystem : GameSubInstance
    {
        public static DMessageSystem Get()
        {
            return GetInstance(typeof(DMessageSystem)) as DMessageSystem;
        }

        public DMessageSystem()
        {
            Events = new();
        }

        private Dictionary<DGameplayTag, List<DDelegate>> Events;

        public static void BroadCast(DGameplayTag tag, object para)
        {
            while (tag.IsValid()) 
            {
                if (Get().Events.TryGetValue(tag, out List<DDelegate> list))
                {
                    foreach (DDelegate d in list)
                    {
                        d.Invoke(para);
                    }
                }

                tag = DGameplayTagsManager.Get().RequestGameplayTagDirectParent(tag);
            }
        }

        public static void Register(DGameplayTag tag, DDelegate d)
        {
            if (tag.IsValid())
            {
                if(!Get().Events.TryGetValue(tag,out List<DDelegate> list))
                {
                    list = new List<DDelegate>() { d };
                    Get().Events.Add(tag, list);
                }
                else
                {
                    list.Add(d);
                }
            }
        }
    }

    public delegate void DDelegate(object para);
}
