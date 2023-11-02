using Game.Config;
using Injection;
using UnityEngine.SceneManagement;

namespace Game.States
{
    public sealed class GameMenuState : GameState
    {
        [Inject] private GameView _gameView;
        [Inject] private GameStateManager _gameStateManager;
        [Inject] private GameConfig _gameConfig;

        public override void Initialize()
        {
            _gameView.CameraFollower.ResetToDefaultPosition();
            _gameView.CameraFollower.ZoomTo(_gameConfig.GetValue(GameParam.CameraMenuZoom), 0);

            _gameView.GameHud.gameObject.SetActive(false);
            _gameView.MenuHud.gameObject.SetActive(true);
            _gameView.MenuHud.BtnHide.onClick.AddListener(OnBtnHideClicked);
            _gameView.MenuHud.BtnSeek.onClick.AddListener(OnBtnSeekClicked);
            _gameView.MenuHud.TxtLevelName.text = SceneManager.GetSceneAt(1).name;
        }
        
        public override void Dispose()
        {
            _gameView.MenuHud.gameObject.SetActive(false);
            _gameView.MenuHud.BtnHide.onClick.RemoveAllListeners();
            _gameView.MenuHud.BtnSeek.onClick.RemoveAllListeners();
        }

        private void OnBtnHideClicked()
        {
            _gameView.CameraFollower.ZoomTo(_gameConfig.GetValue(GameParam.CameraGameZoom), 1);

            _gameStateManager.SwitchToState(typeof(GamePlayHideState));
        }

        private void OnBtnSeekClicked()
        {
            _gameView.CameraFollower.ZoomTo(_gameConfig.GetValue(GameParam.CameraSeekZoom), 1);

            _gameStateManager.SwitchToState(typeof(GamePlaySeekState));
        }
    }
}