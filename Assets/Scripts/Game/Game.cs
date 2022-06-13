using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private TileContentFactory tileContentFactory;
    
    private bool isDragging;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        board.Initialize(tileContentFactory);
    }

    private void Update()
    {
        if (Input.touchCount < 1) return;
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

    private void HandleTouch(Touch touch)
    {
        if (isDragging) return;
        Ray touchRay = mainCamera.ScreenPointToRay(touch.position);
        Tile tile = board.GetTile(touchRay);
        if (tile != null) board.ToggleObstacle(tile);
    }
}