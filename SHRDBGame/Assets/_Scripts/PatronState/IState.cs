using UnityEngine;

namespace State.Interfaces
{
    public interface IState
    {
        public void Enter();
        public void Exit();
        public void Update();
        public void FixedUpdate();
    }
}