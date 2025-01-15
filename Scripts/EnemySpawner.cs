using Godot;
using System.Collections.Generic;
using Godot.Collections;

public partial class EnemySpawner : Node
{
	[Export] public PackedScene EnemyScene { get; set; }
	[Export] public int MaxEnemies { get; set; } = 10;
	[Export] public float SpawnInterval { get; set; } = 2.0f;
	[Export] public Node2D[] SpawnPoints { get; set; }

	private List<Node2D> spawnedEnemies = new List<Node2D>();
	private Timer spawnTimer;

	public override void _Ready()
	{
		spawnTimer = new Timer();
		AddChild(spawnTimer);
		spawnTimer.WaitTime = SpawnInterval;
		spawnTimer.OneShot = false;
		spawnTimer.Timeout += SpawnEnemy;
		spawnTimer.Start();
	}

	private void SpawnEnemy()
	{
		if (spawnedEnemies.Count >= MaxEnemies)
			return;

		Vector2 spawnPoint = SpawnPoints[GD.Randi() % Mathf.Max(1, SpawnPoints.Length)].Position;

		if (EnemyScene != null)
		{
			Node2D enemyInstance = (Node2D)EnemyScene.Instantiate();
			enemyInstance.Position = spawnPoint;

			GetParent().AddChild(enemyInstance);
			spawnedEnemies.Add(enemyInstance);

			enemyInstance.Connect("tree_exited", new Callable(this, nameof(OnEnemyRemoved)));
		}
	}

	private void OnEnemyRemoved(Node enemy)
	{
		if (enemy is Node2D enemyNode && spawnedEnemies.Contains(enemyNode))
		{
			spawnedEnemies.Remove(enemyNode);
		}
	}
}
