using System;
using System.Net.Sockets;

public class Client
{
    public TCP tcp;

    public Player player;
    public bool active;

    //private PlayerAccount account;

    public Client(int _id)
    {
        // Temporary
        player = new Player
        {
            // Create a new TCP that manages all client functions
            id = _id
        };

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

        private NetworkStream stream;
        private byte[] buffer;

        public TCP(Client _client)
        {
            instance = _client;
        }

        public void Connect(TcpClient _client)
        {
            instance.active = true;
            client = _client;

            // Start reading the stream
            stream = client.GetStream();
            buffer = new byte[8196];

            stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, null);
        }

        public void WriteStream(byte[] _msg)
        {
            if (!client.Connected) return;

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
                    ServerManager.Disconnect(instance);
                 
                    return;
                }

                
                PacketManager.Decode(buffer, instance);

                buffer = new byte[buffer.Length];

                stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, null);
            }
            catch (Exception)
            {
                ServerManager.Disconnect(instance);
            }
        }

        public void Disconnect()
        {
            if (!client.Connected) return;

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