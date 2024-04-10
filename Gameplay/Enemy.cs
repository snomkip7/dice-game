using Godot;
using System;

public partial class Enemy : Node2D
{
	// attributes:
	public int maxHealth;
	public int health;
	public int aiLevel;
	public int decisionTime;
	public int dieSides;
	public int handSize;

	// effects:
	public Vector2 poison = new Vector2(0,0); // first=dmg, second=time
	public Vector2 timeSLow = new Vector2(0,0); // first=amount, second=time
	public Vector2 extraDice = new Vector2(0,0); // first=amount, second=time
	public Vector2 reroll = new Vector2(0,0); // first=amount, second=time

	// other stuff
	int[] heldRolls;

	public override void _Ready()
	{
		// set all those attributes based on text file, also add a sprite
		heldRolls = new int[handSize];
		for(int i = 0; i < handSize;i++){
			heldRolls[i] = -1;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void enemyRollDice(){ // decided when/where to call this function
		int roll = new RandomNumberGenerator().RandiRange(1, dieSides);
		//updateDieSprite(roll)
		bool hold = false;
		// decide if to hold or not
		if(hold){
			for(int i = 0; i < handSize; i++){
				heldRolls[i] = roll;
			}
		} else{
			int[] rolls = new int[1];
			rolls[0] = roll;
			GetParent<Gameplay>().useRoll(rolls, false);
		}
	}
}
