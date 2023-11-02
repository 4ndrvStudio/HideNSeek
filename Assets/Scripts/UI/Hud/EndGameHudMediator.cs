using Game.Core.UI;
using Game.States;
using Injection;
using UnityEngine.SceneManagement;

namespace Game.UI
{
    public sealed class EndGameHudMediator : Mediator<EndGameHud>
    {
        [Inject] private GameModel _gameModel;
        [Inject] private GameStateManager _gameStateManager;

        private readonly bool _isWin;

        public EndGameHudMediator(bool isWin)
        {
            _isWin = isWin;
        }

        protected override void Show()
        {
            _view.gameObject.SetActive(true);
            _view.RestartLabel = (_isWin) ? "Next" : "Restart";

            _view.BtnRestart.onClick.AddListener(OnRestartClicked);
        }
        
        protected override void Hide()
        {
            _view.gameObject.SetActive(false);
            _view.BtnRestart.onClick.RemoveAllListeners();
        }

        private void OnRestartClicked()
        {
            NextLevel(_isWin);
        }

        public void NextLevel(bool isWin)
        {
            if (!isWin)
            {
                _gameModel.Level = 10000;
            }

            _gameModel.Level++;
            _gameModel.Save();

            bool isSceneExists = _gameModel.Level < SceneManager.sceneCountInBuildSettings;
            if (!isSceneExists)
            {
                _gameModel.Remove();
            }

            _gameStateManager.SwitchToState(new GameUnloadState());
        }
    }
}