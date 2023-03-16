using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PositionTracker : MonoBehaviour
{
    [HideInInspector] public float x;
    [HideInInspector] public float y;

    [HideInInspector] public float previousX;
    [HideInInspector] public float previousY;

    private GlobalVariables GV;
     

    private void Start()
    {
        x = transform.position.x;
        y = transform.position.y;

        previousX = x;
        previousY = y;

        GV = GameObject.FindGameObjectWithTag("GameController").GetComponent<GlobalVariables>();
    }

    public void move()
    {
        List<GameObject> bodies = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().bodies;

        // Here we correct the edit we made to the player's body in the Player.cs file
        Vector2 temp = new Vector2(bodies.LastOrDefault().GetComponent<Transform>().position.x - GV.extraFloatOnSpawn
            , bodies.LastOrDefault().GetComponent<Transform>().position.y);
        bodies.LastOrDefault().GetComponent<Transform>().position = temp;

        this.previousX = transform.position.x;
        this.previousY = transform.position.y;

        int index = bodies.IndexOf(this.gameObject);
        
        this.x = bodies[index - 1].GetComponent<PositionTracker>().previousX;
        this.y = bodies[index - 1].GetComponent<PositionTracker>().previousY;

        transform.position = new Vector2(this.x, this.y);

    }

}
