using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public int maxPlayers = 4;

    public int port = 11000;

    public InputField input;
    // Start is called before the first frame update
    void Start()
    {
        input.text = "192.168.0.11";
    }

    public void StartServer()
    {
        Server.StartServer(maxPlayers, port, input.text);
    }

}
