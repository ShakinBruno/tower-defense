using System;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private Color baseColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] private ButtonConfig obstacle;
    [SerializeField] private ButtonConfig laserTower;
    [SerializeField] private ButtonConfig mortar;

    public TileContentType SelectedType { get; private set; }
    public TowerType SelectedTowerType { get; private set; }
    
    [Serializable]
    private struct ButtonConfig
    {
        public Button button;
        public Image image;
    }
    
    private void OnEnable()
    {
        obstacle.button.onClick.AddListener(() => 
            ChangeSelectedType(TileContentType.Obstacle, TowerType.None, obstacle.image));
        laserTower.button.onClick.AddListener(() => 
            ChangeSelectedType(TileContentType.Tower, TowerType.Laser, laserTower.image));
        mortar.button.onClick.AddListener(() =>
            ChangeSelectedType(TileContentType.Tower, TowerType.Mortar, mortar.image));
    }

    private void OnDisable()
    {
        obstacle.button.onClick.RemoveListener(() => 
            ChangeSelectedType(TileContentType.Obstacle, TowerType.None, obstacle.image));
        laserTower.button.onClick.RemoveListener(() => 
            ChangeSelectedType(TileContentType.Tower, TowerType.Laser, laserTower.image));
        mortar.button.onClick.RemoveListener(() =>
            ChangeSelectedType(TileContentType.Tower, TowerType.Mortar, mortar.image));
    }
    
    private void ChangeSelectedType(TileContentType type, TowerType towerType, Image image)
    {
        if (type == TileContentType.Tower)
        {
            SelectedType = TileContentType.Tower;

            if (SelectedTowerType == towerType)
            {
                SelectedType = TileContentType.None;
                SelectedTowerType = TowerType.None;
                image.color = baseColor;
            }
            else
            {
                SelectedTowerType = towerType;
                image.color = selectedColor;
                ResetButtonColorsExcept(image);
            }
        }
        else
        {
            SelectedTowerType = TowerType.None;
            
            if (SelectedType == type)
            {
                SelectedType = TileContentType.None;
                image.color = baseColor;
            }
            else
            {
                SelectedType = type;
                image.color = selectedColor;
                ResetButtonColorsExcept(image);
            }
        }
    }

    private void ResetButtonColorsExcept(Image image)
    {
        if (obstacle.image == image)
        {
            laserTower.image.color = baseColor;
            mortar.image.color = baseColor;
        }
        else if (laserTower.image == image)
        {
            obstacle.image.color = baseColor;
            mortar.image.color = baseColor;
        }
        else if (mortar.image == image)
        {
            obstacle.image.color = baseColor;
            laserTower.image.color = baseColor;
        }
    }
}