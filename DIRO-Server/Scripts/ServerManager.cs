using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System;

using Godot;
using System.Linq;

public partial class ServerManager : Node
{
    private static TcpListener tcpListener;

    public static bool on = false;
    public static int playerCount = 0;

    // List of all active ids to negate duplicates
    public static List<int> ids = new();
    public static Dictionary<int, Client> clients = new();

    public static List<TcpClient> clientQueue = new();

    public override void _Ready()
    {
        PrintSame("Starting Server...");

        on = true;

        tcpListener = new TcpListener(IPAddress.Any, 60606);

        // Starts server and looks for incoming clients
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(ClientAcceptCallback, null);

        Print("Started");
    }

    public override void _PhysicsProcess(double delta)
    {
        
    }

    public override void _Process(double delta)
    {
        CheckClientQueue();
    }

    static void CheckClientQueue()
    {
        while (clientQueue.Count > 0)
        {
            TcpClient _client = clientQueue[0];

            // On connection, increase playercount
            playerCount++;

            int _id = 0;

            // Create a unique id for the player
            while (ids.Contains(_id))
            {
                _id++;
            }

            Client newClient = new(_id);

            ids.Add(_id);
            clients.Add(_id, newClient);

            newClient.tcp.Connect(_client);

            newClient.tcp.WriteStream(PacketManager.ToJson(new GP { id = _id }));

            var thescene = ResourceLoader.Load<PackedScene>("res://Scenes/character.tscn").Instantiate().Duplicate();

            Character c = thescene as Character;
            c.id = _id;

            c.Position = new Vector3(new RandomNumberGenerator().RandiRange(-10, 10), c.Position.Y, c.Position.Z);

            newClient.character = c;

            test_scene.sceneTree.CallDeferred(Node.MethodName.AddChild, thescene);

            foreach (var x in clients)
            {
                if (x.Key == _id) continue;

                newClient.tcp.WriteStream(PacketManager.ToJson(new AP { targetId = x.Key, position = x.Value.character.Position, rotation = x.Value.character.Rotation }));
            }

            SendAll(PacketManager.ToJson(new AP { targetId = newClient.player.id, position = newClient.character.Position, rotation = newClient.character.Rotation }));

            Print($"Client connected with id: {_id}, {playerCount} player(s) online!");

            clientQueue.RemoveAt(0);
        }
    }

    private static void ClientAcceptCallback(IAsyncResult result)
    {
        if (!on) return;

        TcpClient _client = new();

        try
        {
            _client = tcpListener.EndAcceptTcpClient(result);
        }
        catch (Exception)
        {
        }

        clientQueue.Add(_client);

        // Listen for new player
        tcpListener.BeginAcceptTcpClient(ClientAcceptCallback, null);
    }

    public static void Disconnect(Client _client)
    {
        // Remove the client from the server if they are not in game
        // Free up the id from the server
        ids.Remove(_client.player.id);

        if (clients.ContainsKey(_client.player.id))
        {
            clients[_client.player.id].Disconnect();
            clients.Remove(_client.player.id);
        }

        playerCount--;

        Print($"Disconnected from client with id: {_client.player.id}, {playerCount} player(s) remain!");
    }

    public static void SendAll(byte[] msg, Client[] exclude = null)
    {
        foreach(Client c in clients.Values)
        {
            if (exclude != null && exclude.Contains(c)) continue;

            c.tcp.WriteStream(msg);
        }
    }

    public static void Print(object msg)
    {
        ConsoleDisplay.text += msg.ToString() + "\n";
    }

    public static void PrintSame(object msg) 
    {
        ConsoleDisplay.text += msg.ToString();
    }
}