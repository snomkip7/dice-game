using Godot;
using System;

public partial class Player : Node2D
{
	// attributes:
	public int maxHealth;
	public float health;

	// effects:
	public Vector2 poisonInfo = new Vector2(0,0); // first=dmg, second=time
	public bool poison = false;
	public Vector2 fireInfo = new Vector2(0,0); // first=dmg, second=time
	public bool fire = false;
	public Vector2 iceInfo = new Vector2(0,0); // first=dmg, second=time
	public bool ice = false;
	// public Vector2 timeSlow = new Vector2(0,0); // first=amount, second=time
	// public Vector2 extraDice = new Vector2(0,0); // first=amount, second=time
	// public Vector2 reroll = new Vector2(0,0); // first=amount, second=time

	// other stuff
	public Gameplay game;
	public Timer effectTimer;

	public override void _Ready()
	{
		game = GetParent<Gameplay>();
		effectTimer = GetNode<Timer>("EffectTimer");
		// read attributes from text files
		// add a thing to make the die have the right amount of sides
		// game.die.sides = sideNum
		// fill out dieEffects in gameplay
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(effectTimer.TimeLeft == 0){ // starts poison effect if applied
			effectTimer.Start(1);
		}
	}

	public void dmgCalculation(){
		// its if else tree time B)
		if(poison&fire&ice){ // poison + fire + ice
			GD.Print("yikes poison fire & ice");
			health -= (poisonInfo.X+fireInfo.X+iceInfo.X)*3; // temporary, need an actual effect
			// put some ui indicator stuff?
			// now to decrease timers
			poisonInfo.Y -= 1;
			fireInfo.Y -= 1;
			iceInfo.Y -= 1;
		} else if(poison&fire){ // poison + fire
			GD.Print("poison + fire = bad time");
			health -= (poisonInfo.X+fireInfo.X)*2; // temporary, need an actual effect
			// put some ui indicator stuff?
			// now to decrease timers
			poisonInfo.Y -= 1;
			fireInfo.Y -= 1;
		} else if(poison&ice){ // poison + ice
			GD.Print("poisoned ice not great");
			health -= (poisonInfo.X+iceInfo.X)*2; // temporary, need an actual effect
			// put some ui indicator stuff?
			// now to decrease timers
			poisonInfo.Y -= 1;
			iceInfo.Y -= 1;
		} else if(fire&ice){ // fire + ice
			GD.Print("bros meltin with fire & ice");
			health -= (fireInfo.X+iceInfo.X)*2; // temporary, need an actual effect
			// put some ui indicator stuff?
			// now to decrease timers
			fireInfo.Y -= 1;
			iceInfo.Y -= 1;
		} else if(poison){ // poison
			GD.Print("that tasted funny...");
			health -= poisonInfo.X; // temporary, need an actual effect
			// put some ui indicator stuff?
			// now to decrease timers
			poisonInfo.Y -= 1;
		} else if(fire){ // fire
			GD.Print("ah, thats hot");
			health -= fireInfo.X; // temporary, need an actual effect
			// put some ui indicator stuff?
			// now to decrease timers
			fireInfo.Y -= 1;
		} else if(ice){ // ice
			GD.Print("antartica moment");
			health -= iceInfo.X; // temporary, need an actual effect
			// put some ui indicator stuff?
			// now to decrease timers
			iceInfo.Y -= 1;
		}
	}
}
