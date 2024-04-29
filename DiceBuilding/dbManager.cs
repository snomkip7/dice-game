using Godot;
using System;
using System.Collections.Generic;

public partial class dbManager : Control
{
	//public static List<DieSlot> DieSlots = new();
	// Called when the node enters the scene tree for the first time.
	private static globalVariables globalVariables = new();
	public override void _Ready()
	{
		globalVariables = GetNode<globalVariables>("/root/GlobalVariables");
		return;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		return;
	}

	public static bool MatchMaker(Area2D face, Area2D slot){
		return false;
	}

	public void SaveDice(){
		String[] savedFaces = new String[7];
		for(int i = 0; i < 6; i++){
			DieFace curDie = GetNode<Node2D>("DieFace" + i) as DieFace;
			savedFaces[i] = curDie.face;
		}
		globalVariables.dieEffects = savedFaces;
		GetTree().ChangeSceneToFile("res://MainMenu.tscn");
	}
}