using Godot;
using System;

public partial class Enemy : Node2D
{
	// attributes:
	public int maxHealth;
	public int health;
	public int aiLevel;
	public int decisionTime;

	// effects:
	public Vector2 poison = new Vector2(0,0); // first=dmg, second=time
	public Vector2 timeSLow = new Vector2(0,0); // first=amount, second=time
	public Vector2 extraDice = new Vector2(0,0); // first=amount, second=time
	public Vector2 reroll = new Vector2(0,0); // first=amount, second=time
	public override void _Ready()
	{
		// set all those attributes based on text file, also add a sprite
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
