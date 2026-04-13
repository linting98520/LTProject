using UnityEngine;

public class BoardInputHandler : MonoBehaviour
{
    public LayerMask CellLayerMask = ~0;

    public float MaxRayDistance = 200f;

    public Camera TargetCamera;

    void Awake()
    {
        if (TargetCamera == null)
            TargetCamera = Camera.main;
    }

    void Update()
    {
        if (TargetCamera == null) return;

        Ray ray = TargetCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, MaxRayDistance, CellLayerMask))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log($"Cast => {hit.collider.name}");
            }
        }
    }
}