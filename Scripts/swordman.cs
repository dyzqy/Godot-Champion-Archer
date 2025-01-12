using System.Linq.Expressions;
using Godot;
using Array = Godot.Collections.Array;

public partial class swordman : CharacterBody2D
{
    AnimatedSprite2D body;
    Area2D trigger;
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
			if(body.Animation == "death_1" && body.Frame >= 28) body.Frame = 29;
			else if(body.Animation == "death_2" && body.Frame >= 18) body.Frame = 19;
			else if(body.Animation == "death_3" && body.Frame >= 21) body.Frame = 2;
			return;
		} 
		

		if (isAttacking)
		{
			body.Play(animation);
			if (body.Frame >= 16 && attacked == false)
			{
				targetClass?.damage();
				attacked = true;
			}
		}
		else
		{
			body.Play((string)RunTypes[type - 1]);
		}
	}


    public void damage()
    {
		if(!isDead) kill();
		else GD.Print("Already dead.");
    }

    private void kill()
    {
        GD.Print("Dead.");
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

        // Mark the character as dead
        isDead = true;
    }

    private void OnAnimationFinished()
    {
		if(isDead)
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
            GD.Print($"Target acquired: {target.Name}");
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
        GD.Print("Target lost.");
    }
}
