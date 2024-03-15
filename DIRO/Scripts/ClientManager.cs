using Godot;
using System;
using System.Collections.Generic;

public partial class ClientManager : Node
{
    public static Client client;

    public static Dictionary<int, CharacterMesh> ids = new();

    public override void _Ready()
    {
        client = new Client();

        client.tcp.Connect();

        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Visible ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
        }
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
