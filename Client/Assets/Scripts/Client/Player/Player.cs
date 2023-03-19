using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    private Directions.Direction direction;
    [SerializeField] private GameObject playerPrefab;
    
    private float timeStep = 0.1f;

    public List<GameObject> _bodies = new List<GameObject>(); 
    public List<GameObject> bodies
    {
        get { return this._bodies; }
        set { this._bodies = value;}
    }
   
    private PositionTracker TP;
    private PositionsSharer PS;

    public GlobalVariables GV;
    private GameObject controller;

    // For the restarting 
    private Vector2 initialLocation;
    private Directions.Direction initialDirection;
   

    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        GV = controller.GetComponent<GlobalVariables>();

        PS = GetComponent<PositionsSharer>();
        // The spawn positions for player 1 and player 2 are different so we need to have the directions facing accordingly
        if (transform.position.x < 0)
        {
            direction = Directions.Direction.RIGHT;
        }
        else
        {
            direction = Directions.Direction.LEFT;
        }

        initialDirection = direction;
        initialLocation = transform.position;


        bodies.Add(this.gameObject);

        TP = GetComponent<PositionTracker>();

        StartCoroutine(moveSnake()); // Can't have delays in the update function
        StartCoroutine(spawnBody());
    }

    public void reset()
    {
        int index = 0;
        foreach (GameObject i in this._bodies) {
            // Don't want to destroy ourselves lmao
            if (i == this._bodies.FirstOrDefault())
            {
                continue;
            }
            Destroy(i);
        }

        this.bodies.Clear();
        this._bodies.Clear();
        transform.position = this.initialLocation;
        this.direction = this.initialDirection;
        bodies.Add(this.gameObject);
        print(this.bodies.ToArray());
    }

    IEnumerator spawnBody()
    {
        while (true)
        {
            if (GV.isStarted)
            {
                yield return new WaitForSeconds(1);

                // We don't want to spawn the new body on the exact same position as the head.
                // because then in the CheckCollision script it'll think that the head is colliding with the body part that just spawned
                // and immidiately count it as a loss. In a way this is working around bugs :)
                // This WILL break collision detection so we take the 0.0f away from the transform.position.x in the PositionTracker class
                GameObject x = Instantiate(playerPrefab, new Vector3(transform.position.x + GV.extraFloatOnSpawn, transform.position.y), Quaternion.identity);
                
                
                x.GetComponent<Player>().enabled = false;
                x.GetComponent<BoxCollider2D>().enabled = false;
                bodies.Add(x);
            }
            else
            {
                yield return new WaitForSeconds(1);
            }
        }

    }

    IEnumerator moveSnake()
    {
        float timeAccumulator = 0f;

        while (true)
        {
            // This makes sure that the delay in the while loop is the same in every single client.
            // This means that the game speed is the same on bad and good devices
            if (GV.isStarted)
            {
                timeAccumulator += Time.deltaTime;
            }

            while (timeAccumulator >= timeStep)
            {
                // The movement
                Vector2 position = transform.position;
                float x = position.x;
                float y = position.y;

                switch (direction)
                {
                    case Directions.Direction.LEFT:
                        position.x -= transform.localScale.x;
                        transform.position = position;
                        break;

                    case Directions.Direction.RIGHT:
                        position.x += transform.localScale.x;
                        transform.position = position;
                        break;

                    case Directions.Direction.UP:
                        position.y += transform.localScale.y;
                        transform.position = position;
                        break;

                    case Directions.Direction.DOWN:
                        position.y -= transform.localScale.y;
                        transform.position = position;
                        break;
                }

                // We hit the left edge
                if (transform.position.x < -4.7918 && direction == Directions.Direction.LEFT)
                {
                    transform.position = new Vector2(5.0582f, transform.position.y);
                }

                // We hit the right edge 
                if (transform.position.x > 5.0918 && direction == Directions.Direction.RIGHT)
                {
                    transform.position = new Vector2(-5.0918f, transform.position.y);
                }

                // We hit the upper edge
                if (transform.position.y > 5.16 && direction == Directions.Direction.UP)
                {
                    transform.position = new Vector2(transform.position.x, -4.8832f);
                }

                // We hit the bottom edge 
                if (transform.position.y < -4.86 && direction == Directions.Direction.DOWN)
                {
                    transform.position = new Vector2(transform.position.x, 4.9168f);
                }


                // Now we want to move every gameObject as well
                foreach (GameObject i in bodies)
                {
                    if (bodies[0].Equals(i))
                    {
                        // We want to do the head (so the first element) a little differently
                        TP.previousX = x;
                        TP.previousY = y;
                        TP.x = transform.position.x;
                        TP.y = transform.position.y;
                    }
                    else
                    {
                        i.GetComponent<PositionTracker>().move();
                    }

                }

                // Might wanna run this from a seperate thread :/
                PS.share();



                timeAccumulator -= timeStep;
            }

            yield return null;
            


        }
    }

    void Update()
    {
        // Input
        if (Input.GetKeyDown(KeyCode.UpArrow) && direction != Directions.Direction.DOWN)
        {
            direction = Directions.Direction.UP;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && direction != Directions.Direction.RIGHT)
        {
            direction = Directions.Direction.LEFT;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && direction != Directions.Direction.UP)
        {
            direction = Directions.Direction.DOWN;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && direction != Directions.Direction.LEFT)
        {
            direction = Directions.Direction.RIGHT;
        }



    }
}
