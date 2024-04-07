using Godot;
using System;

public partial class Gameplay : Node2D
{
	[Export]
	public PackedScene handItem;

	public Enemy enemy;
	public HandItem[] handItems;
	public int handSize = 4; // will be changed with shop items probably
	public int numSelected = 0;
	public bool dieSelected = false;
	
	public override void _Ready()
	{
		handItems = new HandItem[handSize];
		instanceHandItems();
		enemy = GetNode<Enemy>("Enemy");
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
		GetNode<Button>("HoldRollButton").Visible = true; // remember to add an option to play held rolls if hand is full
		dieSelected = true;
	}

	public void playRoll(){
		// do the thing
		Die die = GetNode<Die>("Die");
		int roll = die.nextRoll;
		if(dieSelected){
			GetNode<Button>("HoldRollButton").Visible = false;
		}
		int[] rolls;
		int iter = 0;
		if(dieSelected){
			rolls = new int[numSelected+1];
			rolls[0] = roll;
			iter++;
		} else{
			rolls = new int[numSelected];
		}
		
		for(int i = 0; i < handSize; i++){
			if(handItems[i].selected){
				rolls[iter] = handItems[i].roll;
				handItems[i].updateItem(-1);
				iter++;
			}
		}

		useRoll(rolls, true);
		if(dieSelected){
		die.reset();
		dieSelected = false;
		}
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
			GetNode<Button>("HoldRollButton").Visible = false;
			die.reset();
		} else{
			// do smth man
			GD.Print("Cant hold!");
		}
		dieSelected = false;
	}

	public void useRoll(int[] nums, bool atEnemy){
		// do this function once we decide on what does what
		GD.Print("use: ");
		for(int i = 0; i < nums.Length; i++){
			GD.Print(nums[i]);
		}
		GD.Print("good luck");
	}

	public void unSelect(){
		for(int i = 0; i < handSize; i++){
			handItems[i].selected = false;
		}
		numSelected = 0;
	}
}
