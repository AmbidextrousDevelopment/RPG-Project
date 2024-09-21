using System;
using RPG.Utils;
//using RPG.SceneManagement;
using UnityEngine;
using TMPro;
using RPG.SceneManagement;
using UnityEngine.UI;

namespace RPG.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        LazyValue<SavingWrapper> savingWrapper;

        [SerializeField] TMP_InputField /*InputField*/ newGameNameField;

        private void Awake() {
            savingWrapper = new LazyValue<SavingWrapper>(GetSavingWrapper);
        }

        private void Start()
        {
            Time.timeScale = 1;
        }

        private SavingWrapper GetSavingWrapper()
        {
            return FindObjectOfType<SavingWrapper>();
        }

        public void ContinueGame()
        {
            savingWrapper.value.ContinueGame();
        }

        public void NewGame()
        {
            savingWrapper.value.NewGame(newGameNameField.text);
            print("huynya");
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}