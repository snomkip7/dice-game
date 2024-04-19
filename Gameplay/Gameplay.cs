using Godot;
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
	
	public override void _Ready()
	{
		handItems = new HandItem[handSize]; // setting things up
		instanceHandItems();
		enemy = GetNode<Enemy>("Enemy");
		player = GetNode<Player>("Player");
	}

	
	public override void _Process(double delta)
	{
	}

	public void instanceHandItems(){ // create hand, with formatting
		Vector2 firstHandPosition = new Vector2(81, 369); // can be changed, formatting starts with this as top left
		int downBy = 100; // downwards space between handItems
		int rightBy = 100; // right space between handItems

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
		dieSelected = true;
	}

	public void playRoll(){ // finds all the things that are selected and calls useRoll
		// do the thing
		Die die = GetNode<Die>("Die");
		int roll = die.nextRoll;
		if(dieSelected){ // removing the hold button if die is played
			GetNode<Button>("HoldRollButton").Visible = false;
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
				handItems[i].updateItem(-1);
				iter++;
			}
		}

		useRoll(rolls, true);
		if(dieSelected){ // resetting die if needed
			die.reset();
			dieSelected = false;
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
			die.reset();
		} else{ // if is unable to hold
			// do smth man - figure out what to do pls
			GD.Print("Cant hold!");
		}
		dieSelected = false; // deselects die 
	}

	public void useRoll(int[] nums, bool atEnemy){ // calls the apply effect/heal effect for each effect
		GD.Print("use: ");
		int boost = nums.Length;
		bool healing = false;
		bool extraDmg = false;
		bool extraEffects = false;
		for(int i = 0; i < nums.Length; i++){ // checking if healing or extra damage needs to be true
			GD.Print(nums[i]);
			// if(atEnemy){GD.Print("at enemy: "+ nums[i]+ " - "+ dieEffects[nums[i]-1]);}
			// if(!atEnemy){GD.Print("at player: "+nums[i]+ " - "+ enemyDieEffects[nums[i]-1]);}
			string effect;
			if(atEnemy){
				effect = dieEffects[nums[i]-1];
			} else{
				effect = enemyDieEffects[nums[i]-1];
			}
			if(effect == "healing"){
				healing = true;
				GD.Print("Healin Time");
			} else if(effect == "damage"){
				extraDmg = true;
				GD.Print("DMG moment");
			} else{
				extraEffects = true;
				GD.Print("Extra Effects");
			}
		}
		for(int i=0;i<nums.Length;i++){ // doing the applying of effects
			string effect;
			if(atEnemy){
				effect = dieEffects[nums[i]-1];
			} else{
				effect = enemyDieEffects[nums[i]-1];
			}
			if(effect=="poison"){ // poison moment
				GD.Print("poison");
				if(healing){
					if(atEnemy){ 
						healEffect(player.poison, player.poisonInfo);
					} else{
						healEffect(enemy.poison, enemy.poisonInfo);
					}
				} else{
					if(atEnemy){
						applyEffect(enemy.poison, enemy.poisonInfo, extraDmg, boost);
					} else{
						applyEffect(player.poison, player.poisonInfo, extraDmg, boost);
					}
				}
			}
			if(effect=="fire"){ // fire moment
				GD.Print("fire");
				if(healing){
					if(atEnemy){
						healEffect(player.fire, player.fireInfo);
					} else{
						healEffect(enemy.fire, enemy.fireInfo);
					}
				} else{
					if(atEnemy){
						applyEffect(enemy.fire, enemy.fireInfo, extraDmg, boost);
					} else{
						applyEffect(player.fire, player.fireInfo, extraDmg, boost);
					}
				}
			}
			if(effect=="ice"){ // ice moment
				GD.Print("ice");
				if(healing){
					if(atEnemy){
						healEffect(player.ice, player.iceInfo);
					} else{
						healEffect(enemy.ice, enemy.iceInfo);
					}
				} else{
					if(atEnemy){
						applyEffect(enemy.ice, enemy.iceInfo, extraDmg, boost);
					} else{
						applyEffect(player.ice, player.iceInfo, extraDmg, boost);
					}
				}
			}
			// if you want to add an effect, add it here (also edit functions so combos work)
		}
		if(healing&&extraDmg&&!extraEffects){
			GD.Print("Heal + extradmg");
			if(atEnemy){ // temp +30 value
				if(player.maxHealth>player.health+30){ // making sure it wont go over max health
					player.health = player.maxHealth;
				} else{
					player.health += 30;	
				}
			} else{
				if(enemy.maxHealth>enemy.health+30){ // making sure it wont go over max health
					enemy.health = enemy.maxHealth;
				} else{
					enemy.health += 30;	
				}
			}
		}
		else if(healing&&!extraEffects){
			GD.Print("heal");
			if(atEnemy){ // temp +15 value
				if(player.maxHealth>player.health+15){ // making sure it wont go over max health
					player.health = player.maxHealth;
				} else{
					player.health += 15;	
				}
			} else{
				if(enemy.maxHealth>enemy.health+5){ // making sure it wont go over max health
					enemy.health = enemy.maxHealth;
				} else{
					enemy.health += 15;	
				}
			}
		}
		else if(extraDmg&&!extraEffects){
			GD.Print("dmg");
			if(atEnemy){ // temp +25 value
				enemy.health -= 15;
				GD.Print(enemy.health);
			} else{
				player.health -=15;
			}
		}
		GD.Print("good luck");
	}

	public void healEffect(bool effect, Vector2 effectInfo){ // removes an affect, given is the target's effect bool and info
		effectInfo = new Vector2(0,0);
		effect = false;
	}

	public void applyEffect(bool effect, Vector2 effectInfo, bool extraDmg, int boost){ // doing the applying of effects
		if (effectInfo != new Vector2(0,0)){ // base value is temporary
			if(effectInfo.X < 5*boost||(extraDmg&&effectInfo.X<5*boost+15)){
				effectInfo.X = 5*boost;
				if(extraDmg){
					effectInfo.X += 15; // adding extra dmg if damage is also used (TEMP)
				}
			}
			effectInfo.Y += 4*boost;
		} else{
			effectInfo = new Vector2(5, 4); // base starting value (TEMP)
			effectInfo.X *= boost;
			effectInfo.Y *= boost;
			if(extraDmg){
				effectInfo.X += 15; // adding extra dmg if damage is also used (TEMP)
			}
		}
		effect = true;
	}

	public void unSelect(){ // unselects all hand items
		for(int i = 0; i < handSize; i++){
			handItems[i].selected = false;
			handItems[i].updateSprite(1);
		}
		numSelected = 0;
	}
}
