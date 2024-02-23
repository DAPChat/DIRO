﻿using Newtonsoft.Json;
using System.Text;
using System;

public class PacketManager
{
    // Parse the incoming data into packets and run the packet-specific data
    public static void Decode(byte[] _data, Client client)
    {
        string[] dataList = Encoding.UTF8.GetString(_data).Split("[Packet]");

        for (int i = 1; i < dataList.Length; i++)
        {
            string data = dataList[i];

            try
            {
                var lt = JsonConvert.DeserializeObject<LoadType>(data.ToString());

                switch (lt.type)
                {
                }
            }
            catch (Exception e)
            {
                ServerManager.Print(client.player.id);

                ServerManager.Print(data.ToString());

                ServerManager.Print(e);
            }
        }

    }

    // Convert a class to the properly formatted Json
    public static byte[] ToJson(object json)
    {
        LoadType loadType = new()
        {
            parameters = JsonConvert.SerializeObject(json),
            type = json.GetType().Name
        };

        return ToBytes("[Packet]" + JsonConvert.SerializeObject(loadType));
    }

    // Convert a string to bytes
    public static byte[] ToBytes(string i)
    {
        return Encoding.UTF8.GetBytes(i);
    }

    // Class to discern the packet type
    [Serializable]
    class LoadType
    {
        public string type;
        public string parameters;
    }
}