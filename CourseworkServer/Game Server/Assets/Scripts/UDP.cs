using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDP : MonoBehaviour
{
   /* private int id;
  
    //The send buffer that send writes to
    private byte[] sendBuffer;

    public IPEndPoint endPoint;

    public UDP(int _id)
    {
        id = _id;
    }

    public void setUpUDP(IPEndPoint _endPoint)
    {
        endPoint = _endPoint;

        //Asynchronously begin the connect
        Receive();
    }


    public void SendData(string data)
    {

    }

    private void Receive()
    {
        //A try catch block will attempt to catch any exceptions
        try
        {
            //Start an asynchornous receive method
            
        }
        catch (Exception _e)
        {
            //Write the exception caught to the console
            Debug.Log(_e.ToString());
        }
    }

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

            string message = Encoding.ASCII.GetString(receiveBuffer, 0, byteLength);

            Debug.Log(string.Format("Received {0} bytes from {1}.", byteLength));
            Debug.Log(string.Format("Message Reads {0}", message));
            

            socket.BeginReceive(receiveBuffer, 0, ClientConnection.dataBufferSize, 0, ReceiveCallback, socket);
        }
        catch (Exception _e)
        {
            Debug.Log(string.Format("Error Receiving TCP Data: {0}", _e));
            //Disconnect
        }
    }

    public void Send(string data)
    {
        //Get the bytes of the string
        sendBuffer = Encoding.ASCII.GetBytes(data);
        //Begin Sending chat message to the server.
        socket.BeginSend(sendBuffer, 0, sendBuffer.Length, 0, SendCallback, socket);
    }

    //Triggers when ready to send
    private void SendCallback(IAsyncResult _result)
    {
        try
        {
            int bytesSent = socket.EndSend(_result);
            Debug.Log(string.Format("Sent {0} bytes to the client.", bytesSent));
        }
        catch (Exception _e)
        {
            Debug.Log(_e.ToString());
        }

    }*/

}
