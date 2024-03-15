using Newtonsoft.Json;
using System.Text;
using System;
using Godot;
using System.Net;

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
                var lt = JsonConvert.DeserializeObject<LoadType>(data);

                switch (lt.type)
                {
                    case "GP":
                        var gp = JsonConvert.DeserializeObject<GP>(lt.parameters);

                        client.player = new Player { id = gp.id };

                        break;

                    case "AP":
                        var ap = JsonConvert.DeserializeObject<AP>(lt.parameters);
                        
                        if (ap.targetId == client.player.id)
                        {
                            Character.Move(ap.position);
                            break;
                        }

                        if (ClientManager.ids.ContainsKey(client.player.id))
                        {
                            ClientManager.ids[client.player.id].Move(ap.position, ap.rotation);
                            break;
                        }

                        var thescene = ResourceLoader.Load<PackedScene>("res://Scenes/character.tscn").Instantiate().Duplicate();

                        CharacterMesh c = thescene as CharacterMesh;

                        Main.instance.CallDeferred(Node.MethodName.AddChild, thescene);

                        c.Move(ap.position, ap.rotation);

                        ClientManager.ids.Add(client.player.id, c);

                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(client.player.id);

                Console.WriteLine(data.ToString());

                Console.WriteLine($"{e}");
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