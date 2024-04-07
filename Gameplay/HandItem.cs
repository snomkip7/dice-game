using Godot;
using System;

public partial class HandItem : Area2D
{
	public bool selected = false;
	public bool full = false;
	public int roll = -1; // no roll

	public override void _Ready()
	{
		updateSprite(1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void select(Node viewport, InputEvent @event, int shape_idx){
		if(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed == true && roll != -1){
			selected = !selected;
			if(selected){
				updateSprite(.5f);
				GetParent<Gameplay>().numSelected += 1;
			} else{
				updateSprite(1);
				GetParent<Gameplay>().numSelected -= 1;
			}
			GD.Print(selected+" and all are "+GetParent<Gameplay>().numSelected);
		}
	}

	public void updateItem(int roll){
		this.roll = roll;
		full = true;
		updateSprite(1);
		if(roll==-1){
			selected = false;
			full = false;
		}
	}

	public void updateSprite(float opacity){ // update when sprites are made
		switch(roll){
			case -1:
				Modulate = new Color(0.2f,0.2f,0.2f, opacity); // black
				break;
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
		}
	}
}
