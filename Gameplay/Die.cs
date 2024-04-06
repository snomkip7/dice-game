using Godot;
using System;

public partial class Die : Area2D
{
	public bool rolling = false;
	public int nextRoll = -1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void rollDice(Node viewport, InputEvent @event, int shape_idx){
		if(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed == true && rolling == false){
			GD.Print("ROLL THE DICE");
			roll();
			
		}
	}

	public void roll(){
		rolling = true;
		GetNode<Timer>("RollTimer").Start();
		GD.Print("do be rollin");
		nextRoll = new RandomNumberGenerator().RandiRange(1, 6);
	}

	public void reset(){
		nextRoll = -1;
		updateSprite();
	}

	public void rollEnded(){ // rn it starts after 1 sec, but should be made to start after animation ended
		rolling = false;
		GD.Print("roll ended");
		updateSprite();
		GD.Print("rolled a "+nextRoll);
		GetParent<Gameplay>().rollMade(nextRoll);
	}

	public void updateSprite(){ // update when sprites are made
		switch(nextRoll){
			case 1:
				Modulate = new Color(.8f,.1f,.1f); // red
				break;
			case 2:
				Modulate = new Color(.1f,.8f,.1f); // green
				break;
			case 3:
				Modulate = new Color(.1f,.1f,.8f); // blue
				break;
			case 4:
				Modulate = new Color(.8f,.8f,.1f); // yellow
				break;
			case 5:
				Modulate = new Color(.1f,.8f,.8f); // aqua
				break;
			case 6:
				Modulate = new Color(.8f,.1f,.8f); // purple
				break;
			default:
				Modulate = new Color(1, 1, 1);
				break;
		}
	}
}
