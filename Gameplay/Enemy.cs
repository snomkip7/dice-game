using Godot;
using System;

public partial class Enemy : Node2D
{
	// attributes:
	public int maxHealth;
	public float health;
	public int aiLevel;
	public int decisionTime; // in seconds
	public int dieSides = 6;
	public int handSize;
	public int decisionBuffer = 3; // maybe make changeable
	public string type = "none";
	public int reward = 1;

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
	public int[] heldRolls; // -1 if empty
	public Timer decisionTimer;
	public Timer effectTimer;
	public bool canRoll = true;
	public int roll = -1;
	public Gameplay game;
	public Vector2 healthBarStart = new Vector2(393.5f, 67.011f);
	public Vector2 healthBarStretch = new Vector2(5.461f, 0.485f); // change to 1,1 when real sprite exists
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
			if(ice){
				buffer += 5;
			}
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
			canRoll = false;
		} else{ // else make a decision on whether to play or hold
			makeDecision();
			canRoll = true;
		}
	}

	public void enemyRollDice(){ // decided when/where to call this function
		if(poison){
			health -= (int) (poisonInfo.X/3);
			poisonInfo.Y -= 1;
		}
		roll = new RandomNumberGenerator().RandiRange(1, dieSides);
		// play an animation
		updateDieSprite();
		if(ice){
			decisionTimer.Start(5); // change to when roll animation
		} else{
			decisionTimer.Start(1.5); // change to when roll animation
		}
		
	}

	public void makeDecision(){ // called ~every decisionTime seconds ***subject to change, feel free to change***
		// dont forget to do poison implemenation
		int decision = new RandomNumberGenerator().RandiRange(0,30); // max base decision is 30
		decision += aiLevel; // adjusting for smort enemies
		if(decision<25){ // dumbest move - play immediately
			if(poison){
				health -= (int) poisonInfo.X;
				poisonInfo.Y -= 1;
			}
			int[] rolls = new int[1];
			rolls[0] = roll;
			GetParent<Gameplay>().rollEffects(rolls, false);
			roll = -1;
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
		updateDieSprite();
	}

	public void playCombo(){ // plays a combo based on held rolls & current roll ***needs to be done***
		// dont forget poison implementation
		// to be done
		if(poison){
			health -= (int) poisonInfo.X;
			poisonInfo.Y -= 1;
		}
		int[] rolls = new int[1]; // REMOVE THIS LOGIC, THIS IS TEMPORARY
		rolls[0] = roll;
		GetParent<Gameplay>().rollEffects(rolls, false);
		roll = -1;
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

	public void updateDieSprite(){ // updates the sprite to the correct icon
		string effect = "none";
		if(roll>=1&&roll<=6){
			effect = GetParent<Gameplay>().enemyDieEffects[roll-1];
		}
		
		switch(effect){
			case "none":
				GetNode<Sprite2D>("EnemyDieFace").Visible = false;
				break;
			case "damage":
				GetNode<Sprite2D>("EnemyDieFace").Visible = true;
				GetNode<Sprite2D>("EnemyDieFace").Texture = (Texture2D) ResourceLoader.Load("res://Sprites/Dice/attackIcon.png");
				break;
			case "healing":
				GetNode<Sprite2D>("EnemyDieFace").Visible = true;
				GetNode<Sprite2D>("EnemyDieFace").Texture = (Texture2D) ResourceLoader.Load("res://Sprites/Dice/healIcon.png");
				break;
			case "poison":
				GetNode<Sprite2D>("EnemyDieFace").Visible = true;
				GetNode<Sprite2D>("EnemyDieFace").Texture = (Texture2D) ResourceLoader.Load("res://Sprites/Dice/poisonIcon.png");
				break;
			case "fire":
				GetNode<Sprite2D>("EnemyDieFace").Visible = true;
				GetNode<Sprite2D>("EnemyDieFace").Texture = (Texture2D) ResourceLoader.Load("res://Sprites/Dice/burnIcon.png");
				break;
			case "ice":
				GetNode<Sprite2D>("EnemyDieFace").Visible = true;
				GetNode<Sprite2D>("EnemyDieFace").Texture = (Texture2D) ResourceLoader.Load("res://Sprites/Dice/freezeIcon.png");
				break;
		}
	}
}
