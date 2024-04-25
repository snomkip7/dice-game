using Godot;
using System;

public partial class DieBG : Sprite2D
{
	private Vector2 defaultScale = new();
	private Vector2 scaleDiff;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		defaultScale = this.Scale;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_die_face_lift_up(bool active){
		if(active){
			//GD.Print("activeran");
			this.Scale += new Vector2(0.2f,0.2f);
			//GetNode<AnimatedSprite2D>("Faces").Scale += (new Vector2(0.2f, 0.2f) * scaleDiff);
			GetNode<Sprite2D>("DieShadow").Visible = true;
		} else {
			this.Scale = defaultScale;
			//GetNode<AnimatedSprite2D>("Faces").Scale = defaultFaceScale;
			GetNode<Sprite2D>("DieShadow").Visible = false;
		}
	}
}
