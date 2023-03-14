using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckCollisions : MonoBehaviour
{
    private List<GameObject> players = new List<GameObject>();
    private List<GameObject> enemies = new List<GameObject>();

    Player player;

    public ClientSocket socket;
    private int room;

    public GlobalVariables GV;

    
    public void setPlayers(List<GameObject> p)
    {
        this.players = p;
        this.room = socket._room;
    }

    public void setEnemies(List<GameObject> p)
    {
        this.enemies = p;
    }

    private void Start()
    {
        player = GetComponent<Player>(); 
    }

    // Update is called once per frame
    public void checkCollisions()
    {

        if (this.players.Count != 0 && this.enemies.Count != 0)
        {
            Vector3 playerPos = this.players.FirstOrDefault().transform.position;
            Vector3 enemyHead = enemies.FirstOrDefault().transform.position;

            // Tie
            if (playerPos == enemyHead)
            {
                print("IT TIED");
                socket.sendToServer("gamestatus_" + this.room + "_" + socket.playerNum + "_tie");
                // GV.isStarted = false;
            }

            // Check if player collides with enemy
            foreach (var i in enemies)
            {
                if (i.transform.position == playerPos)
                {
                    // ENEMY WON, YOU LOSE
                    print("Enemey won!");
                    socket.sendToServer("gamestatus_" + this.room + "_" + socket.playerNum + "_lose");
                    // GV.isStarted = false;
                    break;
                }
            }

            foreach (var i in this.players)
            {
                if (i.transform.position == enemyHead)
                {
                    // ENEMY LOST, YOU WIN
                    print("YOU WON");
                    socket.sendToServer("gamestatus_" + this.room + "_" + socket.playerNum + "_win");
                    //GV.isStarted = false;
                    break;
                }
            }


        }

    }
}
