using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(LevelEditor))]
public class LevelEditorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        LevelEditor levelEditor = (LevelEditor)target;
        
        GUILayout.Space(10);
        GUILayout.Label("Level Editor Tools", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Add Waypoint"))
        {
            levelEditor.AddWaypoint();
        }
        
        if (GUILayout.Button("Clear All Waypoints"))
        {
            levelEditor.ClearWaypoints();
        }
        
        if (GUILayout.Button("Auto-Generate Path"))
        {
            levelEditor.AutoGeneratePath();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Save Level"))
        {
            levelEditor.SaveLevel();
        }
        
        if (GUILayout.Button("Load Level"))
        {
            levelEditor.LoadLevel();
        }
    }
}
#endif

public class LevelEditor : MonoBehaviour
{
    [Header("Level Settings")]
    public string levelName = "Level1";
    public Texture2D backgroundTexture;
    public Vector2 levelSize = new Vector2(20, 12);
    
    [Header("Path Settings")]
    public GameObject waypointPrefab;
    public Material pathMaterial;
    public float pathWidth = 0.5f;
    
    [Header("Placement Areas")]
    public GameObject placementAreaPrefab;
    public List<Rect> placementAreas = new List<Rect>();
    
    [Header("Obstacles")]
    public GameObject obstaclePrefab;
    public List<Vector2> obstaclePositions = new List<Vector2>();
    
    private PathManager pathManager;
    private List<GameObject> waypointObjects = new List<GameObject>();
    private List<GameObject> placementAreaObjects = new List<GameObject>();
    private List<GameObject> obstacleObjects = new List<GameObject>();
    
    private void Awake()
    {
        pathManager = GetComponent<PathManager>();
        if (pathManager == null)
        {
            pathManager = gameObject.AddComponent<PathManager>();
        }
    }
    
    public void AddWaypoint()
    {
        Vector3 position = Vector3.zero;
        
        if (waypointObjects.Count > 0)
        {
            // Place new waypoint slightly offset from the last one
            position = waypointObjects[waypointObjects.Count - 1].transform.position + Vector3.right * 2f;
        }
        
        GameObject waypoint = CreateWaypoint(position);
        waypointObjects.Add(waypoint);
        UpdatePath();
    }
    
    private GameObject CreateWaypoint(Vector3 position)
    {
        GameObject waypoint;
        
        if (waypointPrefab != null)
        {
            waypoint = Instantiate(waypointPrefab, position, Quaternion.identity, transform);
        }
        else
        {
            waypoint = new GameObject($"Waypoint_{waypointObjects.Count + 1}");
            waypoint.transform.position = position;
            waypoint.transform.SetParent(transform);
            
            // Add visual indicator
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.transform.SetParent(waypoint.transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = Vector3.one * 0.3f;
            
            Renderer renderer = visual.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.yellow;
            }
        }
        
        return waypoint;
    }
    
    public void ClearWaypoints()
    {
        foreach (GameObject waypoint in waypointObjects)
        {
            if (waypoint != null)
            {
                DestroyImmediate(waypoint);
            }
        }
        
        waypointObjects.Clear();
        UpdatePath();
    }
    
    public void AutoGeneratePath()
    {
        ClearWaypoints();
        
        // Generate a simple S-curve path
        Vector3 startPos = new Vector3(-levelSize.x / 2, 0, 0);
        Vector3 endPos = new Vector3(levelSize.x / 2, 0, 0);
        
        int waypointCount = 8;
        for (int i = 0; i < waypointCount; i++)
        {
            float t = (float)i / (waypointCount - 1);
            Vector3 position = Vector3.Lerp(startPos, endPos, t);
            
            // Add some curves
            position.y += Mathf.Sin(t * Mathf.PI * 2) * 2f;
            
            GameObject waypoint = CreateWaypoint(position);
            waypointObjects.Add(waypoint);
        }
        
        UpdatePath();
    }
    
    private void UpdatePath()
    {
        if (pathManager != null)
        {
            List<Transform> waypoints = new List<Transform>();
            foreach (GameObject waypoint in waypointObjects)
            {
                if (waypoint != null)
                {
                    waypoints.Add(waypoint.transform);
                }
            }
            
            pathManager.waypoints = waypoints;
        }
    }
    
    public void AddPlacementArea(Rect area)
    {
        placementAreas.Add(area);
        CreatePlacementAreaVisual(area);
    }
    
    private void CreatePlacementAreaVisual(Rect area)
    {
        GameObject areaObject;
        
        if (placementAreaPrefab != null)
        {
            areaObject = Instantiate(placementAreaPrefab, transform);
        }
        else
        {
            areaObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            areaObject.transform.SetParent(transform);
            
            Renderer renderer = areaObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0, 1, 0, 0.3f);
            }
        }
        
        areaObject.transform.position = new Vector3(area.center.x, area.center.y, 0);
        areaObject.transform.localScale = new Vector3(area.width, area.height, 0.1f);
        areaObject.name = $"PlacementArea_{placementAreaObjects.Count}";
        
        placementAreaObjects.Add(areaObject);
    }
    
    public void AddObstacle(Vector2 position)
    {
        obstaclePositions.Add(position);
        CreateObstacleVisual(position);
    }
    
    private void CreateObstacleVisual(Vector2 position)
    {
        GameObject obstacle;
        
        if (obstaclePrefab != null)
        {
            obstacle = Instantiate(obstaclePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity, transform);
        }
        else
        {
            obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacle.transform.SetParent(transform);
            obstacle.transform.position = new Vector3(position.x, position.y, 0);
            
            Renderer renderer = obstacle.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.red;
            }
        }
        
        obstacle.name = $"Obstacle_{obstacleObjects.Count}";
        obstacleObjects.Add(obstacle);
    }
    
    public void SaveLevel()
    {
        LevelData levelData = new LevelData
        {
            levelName = this.levelName,
            levelSize = this.levelSize,
            waypoints = new List<Vector3>(),
            placementAreas = new List<Rect>(this.placementAreas),
            obstaclePositions = new List<Vector2>(this.obstaclePositions)
        };
        
        foreach (GameObject waypoint in waypointObjects)
        {
            if (waypoint != null)
            {
                levelData.waypoints.Add(waypoint.transform.position);
            }
        }
        
        string json = JsonUtility.ToJson(levelData, true);
        string path = Application.dataPath + $"/Levels/{levelName}.json";
        
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
        System.IO.File.WriteAllText(path, json);
        
        Debug.Log($"Level saved to: {path}");
    }
    
    public void LoadLevel()
    {
        string path = Application.dataPath + $"/Levels/{levelName}.json";
        
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);
            
            // Clear existing level
            ClearWaypoints();
            ClearPlacementAreas();
            ClearObstacles();
            
            // Load waypoints
            foreach (Vector3 position in levelData.waypoints)
            {
                GameObject waypoint = CreateWaypoint(position);
                waypointObjects.Add(waypoint);
            }
            
            // Load placement areas
            placementAreas = levelData.placementAreas;
            foreach (Rect area in placementAreas)
            {
                CreatePlacementAreaVisual(area);
            }
            
            // Load obstacles
            obstaclePositions = levelData.obstaclePositions;
            foreach (Vector2 position in obstaclePositions)
            {
                CreateObstacleVisual(position);
            }
            
            UpdatePath();
            Debug.Log($"Level loaded from: {path}");
        }
        else
        {
            Debug.LogWarning($"Level file not found: {path}");
        }
    }
    
    private void ClearPlacementAreas()
    {
        foreach (GameObject area in placementAreaObjects)
        {
            if (area != null)
            {
                DestroyImmediate(area);
            }
        }
        placementAreaObjects.Clear();
        placementAreas.Clear();
    }
    
    private void ClearObstacles()
    {
        foreach (GameObject obstacle in obstacleObjects)
        {
            if (obstacle != null)
            {
                DestroyImmediate(obstacle);
            }
        }
        obstacleObjects.Clear();
        obstaclePositions.Clear();
    }
}

[System.Serializable]
public class LevelData
{
    public string levelName;
    public Vector2 levelSize;
    public List<Vector3> waypoints;
    public List<Rect> placementAreas;
    public List<Vector2> obstaclePositions;
}