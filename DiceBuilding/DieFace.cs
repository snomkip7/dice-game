using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
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
	private Vector2 startPos;
	private static globalVariables globalVariables = new();
	private Vector2 lastLerp = new();
	private Godot.Collections.Array<Area2D> oLapAreas;
	private Area2D area;
	public override void _Ready()
	{

	//	this.Connect("LiftUpEventHandler", DieBG, "OnLiftUp");
		globalVariables = GetNode<globalVariables>("/root/GlobalVariables");
		returnPos = this.Position;
		startPos = returnPos;
		area = GetNode<Area2D>("Area2D");
		oLapAreas = area.GetOverlappingAreas();
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
	public override void _Process(double delta)
	{
		//REMOVE THIS LATER
		switch(area.IsInGroup("slotted")){
			case(true):
				GetNode<Sprite2D>("DieBG").Modulate = new Color(0, 0, 0);
				break;
			case(false):
				GetNode<Sprite2D>("DieBG").Modulate = new Color(255,255,255);
				break;
		}


		if(Input.IsActionJustPressed("leftClick")){
			if(lastLerp == new Vector2(0,0)){lastLerp = globalVariables.lastMousePos;}
			area.SetCollisionMaskValue(2, true); // Disables detection with other slots on the way back

			if(globalVariables.mousePosition.DistanceTo(Position) < 125){
				active = true;
				returnPos = this.Position;	
				this.EmitSignal("liftUp", active);
			}
		} else if(!Input.IsActionPressed("leftClick")){
			if(globalVariables.mousePosition.DistanceTo(Position) < 125){
				active = false;
			}
		}

		if(area.HasOverlappingAreas()){
			var oLap = area.GetOverlappingAreas();
			Vector2 closestArea = oLap[0].Position;
			for(int i = 1; i < oLap.Count; i++){
				if(oLap[i].IsInGroup("Slot")){
					if(Position.DistanceTo(oLap[i].Position) < Position.DistanceTo(closestArea)){
						closestArea = oLap[i].Position;
					}
				}
			}
			returnPos = closestArea;
			//GD.Print(closestArea, "");
		} else {
			//GD.Print("else statement");
			//returnPos = startPos;
		}

		if(active){
			this.Position = lastLerp;
			lastLerp = globalVariables.lastMousePos.Lerp(globalVariables.mousePosition, (float)delta * FollowSpeed);
		} else {
			area.SetCollisionMaskValue(2, false);
			if(this.Position != returnPos){
				var tween = GetTree().CreateTween();
				tween.TweenProperty(this, "position", returnPos, 0.2);
			}
			this.EmitSignal("liftUp", active);
		}
	}
}
