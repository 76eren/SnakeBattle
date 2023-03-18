using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class PositionsSharer : MonoBehaviour
{
    private Player player;
    public ClientSocket socket;
    private CheckCollisions checkCollisions;
    private CultureInfo ci;


    private void Start()
    {
        player = GetComponent<Player>();
        checkCollisions = GetComponent<CheckCollisions>();


        // Believe it or not we actually need this.
        // In my case when I had my system language on Dutch it would save floats such as 5.4 as 5,4 breaking some of the code
        // I found this out by accident by the way
        // Alternatively I could've just replaced the "," with somehting like a # or other character when splitting the positions
        System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
        System.Threading.Thread.CurrentThread.CurrentCulture = culture;
        System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

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
