using static Godot.Mathf;
using Godot;
using System;

public partial class arrow : CharacterBody2D
{
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	private float shootPower = 8.0f; // Fixed power value from 1 to 8
	private Vector2 velocity;

	public override void _PhysicsProcess(double delta)
	{
		// Gradually reduce velocity over time
		velocity = velocity.Lerp(Vector2.Zero, 0.76f * (float)delta); // Damping factor

		// Apply gravity to make the arrow eventually fall
		velocity.Y += gravity * (float)delta;

		// Update the arrow's rotation to match its velocity direction
		if (velocity.Length() > 0.01f) // Avoid setting rotation for very small velocities
		{
			Rotation = velocity.Angle();
		}

		// Update velocity and move the arrow
		Velocity = velocity;
		MoveAndSlide();
	}

	public void Shoot(float power)
	{
		shootPower = power;
		float angle = Rotation; // Arrow's rotation in radians
		velocity = new Vector2(Cos(angle), Sin(angle)) * shootPower * 250;

		Area2D area = GetNode<Area2D>("Area2D");
		area.BodyEntered += HitBody;
	}

	public void Hit() // headshot
	{
		GD.Print("hit head with arrow (real)");
	}

	public void HitBody(Node body) // Updated method signature
	{
		if(body is swordman Sword)
		{
			if(body.Name == "HeadCollision") Sword.headshot();
			else Sword.damage(false);
			GD.Print($"Hit {body.Name} with Arrow");
		}
	}
}
