using Godot;
using System;
using static Godot.Mathf;

public partial class BowControl : AnimatedSprite2D
{
    AnimatedSprite2D bow;

    bool isCharged = true;
    bool isAiming = false;
    float power; // Ranges from 1 to 8, determined by the "attack" animation; the 9th frame releases the arrow.
    float side = 1;

    private double deltaTime;
	private float chargedTimer;

    Vector2 pressPosition;
    Vector2 originalDirection;

    public override void _Ready()
    {
        bow = this;
		bow.AnimationLooped += OnAnimationFinished;
        power = 0;
    }

    public override void _Process(double delta)
    {
        deltaTime = delta;
        UpdateAnimation();
        UpdateRotation();
        if (isAiming)
        {
            UpdatePower();
        }

		if (Input.IsActionJustPressed("left_mouse") && isCharged) // Left mouse button by default
		{
			isAiming = true;
            pressPosition = GetGlobalMousePosition();
			GD.Print("clicked left mouse");
		}
		if (Input.IsActionJustReleased("left_mouse")) // Left mouse button by default
		{
			AfterShot();
			GD.Print("released left mouse");
		}
    }

    public void UpdateRotation()
    {
        Vector2 mousePosition = GetGlobalMousePosition();
        Vector2 direction = mousePosition - GlobalPosition;
        float rotation = Atan2(direction.Y, direction.X);

        if (isCharged)
        {
			if(chargedTimer > 0)
			{
				bow.Rotation = Lerp(bow.Rotation, rotation, (float)deltaTime * 5 / (1 * chargedTimer));
				chargedTimer -= 0.1f;
				return;
			}
			bow.Rotation = rotation;
        }
        else
        {
            bow.Rotation = Lerp(bow.Rotation, 0, (float)deltaTime * 2);
        }
    }

    public void UpdateAnimation()
    {
        if (!isCharged)
        {
            bow.Play("charge");
            //isCharged = true;
        }
        else if (isAiming && isCharged)
        {
            float currentFrame = Lerp(bow.Frame, power, 0.5f * (float)deltaTime);
            bow.Play("attack");
            bow.Frame = (int)power;
        }
        else
        {
            bow.Play("idle");
        }
    }

    private void UpdatePower()
    {
        float distance = pressPosition.DistanceTo(GetGlobalMousePosition());
		//GD.Print($"distance {distance}");
        float maxDistance = 350;
        power = Min(8, 1 + 7 * (distance / maxDistance));
    }

    private void AfterShot()
    {
        isCharged = false;
        isAiming = false;
		shoot();
        GD.Print("Power: " + power);
    }

    public void shoot()
	{
		var arrow = ResourceLoader.Load<PackedScene>("res://Scenes/arrow.tscn").Instantiate() as Node2D;
		
		GetParent().GetParent().AddChild(arrow);
		arrow.GlobalPosition = bow.GlobalPosition;
		arrow.RotationDegrees = bow.RotationDegrees;
	}

    private void OnAnimationFinished()
    {
        if (bow.Animation == "charge")
        {
			chargedTimer = 2.5f;
            isCharged = true;
        }
        GD.Print("Animation finished");
    }

    private bool IsWithinRotationLimit(float rotation)
    {
        return (rotation * side) < 1.5 && (rotation * side) > -1.5;
    }
}
