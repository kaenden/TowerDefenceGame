using UnityEngine;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
    [Header("Pathfinding")]
    public List<Transform> waypoints;
    public float moveSpeed = 2f;
    public float waypointThreshold = 0.1f;
    
    private int currentWaypointIndex = 0;
    private Enemy enemy;
    
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        
        // If no waypoints assigned, find them from PathManager
        if (waypoints.Count == 0)
        {
            PathManager pathManager = FindObjectOfType<PathManager>();
            if (pathManager != null)
            {
                waypoints = pathManager.GetWaypoints();
            }
        }
    }
    
    private void Update()
    {
        if (waypoints.Count == 0 || enemy.IsDead) return;
        
        MoveTowardsWaypoint();
    }
    
    private void MoveTowardsWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Count)
        {
            // Reached the end
            enemy.ReachEnd();
            return;
        }
        
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // Rotate to face movement direction
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
        // Check if reached current waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < waypointThreshold)
        {
            currentWaypointIndex++;
        }
    }
    
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
    
    public void SetWaypoints(List<Transform> newWaypoints)
    {
        waypoints = newWaypoints;
        currentWaypointIndex = 0;
    }
    
    public Vector3 GetNextWaypointPosition()
    {
        if (currentWaypointIndex < waypoints.Count)
        {
            return waypoints[currentWaypointIndex].position;
        }
        return transform.position;
    }
    
    public float GetDistanceToEnd()
    {
        if (waypoints.Count == 0) return 0f;
        
        float distance = 0f;
        
        // Distance to current waypoint
        if (currentWaypointIndex < waypoints.Count)
        {
            distance += Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);
        }
        
        // Distance between remaining waypoints
        for (int i = currentWaypointIndex; i < waypoints.Count - 1; i++)
        {
            distance += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
        }
        
        return distance;
    }
}