using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class TCP
{
    //The TCP socket that I connect,send and receive on.
    public Socket socket;

    private readonly int id;
    //The receive buffer that receive writes to
    private byte[] receiveBuffer;
    //The send buffer that send writes to
    private byte[] sendBuffer;

    bool usernameAssigned = false;

    struct ChatMessage
    {
        public char messageType;
        public string message;
    }

 

    public TCP(int _id)
    {
        id = _id;
    }

    public void Connect(Socket _client)
    {

        socket = _client;

        //Set the sizes of both buffers
        socket.ReceiveBufferSize = ClientConnection.dataBufferSize;
        socket.SendBufferSize = ClientConnection.dataBufferSize;
        receiveBuffer = new byte[ClientConnection.dataBufferSize];
        sendBuffer = new byte[ClientConnection.dataBufferSize];
        socket.NoDelay = true;
        //Asynchronously begin the connect
        Receive();
    }




    private void Receive()
    {
        //A try catch block will attempt to catch any exceptions
        try
        {
            //Start an asynchornous receive method
            socket.BeginReceive(receiveBuffer, 0, ClientConnection.dataBufferSize, 0, ReceiveCallback, socket);
        }catch(Exception _e)
        {
            //Write the exception caught to the console
            Debug.Log(_e.ToString());
        }
    }

    //Triggered if something can be received on the TCP socket.
    private void ReceiveCallback(IAsyncResult _result)
    {
        try
        {
            //Get the byte length 
            int byteLength = socket.EndReceive(_result);
            //If its less than or equal to zero, client has disconnected, handle this
            if (byteLength <= 0)
            {
                //Disconnect Function
                return;
            }
            //Create a new byte array to store received message
            byte[] _data = new byte[byteLength];
            //Copy it across
            Array.Copy(receiveBuffer, _data, byteLength);
            //Debug.Log(string.Format("Received {0} bytes from {1}.", byteLength, Server.clients[id].username));
            char typeOfMessage = BitConverter.ToChar(_data,0);
            //Debug.Log(string.Format("The Array Is is {0}", typeOfMessage));
            switch(typeOfMessage)
            {
                //Chat Message
                case 't':
                    //This is a ping test, just send it straight back to the client.
                    //ServerTCPSend.sendToSpecificTCPClient(_data, id);
                    Server.clients[id].tcp.Send(_data);
                    break;
                case 'c':
                string chatMessageString = Encoding.ASCII.GetString(receiveBuffer, 2, byteLength - 2);
                //Debug.Log(string.Format("Received {0} bytes from {1}.", byteLength,Server.clients[id].username));
               // Debug.Log(string.Format("Message Reads {0}", chatMessageString));
                   

                    
                ChatMessage messageToSend;
                messageToSend.messageType = 'c';
                messageToSend.message = string.Format("{0}: {1}", Server.clients[id].username, chatMessageString);
               

                    byte[] data = new byte[messageToSend.message.Length + 2];
                    Array.Copy(BitConverter.GetBytes(messageToSend.messageType), 0, data, 0, 2);
                    Array.Copy(Encoding.ASCII.GetBytes(messageToSend.message), 0, data, 2, messageToSend.message.Length);

                    ServerTCPSend.sendToAllTCPClients(data);
                    
            

                    break;

                case 'u':
                    string usernameMessage = Encoding.ASCII.GetString(receiveBuffer, 2, byteLength - 2);
                   // Debug.Log(string.Format("Received {0} bytes from {1}.", byteLength, Server.clients[id].username));
                   // Debug.Log(string.Format("Message Reads {0}", usernameMessage));
                    if (!usernameAssigned)
                    {
                        usernameAssigned = true;
                        Server.clients[id].username = usernameMessage;
                        char userChar = 'u';
                        byte[] usernameData = new byte[6];
                        Array.Copy(BitConverter.GetBytes(userChar), 0, usernameData,0, 2);
                        Array.Copy(BitConverter.GetBytes(id), 0, usernameData,2, 2);
                        //Send Welcome Packet To This client
                        ServerTCPSend.sendToSpecificTCPClient(usernameData, id);
                    }
                    break;
                case 'b':
                    ServerTCPSend.sendToAllButOneTCPClients(_data,id);
                    float xOrigin = BitConverter.ToSingle(_data, 2);
                    float yOrigin = BitConverter.ToSingle(_data, 6);
                    float dirX = BitConverter.ToSingle(_data, 10);
                    float dirY = BitConverter.ToSingle(_data, 14);

                    Server.clients[id].player.Shoot(dirX, dirY,xOrigin,yOrigin);

                    break;
                case 'p':
                    float xValue = BitConverter.ToSingle(_data, 2);
                    float yValue = BitConverter.ToSingle(_data, 6);
                    float gameTime = BitConverter.ToSingle(_data, 10);
                    float zRot = BitConverter.ToSingle(_data, 14);

                    Server.clients[id].player.positionArray[0] = xValue;
                    Server.clients[id].player.positionArray[1] = yValue;
                    Server.clients[id].player.latestGameTime = gameTime;
                    Server.clients[id].player.zRot = zRot;
                    Server.clients[id].player.hasServerUpdate = true;

                    break;
                case 'z':
                    float timeStamp = BitConverter.ToSingle(_data, 2);
                    int idOfZombie = BitConverter.ToInt32(_data, 6);
                    //Debug.Log(string.Format("Id Of Zombie: {0} TimeStamp: {1}", idOfZombie, timeStamp));
                    bool collided = GameManager.instance.checkCollisions(id, idOfZombie, timeStamp);
                    if(collided)
                    {
                        byte[] collisionData = new byte[10];
                        Array.Copy(BitConverter.GetBytes('k'), 0, collisionData, 0, 2);
                        Array.Copy(BitConverter.GetBytes(id), 0, collisionData, 2, 4);
                        Array.Copy(BitConverter.GetBytes(idOfZombie),0, collisionData, 6, 4);
                        ServerTCPSend.sendToAllTCPClients(collisionData);
                    }
                    break;

            }


           socket.BeginReceive(receiveBuffer, 0, ClientConnection.dataBufferSize, 0, ReceiveCallback, socket);
        }
        catch (Exception _e)
        {
            Debug.Log(string.Format("Error Receiving TCP Data: {0}", _e));
            //Disconnect
        }
    }


    public void Send(byte[] data)
    {
        //Get the bytes of the string
        sendBuffer = data;
        //Begin Sending chat message to the server.
        socket.BeginSend(sendBuffer, 0, sendBuffer.Length,0, SendCallback, socket);
    }

    //Triggers when ready to send
    private void SendCallback(IAsyncResult _result)
    {
        try
        {
        int bytesSent = socket.EndSend(_result);
        //Debug.Log(string.Format("Sent {0} bytes to the client.", bytesSent));
        }catch(Exception _e)
        {
            Debug.Log(_e.ToString());
        }

    }
}
