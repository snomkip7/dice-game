using Godot;
using System;

public partial class Player : Node2D
{
	// attributes:
	public float health;

	// effects:
	public Vector2 poisonInfo = new Vector2(0,0); // first=dmg, second=times
	public bool poison = false;
	public Vector2 fireInfo = new Vector2(0,0); // first=dmg, second=time
	public bool fire = false;
	public int iceInfo = 0; // time
	public bool ice = false;
	public Vector2 thawInfo = new Vector2(0,0); // first=dmg, second = time
	public bool thaw = false;

	// other stuff
	public Gameplay game;
	public Timer effectTimer;
	public Vector2 healthBarStart = new Vector2(1344, 960);
	public Sprite2D healthBar;
	public Sprite2D fireSprite;
	public Sprite2D iceSprite;
	public Sprite2D poisonSprite;
	public Sprite2D thawSprite;

	public override void _Ready()
	{
		game = GetParent<Gameplay>();
		effectTimer = GetNode<Timer>("EffectTimer");
		healthBar = GetNode<Sprite2D>("HealthBarCover");
		globalVariables globalVars = GetNode<globalVariables>("/root/GlobalVariables");
		health = globalVars.maxHealth;
		fireSprite = GetNode<Sprite2D>("FireSprite");
		iceSprite = GetNode<Sprite2D>("IceSprite");
		poisonSprite = GetNode<Sprite2D>("PoisonSprite");
		thawSprite = GetNode<Sprite2D>("ThawSprite");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(effectTimer.TimeLeft == 0){ // starts poison effect if applied
			effectTimer.Start(1);
		}
		// updating health bar
		float healthPercent = health / game.globalVars.maxHealth;
		healthBar.Scale = new Vector2(1-healthPercent, 1);
		healthBar.Position = new Vector2(healthBarStart.X - ((healthBar.Texture.GetSize().X *(1-healthPercent))/2), healthBarStart.Y);
		// effect icons
	}

	public void dmgCalculation(){
		if(poison){ // poison
			GD.Print("poisoned");
			poisonSprite.Visible = true;
			// now to decrease timers
			poisonInfo.Y -= 1;
			if(poisonInfo.Y==0){
				poison = false;
				poisonSprite.Visible = false;
			}
		}
		if(fire){ // fire
			GD.Print("ah, thats hot");
			health -= fireInfo.X;
			// put some ui indicator stuff?
			fireSprite.Visible = true;
			// now to decrease timers
			fireInfo.Y -= 1;
			if(fireInfo.Y==0){
				fire = false;
				fireSprite.Visible = false;
			}
		}
		if(ice){ // ice
			GD.Print("antartica moment");
			iceSprite.Visible = true;
			// now to decrease timers
			iceInfo -= 1;
			if(iceInfo==0){
				ice = false;
				iceSprite.Visible = false;
			}
		}
		if(thaw){
			GD.Print("bros meltin under the pressure");
			thawSprite.Visible = true;
			health -= thawInfo.X;
			thawInfo.Y -= 1;
			if(thawInfo.Y==0){
				thaw = false;
				thawSprite.Visible = false;
			}
		}
	}
}
