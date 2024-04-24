using Godot;
using System;

public partial class HandItem : Area2D
{
	public bool selected = false;
	public bool full = false;
	public int roll = -1; // no roll

	public override void _Ready()
	{
		updateSprite(); // starting value
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void select(Node viewport, InputEvent @event, int shape_idx){ // input event
		if(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed == true && roll != -1){
			selected = !selected; 
			if(selected){ // changing stuff depending on if its selected or nah
				updateSprite();
				GetParent<Gameplay>().numSelected += 1;
			} else{
				updateSprite();
				GetParent<Gameplay>().numSelected -= 1;
			}
			GD.Print(selected+" and all are "+GetParent<Gameplay>().numSelected);
		}
	}

	public void updateItem(int roll){ // used when changing the value of the handItem
		this.roll = roll;
		full = true;
		updateSprite();
		if(roll==-1){ // -1 is used to reset to default
			selected = false;
			full = false;
			GetParent<Gameplay>().numSelected -= 1;
		}
	}

	public void updateSprite(){ // updates the sprite to the correct icon
		string effect = "none";
		if(roll>=1&&roll<=6){
			effect = GetParent<Gameplay>().dieEffects[roll-1];
		}
		
		switch(effect){
			case "none":
				GetNode<Sprite2D>("HandSprite").Visible = false;
				break;
			case "damage":
				GetNode<Sprite2D>("HandSprite").Visible = true;
				GetNode<Sprite2D>("HandSprite").Texture = (Texture2D) ResourceLoader.Load("res://Sprites/Dice/attackIcon.png");
				break;
			case "healing":
				GetNode<Sprite2D>("HandSprite").Visible = true;
				GetNode<Sprite2D>("HandSprite").Texture = (Texture2D) ResourceLoader.Load("res://Sprites/Dice/healIcon.png");
				break;
			case "poison":
				GetNode<Sprite2D>("HandSprite").Visible = true;
				GetNode<Sprite2D>("HandSprite").Texture = (Texture2D) ResourceLoader.Load("res://Sprites/Dice/poisonIcon.png");
				break;
			case "fire":
				GetNode<Sprite2D>("HandSprite").Visible = true;
				GetNode<Sprite2D>("HandSprite").Texture = (Texture2D) ResourceLoader.Load("res://Sprites/Dice/burnIcon.png");
				break;
			case "ice":
				GetNode<Sprite2D>("HandSprite").Visible = true;
				GetNode<Sprite2D>("HandSprite").Texture = (Texture2D) ResourceLoader.Load("res://Sprites/Dice/freezeIcon.png");
				break;
		}
		GetNode<Sprite2D>("SelectedSprite").Visible = selected;
	}
	
}
