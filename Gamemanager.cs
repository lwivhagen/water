using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using water.scripts;
using water.scripts.world;
using water;



public partial class Gamemanager : Node2D
{
	[Export]
	public bool forceRightFlow = false;

	[Export]
	public bool displayLabels = false;

	public TileMeta[][] tiles = new TileMeta[3][];
	int tilesize = 64;
	Texture2D waterTexture;
	Texture2D dirtTexture;
	Texture2D airTexture;
	Node2D tileParent;
	List<TileData> tileDatas;
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

	private Gamemanager()
	{
		// Private constructor to prevent instantiation
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tileParent = GetChild<Node2D>(0);
		GD.Print("Hello, World!");
		waterTexture = (Texture2D)GD.Load("res://water64.png");
		dirtTexture = (Texture2D)GD.Load("res://dirt64.png");
		airTexture = (Texture2D)GD.Load("res://air64.png");

		for (int i = 0; i < tiles.Length; i++)
		{
			tiles[i] = new TileMeta[3];
		}
		// Line2D line2D = new Line2D();
		// line2D.Points = new Vector2[] {new Vector2(0,0), new Vector2(128*30,0)};
		// line2D.Width = 3;
		// tileParent.AddChild(line2D);
		// LoadTiles();
		tiles = CreateMap(50, 15);
		CreateObjects();
		Render();
	}
	// private void LoadTiles()
	// {
	// 	string json = System.IO.File.ReadAllText("res://tiles.json");
	// 	List<TileJsonData> tileJson = JsonSerializer.Deserialize<List<TileJsonData>>(json);
	// 	foreach (TileJsonData tile in tileJson)
	// 	{
	// 		Texture2D texture = (Texture2D)GD.Load(tile.texturePath);
	// 		if(texture == null)
	// 		{
	// 			GD.Print($"Failed to load texture: {tile.texturePath}");
	// 			continue;
	// 		}
	// 		tileDatas.Add(new TileData { name = tile.name, texture = (Texture2D)GD.Load(tile.texturePath) });
	// 		GD.Print(tile.name);
	// 		GD.Print(tile.texturePath);
	// 	}
	// }
	private TileMeta[][] CreateMap(int width, int height)
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

				// if (row == 8 && column > 6 && column < 10)
				// 	map[row][column] = new LiquidMeta(2, 0);

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
	public void Render()
	{
		GD.Print("Show labels: " + displayLabels);

		for (int row = 0; row < tiles.Length; row++)
		{
			for (int column = 0; column < tiles[row].Length; column++)
			{
				Texture2D texture = null;
				switch (tiles[row][column].id)
				{
					case 0:
						texture = airTexture;
						break;
					case 1:
						texture = dirtTexture;
						break;
					case 2:
						texture = waterTexture;
						break;
				}
				Sprite2D sprite2D = GetChild(0).GetChild<Sprite2D>(row * tiles[row].Length + column);
				sprite2D.Texture = texture;
				// if (displayLabels)
				sprite2D.GetChild<Label>(0).Text = $"{row}, {column} \n {(tiles[row][column] is LiquidMeta ? (tiles[row][column] as LiquidMeta).lastTickFlowDirection.ToString() : "N/A")}";
				// else
				// sprite2D.GetChild<Label>(0).Text = "";
			}
		}
	}
	private void CreateObjects()
	{
		GD.Print(tiles.Length + " " + tiles[0].Length);
		for (int row = 0; row < tiles.Length; row++)
		{
			for (int column = 0; column < tiles[row].Length; column++)
			{
				// Create a new Sprite node
				Sprite2D sprite = new Sprite2D();
				// Set the texture of the sprite
				sprite.Texture = airTexture;
				// Set the position of the sprite
				Vector2 offset = new Vector2(tiles[0].Length * tilesize / 2, -tiles.Length * tilesize / 2);
				offset -= new Vector2(tilesize / 2, -tilesize / 2); //Godot renders objects centered
				sprite.Position = new Vector2(column * tilesize, row * tilesize * -1) - offset;
				//Set the name of the sprite
				sprite.Name = $"Tile_{row}_{column}";


				// Draw the coordinates on the sprite
				Label label = new Label();
				label.Position = new Vector2(-tilesize / 2, -tilesize / 2);
				label.SetSize(new Vector2(tilesize, tilesize));
				label.AddThemeFontSizeOverride("font_size", 15);
				string flowDir;
				flowDir = (tiles[row][column] is LiquidMeta) ? (tiles[row][column] as LiquidMeta).lastTickFlowDirection.ToString() : "N/A";
				label.Text = $"{row}, {column} \n {flowDir}";
				label.HorizontalAlignment = HorizontalAlignment.Center;
				label.VerticalAlignment = VerticalAlignment.Center;

				var red = new Color(1.0f, 0.0f, 0.0f, 1.0f);
				var black = new Color(0.0f, 0.0f, 0.0f, 1.0f);
				label.Set("theme_override_colors/font_color", black);



				// label.LabelSettings.FontSize = 5;
				// label.Size = new Vector2(tilesize, tilesize);

				sprite.AddChild(label);
				// sprite.Position -= new Vector2(tiles.Length * tilesize / 2, tiles[0].Length * tilesize) / 2;
				// Add the sprite to the scene
				tileParent.AddChild(sprite);
			}
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustReleased("logic_e"))
		{
			tiles = LogicHandler.Instance.LiquidLogic(tiles);
			Render();
		}
		if (Input.IsActionJustReleased("logic_q"))
		{
			tiles = CreateMap(tiles.Length, tiles[0].Length);
			Render();
		}
	}
}
