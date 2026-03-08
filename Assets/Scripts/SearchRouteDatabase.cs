using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchRouteDatabase : ScriptableObject
{
    public Dictionary<int, List<int>>RouteGraphs = new Dictionary<int, List<int>>();

    public List<int> SearchRoute(int start, int target)
    {
        HashSet<int> visited = new HashSet<int>();
        Queue<List<int>> paths = new Queue<List<int>>();
        paths.Enqueue(new List<int> { start});

        while (paths.Count > 0)
        {
            List<int> path = paths.Dequeue();
            int node = path[path.Count - 1];

            if(node == target)
            {
                return path;
            }

            if (!visited.Contains(node))
            {
                visited.Add(node);
                foreach (int item in RouteGraphs[node])
                {
                    List<int> newPath = new List<int>(path);
                    newPath.Add(item);
                    paths.Enqueue(newPath);
                }
            }
        }

        return null;
    }
}
