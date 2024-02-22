using Godot;
using System;

public partial class test_scene : Node3D
{
	public static test_scene sceneTree;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sceneTree = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
