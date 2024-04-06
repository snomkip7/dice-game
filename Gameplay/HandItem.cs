using Godot;
using System;

public partial class HandItem : Area2D
{
	public bool selected = false;
	public bool full = false;
	public int roll = -1; // no roll

	public override void _Ready()
	{
		updateSprite();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void updateItem(int roll){
		this.roll = roll;
		full = true;
		updateSprite();
	}

	public void updateSprite(){ // update when sprites are made
		switch(roll){
			case -1:
				Modulate = new Color(0.2f,0.2f,0.2f); // black
				break;
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
		}
	}
}
