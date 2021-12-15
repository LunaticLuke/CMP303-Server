using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Handles what is seen on screen UIWise
public class UIManager : MonoBehaviour
{
    //The one instance of this class
    public static UIManager instance;
    //Canvas with connection screen on it
    public GameObject startMenu;

    public Text[] players = new Text[Server._maxPlayers];

    public Text playerPositions;

    public Text timeText;

    //Has the username be sent
    bool sentUsername = false;
    [HideInInspector]
    public bool spawnPlayer = false;



    private void Awake()
    {
        //If there's no instance
        if (instance == null)
        {
            //It is this
            instance = this;
            //Otherwise
        } else if (instance != this)
        {
            //destroy this as we already have an instance
            Debug.Log("Destroying A Previous Exisisting Instance");
            Destroy(this);
        }
        //Update the UI to get rid of editor placeholder text
        DisplayUI();
    }

    private void Update()
    {
        DisplayUI();
    }

    public void DisplayUI()
    {
        for(int i = 0; i < Server._maxPlayers; i++)
        {
            players[i].text = string.Format("{0}. {1}", Server.clients[i].id, Server.clients[i].username);
        }
        if (Server.gameStarted)
        {
            timeText.text = string.Format("Time Elapsed: {0}", Server.instance.gameTime);
            playerPositions.text = string.Format("Player 1 Position: {0} , {1}", Server.clients[0].player.positionArray[0], Server.clients[0].player.positionArray[1]);
        }
    }

    

    
}
