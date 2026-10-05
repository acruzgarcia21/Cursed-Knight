using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuDisplay : MonoBehaviour
    {
        [SerializeField] private Button continueButton;

        private bool _isLoading;

        private void Start()
        {
            continueButton.interactable = SaveManager.HasRunSave();
            GameManager.Instance.AudioManager.PlayMainMenuMusic();
        }

        public void OnNewRunButtonClicked()
        {
            if (_isLoading) return;

            _isLoading = true;
            GameManager.Instance.SetRunStartRequest(GameManager.RunStartRequest.NewRun);
            Loader.Load(Loader.Scene.GameplayScene);
        }

        public void OnContinueButtonClicked()
        {
            if (_isLoading || !SaveManager.HasRunSave()) return;

            _isLoading = true;
            GameManager.Instance.SetRunStartRequest(GameManager.RunStartRequest.Continue);
            Loader.Load(Loader.Scene.GameplayScene);
        }

        public void OnQuitButtonClicked()
        {
            if (_isLoading) return;

            Application.Quit();
        }
    }
}
