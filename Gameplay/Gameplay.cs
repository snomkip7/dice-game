using Godot;
using Godot.Collections;
using System;

public partial class Gameplay : Node2D
{
	[Export]
	public PackedScene handItem;

	public Enemy enemy;
	public Player player;
	public HandItem[] handItems;
	public int handSize = 4; // will be changed with shop items probably
	public int numSelected = 0; // num of selected handItems
	public bool dieSelected = false;
	public string[] dieEffects = new string[7];
	public string[] enemyDieEffects = new string[7];
	public Dictionary<string, string> spellbook = new Dictionary<string, string>();
	public bool gameEnded;
	
	public override void _Ready()
	{
		handItems = new HandItem[handSize]; // setting things up
		instanceHandItems();
		enemy = GetNode<Enemy>("Enemy");
		player = GetNode<Player>("Player");
		// setup spellbook
		loadSpellbook();
	}

	public void loadSpellbook(){ // gets spellbook from txt, will be removed when globalVars has the spellbook
		var file = FileAccess.Open("user://Spellbook.txt", FileAccess.ModeFlags.Read);
		if(file==null){
			file = FileAccess.Open("res://TextFiles/Spellbook.txt", FileAccess.ModeFlags.Read);
		}
		string content = file.GetAsText();
		var json = new Json();
		var parsed = json.Parse(content);
        if (parsed != Error.Ok)
        {
            GD.Print($"JSON Parse Error: {json.GetErrorMessage()} in {content} at line {json.GetErrorLine()}");
        }
		spellbook = new Dictionary<string, string>((Dictionary) json.Data);
		file.Close();
	}
	
	public override void _Process(double delta)
	{
		if(enemy.health<=0&&!gameEnded){ // checking if player wins
			GD.Print("PLAYER WON");
			// globalVars.playerCoins += globalVars.enemyCoinValue
			// ui win banner??
			enemy.healthBar.Visible = false;
			GetNode<Button>("GameEndButton").Visible = true;
			GetNode<Button>("GameEndButton").GetNode<Sprite2D>("WinBanner").Visible = true;
			gameEnded = true;
		} else if(player.health<=0&&!gameEnded){ // checking if enemy wins
			GD.Print("YOU LOST, GET GOOD WHEN");
			// L BOZO 
			// ui lose banner?
			player.healthBar.Visible = false;
			GetNode<Button>("GameEndButton").Visible = true;
			GetNode<Button>("GameEndButton").GetNode<Sprite2D>("LoseBanner").Visible = true;
			gameEnded = true;
		}
	}

	public void instanceHandItems(){ // create hand, with formatting
		Vector2 firstHandPosition = new Vector2(88, 739); // can be changed, formatting starts with this as top left
		int downBy = 130; // downwards space between handItems
		int rightBy = 130; // right space between handItems

		for(int i = 0; i < handSize; i++){
			int down = 0; // counting reasons
			int right = i % 2; // 0 if left, 1 if right

			for(int d = i; d > 1; d-=2){
				down++;
			}

			HandItem item = (HandItem) handItem.Instantiate(); // adding the actual hand items & adjusting position
			AddChild(item);
			handItems[i] = item;
			item.Position = new Vector2(firstHandPosition.X + rightBy * right, firstHandPosition.Y + downBy * down);
		}
	}

	public void rollMade(int roll){ // after a roll is made, shows hold button & selects die
		// put up a ui msg or smth showing what you rolled
		GD.Print("You got a "+roll+" do you want to play or hold?");
		GetNode<Button>("HoldRollButton").Visible = true; // remember to add an option to play held rolls if hand is full
		GetNode<Sprite2D>("HoldButtonSprite").Visible = true;
		dieSelected = true;
	}

	public void playRoll(){ // finds all the things that are selected and calls useRoll
		// do the thing
		Die die = GetNode<Die>("Die");
		int roll = die.nextRoll;
		if(dieSelected){ // removing the hold button if die is played
			GetNode<Button>("HoldRollButton").Visible = false;
			GetNode<Sprite2D>("HoldButtonSprite").Visible = false;
		}
		int[] rolls;
		int iter = 0;
		if(dieSelected){ // adding the die if it is selected
			rolls = new int[numSelected+1];
			rolls[0] = roll;
			iter++;
		} else{
			rolls = new int[numSelected];
		}
		
		for(int i = 0; i < handSize; i++){ // adding all selected hand items and updating them to be empty
			if(handItems[i].selected){
				rolls[iter] = handItems[i].roll;
				handItems[i].selected = false;
				handItems[i].updateItem(-1);
				iter++;
			}
		}

		if(player.poison){ // poison dmg if needed
			player.health -= player.poisonInfo.X;
			player.poisonInfo.Y -= 1;
		}

		rollEffects(rolls, true); // calling the function that does the actual effects
		if(dieSelected){ // resetting die if needed
			dieSelected = false;
			die.reset();
		}
	}

	public void holdRoll(){ // adding a roll to the hand
		// do the thing
		Die die = GetNode<Die>("Die");
		int roll = die.nextRoll;
		GD.Print("Held a "+roll);

		bool held = false; // attempting to add the die to a handItem
		for(int i = 0; i < handSize && held == false; i++){
			if(handItems[i].full == false){
				handItems[i].updateItem(roll);
				held = true;
			}
		}
		if(held){ // resetting die & removing hold button if is sucessfully held
			GetNode<Button>("HoldRollButton").Visible = false;
			GetNode<Sprite2D>("HoldButtonSprite").Visible = false;
			dieSelected = false;
			die.reset();
		} else{ // if is unable to hold
			// do smth man - figure out what to do pls
			GD.Print("Cant hold!");
		}
		dieSelected = false; // deselects die 
	}

	public void rollEffects(int[] nums, bool atEnemy){ // applies effects of rolls & does combos and stuff
		string[] effects = new string[nums.Length];
		bool healing = false;
		bool damage = false;
		bool poison = false;
		bool fire = false;
		bool ice = false;
		for(int i=0;i<nums.Length;i++){ // setting bools based on used faces
			if(atEnemy){
				effects[i] = dieEffects[nums[i]-1];
			} else{
				effects[i] = enemyDieEffects[nums[i]-1];
			}
			if(effects[i]=="healing"){healing=true;}
			if(effects[i]=="damage"){damage=true;}
			if(effects[i]=="poison"){poison=true;}
			if(effects[i]=="fire"){fire=true;}
			if(effects[i]=="ice"){ice=true;}
		}
		if (healing){ // all this for healing
			if(poison&&spellbook["heal_psn"]=="unlocked"){ // heal + poison
				if(atEnemy){
					player.poison = false;
				} else{
					enemy.poison = false;
				}
				poison = false;
			}
			if(fire&&spellbook["heal_fire"]=="unlocked"){ // heal + fire
				if(atEnemy){
					player.fire = false;
				} else{
					enemy.fire = false;
				}
				fire = false;
			}
			if(ice&&spellbook["heal_ice"]=="unlocked"){ // heal + ice
				if(atEnemy){
					player.ice = false;
				} else{
					enemy.ice = false;
				}
				ice = false;
			}
			if(damage&&spellbook["heal_dmg"]=="unlocked"&&!poison&&!fire&&!ice){ // heal + damage + no extra effects
				if(atEnemy){
					player.health += getCount(effects, "healing") * 15 + getCount(effects, "damage") * 10;
					if(player.health>player.maxHealth){player.health=player.maxHealth;}
				} else{
					enemy.health += getCount(effects, "healing") * 15 + getCount(effects, "damage") * 10;
					if(enemy.health>enemy.maxHealth){enemy.health=enemy.maxHealth;}
				}
				damage = false;
			} else if(damage&&spellbook["heal_dmg_effect"]=="unlocked"){ // heal + damage + extra effects
				if(atEnemy){
					player.health += getCount(effects, "healing") * 10 + getCount(effects, "damage") * 5;
					if(player.health>player.maxHealth){player.health=player.maxHealth;}
				} else{
					enemy.health += getCount(effects, "healing") * 10 + getCount(effects, "damage") * 5;
					if(enemy.health>enemy.maxHealth){enemy.health=enemy.maxHealth;}
				}
				damage = false;
			}
			if(!damage&&!poison&&!fire&&!ice){ // regular healing
				if(atEnemy){
					player.health += getCount(effects, "healing") * 20;
					if(player.health>player.maxHealth){player.health=player.maxHealth;}
				} else{
					enemy.health += getCount(effects, "healing") * 20;
					if(enemy.health>enemy.maxHealth){enemy.health=enemy.maxHealth;}
				}
			}
		}
		 // effects without healing
		if((fire||(enemy.type=="fire"&&atEnemy))&&(ice||(enemy.type=="ice"&&atEnemy))&&spellbook["fire_ice"]=="unlocked"){ // melt
			if(atEnemy){
				enemy.meltInfo = new Vector2(5+getCount(effects, "fire")*5, 3+getCount(effects, "ice") * 5);
				enemy.melt = true;
			} else{
				player.meltInfo = new Vector2(5+getCount(effects, "fire")*5, 3+getCount(effects, "ice") * 5);
				player.melt = true;
			}
			fire = false;
			ice = false;
		} 
		if((poison||(enemy.type=="poison"&&atEnemy))&&(fire||(enemy.type=="fire"&&atEnemy))&&spellbook["psn_fire"]=="unlocked"){ // poison fire
			// poison fire implemenation (gotta think of smth)
			fire = false;
			poison = false;
		} 
		if((poison||(enemy.type=="poison"&&atEnemy))&&(ice||(enemy.type=="ice"&&atEnemy))&&spellbook["psn_ice"]=="unlocked"){ // poison ice
			// poison ice implemenation (gotta think of smth)
			ice = false;
			poison = false;
		} 
		if(damage&&poison&&spellbook["dmg_psn"]=="unlocked"){ // damage poison
			if(atEnemy){
				enemy.poisonInfo = new Vector2(15+getCount(effects, "damage")*3, getCount(effects, "poison") * 2);
				enemy.poison = true;
			} else{
				player.poisonInfo = new Vector2(15+getCount(effects, "damage")*3, getCount(effects, "poison") * 2);
				player.poison = true;
			}
			poison = false;
			damage = false;
		} 
		if(damage&&fire&&spellbook["dmg_fire"]=="unlocked"){ // damage fire
			if(atEnemy){
				enemy.fireInfo = new Vector2(5+getCount(effects, "damage")*3, getCount(effects, "fire") * 5);
				enemy.fire = true;
			} else{
				player.fireInfo = new Vector2(5+getCount(effects, "damage")*3, getCount(effects, "fire") * 5);
				player.fire = true;
			}
			fire = false;
			damage = false;
		} 
		if(damage&&ice&&spellbook["dmg_ice"]=="unlocked"){ // damage ice (just extends length cuz idk)
			if(atEnemy){
				enemy.iceInfo = getCount(effects, "ice")*10+getCount(effects, "damage")*5;
				enemy.ice = true;
			} else{
				player.iceInfo = getCount(effects, "ice")*10+getCount(effects, "damage")*5;
				player.ice = true;
			}
			ice = false;
			damage = false;
		}
		// applying effects not used
		if(poison){ // pure poison
			if(atEnemy){
				enemy.poisonInfo = new Vector2(15, getCount(effects, "poison") * 2);
				enemy.poison = true;
			} else{
				player.poisonInfo = new Vector2(15, getCount(effects, "poison") * 2);
				player.poison = true;
			}
		}
		if(fire){ // pure fire
			if(atEnemy){
				enemy.fireInfo = new Vector2(5, getCount(effects, "fire") * 5);
				enemy.fire = true;
			} else{
				player.fireInfo = new Vector2(5, getCount(effects, "fire") * 5);
				player.fire = true;
			}
		}
		if(ice){ // pure ice
			if(atEnemy){
				enemy.iceInfo = getCount(effects, "ice")*10;
				enemy.ice = true;
			} else{
				player.iceInfo = getCount(effects, "ice")*10;
				player.ice = true;
			}
		}
		if(damage){ // pure dmg
			if(atEnemy){
				enemy.health -= getCount(effects, "damage")*10;
			} else{
				player.health -= getCount(effects, "damage")*10;
			}
		}
	}

	public int getCount(string[] effects, string effect){ // returns the amount of times effect appears in effects
		int count = 0;
		for(int i=0;i<effects.Length;i++){
			if(effects[i]==effect){
				count++;
			}
		}
		return count;
	}


	public void unSelect(){ // unselects all hand items
		for(int i = 0; i < handSize; i++){
			handItems[i].selected = false;
			handItems[i].updateSprite();
		}
		numSelected = 0;
	}

	public void gameEnd(){
		GetTree().ChangeSceneToFile("res://MainMenu.tscn");
	}
}
