using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerTCPSend : MonoBehaviour
{
    // Start is called before the first frame update

    //A method that sends bytes to every connected client
    public static void sendToAllTCPClients(byte[] data)
    { 
        for (int i = 0; i < Server.numberOfClients; i++)
        {
            if (Server.clients[i].tcp.socket != null)
            {
                Server.clients[i].tcp.Send(data);
            }
        }
    }

    //A method that sends bytes to a specified client.
    public static void sendToSpecificTCPClient( byte[] data, int clientToSendTo)
    {
        Server.clients[clientToSendTo].tcp.Send(data);
    }

    //A method that sends bytes to all but one client.
    public static void sendToAllButOneTCPClients(byte[] data, int clientToExclude)
    {
        for (int i = 0; i < Server.numberOfClients; i++)
        {
            if (Server.clients[i].tcp.socket != null && Server.clients[i].tcp.socket.Connected && i != clientToExclude)
            {
                Server.clients[i].tcp.Send(data);
            }
        }
    }

}
