using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private Scenario scenario;
    [SerializeField] private TileContentFactory tileContentFactory;
    [SerializeField] private WarFactory warFactory;
    [SerializeField, Range(0, 100)] private int startingPlayerHealth;
    [SerializeField, Range(0, 10000)] private int startingPlayerBank;
    [SerializeField] private float[] gameSpeeds;

    private bool isDragging;
    private bool isPaused;
    private int gameSpeedIndex;
    private int playerHealth;
    private int playerBank;
    private float selectedSpeed;
    private readonly GameBehaviorCollection enemies = new GameBehaviorCollection();
    private readonly GameBehaviorCollection nonEnemies = new GameBehaviorCollection();
    private Tile selectedTile;
    private Scenario.State activeScenario;
    private Camera mainCamera;
    private static Game instance;
    public bool HasGameStarted { get; private set; }
    public static bool SkipTimer { get; set; }
    public TileContentType SelectedType { get; set; }
    public TowerType SelectedTowerType { get; set; }

    private void Awake()
    {
        instance = this;
        mainCamera = Camera.main;
        selectedSpeed = gameSpeeds[gameSpeedIndex];
        playerHealth = startingPlayerHealth;
        playerBank = startingPlayerBank;
        board.Initialize(tileContentFactory);
        activeScenario = scenario.Begin();
    }

    private void Start()
    {
        GameHUD.UpdateWaveInfo("Start a new game.");
        GameHUD.UpdatePrompt("Start");
        GameHUD.UpdateBank(playerBank);
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

        Time.timeScale = isPaused ? 0f : selectedSpeed;
        if (playerHealth <= 0 && startingPlayerHealth > 0) StartCoroutine(GameLost());

        enemies.GameUpdate();
        Physics.SyncTransforms();
        board.GameUpdate();
        nonEnemies.GameUpdate();
    }

    public void StartOrSkipWave(bool gameStarted)
    {
        if (gameStarted)
        {
            SkipTimer = true;
        }
        else
        {
            HasGameStarted = true;
            GameHUD.UpdateHealth(playerHealth);
            GameHUD.SetPromptLabelActive(false);
            GameHUD.UpdatePrompt("Skip");
            instance.StartCoroutine(instance.activeScenario.Progress(ScenarioFinished()));
        }
    }

    public float ChangeGameSpeed()
    {
        gameSpeedIndex = ++gameSpeedIndex % gameSpeeds.Length;
        selectedSpeed = gameSpeeds[gameSpeedIndex];
        return selectedSpeed;
    }

    public void PauseGame(bool pause)
    {
        isPaused = pause;
    }

    public void SellContent()
    {
        switch (selectedTile.Content.Type)
        {
            case TileContentType.Obstacle:
                board.SellObstacle(selectedTile, ref playerBank);
                break;
            case TileContentType.Tower:
                board.SellTower(selectedTile, ref playerBank);
                break;
        }
        
        GameHUD.UpdateBank(playerBank);
        GameHUD.SetManageLayoutActive(false);
        GameHUD.SetContentLayoutActive(true);
    }

    public static void EnemyReachedDestination()
    {
        instance.playerHealth--;
        GameHUD.UpdateHealth(instance.playerHealth);
    }

    public static void EnemyKilled(float prize)
    {
        int prizeRounded = Mathf.RoundToInt(prize);
        instance.playerBank += prizeRounded;
        GameHUD.UpdateBank(instance.playerBank);
    }

    public static Shell SpawnShell()
    {
        Shell shell = instance.warFactory.Shell;
        instance.nonEnemies.Add(shell);
        return shell;
    }

    public static Missile SpawnMissile()
    {
        Missile missile = instance.warFactory.Missile;
        instance.nonEnemies.Add(missile);
        return missile;
    }

    public static Explosion SpawnShellExplosion()
    {
        Explosion explosion = instance.warFactory.ShellExplosion;
        instance.nonEnemies.Add(explosion);
        return explosion;
    }
    
    public static Explosion SpawnMissileExplosion()
    {
        Explosion explosion = instance.warFactory.MissileExplosion;
        instance.nonEnemies.Add(explosion);
        return explosion;
    }

    public static void SpawnEnemy(EnemyFactory factory, EnemyType type)
    {
        Tile spawnPoint = instance.board.GetSpawnPoint(Random.Range(0, instance.board.SpawnPointCount));
        Enemy enemy = factory.Get(type);
        enemy.SpawnOn(spawnPoint);
        instance.enemies.Add(enemy);
    }

    private IEnumerator GameLost()
    {
        Debug.Log("Defeat!");
        yield return new WaitForSecondsRealtime(3f);
        RestartLevel();
    }

    private IEnumerator ScenarioFinished()
    {
        yield return new WaitUntil(() => enemies.IsEmpty);
        Debug.Log("Victory!");
        yield return new WaitForSecondsRealtime(3f);
        RestartLevel();
    }

    private void RestartLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    private void HandleTouch(Touch touch)
    {
        if (EventSystem.current.IsPointerOverGameObject() || instance.isDragging) return;
        GameHUD.SetManageLayoutActive(false);
        GameHUD.SetContentLayoutActive(true);
        selectedTile = null;
        Ray touchRay = instance.mainCamera.ScreenPointToRay(touch.position);
        Tile tile = instance.board.GetTile(touchRay);
        if (tile == null) return;
        selectedTile = tile;

        if (SelectedType != TileContentType.None)
        {
            switch (SelectedType)
            {
                case TileContentType.Obstacle:
                    instance.board.BuyObstacle(tile, ref playerBank);
                    break;
                case TileContentType.Tower:
                    instance.board.BuyTower(tile, SelectedTowerType, ref playerBank);
                    break;
            }
            
            GameHUD.UpdateBank(playerBank);
        }
        else if (selectedTile.Content.isObstacle || selectedTile.Content.isTower)
        {
            GameHUD.SetContentLayoutActive(false);
            GameHUD.SetManageLayoutActive(true);
        }
    }
}