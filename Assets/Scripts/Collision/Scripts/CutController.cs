using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Collections;
using UnityEngine;

public class CutController : MonoBehaviour
{
    public Vector2 InteractiveArea = Vector2.one;
    public float PointDis = 0.1f;

    public bool _isStarting = false;
    public Vector3 _startPos = Vector3.zero;
    public Vector3 _endPos = Vector3.zero;

    public LineRenderer _lineRenderer;
    public float LinePosZ = 5;

    [SerializeField] private GeometryController geometryController;

    private List<Vector3> _lineRendererPoints = new List<Vector3>();
    private Vector3 _curPoint = Vector3.zero;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isStarting = true;
            _lineRendererPoints.Clear();
            _startPos = GetMouseWorldPos(Input.mousePosition);
            AddLineRendererPoint(_startPos);
        }
        else if (Input.GetMouseButtonUp(0) && _isStarting)
        {
            _isStarting = false;
            _endPos = GetMouseWorldPos(Input.mousePosition);
            AddLineRendererPoint(_endPos);
            DoCut();
        }
        LintRendererPointsUpdate();
    }

    private void LintRendererPointsUpdate()
    {
        if (_isStarting)
        {
            Vector3 curMousePos = GetMouseWorldPos(Input.mousePosition);
            float dis = (curMousePos - _curPoint).sqrMagnitude;
            if (dis >= PointDis * PointDis)
            {
                AddLineRendererPoint(curMousePos);
            }
        }
    }

    private void AddLineRendererPoint(Vector3 point)
    {
        _lineRendererPoints.Add(point);
        _curPoint = point;
        _lineRenderer.positionCount = _lineRendererPoints.Count;
        _lineRenderer.SetPosition(_lineRendererPoints.Count - 1, _curPoint);
    }

    private Vector3 GetMouseWorldPos(Vector3 inputPos)
    {
        Vector3 mousePos = inputPos;
        mousePos.z = LinePosZ;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void DoCut()
    {
        Vector3 p1 = new Vector3(_lineRendererPoints[0].x, _lineRendererPoints[0].y, _lineRendererPoints[0].z);
        int last = _lineRendererPoints.Count - 1;
        Vector3 p2 = new Vector3(_lineRendererPoints[last].x, _lineRendererPoints[last].y, _lineRendererPoints[last].z);
        geometryController.ClassifyVertices(p1, p2);
        _lineRenderer.positionCount = 0;
    }
}
