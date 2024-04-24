using Godot;
using System;

public partial class Die : Area2D
{
	public bool canRoll = true;
	public int nextRoll = -1;
	public int sides = 6;
	public Gameplay gameplay;
	public Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameplay = GetParent<Gameplay>();
		player = gameplay.GetNode<Player>("Player");
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
				updateSprite();
			} else{ // deselects die if it cant roll and isnt selected
				GD.Print("reselect die");
				gameplay.dieSelected = true;
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
	}

	public void reset(){ // resets the die to a base value
		nextRoll = -1;
		updateSprite();
		canRoll = true;
	}

	public void rollEnded(){ // rn it starts after 1 sec, but should be made to start after animation ended
		nextRoll = new RandomNumberGenerator().RandiRange(1, sides); // randomly selects a number and updates sprite
		//canRole = false;
		GD.Print("roll ended");
		GD.Print("rolled a "+nextRoll);
		gameplay.rollMade(nextRoll);
		updateSprite();
	}

	public void updateSprite(){ // updates the sprite to the correct icon
		string effect = "none";
		if(nextRoll>=1&&nextRoll<=6){
			effect = gameplay.dieEffects[nextRoll-1];
		}
		
		switch(effect){
			case "none":
				GetNode<Sprite2D>("DieSprite").Visible = false;
				break;
			case "damage":
				GetNode<Sprite2D>("DieSprite").Visible = true;
				GetNode<Sprite2D>("DieSprite").Texture = (Texture2D) ResourceLoader.Load("res://Sprites/Dice/attackIcon.png");
				break;
			case "healing":
				GetNode<Sprite2D>("DieSprite").Visible = true;
				GetNode<Sprite2D>("DieSprite").Texture = (Texture2D) ResourceLoader.Load("res://Sprites/Dice/healIcon.png");
				break;
			case "poison":
				GetNode<Sprite2D>("DieSprite").Visible = true;
				GetNode<Sprite2D>("DieSprite").Texture = (Texture2D) ResourceLoader.Load("res://Sprites/Dice/poisonIcon.png");
				break;
			case "fire":
				GetNode<Sprite2D>("DieSprite").Visible = true;
				GetNode<Sprite2D>("DieSprite").Texture = (Texture2D) ResourceLoader.Load("res://Sprites/Dice/burnIcon.png");
				break;
			case "ice":
				GetNode<Sprite2D>("DieSprite").Visible = true;
				GetNode<Sprite2D>("DieSprite").Texture = (Texture2D) ResourceLoader.Load("res://Sprites/Dice/freezeIcon.png");
				break;
		}
		GetNode<Sprite2D>("SelectedSprite").Visible = gameplay.dieSelected;
	}
}
