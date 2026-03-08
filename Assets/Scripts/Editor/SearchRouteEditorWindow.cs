using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SearchRouteEditorWindow : EditorWindow
{
    public static SearchRouteDatabase routeDatabase;
    private static SearchRouteEditorWindow searchWindow;
    private static string routeDatabasePath = "";

    private int startNode;
    private int targetNode;

    private int nodeLayers = 3;
    private GameObject nodeObject;

    [MenuItem("功能/路徑搜尋")]
    public static void OpenWindow()
    {
        searchWindow = GetWindow<SearchRouteEditorWindow>("路徑搜尋");
        routeDatabase = Resources.Load<SearchRouteDatabase>(routeDatabasePath);
        if(routeDatabase != null)
        {
            Debug.Log("routeDatabase 為 null，請重新載入");
        }
    }

    public void StartSearching()
    {
        List<int> path = routeDatabase.SearchRoute(startNode, targetNode);
        Debug.Log($"Result = {GetPath(path)}");

    }

    private string GetPath(List<int> path)
    {
        string res = "";
        foreach (int item in path)
        {
            res += item + ", ";
        }
        return res.Substring(0, res.Length - 2);
    }
}
