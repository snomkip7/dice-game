using Godot;
using Godot.Collections;
using System;


public partial class globalVariables : Node
{
	// die customization
	public Vector2 mousePosition = new(0, 0);
	public Vector2 lastMousePos = new(0, 0);
	// seed
	public float seed;
	// player stuff (set to default values)
	public Dictionary<string, string> spellbook = new Dictionary<string, string>();
	public int maxHealth = 100;
	public string[] dieEffects = new string[7];
	public int handSize = 2;
	// enemy stuff (set to default value)
	public int enemyMaxHealth = 100;
	public string[] effects = new string[5];
	public string enemyType = "none";
	public int enemyReward = 20;
	public int aiLevel = 1;
	public int decisionTime = 7;
	public int enemyHandSize = 2;
	public int enemyIndex = 1;
	public string enemyName="bob";


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//loadInfoFromTxt();
		loadSpellbook();
		getSeed();
		getEffects();
		getPlayerInfo();
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

	// public void loadInfoFromTxt(){
	// 	var file = FileAccess.Open("user://Player.txt", FileAccess.ModeFlags.Read);
	// 	if(file==null){
	// 		file = FileAccess.Open("res://TextFiles/DefaultPlayer.txt", FileAccess.ModeFlags.Read);
	// 	}
	// 	maxHealth = Convert.ToInt32(file.GetLine());
	// 	dieEffects[0] = file.GetLine();
	// 	dieEffects[1] = file.GetLine();
	// 	dieEffects[2] = file.GetLine();
	// 	dieEffects[3] = file.GetLine();
	// 	dieEffects[4] = file.GetLine();
	// 	dieEffects[5] = file.GetLine();
	// 	dieEffects[6] = file.GetLine(); // should be blank if nothing in that slot
	// 	handSize = Convert.ToInt32(file.GetLine());
	// 	file.Close();
	// }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		mousePosition = GetViewport().GetMousePosition();
		lastMousePos = this.mousePosition;
	}

	public void getSeed(){ // 36 digit number
		seed = new RandomNumberGenerator().RandfRange(1000000000000000000000000000000000000f, 9999999999999999999999999999999999999f);
		GD.Print(seed);
	}

	public void getEffects(){
		effects[0] = "damage";
		effects[1] = "healing";
		effects[2] = "poison";
		effects[3] = "fire";
		effects[4] = "ice";
	}

	public void getPlayerInfo(){
		string values = Convert.ToString(seed);
		values = values.Substring(0, 5);
		dieEffects[0] = "damage";
		dieEffects[1] = effects[0];
		dieEffects[2] = effects[1];
		dieEffects[3] = effects[2];
		dieEffects[4] = effects[3];
		dieEffects[5] = effects[4];
	}
}
