using Godot;
using System;
using System.Collections.Generic;

public partial class dbManager : Control
{
	private static globalVariables globalVariables = new();
	public static Area2D qDieSlot;
	public override void _Ready()
	{
		globalVariables = GetNode<globalVariables>("/root/GlobalVariables");

		if(globalVariables.healPlus){
			GetNode<Node2D>("DieFace11").SetDeferred("visible", true);
			//GetNode<Node2D>("DieFace11").Position = new Vector2(598, 506);
		}
		if(globalVariables.poisonPlus){
			GetNode<Node2D>("DieFace12").SetDeferred("visible", true);
			//GetNode<Node2D>("DieFace12").Position = new Vector2(838, 506);
		}
		if(globalVariables.firePlus){
			GetNode<Node2D>("DieFace13").SetDeferred("visible", true);
			//GetNode<Node2D>("DieFace13").Position = new Vector2(838, 506);
		}
		if(globalVariables.icePlus){
			GetNode<Node2D>("DieFace14").SetDeferred("visible", true);
			//GetNode<Node2D>("DieFace14").Position = new Vector2(838, 506);
		}

		if(globalVariables.quantumUnlocked && GetNode<Area2D>("DieSlotQ").Position != new Vector2(960, 270)){
			qDieSlot = GetNode<Area2D>("DieSlotQ");
			qDieSlot.GetNode<Sprite2D>("Sprite2D").Texture = ResourceLoader.Load("res://DiceBuilding/Sprites/quantumSlot.png") as Texture2D;
			qDieSlot.Position = new Vector2(1665, 700);
			//qDieSlot.SetDeferred("visible", true);
		} else {
			GetNode<Area2D>("DieSlotQ").Position = new Vector2(-999, -999);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		return;
	}

	public void OnWarningButtonPressed(){
		Button wB = GetNode<Button>("WarningButton");
		wB.SetDeferred("disabled", true);
		wB.SetDeferred("visible", false);
	}

	public void SaveDice(){
		List<String> savedFaces = new();
		int i = 0;
		while(i < 15){
			DieFace curDie = GetNode<Node2D>("DieFace" + i) as DieFace;
			GD.Print(curDie.Position);
			if(curDie.Position.Y < 300  || curDie.Position.X > 1500){
				savedFaces.Add(curDie.face);
				GD.Print("face: ", curDie.Position.Y, " = ", curDie.Position.X);
			}
			i++;
		}
		if(savedFaces.Count < 6 || savedFaces.Count > 7){
			savedFaces = new();
			GetNode<Button>("WarningButton").SetDeferred("disabled", false);
			GetNode<Button>("WarningButton").SetDeferred("visible", true);
		} else {
			globalVariables.dieEffects = savedFaces.ToArray();
			GetTree().ChangeSceneToFile("res://Shop/Shop.tscn");
		}
	}
}