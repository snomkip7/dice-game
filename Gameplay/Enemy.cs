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
	public String name;

	// effects:
	public Vector2 poisonInfo = new Vector2(0,0); // first=dmg, second=time
	public bool poison = false;
	public Vector2 fireInfo = new Vector2(0,0); // first=dmg, second=time
	public bool fire = false;
	public int iceInfo = 0; // time
	public bool ice = false;
	public Vector2 thawInfo = new Vector2(0,0); // first=dmg, second = time
	public bool thaw = false;

	// other stuff
	public int[] heldRolls; // -1 if empty
	public Timer decisionTimer;
	public Timer effectTimer;
	public bool canRoll = true;
	public int roll = -1;
	public Gameplay game;
	public Vector2 healthBarStart = new Vector2(812.5f, 68);
	public Sprite2D healthBar;
	public Sprite2D fireSprite;
	public Sprite2D iceSprite;
	public Sprite2D poisonSprite;
	public Sprite2D thawSprite;
	public Player player;

	public override void _Ready()
	{
		game = GetParent<Gameplay>();
		// set all those attributes based on text file, also add a sprite
		// fill out enemyDieEffects in gameplay
		getInfo();
		heldRolls = new int[handSize];
		for(int i = 0; i < handSize;i++){
			heldRolls[i] = -1;
		}
		decisionTimer = GetNode<Timer>("DecisionTimer");
		effectTimer = GetNode<Timer>("EffectTimer");
		healthBar = GetNode<Sprite2D>("HealthBarCover");
		fireSprite = GetNode<Sprite2D>("FireSprite");
		iceSprite = GetNode<Sprite2D>("IceSprite");
		poisonSprite = GetNode<Sprite2D>("PoisonSprite");
		thawSprite = GetNode<Sprite2D>("ThawSprite");
		player = GetNode<Player>("../Player");
	}

	// public void loadInfoFromTxt(){
	// 	FileAccess file = FileAccess.Open("user://EnemyPath.txt", FileAccess.ModeFlags.Read);
	// 	if(file==null){
	// 		GD.Print("cant find file path, default enemy time");
	// 		file = FileAccess.Open("res://TextFiles/Enemies/DefaultEnemy.txt", FileAccess.ModeFlags.Read);
	// 		if(file==null){
	// 			GD.Print("Bruh the backup is wrong");
	// 		}
	// 	} else{
	// 		string path = "res://TextFiles/" + file.GetLine()+".txt";
	// 		file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
	// 		if(file==null){
	// 			GD.Print("bruh the path is wrong");
	// 		}
	// 	}
	// 	maxHealth = Convert.ToInt32(file.GetLine());
	// 	health = maxHealth;
	// 	game.enemyDieEffects[0] = file.GetLine();
	// 	game.enemyDieEffects[1] = file.GetLine();
	// 	game.enemyDieEffects[2] = file.GetLine();
	// 	game.enemyDieEffects[3] = file.GetLine();
	// 	game.enemyDieEffects[4] = file.GetLine();
	// 	game.enemyDieEffects[5] = file.GetLine();
	// 	game.enemyDieEffects[6] = file.GetLine(); // should be blank if nothing in that slot
	// 	type = file.GetLine();
	// 	reward = Convert.ToInt32(file.GetLine());
	// 	aiLevel = Convert.ToInt32(file.GetLine());
	// 	decisionTime = Convert.ToInt32(file.GetLine());
	// 	handSize = Convert.ToInt32(file.GetLine());
	// 	// decisionBuffer = Convert.ToInt32(file.GetLine());
	// 	file.Close();
	// }

	public void getInfo(){
		globalVariables globalVars = GetNode<globalVariables>("/root/GlobalVariables");
		maxHealth = globalVars.enemyMaxHealth;
		health = maxHealth;
		type = globalVars.enemyType;
		reward = globalVars.enemyReward;
		aiLevel = globalVars.aiLevel;
		decisionTime = globalVars.decisionTime;
		handSize = globalVars.enemyHandSize;
		name=globalVars.enemyName;
		RandomNumberGenerator rng = new RandomNumberGenerator();
		game.enemyDieEffects[0] = globalVars.effects[0];
		game.enemyDieEffects[1] = globalVars.effects[rng.RandiRange(0,4)];
		game.enemyDieEffects[2] = globalVars.effects[rng.RandiRange(0,4)];
		game.enemyDieEffects[3] = globalVars.effects[rng.RandiRange(0,4)];
		game.enemyDieEffects[4] = globalVars.effects[rng.RandiRange(0,4)];
		game.enemyDieEffects[5] = globalVars.effects[rng.RandiRange(0,4)];
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
		healthBar.Scale = new Vector2(1-healthPercent, 1);
		healthBar.Position = new Vector2(healthBarStart.X - ((healthBar.Texture.GetSize().X *(1-healthPercent))/2), healthBarStart.Y);
		}

	public void rollOrPlay(){ // called by decision timer
		if(ice){
			return;
		}
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
		}
		roll = new RandomNumberGenerator().RandiRange(1, dieSides);
		// play an animation
		updateDieSprite();
		decisionTimer.Start(1.5); // change to when roll animation
		
	}

	public void makeDecision(){ // called ~every decisionTime seconds ***subject to change, feel free to change***
		// dont forget to do poison implemenation
		int decision = new RandomNumberGenerator().RandiRange(0,30); // max base decision is 30
		decision += aiLevel; // adjusting for smort enemies
		if(decision<25){ // dumbest move - play immediately
			if(poison){
				health -= (int) poisonInfo.X;
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
						game.actionLog.Text +="\n"+name+" added a "+game.enemyDieEffects[roll]+" to their hand.";
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
		}
		// int[] rolls = new int[1]; // REMOVE THIS LOGIC, THIS IS TEMPORARY
		// rolls[0] = roll;

		bool tempPoison = false;
		bool tempDamage = false;
		bool tempHealing = false;
		bool tempIce = false;
		bool tempFire = false;

		for(int i=0;i<handSize;i++){
			if(heldRolls[i]!=-1){
				string effect = game.enemyDieEffects[heldRolls[i]];
				if(effect=="healing"){tempHealing=true;}
				if(effect=="damage"){tempDamage=true;}
				if(effect=="poison"){tempPoison=true;}
				if(effect=="fire"){tempFire=true;}
				if(effect=="ice"){tempIce=true;}
			}
		}
		string rollEffect = game.enemyDieEffects[roll];
		if(rollEffect=="healing"){tempHealing=true;}
		if(rollEffect=="damage"){tempDamage=true;}
		if(rollEffect=="poison"){tempPoison=true;}
		if(rollEffect=="fire"){tempFire=true;}
		if(rollEffect=="ice"){tempIce=true;}

		if(tempHealing&&tempPoison&&tempDamage){
			string[] effects = new string[3];
			effects[0]="healing";
			effects[1]="damage";
			effects[2]="poison";
			callRoll(effects);
		}
		else if(tempHealing&&tempIce&&ice){
			string[] effects = new string[2];
			effects[0]="healing";
			effects[1]="ice";
			callRoll(effects);
		}
		else if(tempHealing&&tempFire&&fire){
			string[] effects = new string[2];
			effects[0]="healing";
			effects[1]="fire";
			callRoll(effects);
		}
		else if(tempHealing&&tempPoison&&poison){
			string[] effects = new string[2];
			effects[0]="healing";
			effects[1]="poison";
			callRoll(effects);
		}
		else if(tempIce&&player.fire){
			string[] effects = new string[1];
			effects[0]="ice";
			callRoll(effects);
		}
		else if(tempFire&&player.ice){
			string[] effects = new string[1];
			effects[0]="ice";
			callRoll(effects);
		}
		else if(tempDamage&&tempPoison){
			string[] effects = new string[2];
			effects[0]="damage";
			effects[1]="poison";
			callRoll(effects);
		}
		else if(tempDamage&&tempFire){
			string[] effects = new string[2];
			effects[0]="damage";
			effects[1]="fire";
			callRoll(effects);
		}
		else if(tempDamage&&tempIce){
			string[] effects = new string[2];
			effects[0]="damage";
			effects[1]="ice";
			callRoll(effects);
		}
		else if(tempDamage&&tempHealing&&(maxHealth-health>20)){
			string[] effects = new string[2];
			effects[0]="damage";
			effects[1]="healing";
			callRoll(effects);
		} else{
			bool freeSlot = false;
			for(int i=0; i<handSize&&!freeSlot;i++){
				if(heldRolls[i]==-1){
					freeSlot = true;
					heldRolls[i]=roll;
					roll=-1;
				}
			}
			if(!freeSlot){
				int[] rolls = new int[1];
				rolls[0] = roll;
				roll = -1;
				game.rollEffects(rolls, false);
			}
		}
	}

	public void callRoll(string[] effects){
		string nums = "";
		for(int i=0;i<handSize;i++){
			for(int j=0;j<effects.Length;j++){
				if(heldRolls[i]!=-1&&effects[j]==game.enemyDieEffects[heldRolls[i]]){
					nums += heldRolls[i];
					heldRolls[i] = -1;
				}
			}
		}
		for(int k=0;k<effects.Length;k++){
			if(effects[k]==game.enemyDieEffects[roll-1]){
				nums+=roll;
				roll = -1;
			}
		}
		int[] rolls = new int[nums.Length];
		for(int i=0;i<rolls.Length;i++){
			rolls[i]=Convert.ToInt32(nums[i]);
		}
		game.rollEffects(rolls, false);
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
