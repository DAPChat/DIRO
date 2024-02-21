using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System;

class Server
{
    private static TcpListener tcpListener;

    public static bool on = false;
    public static int playerCount = 0;
    // List of all active ids to negate duplicates
    public static List<int> ids = new List<int>();

    public static List<Client> clients = new List<Client>();

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

        TcpClient _client = new TcpClient();

        try
        {
            _client = tcpListener.EndAcceptTcpClient(result);
        }
        catch (Exception)
        {
        }

        // On connection, increase playercount
        playerCount++;

        int _id = 1;

        // Create a unique id for the player
        while (ids.Contains(_id))
        {
            _id++;
        }

        ids.Add(_id);
        clients.Add(new Client(_id));

        Console.WriteLine($"Client connected with id: {_id}, {playerCount} player(s) online!");

        clients.Last().tcp.Connect(_client);

        // Listen for new player
        tcpListener.BeginAcceptTcpClient(ClientAcceptCallback, null);
    }
    public static void Disconnect(int id, Client _client)
    {
        // Remove the client from the server if they are not in game
        // Free up the id from the server
        ids.Remove(id);

        if (clients.Contains(_client))
        {
            clients[clients.IndexOf(_client)].Disconnect();
            clients.Remove(_client);
        }

        playerCount--;

        Console.WriteLine($"Disconnected from client with id: {id}, {playerCount} player(s) remain!");
    }
}