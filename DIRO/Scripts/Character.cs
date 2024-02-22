using Godot;
using System;

public partial class Character : CharacterBody3D
{
	public const float Speed = 250.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            Rotation = new Vector3(Rotation.X, Rotation.Y - (eventMouseMotion.Relative.X * 0.001f), Rotation.Z);

            Camera3D cam = GetNode<Camera3D>("Camera3D");
            cam.Rotation = new Vector3(Math.Clamp(cam.Rotation.X - (eventMouseMotion.Relative.Y * 0.001f), -1.2f, 1.5f), cam.Rotation.Y, cam.Rotation.Z);
        }
    }

    public override void _Process(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed * (float)delta;
			velocity.Z = direction.Z * Speed * (float)delta;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed*(float)delta);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed*(float)delta);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
