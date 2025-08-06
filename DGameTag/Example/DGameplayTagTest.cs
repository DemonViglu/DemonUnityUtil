using System.Collections;
using System.Collections.Generic;
using demonviglu.GameplayTag;
using demonviglu.MessageSystem;
using UnityEngine;

public class DGameplayTagTest : MonoBehaviour
{
    private void Start()
    {

        Test_02();
    }

    private void Test_01()
    {
        DGameplayTagsManager.Get();

        if (DGameplayTag.TryGet("B").IsValid())
        {
            Debug.Log("Success");
        }

        if (DGameplayTag.TryGet("A.B").IsValid())
        {
            Debug.Log("Success");
        }

        DGameplayTag tag_01 = DGameplayTag.TryGet("A.B.C");
        DGameplayTag tag_02 = DGameplayTag.TryGet("G.I");

        DGameplayTagContainer dgtc = new();
        dgtc.AddTag(tag_01);
        dgtc.AddTag(tag_02);
        foreach (var node in dgtc.GetGameplayTagParents().GameplayTags)
        {
            Debug.Log(node.GetTagName() + "|");
        }

    }

    private void Test_02()
    {
        DMessageSystem.Register(DGameplayTag.TryGet("Message.Debug"),new DDelegate(Del));
        DMessageSystem.Register(DGameplayTag.TryGet("Message"),new DDelegate(Del));

        DMessageSystem.BroadCast(DGameplayTag.TryGet("Message.Debug"), "HAHAHA");
    }

    public void Del(object para)
    {
        Debug.Log(para as string);
    }
}
