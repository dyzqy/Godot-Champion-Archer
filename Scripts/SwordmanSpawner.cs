using Godot;
using System.Collections.Generic;
using Godot.Collections;

public partial class SwordmanSpawner : Node
{
	//========================= Enemies ===========================
	[Export] public PackedScene EnemyScene { get; set; }
	[Export] public int MaxEnemies { get; set; } = 10;
	[Export] public float EnemySpawnInterval { get; set; } = 2.0f;
	[Export] public Node2D[] EnemySpawnPoints { get; set; }
	//========================= Swords ===========================
	[Export] public PackedScene SwordScene { get; set; }
	[Export] public int MaxSwords { get; set; } = 10;
	[Export] public float SpawnInterval { get; set; } = 2.0f;
	[Export] public Node2D[] SpawnPoints { get; set; }
	

	private List<swordman> spawnedEnemies = new List<swordman>();
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

		Vector2 spawnPoint = SpawnPoints[GD.Randi() % SpawnPoints.Length].Position;

		if (EnemyScene != null)
		{
			Node2D enemyInstance = (Node2D)EnemyScene.Instantiate();
			enemyInstance.Position = spawnPoint;

			GetParent().AddChild(enemyInstance);
			if(enemyInstance is swordman Sword) spawnedEnemies.Add(Sword);

			enemyInstance.Connect("tree_exited", new Callable(this, nameof(OnEnemyRemoved)));
		}
	}

	private void OnEnemyRemoved(Node enemy)
	{
		
	}
}
