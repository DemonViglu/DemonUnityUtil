using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using demonviglu.MissonSystem;
using UnityEngine.UIElements;
using Unity.Mathematics;
using UnityEditor;

public class Enermy : MonoBehaviour
{
    private MissionNodeManager MNM;

    public GameObject PlayerOBJ;

    public bool AutoFollow = false;

    private TimeTick TimeTick = new(0.5f);


    private void Start()
    {
        MNM = new();

        List<MissionNode> list = MNM.DeSerialNode();

        foreach(MissionNode node in list)
        {
            MNM.RegisterMissionNode(node);
        }

        MNM.RefreshLogicNode();
    }

    private void Update()
    { 
        LogicJudge();

        Action();

        if (TimeTick.Tick())
        {
            MNM.SerialNode();
        }
    }

    private void LogicJudge()
    {
        //If Player Near
        if (Vector3.Distance(PlayerOBJ.transform.position, transform.position) < 3)
        {
            if (MNM.GetMissionNode(1).State != MissionState.Success) MNM.MakeProgress(1);
        }
        else MNM.MakeMissionLock(1);

        //If Auto Follow
        if (AutoFollow)
        {
            if (MNM.GetMissionNode(2).State != MissionState.Success) MNM.MakeProgress(2);
        }
        else MNM.MakeMissionLock(2);
    }

    private void Action()
    {
        //If Can Follow
        if(MNM.GetMissionNode(4).State == MissionState.Success)
        {
            Debug.Log("Should Follow");
        }
        //If Should Idle
        if(MNM.GetMissionNode(6).State == MissionState.Success)
        {
            Debug.Log("Idling");
        }
    }
}
class TimeTick
{
    float TimeGap;
    float TimeSet;

    public TimeTick(float time)
    {
        TimeGap = time;
        this.TimeSet = 0;
    }
    public bool Tick()
    {
        if (TimeSet >= TimeGap)
        {
            TimeSet = 0;
            return true;
        }
        TimeSet += Time.deltaTime;
        return false;
    }
}