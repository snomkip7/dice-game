using Godot;
using System;
using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;

public partial class DieFace : Node2D
{
	[Signal]
	public delegate void liftUpEventHandler(bool active);
	public static bool active = false;
	public static int face = 2;
	private const float FollowSpeed = 2.0f;
	private static globalVariables globalVariables = new();
	//private bool draggable = false;
	private bool inSlot = false;
	private Vector2 lastLerp = new();
	public override void _Ready()
	{
	//	this.Connect("LiftUpEventHandler", DieBG, "OnLiftUp");
		globalVariables = GetNode<globalVariables>("/root/GlobalVariables");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(lastLerp == new Vector2(0,0)){
			lastLerp = globalVariables.lastMousePos;
		}
		//GD.Print(active, "");
		if(Input.IsActionJustPressed("leftClick")){
			active = true;
			this.EmitSignal("liftUp", active);
		} else if(!Input.IsActionPressed("leftClick")){
			active = false;
			this.EmitSignal("liftUp", active);
		}
		if(active){
			//this.Position = globalVariables.mousePosition;
			this.Position = lastLerp;
			lastLerp = globalVariables.lastMousePos.Lerp(globalVariables.mousePosition, (float)delta * FollowSpeed);
		}
	}
	
	public static void OnMouseButtonPressed(){
		return;
	}
}
