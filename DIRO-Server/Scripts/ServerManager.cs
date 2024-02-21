using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System;

public class ServerManager
{
    private static TcpListener tcpListener;

    public static bool on = false;
    public static int playerCount = 0;

    // List of all active ids to negate duplicates
    public static List<int> ids = new();
    public static List<Client> clients = new();

    public static void Start()
    {
        Console.Write("Starting Server...");

        on = true;

        tcpListener = new TcpListener(IPAddress.Any, 60606);

        // Starts server and looks for incoming clients
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(ClientAcceptCallback, null);

        Console.WriteLine("Started");
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

        // On connection, increase playercount
        playerCount++;

        int _id = ids.Count;

        // Create a unique id for the player
        while (ids.Contains(_id))
        {
            _id++;
        }

        Client newClient = new(_id);

        ids.Add(_id);
        clients.Add(newClient);

        newClient.tcp.Connect(_client);

        Console.WriteLine($"Client connected with id: {_id}, {playerCount} player(s) online!");

        // Listen for new player
        tcpListener.BeginAcceptTcpClient(ClientAcceptCallback, null);
    }
    public static void Disconnect(Client _client)
    {
        // Remove the client from the server if they are not in game
        // Free up the id from the server
        ids.Remove(_client.player.id);

        if (clients.Contains(_client))
        {
            clients[clients.IndexOf(_client)].Disconnect();
            clients.Remove(_client);
        }

        playerCount--;

        Console.WriteLine($"Disconnected from client with id: {_client.player.id}, {playerCount} player(s) remain!");
    }
}