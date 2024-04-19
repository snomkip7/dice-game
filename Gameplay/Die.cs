using Godot;
using System;

public partial class Die : Area2D
{
	public bool canRoll = true;
	public int nextRoll = -1;
	public int sides = 6;
	public Gameplay gameplay;
	public float opacity = 1;
	public Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameplay = GetParent<Gameplay>();
		player = GetNode<Player>("Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void rollDice(Node viewport, InputEvent @event, int shape_idx){ // input event
		if(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed == true){
			if(canRoll){ // roll the die if nothing else is going on
				GD.Print("ROLL THE DICE");
				gameplay.unSelect();
				roll();
			} else if(canRoll==false&&gameplay.dieSelected){ // deselects die if it cant role and die is selected
				GD.Print("deselect die");
				gameplay.dieSelected = false;
				opacity = 1;
				updateSprite();
			} else{ // deselects die if it cant roll and isnt selected
				GD.Print("reselect die");
				gameplay.dieSelected = true;
				opacity = .8f;
				updateSprite();
			}
		}
	}

	public void roll(){ // starts the rolling animation (when there is an animation)
		if(player.poison){ // poison dmg if needed
			player.health-= (int) (player.poisonInfo.X/3);
			player.poisonInfo.Y -= 1;
		}
		canRoll = false;
		GetNode<Timer>("RollTimer").Start(); // replace timer with animation, with an if/else for slow animation if ice
		GD.Print("do be rollin");
		opacity = 0.8f; // for dice sprite and selection, remove once selection ui exists
	}

	public void reset(){ // resets the die to a base value
		nextRoll = -1;
		opacity = 1;
		updateSprite();
		canRoll = true;
	}

	public void rollEnded(){ // rn it starts after 1 sec, but should be made to start after animation ended
		nextRoll = new RandomNumberGenerator().RandiRange(1, sides); // randomly selects a number and updates sprite
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
