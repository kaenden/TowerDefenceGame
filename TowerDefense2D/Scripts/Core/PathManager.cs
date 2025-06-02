using UnityEngine;
using System.Collections.Generic;

public class PathManager : MonoBehaviour
{
    [Header("Path Configuration")]
    public List<Transform> waypoints;
    public bool showPath = true;
    public Color pathColor = Color.yellow;
    public float pathWidth = 0.1f;
    
    private LineRenderer lineRenderer;
    
    private void Awake()
    {
        SetupLineRenderer();
    }
    
    private void Start()
    {
        UpdatePathVisualization();
    }
    
    private void SetupLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.color = pathColor;
        lineRenderer.startWidth = pathWidth;
        lineRenderer.endWidth = pathWidth;
        lineRenderer.useWorldSpace = true;
        lineRenderer.sortingOrder = -1;
    }
    
    private void UpdatePathVisualization()
    {
        if (!showPath || waypoints.Count < 2)
        {
            lineRenderer.enabled = false;
            return;
        }
        
        lineRenderer.enabled = true;
        lineRenderer.positionCount = waypoints.Count;
        
        for (int i = 0; i < waypoints.Count; i++)
        {
            lineRenderer.SetPosition(i, waypoints[i].position);
        }
    }
    
    public List<Transform> GetWaypoints()
    {
        return new List<Transform>(waypoints);
    }
    
    public void AddWaypoint(Transform waypoint)
    {
        waypoints.Add(waypoint);
        UpdatePathVisualization();
    }
    
    public void RemoveWaypoint(Transform waypoint)
    {
        waypoints.Remove(waypoint);
        UpdatePathVisualization();
    }
    
    public void InsertWaypoint(int index, Transform waypoint)
    {
        if (index >= 0 && index <= waypoints.Count)
        {
            waypoints.Insert(index, waypoint);
            UpdatePathVisualization();
        }
    }
    
    public void ClearWaypoints()
    {
        waypoints.Clear();
        UpdatePathVisualization();
    }
    
    public float GetPathLength()
    {
        if (waypoints.Count < 2) return 0f;
        
        float totalLength = 0f;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            totalLength += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
        }
        
        return totalLength;
    }
    
    public Vector3 GetSpawnPosition()
    {
        return waypoints.Count > 0 ? waypoints[0].position : Vector3.zero;
    }
    
    public Vector3 GetEndPosition()
    {
        return waypoints.Count > 0 ? waypoints[waypoints.Count - 1].position : Vector3.zero;
    }
    
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            UpdatePathVisualization();
        }
    }
    
    private void OnDrawGizmos()
    {
        if (waypoints.Count < 2) return;
        
        Gizmos.color = pathColor;
        
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
        
        // Draw waypoint markers
        Gizmos.color = Color.red;
        foreach (Transform waypoint in waypoints)
        {
            if (waypoint != null)
            {
                Gizmos.DrawWireSphere(waypoint.position, 0.2f);
            }
        }
    }
}