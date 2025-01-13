using static Godot.Mathf;
using Godot;
using System;

public partial class arrow : CharacterBody2D
{
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	private float shootPower = 8.0f; // Fixed power value from 1 to 8
	private Vector2 velocity;

	private bool thatsit;
	private Vector2 pos;

	public override void _PhysicsProcess(double delta)
	{
		if(thatsit) return;

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

		
		if(Position.Y >= pos.Y)
		{
			GetNode<Area2D>("Area2D").QueueFree();
			thatsit = true;
		}
	}

	public void Shoot(float power)
	{
		shootPower = power;
		float angle = Rotation; // Arrow's rotation in radians
		velocity = new Vector2(Cos(angle), Sin(angle)) * shootPower * 250;

		pos = Position + new Vector2(0, 16);

		Area2D area = GetNode<Area2D>("Area2D");
		area.BodyEntered += HitBody;
	}

	public void HitBody(Node body) // Updated method signature
	{
		GD.Print($"Hit {body.Name} with Arrow");
		if(body is swordman Sword)
		{
			if(body.Name == "HeadCollision") Sword.headshot();
			else Sword.damage(false);

			QueueFree();
		}
	}
}
