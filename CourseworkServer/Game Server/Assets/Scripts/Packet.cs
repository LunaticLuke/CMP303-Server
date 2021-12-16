using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Packet 
{
    //18 Bytes
    public struct ZombieStruct
    {
        public char typeOfMessage; // 2 Bytes
        public int indexOfZombie; // 4 bytes
        public float xPos;  //4 Bytes
        public float yPos; //4 Bytes
        public float timestamp; // 4 bytes
    }

    public struct PlayerStruct
    {
        public char typeOfMessage; // 2 bytes
        public float timestamp; // +4 bytes = 6 bytes
        public float XPosP1; // +4 bytes = 10 bytes
        public float YPosP1; // +4 bytes = 14 bytes
        public float ZRotP1; // +4 bytes = 18 Bytes
        public float XPosP2; // +4 bytes = 22 bytes
        public float YPosP2; // +4 bytes = 26 bytes
        public float ZRotP2; // +4 bytes = 30 Bytes
        public float XPosP3; // +4 bytes = 34 bytes
        public float YPosP3; // +4 bytes = 38 bytes
        public float ZRotP3; // +4 bytes = 42 Bytes
        public float XPosP4; // +4 bytes = 46 bytes
        public float YPosP4; // +4 bytes = 50 bytes
        public float ZRotP4; // +4 bytes = 54 Bytes
    }


    public static byte[] CreateZombieStruct(ZombieStruct zombies)
    {
        byte[] data = new byte[18];
        Array.Copy(BitConverter.GetBytes('z'), 0, data, 0, 2);
        Array.Copy(BitConverter.GetBytes(zombies.indexOfZombie), 0, data, 2, 4);
        Array.Copy(BitConverter.GetBytes(zombies.xPos), 0, data, 6, 4);
        Array.Copy(BitConverter.GetBytes(zombies.yPos), 0, data, 10, 4);
        Array.Copy(BitConverter.GetBytes(zombies.timestamp), 0, data, 14, 4);

        return data;
    }

    public static byte[] CreatePlayerStruct()
    {
        int bytesToSend = 6 + (Server.numberOfClients * 12);
        byte[] data = new byte[bytesToSend];
        Array.Copy(BitConverter.GetBytes('p'), 0, data, 0, 2);
        Array.Copy(BitConverter.GetBytes(Server.instance.gameTime), 0, data, 2, 4);
        int index = 6;
        for(int i = 0; i < Server.numberOfClients; i++)
        {
            Array.Copy(BitConverter.GetBytes(Server.clients[i].player.transform.position.x),0,data,index,4);
            //Debug.Log(Server.clients[i].player.transform.position.x);
            Array.Copy(BitConverter.GetBytes(Server.clients[i].player.transform.position.y),0,data,index + 4,4);
           // Debug.Log(Server.clients[i].player.transform.position.y);
            Array.Copy(BitConverter.GetBytes(Server.clients[i].player.transform.eulerAngles.z),0,data,index + 8,4);
            index += 12;
        }

        return data;
    }

 

}
