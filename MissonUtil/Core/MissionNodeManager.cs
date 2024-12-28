using demonviglu.config;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace demonviglu.MissonSystem
{
    public class MissionNodeManager
    {
        public delegate void MissionEventDelegate(MissionNode node);

        private Dictionary<int, MissionNode> Dic = new();

        private Dictionary<int, List<int>> ParentDic = new();

        private Dictionary<int, List<MissionEventDelegate>> EventDic = new();

        private List<MissionEventDelegate> EventList = new();

        public bool RegisterMissionNode(MissionNode node)
        {
            if (Dic.ContainsKey(node.ID))
            {
                return false;
            }
            else
            {
                Dic.Add(node.ID, node);

                node.missionManager = this;

                foreach (var childId in node.Childrens)
                {
                    if (ParentDic.ContainsKey(childId))
                    {
                        ParentDic[childId].Add(node.ID);
                    }
                    else
                    {
                        ParentDic[childId] = new List<int>() { node.ID };
                    }
                }
                return true;
            }
        }

        public void UnRegisterMissionNode(MissionNode node)
        {
            Dic.Remove(node.ID);
        }

        public void UnLinkMissionNode(MissionNode parent, MissionNode child)
        {
            parent.Childrens.Remove(child.ID);
        }

        public void LinkMissionNode(MissionNode parent, MissionNode child)
        {
            Dic[parent.ID].Childrens.Add(child.ID);
        }

        public void RegisterEvent(MissionEventDelegate med, int requestID)
        {
            if (EventDic.ContainsKey(requestID))
            {
                EventDic[requestID].Add(med);
            }
            else
            {
                EventDic[requestID] = new() { med };
            }
        }

        public void RegisterEvent(MissionEventDelegate med)
        {
            EventList.Add(med);
        }

        public static string Failure = "DEMONFAILURE";
        public MissionNode GetMissionNode(int id)
        {
            if (Dic.ContainsKey(id))
            {
                return Dic[id];
            }
            return new()
            {
                GUID = Failure
            };
        }

        public bool MakeProgress(int id)
        {
            MissionNode node = GetMissionNode(id);
            if (node != null)
            {
                switch (node.State)
                {
                    case MissionState.Locked:

                        if (ParentDic.ContainsKey(id) && ParentDic[id].Count > 0)
                        {
                            string tmp = string.Empty;
                            foreach (var sId in ParentDic[id])
                            {
                                tmp += sId + ",";
                            }
                            Debug.Log(tmp + "\t To be finished");

                            return false;
                        }
                        else
                        {
                            node.State = MissionState.Running;
                        }
                        break;
                    case MissionState.Running:

                        node.State = MissionState.Success;
                        foreach (var childId in node.Childrens)
                        {
                            if (ParentDic.ContainsKey(childId))
                            {
                                ParentDic[childId].Remove(node.ID);

                                if (GetMissionNode(childId).AutoUnlock == true && ParentDic[childId].Count == 0)
                                {
                                    MakeProgress(childId);
                                }

                            }
                        }
                        break;
                    case MissionState.Success:
                        node.State = MissionState.Hide;
                        break;
                    case MissionState.Failure:
                        Debug.Log($"{id} Has been failed");
                        return false;
                    case MissionState.Hide:
                        break;
                }
                foreach (var e in EventList)
                {
                    e.Invoke(node);
                }
                return true;
            }
            else
            {
                Debug.Log($"{id} Not Found");
            }
            return false;
        }

        public bool MakeFailure(int id)
        {
            MissionNode node = GetMissionNode(id);
            if (node != null)
            {
                if (node.State == MissionState.Running)
                {
                    node.State = MissionState.Failure;
                    return true;
                }
            }
            return false;
        }

        public void CallEvent(int id, MissionNode node)
        {
            if (EventDic.ContainsKey(id))
            {
                foreach (var e in EventDic[id])
                {
                    e.Invoke(node);
                }
            }
        }


        public void DebugNode()
        {
            foreach (var kv in Dic)
            {
                Debug.Log(kv.Value.ToString());
            }
        }

        public void Reinit()
        {
            foreach(var node in Dic.Values)
            {
                node.State = MissionState.Locked;
            }
        }

        public string SerialNode()
        {
            string ret = string.Empty;
            foreach (var kv in Dic)
            {
                ret += (kv.Value.ToString()) + '\n';
            }

            MissionNodeConfig config = new() { Data = ret };
            config.SaveStr(config.Data);

            return ret;
        }

        public List<MissionNode> DeSerialNode()
        {
            List<MissionNode> ret = new();
            string s = string.Empty;

            MissionNodeConfig config = new() { };
            s = MissionNodeConfig.LoadStr<MissionNodeConfig>();
            List<string> list = s.Split('\n').ToList();
            foreach (var item in list)
            {
                MissionNode node = new();
                if (node.DeSerial(item))
                {
                    RegisterMissionNode(node);
                    ret.Add(node);
                }
            }

            return ret;
        }
    }

    public class MissionNodeConfig : BaseConfig
    {
        public string Data
        {
            get; set;
        }
    }
}