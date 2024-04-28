using Godot;
using System;

public partial class Player : Node2D
{
	// attributes:
	public int maxHealth;
	public float health;

	// effects:
	public Vector2 poisonInfo = new Vector2(0,0); // first=dmg, second=times
	public bool poison = false;
	public Vector2 fireInfo = new Vector2(0,0); // first=dmg, second=time
	public bool fire = false;
	public int iceInfo = 0; // time
	public bool ice = false;
	public Vector2 meltInfo = new Vector2(0,0); // first=dmg, second = time
	public bool melt = false;

	// other stuff
	public Gameplay game;
	public Timer effectTimer;
	public Vector2 healthBarStart = new Vector2(1344, 960);
	public Sprite2D healthBar;

	public override void _Ready()
	{
		game = GetParent<Gameplay>();
		effectTimer = GetNode<Timer>("EffectTimer");
		healthBar = GetNode<Sprite2D>("HealthBarCover");
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
		healthBar.Scale = new Vector2(1-healthPercent, 1);
		healthBar.Position = new Vector2(healthBarStart.X - ((healthBar.Texture.GetSize().X *(1-healthPercent))/2), healthBarStart.Y);
	}

	public void dmgCalculation(){
		if(fire){ // fire
			GD.Print("ah, thats hot");
			health -= fireInfo.X;
			// put some ui indicator stuff?
			// now to decrease timers
			fireInfo.Y -= 1;
			if(fireInfo.Y==0){
				fire = false;
			}
		}
		if(ice){ // ice
			GD.Print("antartica moment");
			// now to decrease timers
			iceInfo -= 1;
			if(iceInfo==0){
				ice = false;
			}
		}
		if(melt){
			GD.Print("bros meltin under the pressure");
			meltInfo.Y -= 1;
			if(meltInfo.Y==0){
				melt = false;
			}
		}
	}
}
