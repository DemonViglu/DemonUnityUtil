using UnityEngine;
namespace demonviglu.bt
{

    public enum NodeState
    {
        Running,
        Failure,
        Success
    }

    public abstract class Node : ScriptableObject
    {
        public NodeState State = NodeState.Running;
        public bool Started = false;
        public string Guid;
        public Vector2 Position;
        public NodeState Tick()
        {
            if (!Started)
            {
                OnStart();
                Started = true;
            }

            State = OnTick();

            if(State == NodeState.Failure||State == NodeState.Success)
            {
                OnStop();
                Started= false;
            }

            return State;
        }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract NodeState OnTick();

    }

}


