using Godot;
using System;

public partial class MousePosition : Panel
{
	Vector2 mousePosition;
	Vector2I mouseTilePosition;
	// Called when the node enters the scene tree for the first time.
	Label label1;
	Label label2;

	public override void _Ready()
	{
		label1 = this.GetChild<Label>(0);
		label2 = this.GetChild<Label>(1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		mousePosition = GetGlobalMousePosition();//Gamemanager.Instance.GetChild<Node2D>(0).GetGlobalMousePosition();//get it relative to
		mouseTilePosition = new Vector2I((int)(mousePosition.X / 64), (int)(mousePosition.Y / 64));
		if (WorldManager.instance != null)
		{
			label1.Text = WorldManager.instance.mouseTilePosition.ToString();

		}
		// label2.Text = "Tile: " + mouseTilePosition.X.ToString() + ", " + mouseTilePosition.Y.ToString();
	}
}
