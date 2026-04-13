using UnityEngine;

[RequireComponent(typeof(Renderer), typeof(Collider))]
public class Cell : MonoBehaviour
{
    public int Column { get; private set; }
    public int Row { get; private set; }

    private Renderer rend;
    private Material originalMaterial;

    public void Initialize(int col, int row)
    {
        Column = col;
        Row = row;
        rend = GetComponent<Renderer>();
        originalMaterial = rend.material;
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
}