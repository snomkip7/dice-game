using Godot;
using System;
using System.Security.Cryptography;


public partial class DieSlot : Area2D
{
	public static bool full = false; //if the slot is empty or full
	public static int face = -1; //(-1 = if the slot is empty), the rest of the ints represent a possible face
	private static globalVariables globalVariables = new();
	private Area2D bodyRef = new();
	public override void _Ready()
	{
		globalVariables = GetNode<globalVariables>("/root/GlobalVariables");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_area_entered(Area2D body){
		//face = body.GetPath().ToString().Substring();
	}
}
