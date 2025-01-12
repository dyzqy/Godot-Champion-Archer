using Godot;
using System;

public partial class arrow : CharacterBody2D
{
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    private float shootPower = 1.0f; // Power value between 1 and 8
    private bool isShot = false; // Track whether the arrow has been shot

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Apply gravity if the arrow has been shot

    	velocity.Y += gravity * (float)delta;

		shootPower = 8.0f;

		float angle = Rotation; // Get the arrow's current rotation in radians
		velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * shootPower * 200;

		isShot = true; // Mark the arrow as shot
        

        Velocity = velocity;
        MoveAndSlide();
    }
}
