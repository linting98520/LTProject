using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GeometryController : MonoBehaviour
{
    [System.Serializable]
    public struct DrawTriangleData
    {
        public Vector2[] vertices;
        public bool Show;
        public DrawTriangleData(Vector2[] vertices, bool show)
        {
            this.vertices = vertices;
            this.Show = show;
        }
    }

    public struct VertexData
    {
        public Vector2 Position;
        public Vector2 UV;
    }

    public SpriteRenderer TemplateSpriteRenderer;

    public List<DrawTriangleData> TriangleShowList = new List<DrawTriangleData>();
    public float ShowSphereRadius = 0.1f;
    public Vector2 DivideLineA;
    public Vector2 DivideLineB;

    private List<VertexData> group01VerticesList = new List<VertexData>();
    private List<VertexData> group02VerticesList = new List<VertexData>();

    private List<VertexData> vertexDatas = new List<VertexData>(); //某 Sprite 全部的點資料
    private List<VertexData> intersectDatas = new List<VertexData>(); // 分割線重疊點的資料

    private List<GameObject> splitObjs = new List<GameObject>();

    public void Clear()
    {
        TriangleShowList = new List<DrawTriangleData>();
        group01VerticesList.Clear();
        group02VerticesList.Clear();
        int count = splitObjs.Count;
        for (int i = 0; i < count; i++)
            DestroyImmediate(splitObjs[i]);
        splitObjs.Clear();
    }

    public void ShowSpriteVertices(SpriteRenderer renderer)
    {

    }

    public void DoCut()
    {
        ClassifyVertices(DivideLineA, DivideLineB);
    }

    [Button]
    public void ClassifyVertices(Vector3 p1, Vector3 p2)
    {
        int vertexCount = TemplateSpriteRenderer.sprite.vertices.Length;

        //得 Sprite 點，並逆時針排列
        int[] indices = GetClockWiseIndices(TemplateSpriteRenderer.sprite.vertices);
        vertexDatas.Clear();
        for (int i = 0; i < vertexCount; i++)
        {
            int index = indices[i];
            VertexData vertexData = new VertexData();
            vertexData.Position = TemplateSpriteRenderer.sprite.vertices[index];
            vertexData.UV = TemplateSpriteRenderer.sprite.uv[index];
            vertexDatas.Add(vertexData);
        }

        //把分割線兩側的點轉成與圖片相同座標體系方便計算
        Vector2 divideP1 = TemplateSpriteRenderer.transform.InverseTransformPoint(p1);
        Vector2 divideP2 = TemplateSpriteRenderer.transform.InverseTransformPoint(p2);

        //嘗試獲得交點，至少有兩個表示成功切割
        RecordIntersectVertexData(vertexDatas, divideP1, divideP2);

        if (intersectDatas.Count <= 1)
        {
            Debug.Log("切割失敗");
            return;
        }

        //初始化頂點群組
        group01VerticesList.Clear();
        group01VerticesList.AddRange(intersectDatas);

        group02VerticesList.Clear();
        group02VerticesList.AddRange(intersectDatas);

        //分類頂點，分為在分割線的左邊或右邊
        Vector2 intersectP1 = intersectDatas[0].Position;
        Vector2 intersectP2 = intersectDatas[1].Position;

        for (int i = 0; i < vertexDatas.Count; i++)
        {
            Vector2 point = vertexDatas[i].Position;

            var res = CollisionPointTool.CaculateCross(intersectP1, point, intersectP1, intersectP2);
            if (res <= 0)
                group01VerticesList.Add(vertexDatas[i]);
            else 
                group02VerticesList.Add(vertexDatas[i]);
        }

        //建立Mesh
        group02VerticesList = GetClockWiseIndices(group02VerticesList);
        CreateVertices("Split02", group02VerticesList);

        group01VerticesList = GetClockWiseIndices(group01VerticesList);
        CreateVertices("Split01", group01VerticesList);
    }

    private List<VertexData> GetClockWiseIndices(List<VertexData> datas)
    {
        List<VertexData> res = new List<VertexData>();

        int vertexCount = datas.Count;
        Vector2 center = Vector2.zero;
        foreach (var data in datas)
        {
            center += data.Position;
        }
        center = center /= vertexCount;

        // 記錄點
        int[] indices = new int[vertexCount];
        float[] angles = new float[vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            indices[i] = i;
            angles[i] = Mathf.Atan2(datas[i].Position.y - center.y, datas[i].Position.x - center.x);
        }

        // 排序
        System.Array.Sort(angles, indices);

        for (int i = 0; i < vertexCount; i++)
        {
            res.Add(datas[indices[i]]);
        }

        return res;
    }

    private int[] GetClockWiseIndices(Vector2[] spriteVertices)
    {
        // 計算中心點
        Vector2 center = Vector2.zero;
        foreach (var vertex in spriteVertices)
        {
            center += vertex;
        }
        center = center /= spriteVertices.Length;

        // 記錄點
        int vertexCount = spriteVertices.Length;
        int[] indices = new int[vertexCount];
        float[] angles = new float[vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            indices[i] = i;
            angles[i] = Mathf.Atan2(spriteVertices[i].y - center.y, spriteVertices[i].x - center.x);
        }

        // 排序
        System.Array.Sort(angles, indices);

        return indices;
    }

    private void RecordIntersectVertexData(List<VertexData> vertexDatas, Vector2 v1, Vector2 v2)
    {
        intersectDatas.Clear();

        for (int i = 0; i < vertexDatas.Count; i++)
        {
            int p1_index = i;
            int p2_index = (i + 1) % vertexDatas.Count;
            Vector2 p1 = vertexDatas[p1_index].Position;
            Vector2 p2 = vertexDatas[p2_index].Position;

            if (CollisionPointTool.TryGetIntersectPoint(p1, p2, v1, v2, out Vector2 point))
            {
                // 計算比例 t
                float t = Vector2.Distance(p1, point) / Vector2.Distance(p1, p2);

                // 計算新的 UV
                Vector2 newUV = Vector2.Lerp(vertexDatas[p1_index].UV, vertexDatas[p2_index].UV, t);

                VertexData vertexData = new VertexData();
                vertexData.Position = point;
                vertexData.UV = newUV;
                intersectDatas.Add(vertexData);
            }
        }
    }

    private void CreateVertices(string objName, List<VertexData> vertexDatas)
    {
        // 提取排序後的頂點與 UV
        int vertexCount = vertexDatas.Count;
        Vector3[] finalVertices = new Vector3[vertexCount];
        Vector2[] finalUVs = new Vector2[vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            finalVertices[i] = vertexDatas[i].Position;
            finalUVs[i] = vertexDatas[i].UV;
        }

        // 紀錄三角形
        int triangleCount = finalVertices.Length - 2;
        int[] triangles = new int[triangleCount * 3];
        for (int i = 0; i < triangleCount; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        // 建立 Mesh 物件
        CreateMeshObject(objName, finalVertices, finalUVs, triangles);
    }

    [Button]
    public void CreateVertices()
    {
        if (TemplateSpriteRenderer == null || TemplateSpriteRenderer.sprite == null) return;

        // 獲取原始數據
        Vector2[] spriteVertices = TemplateSpriteRenderer.sprite.vertices;
        Vector2[] spriteUVs = TemplateSpriteRenderer.sprite.uv;

        int[] indices = GetClockWiseIndices(spriteVertices);

        // 提取排序後的頂點與 UV
        int vertexCount = spriteVertices.Length;
        Vector3[] finalVertices = new Vector3[vertexCount];
        Vector2[] finalUVs = new Vector2[vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            int index = indices[i];
            finalVertices[i] = spriteVertices[index];
            finalUVs[i] = spriteUVs[index];
        }

        // 紀錄三角形
        int triangleCount = finalVertices.Length - 2;
        int[] triangles = new int[triangleCount * 3];
        for (int i = 0; i < triangleCount; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        // 建立 Mesh 物件
        CreateMeshObject("Object", finalVertices, finalUVs, triangles);

        RecordGizmosTraingleList(spriteVertices);
    }

    private void CreateMeshObject(string objName, Vector3[] vertices, Vector2[] uvs, int[] triangles)
    {
        GameObject obj = new GameObject(objName);
        obj.transform.position = TemplateSpriteRenderer.transform.position;
        obj.transform.rotation = TemplateSpriteRenderer.transform.rotation;
        obj.transform.localScale = TemplateSpriteRenderer.transform.localScale;

        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Sprites/Default"));
        mr.material.mainTexture = TemplateSpriteRenderer.sprite.texture;

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mf.mesh = mesh;

        splitObjs.Add(obj);
    }

    private void RecordGizmosTraingleList(Vector2[] spriteVertices)
    {
        TriangleShowList.Clear();
        int triangleCount = spriteVertices.Length - 2;
        for (int i = 0; i < triangleCount; i++)
        {
            Vector2[] vertices = new Vector2[3];
            vertices[0] = spriteVertices[0];
            vertices[1] = spriteVertices[i + 1];
            vertices[2] = spriteVertices[i + 2];
            TriangleShowList.Add(new DrawTriangleData(vertices, false));
        }
    }

    private void OnDrawGizmos()
    {
        if (TriangleShowList.Count != 0)
        {
            for (int i = 0; i < TriangleShowList.Count; i++)
            {
                DrawTriangleData data = TriangleShowList[i];
                if (!data.Show)
                    continue;

                Gizmos.color = new Color(Random.Range(0f, 1.0f), Random.Range(0f, 1.0f), Random.Range(0f, 1.0f), 1);
                for (int j = 0; j < data.vertices.Length; j++)
                {
                    Vector2[] vertices = data.vertices;
                    int count = vertices.Length;
                    int next = (j + 1) % count;
                    Gizmos.DrawLine(vertices[j], vertices[next]);
                }
            }
        }

        Gizmos.color = Color.blue;
        foreach (var vertex in group02VerticesList)
        {
            Gizmos.DrawSphere(TemplateSpriteRenderer.transform.TransformPoint(vertex.Position), ShowSphereRadius);
        }

        Gizmos.color = Color.red;
        foreach (var vertex in group01VerticesList)
        {
            Gizmos.DrawSphere(TemplateSpriteRenderer.transform.TransformPoint(vertex.Position), ShowSphereRadius);
        }

        Gizmos.color = Color.magenta;
        foreach (var p in intersectDatas)
        {
            Gizmos.DrawSphere(TemplateSpriteRenderer.transform.TransformPoint(p.Position), ShowSphereRadius);
        }
    }
}

#region 測試用
//public List<ComplexItem> items = new List<ComplexItem>() { new ComplexItem() };

[System.Serializable]
public class ComplexItem
{
    public bool isEnabled;
    public float speed;
    public int health;
    public List<string> tags; // 內部 List
    public int[] values;      // 內部 Array
    public Vector2 position;
    public string name = "AAA";
}
#endregion
