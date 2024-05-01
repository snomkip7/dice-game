using Godot;
using System;

public partial class TextLabelH2 : RichTextLabel
{
	private static globalVariables globalVariables = new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		globalVariables = GetNode<globalVariables>("/root/GlobalVariables");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.Text = globalVariables.currentHealth.ToString();
	}
}
