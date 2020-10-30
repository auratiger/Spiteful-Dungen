using UnityCore.Audio;
using UnityEngine;
using AudioType = UnityCore.Audio.AudioType;

namespace UnityCore.Menu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private GateController gateController;
        [SerializeField] private MainScreenPlayerMovements player;


        private void Start()
        {
            AudioController.instance.PlayAudio(AudioType.Menu_track, true, 1f);
        }

        public void StartGame()
        {
            gateController.OpenGate();
            player.TriggerExit();
        }

    }
    
}
