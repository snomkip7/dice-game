using Godot;
using System;

public partial class Gameplay : Node2D
{
	[Export]
	public PackedScene handItem;

	public Enemy enemy;
	public HandItem[] handItems;
	public int handSize = 4; // will be changed with shop items probably
	public int numSelected = 0; // num of selected handItems
	public bool dieSelected = false;
	
	public override void _Ready()
	{
		handItems = new HandItem[handSize]; // setting things up
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

			HandItem item = (HandItem) handItem.Instantiate(); // adding the actual hand items & adjusting position
			AddChild(item);
			handItems[i] = item;
			item.Position = new Vector2(firstHandPosition.X + rightBy * right, firstHandPosition.Y + downBy * down);
		}
	}

	public void rollMade(int roll){ // after a roll is made, shows hold button & selects die
		// put up a ui msg or smth showing what you rolled
		GD.Print("You got a "+roll+" do you want to play or hold?");
		GetNode<Button>("HoldRollButton").Visible = true; // remember to add an option to play held rolls if hand is full
		dieSelected = true;
	}

	public void playRoll(){ // finds all the things that are selected and calls useRoll
		// do the thing
		Die die = GetNode<Die>("Die");
		int roll = die.nextRoll;
		if(dieSelected){ // removing the hold button if die is played
			GetNode<Button>("HoldRollButton").Visible = false;
		}
		int[] rolls;
		int iter = 0;
		if(dieSelected){ // adding the die if it is selected
			rolls = new int[numSelected+1];
			rolls[0] = roll;
			iter++;
		} else{
			rolls = new int[numSelected];
		}
		
		for(int i = 0; i < handSize; i++){ // adding all selected hand items and updating them to be empty
			if(handItems[i].selected){
				rolls[iter] = handItems[i].roll;
				handItems[i].updateItem(-1);
				iter++;
			}
		}

		useRoll(rolls, true);
		if(dieSelected){ // resetting die if needed
			die.reset();
			dieSelected = false;
		}
	}

	public void holdRoll(){ // adding a roll to the hand
		// do the thing
		Die die = GetNode<Die>("Die");
		int roll = die.nextRoll;
		GD.Print("Held a "+roll);

		bool held = false; // attempting to add the die to a handItem
		for(int i = 0; i < handSize && held == false; i++){
			if(handItems[i].full == false){
				handItems[i].updateItem(roll);
				held = true;
			}
		}
		if(held){ // resetting die & removing hold button if is sucessfully held
			GetNode<Button>("HoldRollButton").Visible = false;
			die.reset();
		} else{ // if is unable to hold
			// do smth man - figure out what to do pls
			GD.Print("Cant hold!");
		}
		dieSelected = false; // deselects die 
	}

	public void useRoll(int[] nums, bool atEnemy){ // actually applying effects of used dies
		// do this function once we decide on what does what
		GD.Print("use: ");
		for(int i = 0; i < nums.Length; i++){
			GD.Print(nums[i]);
		}
		GD.Print("good luck");
	}

	public void unSelect(){ // unselects all hand items
		for(int i = 0; i < handSize; i++){
			handItems[i].selected = false;
		}
		numSelected = 0;
	}
}
