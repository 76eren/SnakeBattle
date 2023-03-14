using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTracker : MonoBehaviour
{
    [HideInInspector] public float x;
    [HideInInspector] public float y;

    [HideInInspector] public float previousX;
    [HideInInspector] public float previousY;
     

    private void Start()
    {
        x = transform.position.x;
        y = transform.position.y;

        previousX = x;
        previousY = y;
    }

    public void move()
    {
        this.previousX = transform.position.x;
        this.previousY = transform.position.y;

        List<GameObject> bodies = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().bodies;
        int index = bodies.IndexOf(this.gameObject);
        
        this.x = bodies[index - 1].GetComponent<PositionTracker>().previousX;
        this.y = bodies[index - 1].GetComponent<PositionTracker>().previousY;

        transform.position = new Vector2(this.x, this.y);

    }

}
