using Godot;
using System;

public partial class Player : Node2D
{
	// attributes:
	public int maxHealth;
	public int health;

	// effects:
	public Vector2 poison = new Vector2(0,0); // first=dmg, second=time
	public Vector2 timeSLow = new Vector2(0,0); // first=amount, second=time
	public Vector2 extraDice = new Vector2(0,0); // first=amount, second=time
	public Vector2 reroll = new Vector2(0,0); // first=amount, second=time

	public Gameplay game;

	public override void _Ready()
	{
		game = GetParent<Gameplay>();
		// read attributes from text files
		// add a thing to make the die have the right amount of sides
		// game.die.sides = sideNum
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
