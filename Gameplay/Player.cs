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
	public Vector2 healthBarStart = new Vector2(554, 606);
	public Vector2 healthBarStretch = new Vector2(3.258f, 0.289f); // change to 1,1 when real sprite exists
	public Sprite2D healthBar;

	public override void _Ready()
	{
		game = GetParent<Gameplay>();
		effectTimer = GetNode<Timer>("EffectTimer");
		healthBar = GetNode<Sprite2D>("HealthBarForeground");
		// read attributes from text files
		loadInfoFromTxt();
		// add a thing to make the die have the right amount of sides
		// game.die.sides = sideNum
		// fill out dieEffects in gameplay
	}

	public void loadInfoFromTxt(){
		var file = FileAccess.Open("user://Player.txt", FileAccess.ModeFlags.Read);
		if(file==null){
			file = FileAccess.Open("res://TextFiles/DefaultPlayer.txt", FileAccess.ModeFlags.Read);
		}
		maxHealth = Convert.ToInt32(file.GetLine());
		health = maxHealth;
		game.dieEffects[0] = file.GetLine();
		game.dieEffects[1] = file.GetLine();
		game.dieEffects[2] = file.GetLine();
		game.dieEffects[3] = file.GetLine();
		game.dieEffects[4] = file.GetLine();
		game.dieEffects[5] = file.GetLine();
		game.dieEffects[6] = file.GetLine(); // should be blank if nothing in that slot
		game.handSize = Convert.ToInt32(file.GetLine());
		file.Close();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(effectTimer.TimeLeft == 0){ // starts poison effect if applied
			effectTimer.Start(1);
		}
		// updating health bar
		float healthPercent = health / maxHealth;
		healthBar.Scale = new Vector2(healthBarStretch.X*healthPercent, healthBarStretch.Y);
		healthBar.Position = new Vector2(healthBarStart.X - ((healthBar.Texture.GetSize().X * (healthBarStretch.X-healthBar.Scale.X))/2), healthBarStart.Y);
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
