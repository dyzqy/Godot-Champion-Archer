using Godot;
using System;
using static Godot.Mathf;

public partial class BowControl : AnimatedSprite2D
{
	AnimatedSprite2D bow;

	bool isCharged = true;
	bool isAiming = false;
	float power; // from 1 to 8, based on "attack" anim, 9th frame is releasing arrow.
	float side = 1;

	private double deltaTime;

	Vector2 pressPosition;
	Vector2 releasePosition;
	Vector2 originalDirection;

	public override void _Ready()
	{
		bow = this;
		power = 0;
	}

	public override void _Process(double delta)
	{
		deltaTime = delta;
		animate();
		move();
		if (isAiming)
		{
			CalculatePower();
		}
	}

	public void move()
	{
		Vector2 mousePosition = GetGlobalMousePosition();
		Vector2 direction = mousePosition - GlobalPosition;
		float rotation = Mathf.Atan2(direction.Y, direction.X);

		if (isCharged && !isAiming)
		{
			if ((rotation * side) < 1.5 && (rotation * side) > -1.5)
			{
				bow.Rotation = rotation;
			}
		}
		else if (isCharged && isAiming)
		{
			if ((rotation * side) < 1.5 && (rotation * side) > -1.5)
			{
				Vector2 newDirection = direction.Normalized();
				float dot = originalDirection.Dot(newDirection);
				float maxRotation = 0.5f;
				float newRotation = Lerp(bow.Rotation, rotation, dot * maxRotation);
				bow.Rotation = newRotation;
			}
		}
		else if (!isCharged)
		{
			bow.Rotation = 0;
		}
	}

	public void animate()
	{
		if (!isCharged)
		{
			bow.Play("charge");
			isCharged = true;
		}
		else if (isAiming && isCharged)
		{
			float currentFrame = Lerp(bow.Frame, power, 0.5f * (float)deltaTime);
			bow.Play("attack");
			bow.Frame = (int)currentFrame;
		}
		else if (!isAiming && isCharged)
		{
			bow.Play("idle");
		}
		else
		{
			bow.Play("idle");
		}
	}

	private void CalculatePower()
	{
		float distance = pressPosition.DistanceTo(releasePosition);
		float maxDistance = 750;
		float minDistance = 0;

		power = 1 + 7 * (distance - minDistance) / (maxDistance - minDistance);
	}

	private void AfterShooting()
	{
		isCharged = false;
		isAiming = false;
		GD.Print("Power: " + power);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed)
			{
				isAiming = true;
				pressPosition = GetGlobalMousePosition();
			}
			else if (!mouseEvent.Pressed)
			{
				releasePosition = GetGlobalMousePosition();
				AfterShooting();
			}
		}
	}

	private void BowAnimFinished()
	{
		if (bow.Animation == "charge")
		{
			isCharged = true;
		}
		GD.Print("finished");
	}
}
