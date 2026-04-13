using UnityEngine;

public class BoardInputHandler : MonoBehaviour
{
    public event System.Action<Cell> OnCellClicked;
    public event System.Action OnMissClicked;

    public bool StartInput {get; private set;}
    public LayerMask CellLayerMask = ~0;
    public float MaxRayDistance = 200f;
    public Camera TargetCamera;

    private void Awake()
    {
        if (TargetCamera == null)
            TargetCamera = Camera.main;
        StartInput = false;
    }

    public void SetInputState(bool bValue)
    {
        StartInput = bValue;
    }

    private void Update()
    {
        if (TargetCamera == null || !StartInput) return;

        Ray ray = TargetCamera.ScreenPointToRay(Input.mousePosition);

        bool clickEmpty = false;

        if (Physics.Raycast(ray, out RaycastHit hit, MaxRayDistance, CellLayerMask))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.transform.TryGetComponent(out Cell cell))
                {
                    Debug.Log($"cell => Col = {cell.Column}, Row = {cell.Row}");
                    OnCellClicked?.Invoke(cell);
                }
                else
                {
                    clickEmpty = true;
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            clickEmpty = true;
        }

        if (clickEmpty)
        {
            OnMissClicked?.Invoke();
            SetInputState(false);
        }
    }
}