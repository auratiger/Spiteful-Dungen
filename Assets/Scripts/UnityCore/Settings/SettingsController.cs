
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UnityCore.Settings
{
    public class SettingsController : MonoBehaviour
    {
        [Tooltip("Activate debugging messages")]
        [SerializeField] public bool debug;
        
        [Header("Settings buttons")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Dropdown resolutionDropdown;

        [Space]
        [Header("Panels")]
        [SerializeField] private SettingsPanel[] panels;
        
        private Resolution[] resolutions;
        private Dictionary<SettingsType, GameObject> panelTable;
        private GameObject currentPanel;

        [Serializable]
        private struct SettingsPanel
        {
            public SettingsType type;
            public GameObject panelInstance;
        }

#region Unity Functions

        private void Start()
        {
            Configure();
            ManageResolutions();
        }


#endregion

#region OnClick Functions

        public void OpenPanel(int type)
        {
            SettingsType settingsType = (SettingsType) type;

            if(!panelTable.ContainsKey(settingsType))
            {
                LogWarning("You are trying to open panel [" + settingsType + "] which is not present in table");
                return;
            }
            
            GameObject panel = panelTable[settingsType];

            if (currentPanel != null)
            {
                currentPanel.SetActive(false);
            }

            currentPanel = panel;
            panel.SetActive(true);
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

#region Private Functions

        private void Configure()
        {
            panelTable = new Dictionary<SettingsType, GameObject>();
            GeneratePanelTable();
        }

        private void GeneratePanelTable()
        {
            foreach (SettingsPanel panel in panels)
            {
                if (panelTable.ContainsKey(panel.type))
                {
                    LogWarning("You are trying to register audio [" + panel.type + "] that has already been registered");
                }
                else
                {
                    panelTable.Add(panel.type, panel.panelInstance);
                }
            }
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

