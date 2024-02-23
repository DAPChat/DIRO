using Godot;
using System.Collections.Generic;

public partial class Character : CharacterBody3D
{
    public int id;
    public List<Vector3> input = new();
    public List<Vector3> rotation = new();

    int Speed = 5;
    float jumpVelocity = 4.5f;
    float gravity = 9.8f;

    public override void _PhysicsProcess(double delta)
    {
        
    }
}