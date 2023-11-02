using DG.Tweening;
using Game.Config;
using Game.Core;
using Game.Core.UI;
using Injection;
using UnityEngine;

namespace Game.UI
{
    public sealed class GameHudMediator : Mediator<GameHud>
    {
        [Inject] private GameConfig _config;
        [Inject] private Timer _timer;
        [Inject] private LevelView _levelView;
        [Inject] private GameModel _gameModel;
        [Inject] private GameManager _gameManager;

        private readonly bool _isHided;
        private float _startTime;

        public GameHudMediator(bool isHided)
        {
            _isHided = isHided;
        }

        protected override void Show()
        {
            _view.TimeToHideLabelText = (_isHided) ? "Time to Hide" : "Starting in";

            var timeToHide = _config.GetValue(GameParam.GameStartDelayDuration);
            _view.TimeToHide = timeToHide;
            DOTween.To(() => _view.TimeToHide,
                x => _view.TimeToHide = x, 0, timeToHide).
                SetTarget(_view).OnComplete(OnHideTimeCompleted).SetEase(Ease.Linear);

            _view.SetTimeLeft(0, 0, _isHided);

            _view.Coins = 0;
            _gameManager.COINS_COLLECTED += OnCoinsCollected;
        }

        protected override void Hide()
        {
            DOTween.Kill(_view);
            _gameManager.COINS_COLLECTED -= OnCoinsCollected;
            _timer.POST_TICK -= TimerOnPOST_TICK;
        }

        private void OnHideTimeCompleted()
        {
            _startTime = Time.time;

            _timer.POST_TICK += TimerOnPOST_TICK;
            _view.SetTimeLeft(_startTime, _levelView.Duration, _isHided);
        }

        private void TimerOnPOST_TICK()
        {
            if (_view.SetTimeLeft(_startTime, _levelView.Duration, _isHided))
            {
                _timer.POST_TICK -= TimerOnPOST_TICK;
                _gameModel.IsTimeOut = true;
            }
        }

        private void OnCoinsCollected(UnitView unit, int value)
        {
            if (null != _gameManager.Player && _gameManager.Player.View == unit)
            {
                _view.Coins += value;
            }
        }
    }
}