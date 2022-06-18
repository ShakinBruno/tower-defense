using UnityEngine;
using UnityEngine.EventSystems;

public class Game : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private TileContentFactory tileContentFactory;
    [SerializeField] private EnemyFactory enemyFactory;
    [SerializeField] private WarFactory warFactory;
    [SerializeField, Range(0.1f, 10f)] private float spawnSpeed = 1f;

    private bool isDragging;
    private float spawnProgress;
    private readonly GameBehaviorCollection enemies = new GameBehaviorCollection();
    private readonly GameBehaviorCollection nonEnemies = new GameBehaviorCollection();
    private static Game instance;
    private Camera mainCamera;
    private UIHandler uiHandler;

    private void Awake()
    {
        mainCamera = Camera.main;
        uiHandler = GetComponent<UIHandler>();
        board.Initialize(tileContentFactory);
    }

    private void OnEnable()
    {
        instance = this;
    }

    private void OnDisable()
    {
        instance = null;
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
        Physics.SyncTransforms();
        board.GameUpdate();
        nonEnemies.GameUpdate();
    }

    public static Shell SpawnShell()
    {
        Shell shell = instance.warFactory.Shell;
        instance.nonEnemies.Add(shell);
        return shell;
    }

    public static Explosion SpawnExplosion()
    {
        Explosion explosion = instance.warFactory.Explosion;
        instance.nonEnemies.Add(explosion);
        return explosion;
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
        if (EventSystem.current.IsPointerOverGameObject() || isDragging) return;
        Ray touchRay = mainCamera.ScreenPointToRay(touch.position);
        Tile tile = board.GetTile(touchRay);
        if (tile == null) return;

        switch (uiHandler.SelectedType)
        {
            case TileContentType.Obstacle:
                board.ToggleObstacle(tile);
                break;
            case TileContentType.Tower:
                board.ToggleTower(tile, uiHandler.SelectedTowerType);
                break;
        }
    }
}