using System;
using Game.Config;
using Game.Core;
using Game.Enemy.States;
using Injection;
using UnityEngine;

namespace Game.Enemy
{
    public sealed class EnemyController : IDisposable
    {
        [Inject] private GameConfig _config;

        private readonly UnitView _view;
        private readonly StateManager<EnemyState> _enemyStateManager;

        public UnitView View => _view;

        public Vector2 Position
        {
            get { return _view.Position; }
            set { _view.Position = value; }
        }

        public float Speed
        {
            get
            {
                return _config.GetValue(GameParam.EnemySpeed);
            }
        }

        public float RotationSpeed
        {
            get
            {
                return _config.GetValue(GameParam.UnitRotateSpeed);
            }
        }

        public Vector2 LastHearVictim { get; set; }

        public EnemyController(UnitView view, Context context)
        {
            _view = view;
            _view.SetOutline(null);

            var subContext = new Context(context);
            var injector = new Injector(subContext);

            subContext.Install(this);
            subContext.Install(injector);

            _enemyStateManager = new StateManager<EnemyState>();
            injector.Inject(_enemyStateManager);
        }

        public void Dispose()
        {
            _enemyStateManager.Dispose();
        }

        public void Kill()
        {
            _enemyStateManager.Dispose();
            _view.Die();
        }

        public void Live(bool isVictim)
        {
            _view.MeshAgent.speed = Speed;
            _view.MeshAgent.angularSpeed = RotationSpeed;
            _view.MeshAgent.acceleration = 10000;


            _view.IsVictim = isVictim;
            _view.IsAI = true;
            _view.IsVissible = true;

            _view.Idle();

            _enemyStateManager.SwitchToState(new EnemyLiveState());

            if (isVictim)
            {
                _view.name = "Enemy_Victim";
            }
            else
            {
                _view.name = "Enemy";
            }
        }

        public Collider GetSeenVictim()
        {
            return _view.GetSeenVictim();
        }
    }
}