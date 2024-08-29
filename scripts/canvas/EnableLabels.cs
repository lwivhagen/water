using Godot;
using System;

public partial class EnableLabels : CheckButton
{
	// Called when the node enters the scene tree for the first time.
	public override void _Pressed()
	{
		Gamemanager.Instance.displayLabels = !Gamemanager.Instance.displayLabels;
		GD.Print("Display labels: " + Gamemanager.Instance.displayLabels);
		base._Pressed();
	}

}
