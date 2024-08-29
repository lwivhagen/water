using Godot;
using System;

public partial class ForceRight : CheckButton
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	public override void _Pressed()
	{
		Gamemanager.Instance.forceRightFlow = !Gamemanager.Instance.forceRightFlow;
		base._Pressed();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
