using UnityEngine;

[RequireComponent(typeof(Renderer), typeof(Collider))]
public class Cell : MonoBehaviour
{
    public int Column { get; private set; }
    public int Row { get; private set; }
    public bool IsBuild { get; private set; }
    public int ID { get; private set; }

    private Renderer rend;
    private Material originalMaterial;

    public void Initialize(int col, int row, int id)
    {
        Column = col;
        Row = row;
        rend = GetComponent<Renderer>();
        originalMaterial = rend.material;
        ID = id;
    }

    public void SetMaterial(Material mat)
    {
        if (rend != null) 
            rend.material = mat;
    }

    public void ResetMaterial()
    {
        if (rend != null && originalMaterial != null)
            rend.material = originalMaterial;
    }

    public void SetBuildingState(bool isBuild)
    {
        IsBuild = isBuild;
    }
}