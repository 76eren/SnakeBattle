using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

public class PositionsSharer : MonoBehaviour
{
    private Player player;
    public ClientSocket socket;
    private CheckCollisions checkCollisions;

    private void Start()
    {
        player = GetComponent<Player>();
        checkCollisions = GetComponent<CheckCollisions>();
    }

    public void share()
    {
        List<GameObject> bodies = player.bodies;

        int room = socket._room;
        int playerNum = int.Parse(socket.playerNum);

        StringBuilder bodiesMessage = new StringBuilder();
        bodiesMessage.Append("positions" + "_" + room + "_" + playerNum + "_");

        foreach (GameObject i in bodies)
        {
            Vector2 positions = i.transform.position;
            string positionsString = positions.x + ";" + positions.y;

            if (i != bodies[bodies.Count - 1])
            {
                bodiesMessage.Append(positionsString + ",");
            }
            else
            {
                bodiesMessage.Append(positionsString);
            }
        }

        if (bodiesMessage.ToString() != "positions_" + room + "_1_"
            && bodiesMessage.ToString() != "positions_" + room + "_2_")
        {
            this.socket.sendToServer(bodiesMessage.ToString());
        }

        // At the last we want to check for new collisions because we've moved the player (because this function is being ran from the Player class moveSnake function)
        checkCollisions.setPlayers(bodies);
        checkCollisions.checkCollisions();


    }

}
