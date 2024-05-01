using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;


public partial class DieSlot : Area2D 
{	
	[Signal]
	public delegate void toggleSlottedEventHandler(Area2D body, bool tf);
	private static globalVariables globalVariables = new();
	private RichTextLabel info;
	private Godot.Collections.Array<Area2D> oLapAreas;
	private Area2D slottedFace = null;
	public override void _Ready()
	{
		globalVariables = GetNode<globalVariables>("/root/GlobalVariables");
		info = GetNode<RichTextLabel>("Info");
		oLapAreas = GetOverlappingAreas();
	}
	public override void _Process(double delta)
	{
		oLapAreas = GetOverlappingAreas();
		if(slottedFace != null && slottedFace.IsInGroup("active") == false){
			slottedFace.AddToGroup("slotted)");
			//this.EmitSignal("toggleSlotted", true);
			SetDeferred("monitorable", false);
			info.Text = (this.Monitorable).ToString();
		} else {
			slottedFace?.RemoveFromGroup("slotted");
			//this.EmitSignal("toggleSlotted", false);
			SetDeferred("monitorable", true);
			info.Text = (this.Monitorable).ToString();
		}
	}
	private void OnAreaEntered(Area2D body){
		slottedFace ??= body;
	}
	
	private void OnAreaExited(Area2D body){
		if(body == slottedFace){
			slottedFace = null;
		}
	}

}
