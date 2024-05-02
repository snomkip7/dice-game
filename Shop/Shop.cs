using Godot;
using System;

public partial class Shop : Control
{
	private static globalVariables globalVariables = new();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		globalVariables = GetNode<globalVariables>("/root/GlobalVariables");
		if(globalVariables.quantumUnlocked){
			GetNode<AnimatedSprite2D>("Background").Frame = 1;
			GetNode<Control>("SlotQ").GetNode<Button>("ButtonQ").SetDeferred("disabled", true);
		}
		if(globalVariables.shopsVisited == true){
			GD.Print("this ran");
			GetNode<Control>("Slot1").GetNode<AnimatedSprite2D>("Label1").Frame = 2;
			GetNode<Control>("Slot2").GetNode<AnimatedSprite2D>("Label2").Frame = 2;
			GetNode<Control>("Slot3").GetNode<AnimatedSprite2D>("Label3").Frame = 0;
			GetNode<Control>("Slot1").GetNode<Button>("Button1").SetDeferred("disabled", false);
			GetNode<Control>("Slot2").GetNode<Button>("Button2").SetDeferred("disabled", false);
			GetNode<Control>("Slot3").GetNode<Button>("Button3").SetDeferred("disabled", false);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void BuyItem(string item){
		Control node1 = GetNode<Control>("Slot1");
		Control node2 = GetNode<Control>("Slot2");
		switch(item){
			case "quantum":
				BuyQuantum();
				break;
			case "slot":
				globalVariables.handSize += 1;
				GetNode<Control>("Slot3").GetNode<AnimatedSprite2D>("Item3").Frame = 1;
				GetNode<Control>("Slot3").GetNode<Button>("Button3").SetDeferred("disabled", true);
				break;
			case "healing":
				globalVariables.money -= 2;
				node1.GetNode<AnimatedSprite2D>("Item1").Frame = 1;
				node1.GetNode<Control>("BuyButton").SetDeferred("item", "fire");
				globalVariables.healPlus = true;
				GetNode<Control>("Slot1").GetNode<Button>("Button1").SetDeferred("disabled", true);
				break;
			case "poison":
				globalVariables.money -= 2;
				GetNode<Control>("Slot2").GetNode<AnimatedSprite2D>("Item2").Frame = 1;
				node2.GetNode<Control>("BuyButton").SetDeferred("item", "ice");
				globalVariables.poisonPlus = true;
				GetNode<Control>("Slot2").GetNode<Button>("Button2").SetDeferred("disabled", true);
				break;
			case "fire":
				globalVariables.money -= 4;
				GetNode<Control>("Slot1").GetNode<AnimatedSprite2D>("Item1").Frame = 3;
				globalVariables.firePlus = true;
				GetNode<Control>("Slot1").GetNode<Button>("Button1").SetDeferred("disabled", true);
				break;
			case "ice":
				globalVariables.money -= 4;
				GetNode<Control>("Slot2").GetNode<AnimatedSprite2D>("Item2").Frame = 3;
				globalVariables.icePlus = true;
				GetNode<Control>("Slot2").GetNode<Button>("Button2").SetDeferred("disabled", true);
				break;
			case "health":
				globalVariables.money -= 1;
				globalVariables.currentHealth += 50;
				if(globalVariables.currentHealth > 100){
					globalVariables.currentHealth = 100;
				}
				break;
		}
	}

	public void BuyQuantum(){
		globalVariables.quantumUnlocked = true;
		GetNode<AnimatedSprite2D>("GlitchEffect").SetDeferred("visible", true);
		GetNode<AnimatedSprite2D>("GlitchEffect").Play();
		GetNode<Control>("SlotQ").GetNode<Button>("ButtonQ").SetDeferred("disabled", true);
	}

	public void glitchFinished(){
		GetNode<AnimatedSprite2D>("Background").Frame = 1;
		GetNode<AnimatedSprite2D>("GlitchEffect").SetDeferred("visible", false);
	}
	public void QButton(){

	}

	private void OnEditDiePressed(){
		GetTree().ChangeSceneToFile("res://DiceBuilding/Menu.tscn");
	}

	private void OnExitPressed(){
		switch(globalVariables.battleNum){
			case 1:
				break;
			case 2:
				globalVariables.enemyHandSize = 3;
				globalVariables.enemyMaxHealth = 250;
				globalVariables.decisionTime = 5;
				globalVariables.aiLevel = 10;
				globalVariables.enemySpritePath = "res://Sprites/Enemies/enemyGoblin.png";
				break;
			case 3:
				globalVariables.enemyHandSize = 4;
				globalVariables.enemyMaxHealth = 500;
				globalVariables.decisionTime = 3;
				globalVariables.aiLevel = 15;
				globalVariables.enemySpritePath = "res://Sprites/Enemies/enemyDude.png";
				break;
		}
		if(globalVariables.shopsVisited == false){
			globalVariables.shopsVisited = true;
		}
		GetTree().ChangeSceneToFile("res://Gameplay/Gameplay.tscn");
	}
}
