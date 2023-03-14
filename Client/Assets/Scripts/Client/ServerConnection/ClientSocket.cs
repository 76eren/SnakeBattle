using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System.Threading;

public class ClientSocket : MonoBehaviour
{
    // References to the UI
    public GameObject button;
    public GameObject text;
    public InputField IF;
    public GameObject inputField;

    // References to game objects
    public GameObject player;

    // All connection related matters
    private bool isConnected = false;
    private bool isListening = false;
    
    private int room;
    public int _room
    {
        get { return room; }    
        set { room = value; }
    }


    private UdpClient client;
    private IPEndPoint ep;
    public PositionsCollector positionsCollector;

    // Threading
    private Thread receiveThread;

    // ---Commands---
    
    // Start game
    private bool startGame = false;
    public string playerNum = "";


    private void Start()
    {
        this.receiveThread = new Thread(new ThreadStart(listenToServer));
        this.receiveThread.IsBackground = true;
    }


    public void connectToRoom()
    {
        // Use threading so code isn't blocking
        Thread thread = new Thread (() => {

            this._room = int.Parse(IF.text);

            this.client = new UdpClient();
            this.ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000); // 209.250.240.118
            this.client.Connect(ep);

            // send data
            var message = "joinRoom_" + this._room;
            var messageBytes = Encoding.ASCII.GetBytes(message);
            this.client.Send(messageBytes, messageBytes.Length);

            // then receive data
            print("Waiting for message back...");
            var receivedData = this.client.Receive(ref ep);
            var receivedMessage = Encoding.ASCII.GetString(receivedData);


            print("Host: " + ep.ToString() + " Message: " + receivedMessage);

            if (receivedMessage.Split("_")[0] == "joinRoom" && receivedMessage.Split("_")[1] == "1")
            {
                // From now on we are going to listen for packets from the server
                isConnected = true;

                playerNum = receivedMessage.Split("_")[2];

                print("Connected to room. You are player: " + playerNum);

            }
            else
            {
                // Lol do something here

            }
        
        
        
        });
        thread.Start();



    }


    public void sendToServer(String message)
    {
        Thread thread = new Thread(() => {
            var messageBytes = Encoding.ASCII.GetBytes(message);
            this.client.Send(messageBytes, messageBytes.Length);
        });
        thread.Start();


    }


    private void listenToServer()
    {
        while (true)
        {
            var receivedData = this.client.Receive(ref ep);
            var receivedMessage = Encoding.ASCII.GetString(receivedData);
            //print("Host: " + ep.ToString() + " Message: " + receivedMessage);
            handleServerCommand(receivedMessage);
        }

    }


    private void handleServerCommand(String message)
    {

        // Tells us we can start the game, the lobby can be considered full now
        if (message.Split("_")[0] == "starting" && message.Split("_")[1] == "1")
        {
            print("Got start signal, go nuts");
            startGame = true;
        }

        if (message.Split("_")[0] == "positions")
        {
            this.positionsCollector.updateEnemeyPositions(message);

        }
    }



    private void Update() 
    {
        if (this.isConnected && !this.isListening)
        {
            // We disable the widgets if we are connected
            // This needs to happen from the main thread so I put the code here
            // isDisconnected gets put on true via the thread
            button.SetActive(false);
            text.SetActive(false);
            inputField.SetActive(false);


            isListening = true;
            this.receiveThread.Start();           
        }

        // This code needs to be ran from the main thread, so I am putting this in the Update function of the main thread
        // startGame is gonna be set on true from a different thread
        if (startGame)
        {
            startGame = false;

           
            // The default for this is already on the right position facing the right
            // We only need to take player two in account
            if (playerNum == "2")
            {
                // Start on the right side facing the left
                player.transform.position = new Vector2(4.3582f, player.transform.position.y);

            }

            player.SetActive(true);


        }
    }

}
