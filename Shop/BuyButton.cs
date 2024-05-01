using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class BuyButton : Control
{
	[Signal]
	public delegate void buyEventHandler(string item);
	private static bool tog = true;
	public static Sprite2D window = new();
	[Export(PropertyHint.Enum,"healing,poison,fire,ice,health,slot,quantum")]
	public String item = "";
	public override void _Ready()
	{
		window = GetNode<Sprite2D>("Sprite2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Toggle(){
		SetDeferred("visible", tog);
		tog = !tog;
	}
	public static void HoverBuy(){
		window.SelfModulate = new Color(0.8f, 1f, 0.8f);
	}
	public static void HoverCancel(){
		window.SelfModulate = new Color(1f, 0.8f, 0.8f);
	}
	public static void LeftButton(){
		window.SelfModulate = new Color(1f, 1f, 1f);
	}

	public void BuyPressed(){
		Toggle();
		EmitSignal("buy", item);
	}

	public void CancelPressed(){
		Toggle();
	}
}
