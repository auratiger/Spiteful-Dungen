using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnityCore.Scene
{
    public class SceneLoader : MonoBehaviour
    {
        [Tooltip("Activate debugging messages")]
        [SerializeField] public bool debug;
        
        [Space]
        [SerializeField] private GameObject loadingScreenPrefab;
        
        private GameObject screen;
        private Slider loader;
        private Text progressText;
        
#region Unity Functions

        private void Awake()
        {
            Singleton();
        }
        
#endregion

#region Public Functions

        public void LoadNextScene()
        {
            var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            StartCoroutine(LoadAsynchronously(currentSceneIndex + 1));
        }
        
        public void LoadSceneByName(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        
        public void LoadSceneByNameAsync(string sceneName)
        {
            StartCoroutine(LoadAsynchronously(sceneName));
        }
        
        
        public void LoadStartLevel()
        {
            StartCoroutine(LoadAsynchronously("Level 1"));
        }
        
        public void LoadStartMenu()
        {
            SceneManager.LoadScene("Start Menu");
        }

        public void LoadSettingsMenu()
        {
            SceneManager.LoadScene("Settings Menu");
        }

        public void Quit()
        {
            Application.Quit();
        }

#endregion

#region Private Functions

        private void Singleton()
        {
            if (FindObjectsOfType<SceneLoader>().Length > 1)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        IEnumerator LoadAsynchronously(string sceneName)
        {
            CreateLoadScreen();
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                
                loader.value = progress;
                
                yield return null;
            }
        }
        IEnumerator LoadAsynchronously(int sceneIndex)
        {
            CreateLoadScreen();
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);

                loader.value = progress;
                progressText.text = progress * 100f + "%";
                
                yield return null;
            }

        }

        private void CreateLoadScreen()
        {
            screen = Instantiate(loadingScreenPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            loader = screen.GetComponentInChildren<Slider>();
            progressText = screen.GetComponentInChildren<Text>();
        }
        
        private void Log(object _msg)
        {
            if (!debug) return;
            Debug.Log("[Audio Controller]: " + _msg);
        }

        private void LogWarning(object _msg)
        {
            if (!debug) return;
            Debug.LogWarning("[Audio Controller]: " + _msg);
        }
        

#endregion


    }
}
