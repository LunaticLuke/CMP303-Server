using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;




public class Server : MonoBehaviour
{
    public static int _maxPlayers { get; private set; }

    public static int _port { get; private set; }

    private static Socket tcpListener;

    public float gameTime;

    public static Dictionary<int, ClientConnection> clients = new Dictionary<int, ClientConnection>();

    public static Server instance;

    //The receive buffer that receive writes to
    private static byte[] receiveBuffer;

    private static byte[] sendBuffer;

    public static int numberOfClients;

    public static bool gameStarted = false;

    float packetTimer;

    float packetInterval;

    int updatesPerSecond = 4;


    struct SpawnMessage
    {
        public char messageType;
        public int amountOfPlayers;
    }


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
 
    }

    private void Update()
    {
        if(gameStarted)
        {
            gameTime += Time.deltaTime;
            packetTimer += Time.deltaTime;

            if(packetTimer >= 0.25f)
            {
                byte[] message = Packet.CreatePlayerStruct();
                ServerTCPSend.sendToAllTCPClients(message);
                packetTimer = 0;
            }

        }
    }

    public static void StartServer(int maxPlayers, int port, string hostIp)
    {
        _maxPlayers = maxPlayers; _port = port;

        

        InitializeServerData();
        IPAddress ipAddress = IPAddress.Parse(hostIp);
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
        Debug.Log(string.Format("Creating Server On {0}", port));
        receiveBuffer = new byte[ClientConnection.dataBufferSize]; 
        sendBuffer = new byte[ClientConnection.dataBufferSize]; 

        tcpListener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        tcpListener.Bind(localEndPoint);
        tcpListener.Listen(1000);

       
    
        tcpListener.BeginAccept(new AsyncCallback(ConnectCallback), tcpListener);
        //udpSocket.BeginReceive(receiveBuffer, 0, ClientConnection.dataBufferSize,0, ReceiveUDPCallback, 
    }

    /*private static void ReceiveUDPCallback(IAsyncResult ar)
    {
       try
        {
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            int byteLength = udpSocket.EndReceive(ar);

        }
    }*/ 

    private static void ConnectCallback(IAsyncResult _result)
   {
        //Access the client who has connected
        Socket _client = tcpListener.EndAccept(_result);
        //Resume Listening for other clients
        tcpListener.BeginAccept(new AsyncCallback(ConnectCallback), tcpListener);
        Debug.Log(string.Format("Incoming Connection from {0}", _client.RemoteEndPoint.ToString()));

        for (int i = 0; i <= _maxPlayers; i++)
        {
            //Is the slot empty?
            if(clients[i].tcp.socket == null)
            {
                //Connect the new client to the ID
                clients[i].tcp.Connect(_client);
                numberOfClients++;
                //Return as we don't need to fill anymore slots
                return;
            }
        }
        Debug.Log(string.Format("{0} failed to connect: Server Full!",_client.RemoteEndPoint));
    }

    

    private static void InitializeServerData()
    {
        for(int i = 0; i <= _maxPlayers; i++)
        {
            clients.Add(i, new ClientConnection(i));
        }
    }


    public void SpawnPlayers()
    {
        SpawnMessage messageToSend;
        messageToSend.messageType = 's';
        messageToSend.amountOfPlayers = numberOfClients;

        byte[] data = new byte[6];
        Array.Copy(BitConverter.GetBytes(messageToSend.messageType), 0, data, 0, 2);
        Array.Copy(BitConverter.GetBytes(messageToSend.amountOfPlayers), 0, data, 2, 4);

        ServerTCPSend.sendToAllTCPClients(data);
        GameManager.instance.SpawnPlayers();
        GameManager.instance.SpawnZombies();
        gameStarted = true;
    }

}

