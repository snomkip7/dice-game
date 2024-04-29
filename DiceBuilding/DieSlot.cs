using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;


public partial class DieSlot : Area2D 
{
	private static globalVariables globalVariables = new();
	private RichTextLabel info;
	private Godot.Collections.Array<Area2D> oLapAreas;
	private Area2D slottedFace;
	public override void _Ready()
	{
		globalVariables = GetNode<globalVariables>("/root/GlobalVariables");
		info = GetNode<RichTextLabel>("Info");
		oLapAreas = GetOverlappingAreas();
	}
	public override void _Process(double delta)
	{
		return;
	}

	private void OnAreaEntered(Area2D body){
		var oLap = GetOverlappingAreas();
		slottedFace = body;
		body.AddToGroup("slotted");
		SetDeferred("monitorable", false);
		info.Text = (this.Monitorable).ToString();
		oLapAreas = GetOverlappingAreas();
	}
	private void OnAreaExited(Area2D body){
		if(body == slottedFace){
			body.RemoveFromGroup("slotted");
			SetDeferred("monitorable", true);
			info.Text = (this.Monitorable).ToString();
			slottedFace = null;
		}
	}

}
