using Godot;
using System;

public partial class Die : Area2D
{
	public bool canRoll = true;
	public int nextRoll = -1;
	public int sides = 4;
	public Gameplay gameplay;
	public float opacity = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameplay = GetParent<Gameplay>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void rollDice(Node viewport, InputEvent @event, int shape_idx){
		if(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed == true){
			if(canRoll){
				GD.Print("ROLL THE DICE");
				gameplay.unSelect();
				roll();
			} else if(canRoll==false&&gameplay.dieSelected){
				GD.Print("deselect die");
				gameplay.dieSelected = false;
				opacity = 1;
				updateSprite();
			} else{
				GD.Print("reselect die");
				gameplay.dieSelected = true;
				opacity = .8f;
				updateSprite();
			}
		}
	}

	public void roll(){
		canRoll = false;
		GetNode<Timer>("RollTimer").Start();
		GD.Print("do be rollin");
		opacity = 0.8f;
	}

	public void reset(){
		nextRoll = -1;
		opacity = 1;
		updateSprite();
		canRoll = true;
	}

	public void rollEnded(){ // rn it starts after 1 sec, but should be made to start after animation ended
		nextRoll = new RandomNumberGenerator().RandiRange(1, sides);
		//canRole = false;
		GD.Print("roll ended");
		updateSprite();
		GD.Print("rolled a "+nextRoll);
		GetParent<Gameplay>().rollMade(nextRoll);
	}

	public void updateSprite(){ // update when sprites are made
		switch(nextRoll){
			case 1:
				Modulate = new Color(.8f,.1f,.1f, opacity); // red
				break;
			case 2:
				Modulate = new Color(.1f,.8f,.1f, opacity); // green
				break;
			case 3:
				Modulate = new Color(.1f,.1f,.8f, opacity); // blue
				break;
			case 4:
				Modulate = new Color(.8f,.8f,.1f, opacity); // yellow
				break;
			case 5:
				Modulate = new Color(.1f,.8f,.8f, opacity); // aqua
				break;
			case 6:
				Modulate = new Color(.8f,.1f,.8f, opacity); // purple
				break;
			default:
				Modulate = new Color(1, 1, 1, opacity);
				break;
		}
	}
}
