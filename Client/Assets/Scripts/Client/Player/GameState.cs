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
    public GameObject restartgameButton;
    private bool canEnable = false;
    private string textToWrite = "";

    // A reference to most variables needed for restarting
    private bool canRestart = false;
    public PositionsCollector PC;
    public PositionsSharer PS;
    public PositionTracker PT;
    public CheckCollisions CC;
    public Player player;

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
            text.text = textToWrite;
            textGameobject.active = true;
            restartgameButton.active = true;
        }

        if (canRestart)
        {
            canRestart = false;

            player.reset();
            PT.reset();
            PC.reset();

            textGameobject.active = false;
            restartgameButton.active = false;

            GV.isStarted = true;


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
                    textToWrite = "YOU WON";
                    break;


                case "lose":
                    textToWrite = "YOU LOST";
                    break;


                case "tie":
                    textToWrite = "TIE";
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
        print("GOT THE RESTART SIGNAL, I AM GOING ALL HOG");
        // Code to restart the game
        canRestart = true;
     
    }


}
