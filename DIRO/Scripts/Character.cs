using Godot;
using System;

public partial class Character : Node3D
{
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            RotationDegrees = new Vector3(RotationDegrees.X, RotationDegrees.Y - (eventMouseMotion.Relative.X * 0.5f), RotationDegrees.Z);

            Camera3D cam = GetNode<Camera3D>("Camera3D");
            cam.RotationDegrees = new Vector3(Math.Clamp(cam.RotationDegrees.X - (eventMouseMotion.Relative.Y * 0.5f), -80f, 85f), cam.RotationDegrees.Y, cam.RotationDegrees.Z);
        }
    }

    public override void _Process(double delta)
	{
		Vector3 velocity = Vector3.Zero;
		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept"))
			velocity.Y = 1;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		
		velocity.X = inputDir.X;
		velocity.Z = inputDir.Y;

		ClientManager.client.tcp.WriteStream(PacketManager.ToJson(new PIP {
			vector = velocity,
			rotation = RotationDegrees 
		}));
	}

	public void Move(Vector3 v)
	{
		SetDeferred("position", v);
	}

    public override void _Ready()
    {
		
    }
}
