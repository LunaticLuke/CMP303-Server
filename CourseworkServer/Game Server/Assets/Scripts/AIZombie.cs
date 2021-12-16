using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIZombie : MonoBehaviour
{

    float speed = 0.5f;

    public int id;

    public Rigidbody2D body;

    Transform target;

    float elapsedTime;

    float[] playerDistances = new float[4];

    public bool alive = false;

    float respawnTimer = 4.0f;

    bool respawning = false;

    bool attacking = false;

    Vector2 Direction;

    float stoppingValue = 1.5f;

    Vector2 lastSentPosition;

    float maxRangeToFollow = 5.0f;


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
                float distance = Vector2.Distance(transform.position, target.position);
                if(distance <= stoppingValue)
                {
                    Direction = new Vector2(0.0f, 0.0f);
                    if(!attacking)
                    {
                        attacking = true;
                        StartCoroutine(Attack());
                    }
                }else if(distance >= maxRangeToFollow)
                {
                    Direction = new Vector2(0.0f, 0.0f);
                }
                else
                {
                    Direction = target.position - transform.position;
                }
                elapsedTime += Time.deltaTime;
                if(elapsedTime >= 0.25f)
                {
                    elapsedTime = 0.0f;

                    if (distance < maxRangeToFollow)
                    {
                        Packet.ZombieStruct zomb;
                        zomb.typeOfMessage = 'z';
                        zomb.indexOfZombie = id;
                        zomb.xPos = transform.position.x;
                        zomb.yPos = transform.position.y;
                        zomb.timestamp = Server.instance.gameTime;

                        byte[] data = Packet.CreateZombieStruct(zomb);
                        ServerTCPSend.sendToAllTCPClients(data);
                    }
                }
            }
            TargetPlayer();
        }
        if(Server.gameStarted && !alive)
        {
            if (!respawning)
            {
                respawning = true;
                StartCoroutine(Respawn());
            }
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

    public IEnumerator Respawn()
    {
        SetPosition(new Vector2(-500, 0));
        yield return new WaitForSeconds(respawnTimer);
        SetPosition(GameManager.instance.zombieStarts[id]);
        gameObject.SetActive(true);
        alive = true;
        respawning = false;
    }

   IEnumerator Attack()
    {
        byte[] data = new byte[6];
        Array.Copy(BitConverter.GetBytes('h'), 0, data, 0, 2);
        Array.Copy(BitConverter.GetBytes(10), 0, data, 2, 4);
        ServerTCPSend.sendToSpecificTCPClient(data, target.GetComponent<Player>()._id);
        yield return new WaitForSeconds(2);
        attacking = false;
    }

}
