using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIZombie : MonoBehaviour
{

    float speed = 0.5f;

    public Rigidbody2D body;

    Transform target;

    float[] playerDistances = new float[4];

    public bool alive = false;

    Vector2 Direction;

    float stoppingValue = 1.5f;

    Vector2 lastSentPosition;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Server.gameStarted && alive)
        {
            if (target != null)
            {
                //Debug.Log(Vector2.Distance(transform.position, target.position));
                if (Vector2.Distance(transform.position, target.position) < stoppingValue)
                {
                    Direction = Vector2.zero;
                }
                else
                {
                    Direction = target.transform.position - transform.position;
                }
            }
            TargetPlayer();
        }
    }

    private void FixedUpdate()
    {
        if (alive)
        {
            body.velocity = (Direction.normalized * speed);
        }
        
    }

    void TargetPlayer()
    {
        int closestIndex = 0;
        for(int i = 0; i < Server.numberOfClients; i++)
        {
            playerDistances[i] = Vector2.Distance(transform.position, Server.clients[i].player.transform.position);
            if(playerDistances[i] < playerDistances[closestIndex])
            {
                closestIndex = i;
            }
        }
        target = Server.clients[closestIndex].player.transform;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

   
}
