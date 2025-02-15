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
	private List<swordman> spawnedSwords = new List<swordman>();
	private Timer enemySpawnTimer, spawnTimer;

	public override void _Ready()
	{
		enemySpawnTimer = new Timer();
		AddChild(enemySpawnTimer);
		enemySpawnTimer.WaitTime = EnemySpawnInterval;
		enemySpawnTimer.OneShot = false;
		enemySpawnTimer.Timeout += SpawnEnemy;
		enemySpawnTimer.Start();

		spawnTimer = new Timer();
		AddChild(spawnTimer);
		spawnTimer.WaitTime = SpawnInterval;
		spawnTimer.OneShot = false;
		spawnTimer.Timeout += SpawnSword;
		spawnTimer.Start();
	}

	private void SpawnEnemy()
	{
		if (spawnedEnemies.Count >= MaxEnemies)
			return;

		Vector2 spawnPoint = EnemySpawnPoints[GD.Randi() % EnemySpawnPoints.Length].Position;

		if (EnemyScene != null)
		{
			swordman enemyInstance = (swordman)EnemyScene.Instantiate();
			enemyInstance.Position = spawnPoint;

			GetParent().AddChild(enemyInstance);
			spawnedEnemies.Add(enemyInstance);

			enemyInstance.Connect("tree_exited", new Callable(this, nameof(OnEnemyRemoved)));
		}
	}

	private void SpawnSword()
	{
		if (spawnedSwords.Count >= MaxSwords)
			return;

		Vector2 spawnPoint = SpawnPoints[GD.Randi() % SpawnPoints.Length].Position;

		if (EnemyScene != null)
		{
			swordman swordInstance = (swordman)SwordScene.Instantiate();
			swordInstance.Position = spawnPoint;

			GetParent().AddChild(swordInstance);
			spawnedEnemies.Add(swordInstance);

			swordInstance.Connect("tree_exited", new Callable(this, nameof(OnEnemyRemoved)));
		}
	}

	private void OnEnemyRemoved(Node enemy)
	{
		
	}
}
