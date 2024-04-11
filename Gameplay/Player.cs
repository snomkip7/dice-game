using Godot;
using System;

public partial class Player : Node2D
{
	// attributes:
	public int maxHealth;
	public float health;

	// effects:
	public Vector2 poison = new Vector2(0,0); // first=dmg, second=time
	public Vector2 timeSLow = new Vector2(0,0); // first=amount, second=time
	public Vector2 extraDice = new Vector2(0,0); // first=amount, second=time
	public Vector2 reroll = new Vector2(0,0); // first=amount, second=time

	// other stuff
	public Gameplay game;
	public Timer poisonTimer;

	public override void _Ready()
	{
		game = GetParent<Gameplay>();
		poisonTimer = GetNode<Timer>("PoisonTimer");
		// read attributes from text files
		// add a thing to make the die have the right amount of sides
		// game.die.sides = sideNum
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(poisonTimer.TimeLeft == 0 && poison.Y !=0){ // starts poison effect if applied
			poisonTimer.Start(1);
		}
	}

	public void poisonDmg(){ // called every second when poison dmg is inflicted
		health -= poison.X;
		poison.Y--;
	}
}
