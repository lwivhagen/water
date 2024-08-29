using Godot;
using System;

public partial class CameraScript : Camera2D
{
    // Speed of the camera movement
    private float speed = 200.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Vector2 direction = new Vector2();


        // Check for input and set the direction vector
        if (Input.IsActionPressed("ui_up"))
        {
            direction.Y -= 1;
        }
        if (Input.IsActionPressed("ui_down"))
        {
            direction.Y += 1;
        }
        if (Input.IsActionPressed("ui_left"))
        {
            direction.X -= 1;
        }
        if (Input.IsActionPressed("ui_right"))
        {
            direction.X += 1;
        }

        // Normalize the direction vector to ensure consistent speed
        if (direction.Length() > 0)
        {
            direction = direction.Normalized();
        }

        // Update the position of the camera
        Position += direction * speed * (Input.IsKeyPressed(Key.Shift) ? 2.5f : 1) * (float)delta;
    }
}