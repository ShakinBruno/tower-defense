using UnityEngine;
using UnityEngine.EventSystems;

public class TouchGestures : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector3 positionMin, positionMax;
    [SerializeField] private Vector3 zoomMin, zoomMax;

    private new Camera camera;
    private Plane plane;
    private Vector3 newPosition;
    private Vector3 newZoom;
    private Vector3 newRotation;

    private void Awake()
    {
        camera = cameraTransform.GetComponent<Camera>();
        newPosition = transform.localPosition;
        newZoom = cameraTransform.localPosition;
        newRotation = transform.localEulerAngles;
    }

    private void LateUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        plane.SetNormalAndPosition(-transform.forward, transform.localPosition);

        if (Input.touchCount == 1)
        {
            SwipeToMove();
        }
        else if (Input.touchCount >= 2)
        {
            Vector3 pos1Begin = TouchPosition(Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition);
            Vector3 pos2Begin = TouchPosition(Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);
            Vector3 pos1End = TouchPosition(Input.GetTouch(0).position);
            Vector3 pos2End = TouchPosition(Input.GetTouch(1).position);

            PinchToZoom(pos1Begin, pos1End, pos2Begin, pos2End);
            ScrollToRotate(pos1Begin, pos1End, pos2Begin, pos2End);
        }
    }

    private void ScrollToRotate(Vector3 pos1Begin, Vector3 pos1End, Vector3 pos2Begin, Vector3 pos2End)
    {
        if (pos2Begin == pos2End) return;
        float angleY = Vector3.SignedAngle(pos2End - pos1End, pos2Begin - pos1Begin, plane.normal);
        newRotation.y += angleY;
        transform.rotation = Quaternion.Euler(newRotation.x, newRotation.y, 0f);
    }

    private void PinchToZoom(Vector3 pos1Begin, Vector3 pos1End, Vector3 pos2Begin, Vector3 pos2End)
    {
        float zoom = Vector3.Distance(pos1End, pos2End) - Vector3.Distance(pos1Begin, pos2Begin);
        newZoom.z += zoom;
        newZoom.z = Mathf.Clamp(newZoom.z, zoomMin.z, zoomMax.z);
        cameraTransform.localPosition = new Vector3(0f, 0f, newZoom.z);
    }

    private void SwipeToMove()
    {
        Vector3 delta = transform.InverseTransformDirection(TouchPositionDelta(Input.GetTouch(0)));
        newPosition += transform.TransformDirection(new Vector3(delta.x , 0f, delta.y));
        newPosition.x = Mathf.Clamp(newPosition.x, positionMin.x, positionMax.x);
        newPosition.z = Mathf.Clamp(newPosition.z, positionMin.z, positionMax.z);
        transform.localPosition = new Vector3(newPosition.x, 0f, newPosition.z);
    }

    private Vector3 TouchPositionDelta(Touch touch)
    {
        if (touch.phase != TouchPhase.Moved) return Vector3.zero;
        Ray rayBegin = camera.ScreenPointToRay(touch.position - touch.deltaPosition);
        Ray rayEnd = camera.ScreenPointToRay(touch.position);
        
        if (plane.Raycast(rayBegin, out float enterStart) && plane.Raycast(rayEnd, out float enterEnd))
        {
            return rayBegin.GetPoint(enterStart) - rayEnd.GetPoint(enterEnd);
        }

        return Vector3.zero;
    }

    private Vector3 TouchPosition(Vector2 screenPosition)
    {
        Ray ray = camera.ScreenPointToRay(screenPosition);
        return plane.Raycast(ray, out float enter) ? ray.GetPoint(enter) : Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(positionMax.x * 2f, 0f, positionMax.z * 2f));
    }
}