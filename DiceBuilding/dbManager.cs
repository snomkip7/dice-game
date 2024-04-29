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
		List<String> savedFaces = new();
		int i = 0;
		while(GetNode<Node2D>("DieFace" + i) != null){
			DieFace curDie = GetNode<Node2D>("DieFace" + i) as DieFace;
			if(curDie.GetNode<Area2D>("Area2D").IsInGroup("slotted")){
				savedFaces.Add(curDie.face);
			}
			i++;
		}
		globalVariables.dieEffects = savedFaces.ToArray();
		GetTree().ChangeSceneToFile("res://MainMenu.tscn");
	}
}