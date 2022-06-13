using TMPro;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(TextMeshPro))]
public class CoordinateLabeler : MonoBehaviour
{
    private TextMeshPro label;
    private Vector2Int coordinates;

    private void Awake()
    {
        label = GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        DisplayCoordinates();
    }

    private void DisplayCoordinates()
    {
        coordinates.x = Mathf.RoundToInt(transform.parent.position.x);
        coordinates.y = Mathf.RoundToInt(transform.parent.position.z);
        label.text = $"{coordinates.x},{coordinates.y}";
        transform.parent.name = coordinates.ToString();
    }
}