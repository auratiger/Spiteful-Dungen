using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UnityCore.Settings
{
    public class SettingsController : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Dropdown resolutionDropdown;

        private Resolution[] resolutions;

#region Unity Functions

        private void Start()
        {
            ManageResolutions();
        }


#endregion

#region Resolution Settings

        private void ManageResolutions()
        {
            // saves all available resolutions 
            resolutions = Screen.resolutions;
            
            // clears the dropdown and inserts the resolution options
            resolutionDropdown.ClearOptions();

            int currentResolutionIndex = 0;
            List<string> options = new List<string>();

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Insert(0, option);
                
                // if (resolutions[i].width == Screen.currentResolution.width &&
                //     resolutions[i].height == Screen.currentResolution.height)
                // {
                //     currentResolutionIndex = i;
                // }
            }
            
            resolutionDropdown.AddOptions(options);
        }
        
        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void SetResolution(int resolutionIndex)
        {
            var resolution = resolutions[resolutions.Length - resolutionIndex - 1];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

#endregion

#region Adio Settings

        public void SetMasterVolume(float volume)
        {
            audioMixer.SetFloat("MasterVolume", volume);
        }

        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("MusicVolume", volume);
        }

        public void SetSFXVolume(float volume)
        {
            audioMixer.SetFloat("SFXVolume", volume);
        }

        
#endregion
    
    }
}

