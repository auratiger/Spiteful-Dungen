using System;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityCore.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;
using AudioType = UnityCore.Audio.AudioType;

public class PassableExit : MonoBehaviour
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
        
        AudioController.instance.StopAudio(AudioType.Menu_track);

        _loader.LoadNextScene();
    }
}
