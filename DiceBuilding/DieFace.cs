using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;

public partial class DieFace : Node2D
{
	[Signal]
	public delegate void liftUpEventHandler(bool active);
	public bool active = false;
	[Export(PropertyHint.Enum,"damage,healing,poison,fire,ice")]
	public String face = "";
	private const float FollowSpeed = 2.0f;
	private Vector2 returnPos;
	private static globalVariables globalVariables = new();
	//private bool draggable = false;
	private bool inSlot = false;
	private Vector2 lastLerp = new();
	public override void _Ready()
	{
	//	this.Connect("LiftUpEventHandler", DieBG, "OnLiftUp");
		globalVariables = GetNode<globalVariables>("/root/GlobalVariables");
		returnPos = this.Position;
		GetNode<Sprite2D>("DieBG").GetNode<AnimatedSprite2D>("Faces").Frame = Face2int(face);
	}
	public static int Face2int(string newFace){
        return newFace switch
        {
            "damage" => 0,
            "healing" => 1,
            "poison" => 2,
            "fire" => 3,
            "ice" => 4,
            _ => 0,
        };
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("leftClick")){
			if(globalVariables.mousePosition.DistanceTo(Position) < 125){
				GD.Print(face, "");
				active = true;
				returnPos = this.Position;
				this.EmitSignal("liftUp", active);
			}
		} else if(!Input.IsActionPressed("leftClick")){
			if(globalVariables.mousePosition.DistanceTo(Position) < 125){
				active = false;
			}
		}

		if(lastLerp == new Vector2(0,0)){
			lastLerp = globalVariables.lastMousePos;
		}
		//GD.Print(active, "");
		
		if(active){
			//GD.Print(face, "");
			//this.Position = globalVariables.mousePosition;
			this.Position = lastLerp;
			lastLerp = globalVariables.lastMousePos.Lerp(globalVariables.mousePosition, (float)delta * FollowSpeed);
		} else {
			if(this.Position != returnPos){
				var tween = GetTree().CreateTween();
				tween.TweenProperty(this, "position", returnPos, 0.2);
			}
			this.EmitSignal("liftUp", active);
		}
	}
	
	public static void OnMouseButtonPressed(){
		return;
	}

	private void OnAreaEntered(Area2D body){
		if(active){
			returnPos = body.Position;
		}
	}
}
