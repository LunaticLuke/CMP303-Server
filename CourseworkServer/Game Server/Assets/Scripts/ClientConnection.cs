using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

//This is the C# equivalent of the Connection Class From The labs
public class ClientConnection 
{

    //The player's ID
    public int id;
    //A reference to the class that will handle TCP connections
    public TCP tcp;
    //Set up the data buffer
    public static int dataBufferSize = 8192;

    //public UDP udp;

    public Player player;

    public string username;

    public ClientConnection(int _clientId)
    {
        //Initialise the ID within the constructor
        id = _clientId;
        //Initialise the TCP for this client.
        tcp = new TCP(id);
    }

}
