using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
    [SerializeField] private Transform target; // objeto que mira al punto
    [SerializeField] private LayerMask floorLayer; // asigna la layer "Floor" en el inspector

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, floorLayer))
        {
            Vector3 lookPoint = hit.point;
            lookPoint.y = target.position.y; // mantener la altura del target
            target.LookAt(lookPoint);
        }
    }
}
