using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{

    [SerializeField] private float levelLoadDelay = 2f;
    [SerializeField] private float levelExitSlowFactor = 0.2f;

    private SceneLoader _loader;

    private void Awake()
    {
        _loader = FindObjectOfType<SceneLoader>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        Time.timeScale = levelExitSlowFactor;
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        Time.timeScale = 1f;

        _loader.LoadNextScene();
    }
}
