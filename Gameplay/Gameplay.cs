using Godot;
using System;

public partial class Gameplay : Node2D
{
	[Export]
	public PackedScene handItem;

	public HandItem[] handItems;
	public int handSize = 4; // will be changed with shop items probably
	
	public override void _Ready()
	{
		handItems = new HandItem[handSize];
		instanceHandItems();
	}

	
	public override void _Process(double delta)
	{
	}

	public void instanceHandItems(){ // create hand, with formatting
		Vector2 firstHandPosition = new Vector2(81, 369); // can be changed, formatting starts with this as top left
		int downBy = 100; // downwards space between handItems
		int rightBy = 100; // right space between handItems

		for(int i = 0; i < handSize; i++){
			int down = 0; // counting reasons
			int right = i % 2; // 0 if left, 1 if right

			for(int d = i; d > 1; d-=2){
				down++;
			}

			HandItem item = (HandItem) handItem.Instantiate();
			AddChild(item);
			handItems[i] = item;
			item.Position = new Vector2(firstHandPosition.X + rightBy * right, firstHandPosition.Y + downBy * down);
		}
	}

	public void rollMade(int roll){
		// put up a ui msg or smth showing what you rolled
		GD.Print("You got a "+roll+" do you want to play or hold?");
		GetNode<Button>("PlayRollButton").Visible = true;
		GetNode<Button>("HoldRollButton").Visible = true; // remember to add an option to play held rolls if hand is full
	}

	public void playRoll(){
		// do the thing
		Die die = GetNode<Die>("Die");
		int roll = die.nextRoll;
		GD.Print("Played a "+roll);
		GetNode<Button>("PlayRollButton").Visible = false;
		GetNode<Button>("HoldRollButton").Visible = false;
		die.reset();
	}

	public void holdRoll(){
		// do the thing
		Die die = GetNode<Die>("Die");
		int roll = die.nextRoll;
		GD.Print("Held a "+roll);

		bool held = false;
		for(int i = 0; i < handSize && held == false; i++){
			if(handItems[i].full == false){
				handItems[i].updateItem(roll);
				held = true;
			}
		}
		if(held){
			GetNode<Button>("PlayRollButton").Visible = false;
			GetNode<Button>("HoldRollButton").Visible = false;
			die.reset();
		} else{
			// do smth man
			GD.Print("Cant hold!");
		}
	}
}
