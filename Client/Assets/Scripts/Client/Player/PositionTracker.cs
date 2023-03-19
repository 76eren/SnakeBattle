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

    // Saves the initials for the restarting
    private float initialX;
    private float initialY;
    private float initialPreviousX;
    private float initialPreviousY;

    private void Start()
    {
        this.x = transform.position.x;
        this.y = transform.position.y;

        this.previousX = x;
        this.previousY = y;

        // For the restarting later
        this.initialX = x;
        this.initialY = y;
        this.initialPreviousX = previousX;
        this.initialPreviousY = previousY;
        

        this.GV = GameObject.FindGameObjectWithTag("GameController").GetComponent<GlobalVariables>();
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

    public void reset()
    {
        this.x = this.initialX;
        this.y = this.initialY;
        this.previousX = this.initialPreviousX;
        this.previousY = this.initialPreviousY;
    }

}
