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

    private GameState GS;

    
        //------------------\\
       // *SCREAMS IN JAVA*  \\
      //----------------------\\
    public void setPlayers(List<GameObject> p)
    {
        this.players = p;
        this.room = socket._room;
        this.GS = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState>();
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
        // Checks for collisions and determines if the game is a win/loss/tie.
        if (this.players.Count != 0 && this.enemies.Count != 0)
        {
            Vector3 playerPos = this.players.FirstOrDefault().transform.position;
            Vector3 enemyHead = enemies.FirstOrDefault().transform.position;

            // Tie
            if (playerPos == enemyHead)
            {
                print("IT TIED");
                this.GS.endGame("gamestatus_" + this.room + "_" + socket.playerNum + "_gameEnd_tie", true);
                return;
            }

            // Check if player collides with enemy
            foreach (var i in enemies)
            {
                if (i.transform.position == playerPos)
                {
                    // ENEMY WON, YOU LOSE
                    print("Enemey won!");
                    this.GS.endGame("gamestatus_" + this.room + "_" + socket.playerNum + "_gameEnd_lose", true); // _lose as in we lost
                    break;
                }
            }


            foreach (var i in this.players)
            {
                if (i.transform.position == enemyHead)
                {
                    // ENEMY LOST, YOU WIN
                    print("YOU WON");
                    this.GS.endGame("gamestatus_" + this.room + "_" + socket.playerNum + "_gameEnd_win", true); // _win as in we won                 
                    break;
                }
            }

            // Check if player collides with itself
            foreach (var i in this.players)
            {
                if (i.transform.position == playerPos && i != this.players.FirstOrDefault())
                {
                    // ENEMY WON, YOU LOSE
                    print("Enemey won!");
                    this.GS.endGame("gamestatus_" + this.room + "_" + socket.playerNum + "_gameEnd_lose", true); // _lose as in we lost
                    break;
                }              
            }




        }

    }
}
