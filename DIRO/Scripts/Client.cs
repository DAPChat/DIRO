using System.Net.Sockets;
using System;
using System.Net;
using System.Threading;

public class Client
{
    public TCP tcp;

    public Player player;
    public bool active = true;

    //private PlayerAccount account;

    public Client()
    {
        // Create a new TCP that manages all client functions
        // Pass the Client class so variables are accessible in TCP
        tcp = new TCP(this);
    }

    //public PlayerAccount Acc()
    //{
    //    return account;
    //}

    public class TCP
    {
        public TcpClient client;
        public Client instance;

        private readonly IPEndPoint endPoint = new(IPAddress.Parse("127.1.1.0"), 60606);

        private CancellationTokenSource cts = new();

        private NetworkStream stream;
        private byte[] buffer;
        private bool connected = false;

        public TCP(Client _client)
        {
            instance = _client;
        }

        public void Connect()
        {
            if (connected || !instance.active) return;

            // Try to connect to the client
            // If it fails, try again
            try
            {
                if (client == null)
                {
                    client = new TcpClient();
                }
                // 127.1.1.0
                TryConnect();
            }
            catch (Exception)
            {
                Connect();
            }
        }

        private async void TryConnect()
        {
            if (client == null)
            {
                client = new TcpClient();
            }

            // Continues to try connection until one is made
            while (!client.Connected)
            {
                if (!instance.active) return;

                cts = new CancellationTokenSource();
                cts.CancelAfter(5000);

                try
                {
                    await client.ConnectAsync(endPoint.Address, endPoint.Port, cts.Token);
                }
                catch (Exception) { }
                finally { cts.Cancel(); }
            }

            ConnectCallback();
        }

        private void ConnectCallback()
        {
            connected = true;

            buffer = new byte[8196];

            stream = client.GetStream();

            // Read the incoming messages
            stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, stream);
        }

        public void WriteStream(byte[] _msg)
        {
            if (!client.Connected || !instance.active || !connected) return;

            byte[] byteArr = new byte[8196];

            _msg.CopyTo(byteArr, 0);

            // Write the message to the stream to the correct client
            stream.BeginWrite(byteArr, 0, byteArr.Length, null, null);
        }

        private void ReadCallback(IAsyncResult _result)
        {
            try
            {
                int _readBytesLength = stream.EndRead(_result);

                // Check if the client disconnected
                // (the TCP sends a packet of length 0 on disconnect)
                if (_readBytesLength <= 0)
                {
                    // Disconnects from server if not in game
                    // Disconnect from game if in game
                    Connect();

                    return;
                }


                PacketManager.Decode((byte[])buffer.Clone(), instance);

                buffer = new byte[buffer.Length];

                stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, null);
            }
            catch (Exception)
            {
                Connect();
            }
        }

        public void Disconnect()
        {
            if (!client.Connected) return;

            connected = false;

            // Closes the client and stream
            stream.Close();
            client.Close();

            stream.Dispose();
            client.Dispose();

            client = null;
            stream = null;

            buffer = null;
        }
    }

    //public void Login(PlayerAccount _account)
    //{
    //    account = _account;
    //}


    public void Disconnect()
    {
        active = false;
        // Log out of the account on disconnect
        //if (account != null)
        //    Database.Logout(account);
        tcp.Disconnect();
    }
}