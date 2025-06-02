using UnityEngine;
using System.Collections.Generic;

public class TowerPlacement : MonoBehaviour
{
    [Header("Placement Settings")]
    public LayerMask placementLayer = 1;
    public LayerMask obstacleLayer = 1;
    public Material validPlacementMaterial;
    public Material invalidPlacementMaterial;
    
    [Header("Grid Settings")]
    public bool useGrid = true;
    public float gridSize = 1f;
    public bool showGrid = true;
    
    private Camera mainCamera;
    private GameObject selectedTowerPrefab;
    private GameObject previewTower;
    private Tower selectedTower;
    private bool isPlacingTower = false;
    private List<Vector3> occupiedPositions = new List<Vector3>();
    
    public delegate void TowerPlacementEvent(Tower tower);
    public static event TowerPlacementEvent OnTowerPlaced;
    public static event TowerPlacementEvent OnTowerSelected;
    public static event TowerPlacementEvent OnTowerDeselected;
    
    private void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
    }
    
    private void Update()
    {
        HandleInput();
        
        if (isPlacingTower)
        {
            UpdatePreviewTower();
        }
    }
    
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
    }
    
    private void HandleMouseClick()
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        
        if (isPlacingTower)
        {
            TryPlaceTower(mousePosition);
        }
        else
        {
            TrySelectTower(mousePosition);
        }
    }
    
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.nearClipPlane;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
    
    private Vector3 SnapToGrid(Vector3 position)
    {
        if (!useGrid) return position;
        
        float snappedX = Mathf.Round(position.x / gridSize) * gridSize;
        float snappedY = Mathf.Round(position.y / gridSize) * gridSize;
        
        return new Vector3(snappedX, snappedY, position.z);
    }
    
    public void StartPlacingTower(GameObject towerPrefab)
    {
        if (towerPrefab == null) return;
        
        selectedTowerPrefab = towerPrefab;
        isPlacingTower = true;
        
        DeselectTower();
        CreatePreviewTower();
    }
    
    private void CreatePreviewTower()
    {
        if (selectedTowerPrefab != null)
        {
            previewTower = Instantiate(selectedTowerPrefab);
            
            // Disable components for preview
            Tower towerComponent = previewTower.GetComponent<Tower>();
            if (towerComponent != null)
            {
                towerComponent.enabled = false;
            }
            
            Collider2D collider = previewTower.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            
            // Make it semi-transparent
            SpriteRenderer[] renderers = previewTower.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer renderer in renderers)
            {
                Color color = renderer.color;
                color.a = 0.7f;
                renderer.color = color;
            }
        }
    }
    
    private void UpdatePreviewTower()
    {
        if (previewTower == null) return;
        
        Vector3 mousePosition = GetMouseWorldPosition();
        Vector3 snapPosition = SnapToGrid(mousePosition);
        
        previewTower.transform.position = snapPosition;
        
        // Update preview color based on validity
        bool canPlace = CanPlaceTowerAt(snapPosition);
        UpdatePreviewColor(canPlace);
    }
    
    private void UpdatePreviewColor(bool canPlace)
    {
        SpriteRenderer[] renderers = previewTower.GetComponentsInChildren<SpriteRenderer>();
        Color targetColor = canPlace ? Color.green : Color.red;
        targetColor.a = 0.7f;
        
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.color = targetColor;
        }
    }
    
    private bool CanPlaceTowerAt(Vector3 position)
    {
        // Check if position is occupied
        foreach (Vector3 occupiedPos in occupiedPositions)
        {
            if (Vector3.Distance(position, occupiedPos) < gridSize * 0.5f)
            {
                return false;
            }
        }
        
        // Check for obstacles
        Collider2D obstacle = Physics2D.OverlapCircle(position, gridSize * 0.4f, obstacleLayer);
        if (obstacle != null)
        {
            return false;
        }
        
        // Check if on valid placement area
        Collider2D placementArea = Physics2D.OverlapCircle(position, gridSize * 0.4f, placementLayer);
        
        return placementArea != null;
    }
    
    private void TryPlaceTower(Vector3 position)
    {
        Vector3 snapPosition = SnapToGrid(position);
        
        if (!CanPlaceTowerAt(snapPosition))
        {
            return;
        }
        
        Tower towerComponent = selectedTowerPrefab.GetComponent<Tower>();
        if (towerComponent == null)
        {
            return;
        }
        
        // Check if player has enough money
        if (!GameManager.Instance.SpendMoney(towerComponent.cost))
        {
            return;
        }
        
        // Place the tower
        GameObject newTower = Instantiate(selectedTowerPrefab, snapPosition, Quaternion.identity);
        Tower newTowerComponent = newTower.GetComponent<Tower>();
        
        // Add to occupied positions
        occupiedPositions.Add(snapPosition);
        
        // Notify listeners
        OnTowerPlaced?.Invoke(newTowerComponent);
        
        // Continue placing or stop
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            CancelPlacement();
        }
    }
    
    private void TrySelectTower(Vector3 position)
    {
        Collider2D hit = Physics2D.OverlapPoint(position);
        
        if (hit != null)
        {
            Tower tower = hit.GetComponent<Tower>();
            if (tower != null)
            {
                SelectTower(tower);
                return;
            }
        }
        
        // Clicked on empty space, deselect current tower
        DeselectTower();
    }
    
    private void SelectTower(Tower tower)
    {
        if (selectedTower == tower) return;
        
        DeselectTower();
        
        selectedTower = tower;
        selectedTower.Select();
        
        OnTowerSelected?.Invoke(selectedTower);
    }
    
    private void DeselectTower()
    {
        if (selectedTower != null)
        {
            selectedTower.Deselect();
            OnTowerDeselected?.Invoke(selectedTower);
            selectedTower = null;
        }
    }
    
    public void CancelPlacement()
    {
        isPlacingTower = false;
        selectedTowerPrefab = null;
        
        if (previewTower != null)
        {
            Destroy(previewTower);
            previewTower = null;
        }
    }
    
    public void RemoveTowerAt(Vector3 position)
    {
        occupiedPositions.Remove(position);
    }
    
    public Tower GetSelectedTower()
    {
        return selectedTower;
    }
    
    public bool IsPlacingTower()
    {
        return isPlacingTower;
    }
    
    private void OnDrawGizmos()
    {
        if (!showGrid || !useGrid) return;
        
        Gizmos.color = Color.white;
        
        // Draw grid
        Vector3 cameraPos = mainCamera != null ? mainCamera.transform.position : Vector3.zero;
        int gridRange = 20;
        
        for (int x = -gridRange; x <= gridRange; x++)
        {
            Vector3 start = new Vector3(x * gridSize, -gridRange * gridSize, 0);
            Vector3 end = new Vector3(x * gridSize, gridRange * gridSize, 0);
            Gizmos.DrawLine(start, end);
        }
        
        for (int y = -gridRange; y <= gridRange; y++)
        {
            Vector3 start = new Vector3(-gridRange * gridSize, y * gridSize, 0);
            Vector3 end = new Vector3(gridRange * gridSize, y * gridSize, 0);
            Gizmos.DrawLine(start, end);
        }
    }
}