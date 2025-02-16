using System.Linq.Expressions;
using Godot;
using Array = Godot.Collections.Array;

public partial class swordman : CharacterBody2D
{
	AnimatedSprite2D body;
	Area2D trigger, headTrigger;
	Array RunTypes = new Array();

	private float Speed = 100.0f;
	private bool isAttacking = false;
	private bool attacked = false;

	private bool isDead;
	private bool isArrowDeath;
	private bool stopped;

	private string animation = "attack_1";

	private CharacterBody2D target;
	private swordman targetClass;

	[Export] public bool isEnemy { get; set; } = false;
	[Export] private int type { get; set; } = 2;
	[Export] public int health { get; set; } = 5;

	private bool finishedDeathAnimation; //omg this is so long lamoaoa

	public override void _Ready()
	{
		body = GetNode<AnimatedSprite2D>("Body");
		trigger = GetNode<Area2D>("Area2D");
		body.AnimationLooped += OnAnimationFinished;
		
		if(isEnemy)
		{
			headTrigger = GetNode<Area2D>("HeadArea2D");
			headTrigger.BodyEntered += headshot;
		}

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
		if (direction != Vector2.Zero && !isAttacking && !isDead)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity = Vector2.Zero;
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void animate()
	{
		if (isDead)
		{
			if (body.Animation == "death_1" && finishedDeathAnimation) { body.Stop(); body.Frame = 29; }
			else if (body.Animation == "death_2" && finishedDeathAnimation) { body.Stop(); body.Frame = 19; }
			else if (body.Animation == "death_3" && finishedDeathAnimation) { body.Stop(); body.Frame = 22; }

			if (body.Animation == "death_1" && body.Frame >= 28) { finishedDeathAnimation = true; }
			else if (body.Animation == "death_2" && body.Frame >= 18) { finishedDeathAnimation = true; }
			else if (body.Animation == "death_3" && body.Frame >= 21) { finishedDeathAnimation = true; }
			return;
		}

		if (isAttacking)
		{
			body.Play(animation);
			if (body.Frame >= 16 && attacked == false)
			{
				targetClass?.damage(isEnemy);
				attacked = true;
			}
		}
		else
		{
			body.Play((string)RunTypes[type - 1]);
		}
	}

	public void damage(bool enemy)
	{
		if (!isDead && isEnemy != enemy) health--;
		else GD.Print("Already dead.");

		if (health <= 0) kill();
	}

	private void kill()
	{
		body.Play($"death_{GD.Randi() % 2 + 1}");

		// Disconnect signals before freeing the trigger
		if (trigger != null)
		{
			trigger.BodyEntered -= OnBodyEntered;
			trigger.BodyExited -= OnBodyExited;

			target = null;
			targetClass = null;

			trigger.QueueFree();
		}

		if(isEnemy) GetNode<CollisionShape2D>("HeadCollision").QueueFree();
		GetNode<CollisionShape2D>("BodyCollision").QueueFree();

		// Mark the swordman as dead
		isDead = true;
	}

	public void headshot(Node node)
	{
		body.Play("death_3");

		if (trigger != null)
		{
			// trigger.BodyEntered -= OnBodyEntered;
			// trigger.BodyExited -= OnBodyExited;

			target = null;
			targetClass = null;

			trigger.QueueFree();
		}
		
		if(node is arrow Arrow) 
		{ 
			if(!Arrow.hit) Arrow.HitHead(); 
		}

		GetNode<Area2D>("HeadArea2D").QueueFree();
		GetNode<CollisionShape2D>("BodyCollision").QueueFree();

		// Mark the swordman as dead
		GD.Print("Headshot les go");
		isDead = true;
	}

	private void OnAnimationFinished()
	{
		if (isDead)
		{
			return;
		}
		attacked = false;
		animation = $"attack_{GD.Randi() % 2 + 1}";
	}

	private void OnBodyEntered(Node node)
	{
		if (node is CharacterBody2D characterBody) // Check if the entering node is of the correct type
		{
			target = characterBody;
			isAttacking = true;
			//GD.Print($"Target acquired: {target.Name}");
		}
		if (node is swordman customNode)
		{
			targetClass = customNode;
		}
	}

	private void OnBodyExited(Node node)
	{
		target = null;
		targetClass = null;
		isAttacking = false;
		//GD.Print("Target lost.");
	}
}
