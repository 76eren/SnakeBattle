using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SnakeBattleServer
{
    class Server
    {
        // private Dictionary<int, IPEndPoint> connection;
        private Dictionary<int, Player[]> connection;
        
        private UdpClient udpServer;

        public Server()
        {
            this.connection = new Dictionary<int, Player[]>();
            this.udpServer = new UdpClient(11000);
        }
  

        public void getMessage()
        {
            while (true)
            {
                var remoteEP = new IPEndPoint(IPAddress.Any, 11000);
                var data = this.udpServer.Receive(ref remoteEP); // listen on port 11000
                string messageReceived = Encoding.ASCII.GetString(data);

                //  Console.WriteLine("Host: " + remoteEP.ToString() + " Data: " + messageReceived);

                // We don't want blocking code
                Thread newThread = new Thread (() => {
                    handleMessage(messageReceived, remoteEP);
                });
                newThread.Start();  
                
                
            }
        }

        // All messages are being sent from this function
        private void sendMessage(String message, IPEndPoint target)
        {
            var messageBytes = Encoding.ASCII.GetBytes(message);
            this.udpServer.Send(messageBytes, messageBytes.Length, target);
        }



        private void handleMessage(String message, IPEndPoint ipEndPoint)
        {
            if (message.Split("_")[0] == "joinRoom")
            {
                joinRoom(message, ipEndPoint);
            }

            if (message.Split("_")[0] == "positions")
            {
                handlePositions(message, ipEndPoint);
            }

            if (message.Split("_")[0] == "gamestatus")
            {
                
            }
           
        }

        private void changeGameStatus(String message, IPEndPoint ip)
        {
            // message format: gamestatus_room_playerNum_event_info
            String[] splittedMessage = message.Split("_"); 

            if (splittedMessage[3]  == "gameEnd")
            {
                int room = int.Parse(splittedMessage[1]);
                int player = int.Parse(splittedMessage[2]);
                int target = 0;
                if (player == 1)
                {
                    target = 2;
                }

                if (player == 2)
                {
                    target = 1;
                }


                Player[] players = this.connection[room];
                switch (splittedMessage[4])
                {
                    case "win":
                        players[player - 1].playerState = playerStates.WIN;
                        sendMessage(splittedMessage[0] + "_" + splittedMessage[1] + "_" + target + "_gameEnd_" + "win"
                            , players[player - 1].ip);
                        

                        players[target - 1].playerState = playerStates.LOSE;
                        sendMessage(splittedMessage[0] + "_" + splittedMessage[1] + "_" + target + "_gameEnd_" + "lose"
                            ,players[target - 1].ip);

                        break;


                    case "lose":
                        
                        players[player - 1].playerState = playerStates.LOSE;
                        sendMessage(splittedMessage[0] + "_" + splittedMessage[1] + "_" + target + "_gameEnd_" + "lose"
                            , players[player - 1].ip);
                        

                        players[target - 1].playerState = playerStates.WIN;
                        sendMessage(splittedMessage[0] + "_" + splittedMessage[1] + "_" + target + "_gameEnd_" + "win"
                            ,players[target - 1].ip);


                        break;


                    case "tie":
                        players[player - 1].playerState = playerStates.TIE;
                        sendMessage(splittedMessage[0] + "_" + splittedMessage[1] + "_" + target + "_gameEnd_" + "tie"
                            , players[player - 1].ip);


                        players[target - 1].playerState = playerStates.LOSE;
                        sendMessage(splittedMessage[0] + "_" + splittedMessage[1] + "_" + target + "_gameEnd_" + "tie"
                            , players[target - 1].ip);

                        break;
                }

            }


            if (message.Split("_")[3] == "gameReset")
            {

            }
        }


        private void handlePositions(String message, IPEndPoint ipEndpoint)
        {
            // The message: positions_roomNumber_playerNum_actualPositionsString
            // We get all the users in the room and send the same message to the other person in the same room
            // If both players do this then we'll create a chain of them sending each other the accurate positions (via the server of course)
            Console.WriteLine(message);

            int roomNum = int.Parse(message.Split("_")[1]);
            int playerNum = int.Parse(message.Split("_")[2]);
            Player[] players = this.connection[roomNum];

            int targetNum = 0;
            if (playerNum == 1)
            {
                targetNum = 2;    
            }
            else
            {
                targetNum = 1;
            }
            
            IPEndPoint target = players[targetNum-1].ip;
           

            if (target != null) {
                sendMessage(message, target);

            }
            else
            {
                Console.WriteLine("Hey something is wrong, the target appears to be null");
            }

        }


        private void joinRoom(String message, IPEndPoint ipEndpoint)
        {
            int room = Int32.Parse(message.Split("_")[1]);

            Player[] roomArray;
            if (this.connection.ContainsKey(room))
            {
                Console.WriteLine("Using already used room");
                roomArray = this.connection[room];
            }
            else
            {
                Console.WriteLine("Creating new room from scratch");
                roomArray = new Player[] { null, null };
            }


            // Grant access to join room
            if (roomArray[0] == null || roomArray[1] == null)
            {
                Console.WriteLine("Granting access to room");

                string playernum = "";
                if (roomArray[0] == null && roomArray[1] == null)
                {
                    this.connection.Add(room, new Player[] { new Player(room, ipEndpoint), null});
                    playernum = "1";
                }
                else if (roomArray[0] != null && roomArray[1] == null)
                {
                    // We store player 1
                    Player player1 = roomArray[0];

                    // We update the connection
                    this.connection[1] = new Player[] { player1, new Player(room, ipEndpoint) };
                    playernum = "2";
                }
                sendMessage("joinRoom_1_"+playernum, ipEndpoint);


                // We can start the game
                if (roomArray[0] != null && roomArray[1] == null)
                {
                    Player[] players = this.connection[room];
                    foreach (Player i in players)
                    {
                        Console.WriteLine("Sending start signal");
                        sendMessage("starting_1", i.ip);
                    }

                }
            }

            // Room is full, deny access
            else
            {
                Console.WriteLine("Denying access to room");
                this.sendMessage("joinRoom_0", ipEndpoint);

            }

        }
    }
}
