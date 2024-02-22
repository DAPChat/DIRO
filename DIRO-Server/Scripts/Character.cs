using Godot;
using System.Linq;

public partial class Character : CharacterBody3D
{
    public int id;

    int Speed = 5;
    float jumpVelocity = 4.5f;
    float gravity = 16;

    public void Move(Vector3 moveVector, Vector3 rotVector, double delta)
    {
        Vector3 velocity = Velocity;

        RotationDegrees = rotVector;

        // Handle Jump.
        if (moveVector.Y != 0 && IsOnFloor()) velocity.Y = jumpVelocity;

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector3 direction = (Transform.Basis * moveVector).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }

        Velocity = new Vector3(velocity.X, moveVector.Y == 0 ? Velocity.Y : velocity.Y, velocity.Z);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsOnFloor())
            Velocity = new Vector3(Velocity.X, Velocity.Y-gravity * (float)delta, Velocity.Z);

        ServerManager.clients[id].tcp.WriteStream(PacketManager.ToJson(new AP { position = Position }));

        MoveAndSlide();
    }
}