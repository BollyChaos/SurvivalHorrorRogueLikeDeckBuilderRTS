using UnityEngine;
// Clase absracta en especifico de los enemigos, hereda de IState
namespace State.Interfaces
{
    public abstract class AEnemyState : IState
    {
        protected IEnemy enemy;

        public AEnemyState(IEnemy enemy)
        {
            this.enemy = enemy;
        }

        public abstract void Enter();

        public abstract void Exit();

        public abstract void FixedUpdate();

        public abstract void Update();
    }
}