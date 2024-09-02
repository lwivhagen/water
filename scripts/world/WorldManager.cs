using Godot;
using System;
using water;
using water.scripts.world;

public partial class WorldManager : Node2D
{
	public static WorldManager instance;
	public Vector2I mouseTilePosition { get; private set; } = new Vector2I(0, 0);



	public static Vector2I GetWorldDimensionsPT()
	{
		Gamemanager gm = Gamemanager.Instance;

		return new Vector2I(gm.tiles[0].Length * gm.tilesize, gm.tiles.Length * gm.tilesize);
	}

	private Vector2 mousePosition;
	private Timer updateTimer;

	public static WorldManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new WorldManager();
			}
			return instance;
		}
	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		instance = this;
	}
	public override void _Process(double delta)
	{
		Vector2 offset = new Vector2(-Gamemanager.Instance.tiles[0].Length * Gamemanager.Instance.tilesize / 2, Gamemanager.Instance.tiles.Length * Gamemanager.Instance.tilesize / 2);

		mousePosition = GetGlobalMousePosition() - offset;
		// mousePosition -= new Vector2(GetWorldDimensionsPT().X / 2, GetWorldDimensionsPT().Y / 2);
		mouseTilePosition = new Vector2I((int)(mousePosition.X / Gamemanager.Instance.tilesize), -(int)(mousePosition.Y / Gamemanager.Instance.tilesize));
		if (Input.IsActionJustPressed("logic_r"))
		{
			Gamemanager.Instance.tiles[mouseTilePosition.Y][mouseTilePosition.X] = new LiquidMeta(2, 0);
			WorldRenderer.Instance.Render(Gamemanager.Instance.tiles);
		}
		if (Input.IsActionJustPressed("logic_t"))
		{
			GD.Print("Pressed t");
			Gamemanager.Instance.tiles[mouseTilePosition.Y][mouseTilePosition.X] = new TileMeta(3);
			WorldRenderer.Instance.Render(Gamemanager.Instance.tiles);
		}
		if (Input.IsActionJustPressed("logic_e"))
		{
			Gamemanager.Instance.tiles = LogicHandler.Instance.GranularLogic(Gamemanager.Instance.tiles);
			WorldRenderer.Instance.Render(Gamemanager.Instance.tiles);
		}

		if (Input.IsActionJustPressed("logic_q"))
		{
			Gamemanager.Instance.Reset();
			// WorldRenderer.Instance.Render(Gamemanager.Instance.tiles);
		}


	}
}

