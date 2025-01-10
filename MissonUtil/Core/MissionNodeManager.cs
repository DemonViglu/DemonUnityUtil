using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace demonviglu.MissonSystem
{
    public class MissionNodeManager
    {
        public delegate void MissionEventDelegate(MissionNode node);

        //ID -> MissionNode
        private Dictionary<int, MissionNode> Dic = new();

        //The Mission's Node is been Locked because:
        private Dictionary<int, List<int>> LockedParentDic = new();

        //The Mission's whole parent nodes;
        private Dictionary<int, List<int>> ParentDic = new();

        private Dictionary<int, List<MissionEventDelegate>> EventDic = new();

        private List<MissionEventDelegate> EventList = new();

        private List<MissionNode> LogicNode = new();

        private MissionNodeConfig ConfigTool;

        public MissionNodeManager(string ConfigName = "ActionConfig")
        {
            ConfigTool = new(ConfigName);
        }

        #region For init and graphview
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

                if (node.MissionType != MissionType.Normal)
                {
                    //All State should re cal
                    node.State = MissionState.Locked;

                    LogicNode.Add(node);
                }
                else if (node.State != MissionState.Success && node.State != MissionState.Hide)
                {
                    foreach (var childId in node.Childrens)
                    {
                        if (LockedParentDic.ContainsKey(childId))
                        {
                            LockedParentDic[childId].Add(node.ID);
                        }
                        else
                        {
                            LockedParentDic[childId] = new List<int>() { node.ID };
                        }
                    }
                }

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
        #endregion

        public static string Failure = "DEMONFAILURE";

        /// <summary>
        /// If Get the GUID == DEMONFAILURE, means no such mission!
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MissionNode GetMissionNode(int id)
        {
            if (Dic.ContainsKey(id))
            {
                return Dic[id];
            }

            MissionNode ret = ScriptableObject.CreateInstance<MissionNode>();
            ret.GUID = Failure;
            return ret;
        }

        /// <summary>
        /// The function should be called after all the mission node has been registered
        /// </summary>
        public void RefreshLogicNode()
        {
            bool ret = false;

            do
            {
                ret = false;

                foreach (var node in LogicNode)
                {
                    switch (node.MissionType)
                    {
                        case MissionType.And:
                            ret = MakeAndProgress(node.ID, node, true, ret);
                            break;
                        case MissionType.Not:
                            ret = MakeNotProgress(node.ID, node, true, ret);
                            break;
                        case MissionType.Or:
                            ret = MakeOrProgress(node.ID, node, true, ret);
                            break;
                    }
                }
            }
            while (ret);

        }

        #region Make Progress;
        public bool MakeProgress(int id, bool fromParent = false)
        {
            bool ret = false;
            MissionNode node = GetMissionNode(id);
            if (node != null)
            {

                switch (node.MissionType)
                {
                    case MissionType.Normal:
                        ret = MakeNormalProgress(id, node, fromParent, ret);
                        break;
                    case MissionType.And:
                        ret = MakeAndProgress(id, node, fromParent, ret);
                        break;
                    case MissionType.Not:
                        ret = MakeNotProgress(id, node, fromParent, ret);
                        break;
                    case MissionType.Or:
                        ret = MakeOrProgress(id, node, fromParent, ret);
                        break;
                    case MissionType.Locked:
                        if (!LockedParentDic.ContainsKey(id) || LockedParentDic[id].Count == 0)
                        {
                            foreach (var childID in node.Childrens)
                            {
                                MakeMissionLock(childID);
                            }
                        }
                        break;
                }

                //��Ҫ�Ż���������ʲôʱ�򴥷�ȫ�庯��
                if (true)
                {
                    foreach (var e in EventList)
                    {
                        e.Invoke(node);
                    }
                }
            }
            else
            {
                Debug.Log($"{id} Not Found");
                ret = false;
            }
            return ret;
        }

        public bool MakeNormalProgress(int id, MissionNode node, bool fromParent, bool ret)
        {
            MissionState nextState = node.State;
            if (fromParent)
            {
                if (LockedParentDic.ContainsKey(id) && LockedParentDic[id].Count > 0)
                {
                    nextState = MissionState.Locked;
                }
                else
                {
                    if (node.AutoUnlock && node.State < MissionState.Running)
                    {
                        nextState = MissionState.Running;
                    }
                    if (node.AutoSuccess && node.State < MissionState.Success)
                    {
                        nextState = MissionState.Success;
                    }
                }
            }
            else
            {
                switch (node.State)
                {
                    case MissionState.Locked:

                        if (LockedParentDic.ContainsKey(id) && LockedParentDic[id].Count > 0)
                        {
                            string tmp = string.Empty;
                            foreach (var sId in LockedParentDic[id])
                            {
                                tmp += sId + ",";
                            }
                            Debug.Log(tmp + "\t To be finished");
                            break;
                        }
                        else
                        {
                            nextState = MissionState.Running;

                            if (node.AutoSuccess) nextState = MissionState.Success;

                            break;
                        }
                    case MissionState.Running:
                        nextState = MissionState.Success;
                        break;
                    case MissionState.Success:
                        nextState = MissionState.Hide;
                        break;
                    case MissionState.Failure:
                        Debug.Log($"{id} Has been failed");
                        break;
                    case MissionState.Hide:
                        break;
                }
            }

            //When node state has changed, try to call the childrenNodde, especailly the logic node
            //If the node state is success, unlock the children node;
            //If the node state is locked, lock the chlidren
            if (nextState != node.State)
            {
                node.State = nextState;

                if (nextState != MissionState.Locked)
                {
                }

                ret = true;


                foreach (var childId in node.Childrens)
                {

                    if (nextState == MissionState.Success && LockedParentDic.ContainsKey(childId))
                    {
                        LockedParentDic[childId].Remove(node.ID);
                    }
                    else if (nextState == MissionState.Locked)
                    {
                        if (LockedParentDic.ContainsKey(childId))
                        {
                            LockedParentDic[childId].Add(node.ID);
                        }
                        else
                        {
                            LockedParentDic[childId] = new List<int>() { node.ID };
                        }
                    }

                    MakeProgress(childId, true);
                }
            }
            else
            {
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// Only parent can call the logic node
        /// </summary>
        /// <param name="id"></param>
        /// <param name="node"></param>
        /// <param name="fromParent"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private bool MakeAndProgress(int id, MissionNode node, bool fromParent, bool ret)
        {
            MissionState nextState = node.State;
            if (fromParent)
            {
                if (LockedParentDic.ContainsKey(id) && LockedParentDic[id].Count > 0)
                {
                    nextState = MissionState.Failure;
                }
                else
                {
                    nextState = MissionState.Success;
                }

                //Logic state has changed, so change the children
                if (nextState != node.State)
                {
                    node.State = nextState;

                    ret = true;

                    if (nextState == MissionState.Success)
                    {
                        foreach (var childId in node.Childrens)
                        {
                            if (LockedParentDic.ContainsKey(childId))
                            {
                                LockedParentDic[childId].Remove(node.ID);
                            }
                            MakeProgress(childId, true);
                        }
                    }
                    else if (nextState == MissionState.Failure)
                    {
                        foreach (var childId in node.Childrens)
                        {
                            if (LockedParentDic.ContainsKey(childId))
                            {
                                LockedParentDic[childId].Add(node.ID);
                            }
                            else
                            {
                                LockedParentDic[childId] = new List<int>() { node.ID };
                            }
                            MakeProgress(childId, true);
                        }
                    }
                }
            }
            return ret;
        }

        private bool MakeOrProgress(int id, MissionNode node, bool fromParent, bool ret)
        {
            MissionState nextState = node.State;
            if (fromParent)
            {
                //Has at least one node has been success
                if (LockedParentDic.ContainsKey(id) && LockedParentDic[id].Count == ParentDic[id].Count)
                {
                    nextState = MissionState.Failure;
                }
                else
                {
                    nextState = MissionState.Success;
                }

                if (nextState != node.State)
                {
                    node.State = nextState;

                    ret = true;

                    if (nextState == MissionState.Success)
                    {
                        foreach (var childId in node.Childrens)
                        {
                            if (LockedParentDic.ContainsKey(childId))
                            {
                                LockedParentDic[childId].Remove(node.ID);
                            }
                            MakeProgress(childId, true);
                        }
                    }
                    else if (nextState == MissionState.Failure)
                    {
                        foreach (var childId in node.Childrens)
                        {
                            if (LockedParentDic.ContainsKey(childId))
                            {
                                LockedParentDic[childId].Add(node.ID);
                            }
                            else
                            {
                                LockedParentDic[childId] = new List<int>() { node.ID };
                            }
                            MakeProgress(childId, true);
                        }
                    }
                }
            }
            return ret;
        }

        private bool MakeNotProgress(int id, MissionNode node, bool fromParent, bool ret)
        {
            MissionState nextState = node.State;
            if (fromParent)
            {
                //Has at least one node has not been success
                if (LockedParentDic.ContainsKey(id) && LockedParentDic[id].Count > 0)
                {
                    nextState = MissionState.Success;
                }
                else
                {
                    nextState = MissionState.Failure;
                }

                if (nextState != node.State)
                {
                    node.State = nextState;

                    ret = true;

                    if (nextState == MissionState.Success)
                    {
                        foreach (var childId in node.Childrens)
                        {
                            if (LockedParentDic.ContainsKey(childId))
                            {
                                LockedParentDic[childId].Remove(node.ID);
                            }
                            MakeProgress(childId, true);
                        }
                    }
                    else if (nextState == MissionState.Failure)
                    {
                        foreach (var childId in node.Childrens)
                        {
                            if (LockedParentDic.ContainsKey(childId))
                            {
                                LockedParentDic[childId].Add(node.ID);
                            }
                            else
                            {
                                LockedParentDic[childId] = new List<int>() { node.ID };
                            }
                            MakeProgress(childId, true);
                        }
                    }
                }
            }
            return ret;
        }

        public bool MakeMissionLock(int id)
        {
            bool ret = false;
            MissionNode node = GetMissionNode(id);

            MissionState nextState;

            if (node != null)
            {
                if (node.State != MissionState.Locked)
                {
                    nextState = MissionState.Locked;

                    if (node.State == MissionState.Success || node.State == MissionState.Hide)
                    {
                        foreach (var childId in node.Childrens)
                        {
                            if (LockedParentDic.ContainsKey(childId))
                            {
                                LockedParentDic[childId].Add(node.ID);
                            }
                            else
                            {
                                LockedParentDic[childId] = new List<int>() { node.ID };
                            }
                            MakeProgress(childId, true);
                        }
                    }
                    ret = true;

                    node.State = nextState;
                }
                foreach (var e in EventList)
                {
                    e.Invoke(node);
                }
            }
            return ret;
        }
        #endregion

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
            foreach (var node in Dic.Values)
            {
                node.State = MissionState.Locked;
            }
        }

        public void Clear()
        {
            Dic.Clear();

            LockedParentDic.Clear();

            ParentDic.Clear();

            EventDic.Clear();

            EventList.Clear();

            LogicNode.Clear();
        }

        public string SerialNode()
        {
            string ret = string.Empty;
            foreach (var kv in Dic)
            {
                ret += (kv.Value.ToString()) + '\n';
            }

            ConfigTool.Data = ret;
            ConfigTool.SaveStr(ConfigTool.Data);

            return ret;
        }

        public List<MissionNode> DeSerialNode()
        {
            List<MissionNode> ret = new();
            string s = string.Empty;

            s = ConfigTool.LoadStr<MissionNodeConfig>();
            List<string> list = s.Split('\n').ToList();
            foreach (var item in list)
            {
                MissionNode node = ScriptableObject.CreateInstance<MissionNode>();
                if (node.DeSerial(item))
                {
                    ret.Add(node);
                }
            }

            return ret;
        }
    }

    public class MissionNodeConfig
    {
        public static string SaveFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string SaveFolderName = "DemonConfig";

        public string SaveFileName = "Default";

        public string Data
        {
            get; set;
        }

        public MissionNodeConfig(string saveName = "DemonConfig")
        {
            SaveFileName = saveName;
        }

        public void SaveStr(string s)
        {
            string saveFullPath = Path.Combine(SaveFolderPath, SaveFolderName, SaveFileName + ".txt");


            if (!Directory.Exists(Path.Combine(SaveFolderPath, SaveFolderName)))
            {
                Directory.CreateDirectory(Path.Combine(SaveFolderPath, SaveFolderName));
            }


            File.WriteAllText(saveFullPath, s);
        }

        public string LoadStr<T>()
        {
            if (File.Exists(Path.Combine(SaveFolderPath, SaveFolderName, SaveFileName + ".txt")))
            {
                string data = File.ReadAllText(Path.Combine(SaveFolderPath, SaveFolderName, SaveFileName + ".txt"));

                return data;
            }
            else
            {
                return string.Empty;
            }
        }
    }

}