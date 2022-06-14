using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private TileContentFactory tileContentFactory;
    [SerializeField] private EnemyFactory enemyFactory;
    [SerializeField, Range(0.1f, 10f)] private float spawnSpeed = 1f;

    private bool isDragging;
    private float spawnProgress;
    private readonly EnemyCollection enemies = new EnemyCollection();
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        board.Initialize(tileContentFactory);
    }

    private void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch primaryTouch = Input.GetTouch(0);

            switch (primaryTouch.phase)
            {
                case TouchPhase.Began:
                    isDragging = false;
                    break;
                case TouchPhase.Moved:
                    isDragging = true;
                    break;
                case TouchPhase.Ended:
                    HandleTouch(primaryTouch);
                    break;
            }
        }

        spawnProgress += spawnSpeed * Time.deltaTime;
        
        while (spawnProgress >= 1f)
        {
            spawnProgress -= 1f;
            SpawnEnemy();
        }
        
        enemies.GameUpdate();
    }

    private void SpawnEnemy()
    {
        Tile spawnPoint = board.GetSpawnPoint(Random.Range(0, board.SpawnPointCount));
        Enemy enemy = enemyFactory.Get();
        enemy.SpawnOn(spawnPoint);
        enemies.Add(enemy);
    }

    private void HandleTouch(Touch touch)
    {
        if (isDragging) return;
        Ray touchRay = mainCamera.ScreenPointToRay(touch.position);
        Tile tile = board.GetTile(touchRay);
        if (tile != null) board.ToggleObstacle(tile);
    }
}