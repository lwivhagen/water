using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using water.scripts;
using water.scripts.world;
using water;
using System.Threading;



public partial class Gamemanager : Node2D
{
	[Export]
	public bool forceRightFlow = false;

	[Export]
	public bool displayLabels = false;
	public Vector2 mouseposition;
	public Vector2I mouseTilePosition;

	public TileMeta[][] tiles = new TileMeta[3][];
	public int tilesize { get; private set; } = 64;
	Texture2D waterTexture;
	Texture2D dirtTexture;
	Texture2D airTexture;
	// Node2D tileParent;

	public delegate void StartGameHandler(Gamemanager sender);
	public event StartGameHandler StartGameEvent;


	#region Singleton
	private static Gamemanager instance;

	public static Gamemanager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new Gamemanager();
			}
			return instance;
		}
	}
	#endregion
	private Gamemanager()
	{
		StartGameEvent += WorldRenderer.Instance.OnStartEvent;

		// Private constructor to prevent instantiation
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		instance = this;
		// tileParent = GetChild<Node2D>(0);

		// waterTexture = (Texture2D)GD.Load("res://water64.png");
		// dirtTexture = (Texture2D)GD.Load("res://dirt64.png");
		// airTexture = (Texture2D)GD.Load("res://air64.png");

		tiles = Generatemap(50, 15);
		// WorldRenderer.Instance.CreateObjects(tiles, tilesize);
		Thread.Sleep(1000);
		StartGameEvent(this);

		GD.Print("Hello, World!");
	}
	public void Reset()
	{
		tiles = new TileMeta[0][];
		tiles = Generatemap(50, 15);
		WorldRenderer.Instance.Render(tiles);
	}
	private TileMeta[][] Generatemap(int width, int height)
	{
		TileMeta[][] map = new TileMeta[height][];
		for (int row = 0; row < height; row++)
		{
			map[row] = new TileMeta[width];
			for (int column = 0; column < width; column++)
			{
				if (row > 2)
				{
					// GD.Print("AIR! Row: " + row + " Column: " + column);
					map[row][column] = new TileMeta(0);
				}

				else
					// GD.Print("GROUND! Row: " + row + " Column: " + column);
					map[row][column] = new TileMeta(1);

				if (row == 8 && column > 20 && column < 25)
					map[row][column] = new LiquidMeta(2, 0);

				// if (row == 9 && column == 8)
				// 	map[row][column] = new LiquidMeta(2, 0);
				// if (row == 9 && column == 9)
				// 	map[row][column] = new LiquidMeta(2, 0);

				// if (row == 9 && column == 15)
				// 	map[row][column] = new LiquidMeta(2, 0);
				// if (row == 9 && column == 16)
				// 	map[row][column] = new LiquidMeta(2, 0);
				// if (row == 5 && column > 7 && column <= 10)
				// 	map[row][column] = new LiquidMeta(2, 0);

				// if (row == 3 && column == 4)
				// 	map[row][column] = new TileMeta(1);
				// if (row == 3 && column == 12)
				// 	map[row][column] = new TileMeta(1);
			}

		}
		return map;
	}

	public override void _Process(double delta)
	{

		// if (Input.IsActionJustReleased("logic_e"))
		// {
		// 	tiles = LogicHandler.Instance.LiquidLogic(tiles);
		// 	WorldRenderer.Instance.Render(tiles);
		// }
		// if (Input.IsActionJustReleased("logic_q"))
		// {
		// 	tiles = Generatemap(tiles.Length, tiles[0].Length);
		// 	WorldRenderer.Instance.Render(tiles);
		// }
	}
}
