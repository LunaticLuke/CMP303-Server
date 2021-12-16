using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject[] playerPrefabs = new GameObject[4];

    public GameObject zombiePrefab;

    public Vector3[] startPositions = new Vector3[4];

    public int numberOfZombies = 5;

    public AIZombie[] zombies = new AIZombie[5];

    public Vector3[] zombieStarts = new Vector3[5];

    float msAllowance = 1f;

    float elapsedTime;

    public List<Bullet.collisionInfo> collisions = new List<Bullet.collisionInfo>();
    
    //public bool spawnPlayer = false;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else
        {
            Destroy(this);
        }

    }

    private void Update()
    {
    }

    //Spawns All Players Within The Servers World
    public void SpawnPlayers()
    {   
    
        for(int i = 0; i < Server._maxPlayers; i++)
        {
            if(Server.clients[i].tcp.socket != null)
            {
                GameObject spawnedPlayer = Instantiate(playerPrefabs[i]);
                Server.clients[i].player = spawnedPlayer.GetComponent<Player>();
                Server.clients[i].player._id = i;
                Server.clients[i].player.SetPosition(startPositions[i]);
            }
        }

    }

    public void SpawnZombies()
    {
        for(int i = 0;i < 5; i++)
        {
            GameObject spawnedZombie = Instantiate(zombiePrefab);
            zombies[i] = spawnedZombie.GetComponent<AIZombie>();
            zombies[i].SetPosition(zombieStarts[i]);
            zombies[i].id = i;
            zombies[i].alive = true;
        }
    }

    public bool checkCollisions(int playerID, int zombieID, float timestamp)
    {
        Debug.Log(string.Format("Collision List: {0}", collisions.Count));
        for(int i = 0; i < collisions.Count; i++)
        {
            if(collisions[i].idOfPlayer == playerID && collisions[i].idOfZombie == zombieID)
            {
                
                    //Within a reasonable amount of latency, kill the zombie
                    collisions.RemoveAt(i);
                    zombies[zombieID].alive = false;
                    return true;
                
            }
        }

        return false;
    }

}
