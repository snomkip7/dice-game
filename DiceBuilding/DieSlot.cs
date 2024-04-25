using Godot;
using System;
using System.ComponentModel;
using System.Security.Cryptography;


public partial class DieSlot : Area2D
{
	public static bool full = false; //if the slot is empty or full
	public static int face = -1; //(-1 = if the slot is empty), the rest of the ints represent a possible face
	private static Area2D dieID;
	private static int id = 0;
	private static globalVariables globalVariables = new();
	private Area2D bodyRef = new();
	private RichTextLabel info;

	public DieSlot(int iden){
		id = iden;
	}

	public DieSlot(){
		return;
	}
	public static int GetID(){
		return id;
	}

	public static void NewID(int newIden){
		id = newIden;
	}
	public override void _Ready()
	{
		globalVariables = GetNode<globalVariables>("/root/GlobalVariables");
		info = GetNode<RichTextLabel>("Info");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!full){
			info.Text = "Empty";
		}
		//GD.Print("asfewybufxref");
	}

	private void OnAreaEntered(Area2D body){
		dieID = body;
		info.Text = "Attack";
		full = true;
	}

	private void OnAreaExited(Area2D body){
		info.Text = "Empty";
	}

}
