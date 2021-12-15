using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject[] playerPrefabs = new GameObject[4];

    public GameObject zombiePrefab;

    public Vector3[] startPositions = new Vector3[4];

    public int numberOfZombies = 10;

    public AIZombie[] zombies = new AIZombie[10];

    public Vector3[] zombieStarts = new Vector3[10];

    float elapsedTime;
    

    
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
        if(Server.gameStarted)
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime >= 0.5f)
            {
                byte[] data = Packet.CreateZombieStruct();
                ServerTCPSend.sendToAllTCPClients(data);
                elapsedTime = 0;
            }
        }
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
        for(int i = 0; i < numberOfZombies; i++)
        {
            GameObject spawnedZombie = Instantiate(zombiePrefab);
            zombies[i] = spawnedZombie.GetComponent<AIZombie>();
            zombies[i].SetPosition(zombieStarts[i]);
            zombies[i].alive = true;
        }
    }

}
