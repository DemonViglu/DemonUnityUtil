using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace demonviglu.MissonSystem
{
    public enum MissionState
    {
        Locked,
        Running,
        Success,
        Failure,
        Hide
    }
    [SerializeField]
    public class MissionNode : BaseNode
    {
        public string Content;
        public int ID;

        public MissionNodeManager missionManager;

        private MissionState _State;
        public MissionState State
        {
            get
            {
                return _State;
            }
            set
            {
                if (_State != value)
                {
                    _State = value;

                    if (CallIDs.Count > 0)
                    {
                        foreach(var id in CallIDs)
                        {
                            missionManager.CallEvent(id, this);
                        }
                    }

                }
            }
        }

        public List<int> Childrens = new();

        public bool AutoUnlock;

        public List<int> CallIDs = new();

        public Vector2 Position;

        public override string ToString()
        {
            string s = string.Empty;
            for (int i = 0; i < Childrens.Count - 1; ++i)
            {
                s += Childrens[i].ToString() + ',';
            }
            if (Childrens.Count > 0) s += Childrens[^1];

            string s2 = string.Empty;
            for (int i = 0; i < CallIDs.Count - 1; ++i)
            {
                s2 += CallIDs[i].ToString() + ',';
            }
            if (CallIDs.Count > 0) s2 += CallIDs[^1];

            return $"{ID}\t{Content}\t{State}\t{s}\t{AutoUnlock}\t{s2}\t{Position.x},{Position.y}";
        }

        public bool DeSerial(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;

            List<string> list = str.Split("\t",StringSplitOptions.None).ToList();
            List<string> tmpList;

            {
                ID = int.Parse(list[0]);
                Content = list[1];
                State = (MissionState)Enum.Parse(State.GetType(), list[2]);
                AutoUnlock = bool.Parse(list[4]);
            }
            tmpList = list[3].Split(',').ToList();
            foreach (var child in tmpList)
            {
                if (int.TryParse(child, out int id))
                {
                    Childrens.Add(id);
                }
            }

            tmpList = list[5].Split(',').ToList();
            foreach (var child in tmpList)
            {
                if (int.TryParse(child, out int id))
                {
                    CallIDs.Add(id);
                }
            }

            tmpList = list[6].Split(',').ToList();
            if(tmpList.Count > 0)
            {
                Position.x = float.Parse(tmpList[0]);
                Position.y = float.Parse(tmpList[1]);
            }


            return true;
        }
    }
}