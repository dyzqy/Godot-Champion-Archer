using Godot;
using Array = Godot.Collections.Array;
using System;

public partial class swordman : CharacterBody2D
{
	AnimatedSprite2D body;
	Area2D trigger;
	Array RunTypes = new Array();

	private float Speed = 100.0f;
	private bool isAttacking = false;
	private string animation = "attack_1";

	[Export] public bool isEnemy { get; set; } = false;
	[Export] private int type { get; set; } = 2;

	public int health;

	public override void _Ready()
	{
		body = GetNode<AnimatedSprite2D>("Body");
		trigger = GetNode<Area2D>("Area2D");
		body.AnimationLooped += OnAnimationFinished;

		// Connect signals for Area2D collision detection
		trigger.BodyEntered += OnBodyEntered;
		trigger.BodyExited += OnBodyExited;

		Speed = Speed * type;

		RunTypes.Add("walk_slow");
		RunTypes.Add("walk_fast");
		RunTypes.Add("run");
	}

	public override void _Process(double delta)
	{
		animate();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 direction = new Vector2(!isEnemy ? 1 : -1, 0);
		if (direction != Vector2.Zero && !isAttacking)
		{
			velocity.X = direction.X * Speed;
		}
		else if (!isAttacking)
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}
		else velocity = Vector2.Zero;

		Velocity = velocity;
		MoveAndSlide();
	}

	private void animate()
	{
		if (isAttacking)
		{
			body.Play(animation);
		}
		else
		{
			body.Play((string)RunTypes[type - 1]);
		}
	}

	private void OnAnimationFinished()
	{
		animation = $"attack_{GD.Randi() % 2 + 1}";
	}

	// Signal handlers for collision detection
	private void OnBodyEntered(Node body)
	{
		GD.Print("Entered attack area");
		isAttacking = true;
	}

	private void OnBodyExited(Node body)
	{
		GD.Print("Left attack area");
		isAttacking = false;
	}
}
