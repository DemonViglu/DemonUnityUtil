using System.Collections.Generic;
using UnityEngine;
using demonviglu.MissonSystem;
using UnityEngine.UI;

public class MissionNetFactory : MonoBehaviour
{
    public int ID;

    public MissionNodeManager MissionNodeManager = new();

    public GameObject MissionUI;

    public GameObject Panel;

    List<MissionNode> missionNodes;

    Dictionary<int,Button> gameObjects = new();
    private void Start()
    {
        BuildMissionNode();

        BuildUI();
    }
    private void BuildMissionNode()
    {

        missionNodes = MissionNodeManager.DeSerialNode();

        foreach(var node in missionNodes)
        {
            MissionNodeManager.RegisterMissionNode(node);
        }

        MissionNodeManager.RefreshLogicNode();

        MissionNodeManager.RegisterEvent(BeCalledFunc, 520);
        MissionNodeManager.RegisterEvent(RefreshUIColor);
    }
    public void BeCalledFunc(MissionNode node)
    {
        Debug.Log("The register Mission Has Been Called!");
    }
    private void BuildUI()
    {
        foreach(var node in missionNodes)
        {
            GameObject obj = Instantiate(MissionUI, Panel.transform);
            Button btn = obj.GetComponent<Button>();
            int id = node.ID;
            obj.GetComponentInChildren<Text>().text = id.ToString()+":"+node.State;
            btn.onClick.AddListener(() =>
            {
                ID = (int.Parse(btn.gameObject.GetComponentInChildren<Text>().text.Split(":")[0]));
                MakeProgress();
            });
            gameObjects.Add(id,btn);
        }
    }

    private void RefreshUIColor(MissionNode node)
    {
        Button obj = gameObjects[node.ID];

        obj.gameObject.GetComponentInChildren<Text>().text = node.ID.ToString() + ":" + node.State;

        ColorBlock cb = new ColorBlock();

        switch (node.State)
        {
            case MissionState.Locked:
                break;
            case MissionState.Running:
                cb.normalColor = Color.blue;
                obj.colors = cb;
                break;
            case MissionState.Success:
                cb.normalColor = Color.green;
                break;
            case MissionState.Failure:
                break;
            case MissionState.Hide:
                cb.normalColor = Color.gray;
                break;
        }

        MissionNodeManager.SerialNode();
    }

    [ContextMenu("MakeProgress")]
    public void MakeProgress()
    {
        if (MissionNodeManager.MakeProgress(ID))
        {
            MissionNodeManager.SerialNode();
        }
    }

    [ContextMenu("MakeFailure")]
    public void MakeFailure()
    {
        MissionNodeManager.MakeMissionLock(ID);
    }


}
