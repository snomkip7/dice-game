using Godot;
using System;

public partial class Enemy : Node2D
{
	// attributes:
	public int maxHealth;
	public float health;
	public int aiLevel;
	public int decisionTime; // in seconds
	public int dieSides;
	public int handSize;
	public int decisionBuffer = 5; // maybe make changeable
	public string type = "none";
	public int reward = 1;

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
	public int[] heldRolls; // -1 if empty
	public Timer decisionTimer;
	public Timer effectTimer;
	public bool canRoll = true;
	public int roll = -1;
	public Gameplay game;
	public Vector2 healthBarStart = new Vector2(235, 50);
	public Vector2 healthBarStretch = new Vector2(3.258f, 0.289f); // change to 1,1 when real sprite exists
	public Sprite2D healthBar;

	public override void _Ready()
	{
		game = GetParent<Gameplay>();
		// set all those attributes based on text file, also add a sprite
		// fill out enemyDieEffects in gameplay
		loadInfoFromTxt();
		heldRolls = new int[handSize];
		for(int i = 0; i < handSize;i++){
			heldRolls[i] = -1;
		}
		decisionTimer = GetNode<Timer>("DecisionTimer");
		effectTimer = GetNode<Timer>("EffectTimer");
		healthBar = GetNode<Sprite2D>("HealthBarForeground");
	}

	public void loadInfoFromTxt(){
		FileAccess file = FileAccess.Open("user://EnemyPath.txt", FileAccess.ModeFlags.Read);
		if(file==null){
			GD.Print("cant find file path, default enemy time");
			file = FileAccess.Open("res://TextFiles/Enemies/DefaultEnemy.txt", FileAccess.ModeFlags.Read);
			if(file==null){
				GD.Print("Bruh the backup is wrong");
			}
		} else{
			string path = "res://TextFiles/" + file.GetLine()+".txt";
			file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
			if(file==null){
				GD.Print("bruh the path is wrong");
			}
		}
		maxHealth = Convert.ToInt32(file.GetLine());
		health = maxHealth;
		game.enemyDieEffects[0] = file.GetLine();
		game.enemyDieEffects[1] = file.GetLine();
		game.enemyDieEffects[2] = file.GetLine();
		game.enemyDieEffects[3] = file.GetLine();
		game.enemyDieEffects[4] = file.GetLine();
		game.enemyDieEffects[5] = file.GetLine();
		game.enemyDieEffects[6] = file.GetLine(); // should be blank if nothing in that slot
		type = file.GetLine();
		reward = Convert.ToInt32(file.GetLine());
		aiLevel = Convert.ToInt32(file.GetLine());
		decisionTime = Convert.ToInt32(file.GetLine());
		handSize = Convert.ToInt32(file.GetLine());
		// decisionBuffer = Convert.ToInt32(file.GetLine());
		file.Close();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(decisionTimer.TimeLeft == 0){ // starts decision time with a randomish time (with buffer)
			int buffer = new RandomNumberGenerator().RandiRange(-decisionBuffer, decisionBuffer);
			decisionTimer.Start(decisionTime+buffer);
		}
		if(effectTimer.TimeLeft == 0){ // calls dmg calculations
			effectTimer.Start(1);
		}
		// updating health bar
		float healthPercent = health / maxHealth;
		healthBar.Scale = new Vector2(healthBarStretch.X*healthPercent, healthBarStretch.Y);
		healthBar.Position = new Vector2(healthBarStart.X - ((healthBar.Texture.GetSize().X * (healthBarStretch.X-healthBar.Scale.X))/2), healthBarStart.Y);
	}

	public void rollOrPlay(){ // called by decision timer
		if(canRoll){ // if it can roll, then roll
			enemyRollDice();
		} else{ // else make a decision on whether to play or hold
			makeDecision();
			canRoll = true;
		}
	}

	public void enemyRollDice(){ // decided when/where to call this function
		roll = new RandomNumberGenerator().RandiRange(1, dieSides);
		// play an animation
		//updateDieSprite(roll)
		decisionTimer.Start(1.5); // change to when roll animation
	}

	public void makeDecision(){ // called ~every decisionTime seconds ***subject to change, feel free to change***
		int decision = new RandomNumberGenerator().RandiRange(0,30); // max base decision is 30
		decision += aiLevel; // adjusting for smort enemies
		if(decision<25){ // dumbest move - play immediately
			int[] rolls = new int[1];
			rolls[0] = roll;
			GetParent<Gameplay>().useRoll(rolls, false);
		} else{ // smart move - hold roll for combo
			bool held = false; // try to hold the die
			for(int i = 0; i < handSize && held==false; i++){
				if(heldRolls[i]==-1){
					heldRolls[i] = roll;
					held = true;
				}
			}
			playCombo();
			if(!held&&roll!=-1){ // holding die if it failed to hold earlier (and didn't play the die)
				for(int i = 0; i < handSize && held==false; i++){
					if(heldRolls[i]==-1){
						heldRolls[i] = roll;
						held = true;
						roll = -1;
					}
				}
			}
		}
	}

	public void playCombo(){ // plays a combo based on held rolls & current roll ***needs to be done***
		// to be done
	}

	public void dmgCalculation(){
		GD.Print("dmg calc");
		float healthCheck = health;
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
		} else{
			GD.Print("should be no effects?");
		}
		if(healthCheck!=health){
			GD.Print("Enemy health down to "+health+" from "+healthCheck);
		}
	}
}
