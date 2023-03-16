using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    private GlobalVariables GV;
    private ClientSocket socket;

    public Text text;
    public GameObject textGameobject;

    private bool canEnable = false;

    private void Start()
    {
        socket = GetComponent<ClientSocket>();
        GV = GetComponent<GlobalVariables>();
    }

    // Code that needs to be ran from the main thread
    private void Update()
    {
        if (canEnable)
        {
            canEnable = false;
            textGameobject.active = true;
        }
    }


    public void endGame(string message, bool sendToServer = false)
    {
        // We perform this if statement so this function doesn't get ran twice by the client.
        if (GV.isStarted) 
        {
            GV.isStarted = false;
            canEnable = true;
            switch (message.Split("_")[4])
            {
                case "win":
                    text.text = "YOU WON";
                    break;


                case "lose":
                    text.text = "YOU LOST";
                    break;


                case "tie":
                    text.text = "TIE";
                    break;
            }

            if (sendToServer)
            {
                socket.sendToServer(message);
            }

        }

    }

    public void restartGame()
    {

    }


}
