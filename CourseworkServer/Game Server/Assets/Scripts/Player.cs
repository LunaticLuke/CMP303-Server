
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;





public struct PlayerMessage
{
    public char messageType;
    public float xPos;
    public float yPos;
    public int _id;
    public float timeStamp;
}
public class Player : MonoBehaviour
{
    private readonly string userName;

    [HideInInspector]
    public int _id;


    public float[] positionArray = new float[2];

    public float latestGameTime = 0;

    public float differenceInTime = 0;

    public bool hasServerUpdate = false;

    public List<Vector2> lastSentMessages = new List<Vector2>();

    const int ammoClip = 10;

    public float zRot;

    public Bullet[] bullets = new Bullet[ammoClip];

    float packetTimer;

    float packetInterval;

    int updatesPerSecond = 4;

    bool upToDate = false;

    public Rigidbody2D body;

    bool spawnBullet = false;

    float[] bulletDir = new float[2];

    float[] bulletOrigin = new float[2];

    int idToSpawn;

    public Transform origin;

   


    private void Start()
    {
        positionArray[0] = transform.position.x;
        positionArray[0] = transform.position.y;
        lastSentMessages.Add(new Vector2(transform.position.x, transform.position.y));
        lastSentMessages.Add(new Vector2(transform.position.x, transform.position.y));
        packetInterval = 1 / updatesPerSecond;
        packetTimer = 0;
    }

    public void Update()
    {

        if(Server.gameStarted)
        {

            if(hasServerUpdate)
            {
                transform.position = new Vector2(positionArray[0],positionArray[1]);
                transform.rotation = Quaternion.Euler(0,0,zRot);
                hasServerUpdate = false;
            }


          
            if(spawnBullet)
            {             
                spawnBullet = false;
                Shoot();
            }


        }

      
    }

  

    public void SetPosition(Vector3 _pos)
    {
        transform.position = _pos; 
    }

 

    public void Shoot( float dirX, float dirY, float originX, float originY)
    {

        bulletDir[0] = dirX; bulletDir[1] = dirY;
        bulletOrigin[0] = originX; bulletOrigin[1] = originY;
        spawnBullet = true;
    }


    public void SendPositionData()
    {
        //If something has changed
       
            //TODO - Create a Class encapsulating all structs
            PlayerMessage playerData;
            playerData.messageType = 'p'; // 2 bytes
            playerData.xPos = transform.position.x; // 4 bytes
            playerData.yPos = transform.position.y; // 4 bytes
            playerData.timeStamp = Server.instance.gameTime;


        byte[] data = new byte[18];
            Array.Copy(BitConverter.GetBytes(playerData.messageType), 0, data, 0, 2);
            Array.Copy(BitConverter.GetBytes(playerData.xPos), 0, data, 2, 4);
            Array.Copy(BitConverter.GetBytes(playerData.yPos), 0, data, 6, 4);
            Array.Copy(BitConverter.GetBytes(_id), 0, data, 10, 4);
            Array.Copy(BitConverter.GetBytes(playerData.timeStamp), 0, data, 14, 4);

            ServerTCPSend.sendToAllTCPClients(data);
        
        lastSentMessages.RemoveAt(0);
        lastSentMessages.Add(new Vector2(playerData.xPos,playerData.yPos));
        upToDate = true;

    }

    public void Shoot()
    {
        for (int i = 0; i < ammoClip; i++)
        {
            if (!bullets[i].simulating)
            {
                bullets[i].gameObject.SetActive(true);
                bullets[i].Fire(new Vector2(bulletOrigin[0], bulletOrigin[1]), new Vector2(bulletDir[0], bulletDir[1]));
                Physics2D.IgnoreCollision(bullets[i].GetComponent<Collider2D>(), GetComponent<Collider2D>());
                break;
            }
        }
    }

}
