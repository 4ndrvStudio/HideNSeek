using Game.Config;
using Game.UI;
using UnityEngine;

namespace Game.States
{
    public sealed class GamePlaySeekState : BaseGamePlayState
    {
        public override void Initialize()
        {
            base.Initialize();
            _gameView.GameHud.Joystick.gameObject.SetActive(false);

            for (int i = 1; i < _levelView.Units.Length; i++)
            {
                _gameManager.CreateEnemy(_levelView.Units[i], true);
            }

            _gameView.CameraFollower.SetTarget(_levelView.Units[0].gameObject);
            _levelView.Units[0].SetOutline(_gameView.OutlineMaterials);

            AddMediator(new GameHudMediator(false), _gameView.GameHud);
            AddMediator(new UnitsHolderHudMediator(false), _gameView.UnitsHolderHud);
            
            AddMediator(new HearStepsManager(true), _levelView);

            _timer.ONE_SECOND_TICK += TimerOnONE_SECOND_TICK;
        }

        public override void Dispose()
        {
            base.Dispose();
            _gameView.CameraFollower.SetTarget(null);
            _timer.ONE_SECOND_TICK -= TimerOnONE_SECOND_TICK;
            _gameManager.Dispose();
        }

        private void TimerOnONE_SECOND_TICK()
        {
            if (Time.time > _startTime)
            {
                _gameView.CameraFollower.ZoomTo(_config.GetValue(GameParam.CameraGameZoom), 1);

                _gameManager.CreatePlayer(_levelView.Units[0], false);
                _gameView.GameHud.Joystick.gameObject.SetActive(true);

                _startTime = float.MaxValue;

                for (int i = 1; i < _levelView.Units.Length; i++)
                {
                    _levelView.Units[i].IsVissible = false;
                }
            }

            if (_gameModel.IsTimeOut || _gameManager.EnemiesCount <= 0)
            {
                _timer.ONE_SECOND_TICK -= TimerOnONE_SECOND_TICK;

                _gameManager.Player.View.Idle();
                _gameManager.Player.Dispose();

                _gameView.GameHud.gameObject.SetActive(false);
                
                AddMediator(new EndGameHudMediator(_gameManager.EnemiesCount <= 0), _gameView.EndGameHud);
            }
        }
    }
}