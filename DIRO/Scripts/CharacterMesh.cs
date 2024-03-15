using Godot;
using System;

public partial class CharacterMesh : MeshInstance3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void Move(Vector3 position, Vector3 rotation)
	{
		Position = position;
		Rotation = rotation;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
