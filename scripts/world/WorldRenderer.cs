using Godot;
using System;
using System.Runtime.CompilerServices;
using water;

public partial class WorldRenderer : Node2D
{

	[Export]
	Texture2D waterTexture = null;
	[Export]
	Texture2D dirtTexture = null;
	[Export]
	Texture2D airTexture = null;


	#region Singleton
	private static WorldRenderer instance;

	public static WorldRenderer Instance
	{
		get
		{
			if (instance == null)
			{
				GD.Print("Creating new instance of WorldRenderer");
				instance = new WorldRenderer();
			}
			return instance;
		}
	}
	private WorldRenderer()
	{
		// Private constructor to prevent instantiation
	}
	#endregion
	public void Render(TileMeta[][] tiles)
	{
		if (GetChildCount() <= 0)
		{
			GD.Print("No children found render not complete...");
			return;
			// CreateObjects(tiles, Gamemanager.Instance.tilesize);
		}
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
				Sprite2D sprite2D = GetChild<Sprite2D>(row * tiles[row].Length + column);
				if (sprite2D != null && texture != null)
				{
					sprite2D.Texture = texture;
					sprite2D.GetChild<Label>(0).Text = $"{column}, {row} \n {(tiles[row][column] is LiquidMeta ? (tiles[row][column] as LiquidMeta).lastTickFlowDirection.ToString() : "N/A")}";
				}
				// if (displayLabels)
				// else
				// sprite2D.GetChild<Label>(0).Text = "";
			}
		}
	}
	private void CreateObjects(TileMeta[][] tiles, int tilesize)
	{

		// GD.Print("Creating world objects: " + tiles.Length + " " + tiles[0].Length);
		for (int row = 0; row < tiles.Length; row++)
		{
			for (int column = 0; column < tiles[row].Length; column++)
			{
				// Create a new Sprite node
				Sprite2D sprite = new Sprite2D();
				// Set the texture of the sprite
				// sprite.Texture = airTexture;
				// Set the position of the sprite
				Vector2 offset = new Vector2(tiles[0].Length * tilesize / 2, -tiles.Length * tilesize / 2);
				// offset = new Vector2(0, 0);

				offset -= new Vector2(tilesize / 2, -tilesize / 2); //Godot renders objects centered
				sprite.Position = new Vector2(column * tilesize, row * tilesize * -1) - offset;
				//Set the name of the sprite
				sprite.Name = $"Tile_{column}_{row}";


				// Draw the coordinates on the sprite
				Label label = new Label();
				label.Position = new Vector2(-tilesize / 2, -tilesize / 2);
				label.SetSize(new Vector2(tilesize, tilesize));
				label.AddThemeFontSizeOverride("font_size", 15);
				string flowDir;
				flowDir = (tiles[row][column] is LiquidMeta) ? (tiles[row][column] as LiquidMeta).lastTickFlowDirection.ToString() : "N/A";
				label.Text = $"{column}, {row} \n {flowDir}";
				label.HorizontalAlignment = HorizontalAlignment.Center;
				label.VerticalAlignment = VerticalAlignment.Center;

				var red = new Color(1.0f, 0.0f, 0.0f, 1.0f);
				var black = new Color(0.0f, 0.0f, 0.0f, 1.0f);
				label.Set("theme_override_colors/font_color", black);



				// label.LabelSettings.FontSize = 5;
				// label.Size = new Vector2(tilesize, tilesize);
				// GD.Print("Creating world objects: " + row + " " + column);
				sprite.AddChild(label);
				// sprite.Position -= new Vector2(tiles.Length * tilesize / 2, tiles[0].Length * tilesize) / 2;
				// Add the sprite to the scene
				this.AddChild(sprite);
			}
		}
	}
	public void OnStartEvent(Gamemanager gm)
	{
		waterTexture = GD.Load<Texture2D>("res://assets/water64.png");
		dirtTexture = GD.Load<Texture2D>("res://assets/dirt64.png");
		airTexture = GD.Load<Texture2D>("res://assets/air64.png");

		Render(gm.tiles);

	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		instance = this;
		CreateObjects(Gamemanager.Instance.tiles, Gamemanager.Instance.tilesize);
		Render(Gamemanager.Instance.tiles);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
