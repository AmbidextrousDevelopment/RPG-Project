using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement                                              //C:/Users/User/AppData/LocalLow/DefaultCompany/RPG study 18_4
{
    public class SavingWrapper : MonoBehaviour  
    {
        const string defaultSaveFile = "save"; //потом удалить
        private const string currentSaveKey = "currentSaveName";

        [SerializeField] float fadeInTime = 0.2f;
         //* new
        [SerializeField] float fadeOutTime = 0.2f;
        [SerializeField] int firstLevelBuildIndex = 1;
        [SerializeField] int menuLevelBuildIndex = 0;
        
        // *
        private void Awake()
        {
          /*if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                StartCoroutine(LoadLastScene());     //раньше загружалась последняя сохраненная
            }*/
           
        } 
        //new
        public void ContinueGame()
        {
            StartCoroutine(LoadLastScene());
        }

        public void NewGame(string saveFile)
        {
            SetCurrentSave(saveFile);  // здесь появляется новый сейв
            StartCoroutine(LoadFirstScene());
        }

        public void LoadGame(string saveFile)
        {
            SetCurrentSave(saveFile);
            ContinueGame();
        }

        public void DeleteGame(string saveFile)
        {
            SetCurrentSave(saveFile);
            Delete();
        }

        public void LoadMenu()
        {
            StartCoroutine(LoadMenuScene());
        }

        private void SetCurrentSave(string saveFile)
        {
            PlayerPrefs.SetString(currentSaveKey, saveFile);
        }

        private string GetCurrentSave()
        {
            return PlayerPrefs.GetString(currentSaveKey);
        }

        private IEnumerator LoadFirstScene()
        {
            //Fader fader = FindObjectOfType<Fader>();
           // yield return fader.FadeOut(fadeOutTime);
            yield return SceneManager.LoadSceneAsync(firstLevelBuildIndex);
           // yield return fader.FadeIn(fadeInTime);
        }

        private IEnumerator LoadMenuScene()
        {
            /*Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);*/
            yield return SceneManager.LoadSceneAsync(menuLevelBuildIndex);
           // yield return fader.FadeIn(fadeInTime);
        }
        //*
        private IEnumerator LoadLastScene()       
        {
            yield return GetComponent<SavingSystem>().LoadLastScene(GetCurrentSave()); //загружает сцену с нынешним сейвом
           // yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeInTime);
        }

        private void Update()
        {
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
                {
                   Save(); 
                }


                if (Input.GetKeyDown(KeyCode.L))
                {
                    Load();
                }
                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    Delete();
                }
            }
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(GetCurrentSave()); //было (defaultSaveFile)
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(GetCurrentSave());  // было (defaultSaveFile)
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(GetCurrentSave()); // было (defaultSaveFile)
        }
        //new
        public IEnumerable<string> ListSaves()
        {
            return GetComponent<SavingSystem>().ListSaves();
        }
        //*
    }
}