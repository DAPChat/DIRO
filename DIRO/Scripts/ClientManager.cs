using Godot;
using System;

public partial class ClientManager : Node
{
    public static Client client;
    
    public override void _Ready()
    {
        client = new Client();

        client.tcp.Connect();

        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    // Defines a print function non-godot classes can use
    public static void Print(string str)
    {
        GD.Print(str);
    }

    // Disconnect the client on close
    public override void _ExitTree()
    {
        client.Disconnect();

        base._ExitTree();
    }
}
