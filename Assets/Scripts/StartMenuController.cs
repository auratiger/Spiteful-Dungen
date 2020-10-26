using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private GateController gateController;
    [SerializeField] private MainScreenPlayerMovements player;

    public void StartGame()
    {
        gateController.OpenGate();
        player.TriggerExit();
    }

}
