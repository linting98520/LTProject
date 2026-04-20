using UnityEngine;

public class BoardInputHandler : MonoBehaviour
{
    public event System.Action<Cell> OnCellClicked;
    public event System.Action OnMissClicked;

    public BoardManager BoardManager;
    public LayerMask CellLayerMask = ~0;
    public float MaxRayDistance = 200f;
    public Camera TargetCamera;
    public bool StartInput {get; private set;}

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
            if (hit.transform.TryGetComponent(out Cell cell))
            {
                BoardManager.EnterCell(cell);
            }
        }
        else
        {
            BoardManager.ClearCell();
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if (BoardManager.HoveredCell == null)
                clickEmpty = true;
            else
                OnCellClicked?.Invoke(BoardManager.HoveredCell);
        }

        if (clickEmpty)
        {
            OnMissClicked?.Invoke();
            SetInputState(false);
        }
    }
}