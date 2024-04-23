using Godot;
using System;


public partial class globalVariables : Node
{
	public Vector2 mousePosition = new(0, 0);
	public Vector2 lastMousePos = new(0, 0);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		mousePosition = GetViewport().GetMousePosition();
		lastMousePos = this.mousePosition;
	}
}
