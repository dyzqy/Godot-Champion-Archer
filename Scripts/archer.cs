using Godot;
using System;

// the acton empire :kekw:
// https://cdn.discordapp.com/attachments/931644550462722189/1008050885093494946/wheeze07.mp4?ex=678497cb&is=6783464b&hm=488c33853438b9aceaef7d47301309009f9e531a244352cfdb7e3a3608e349c9&
public partial class archer : CharacterBody2D
{
	AnimatedSprite2D bow;
	AnimatedSprite2D body;
	
	public const float Speed = 200.0f;

	public override void _Ready()
	{
		bow = GetNode<AnimatedSprite2D>("Arms");
		body = GetNode<AnimatedSprite2D>("Body");
	}
	
	public override void _Process(double delta)
	{
		if(Input.IsActionPressed("ui_accept"))
		{
			shoot();
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity = direction * Speed;
			body.Play("run");
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
			body.Play("idle");
		}

		Velocity = velocity;
		MoveAndSlide();
	}
	
	public void shoot()
	{
		var arrow = ResourceLoader.Load<PackedScene>("res://Scenes/arrow.tscn").Instantiate() as Node2D;
		
		GetParent().AddChild(arrow);
		arrow.GlobalPosition = bow.GlobalPosition;
		arrow.RotationDegrees = bow.RotationDegrees;
	}
}
