using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;
using AudioType = UnityCore.Audio.AudioType;

public class TestAudio : MonoBehaviour
{
    [SerializeField]
    public AudioController audioController;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyUp(KeyCode.T))
        {
            audioController.PlayAudio(AudioType.SFX_01, true, 1.0f);
        }        
        if (Input.GetKeyUp(KeyCode.G))
        {
            audioController.StopAudio(AudioType.SFX_01);
        }        
        if (Input.GetKeyUp(KeyCode.B))
        {
            audioController.RestartAudio(AudioType.SFX_01);
        }     
        
        
        if (Input.GetKeyUp(KeyCode.Y))
        {
            audioController.PlayAudio(AudioType.SFX_02);
        }        
        if (Input.GetKeyUp(KeyCode.H))
        {
            audioController.StopAudio(AudioType.SFX_02);
        }        
        if (Input.GetKeyUp(KeyCode.N))
        {
            audioController.RestartAudio(AudioType.SFX_02);
        }   
    }
}
