using Godot;
using System.Collections.Generic;
using Godot.Collections;

public partial class EnemySpawner : Node
{
    [Export] public PackedScene EnemyScene { get; set; } // Reference to the enemy scene to instantiate
    [Export] public int MaxEnemies { get; set; } = 10; // Maximum number of enemies to spawn
    [Export] public float SpawnInterval { get; set; } = 2.0f; // Time interval between spawns
    [Export] public Node2D[] SpawnPoints { get; set; } = {}; // Predefined spawn points

    private List<Node2D> spawnedEnemies = new List<Node2D>(); // Track spawned enemies
    private Timer spawnTimer;

    public override void _Ready()
    {
        // Initialize the spawn timer
        spawnTimer = new Timer();
        AddChild(spawnTimer);
        spawnTimer.WaitTime = SpawnInterval;
        spawnTimer.OneShot = false;
        spawnTimer.Timeout += SpawnEnemy;
        spawnTimer.Start();
    }

    private void SpawnEnemy()
    {
        // Don't spawn if the maximum number of enemies is reached
        if (spawnedEnemies.Count >= MaxEnemies)
            return;

        // Choose a random spawn point
        Vector2 spawnPoint = SpawnPoints[GD.Randi() % SpawnPoints.Length].Position;

        // Instance the enemy and set its position
        if (EnemyScene != null)
        {
            Node2D enemyInstance = (Node2D)EnemyScene.Instantiate();
            enemyInstance.Position = spawnPoint;

            // Add the enemy to the scene and track it
            GetParent().AddChild(enemyInstance);
            spawnedEnemies.Add(enemyInstance);

            // Use a Callable for the method connection
            enemyInstance.Connect("tree_exited", new Callable(this, nameof(OnEnemyRemoved)));
        }
    }

    private void OnEnemyRemoved(Node enemy)
    {
        // Remove the enemy from tracking when it exits the tree
        if (enemy is Node2D enemyNode && spawnedEnemies.Contains(enemyNode))
        {
            spawnedEnemies.Remove(enemyNode);
        }
    }
}
