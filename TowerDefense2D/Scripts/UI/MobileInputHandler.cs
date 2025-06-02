using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInputHandler : MonoBehaviour
{
    [Header("Touch Settings")]
    public float touchSensitivity = 1f;
    public float doubleTapTime = 0.3f;
    public float longPressTime = 0.8f;
    public float dragThreshold = 50f;
    
    [Header("Camera Control")]
    public bool enableCameraControl = true;
    public float cameraSpeed = 2f;
    public float zoomSpeed = 1f;
    public float minZoom = 3f;
    public float maxZoom = 8f;
    
    private Camera mainCamera;
    private TowerPlacement towerPlacement;
    private Vector2 lastTouchPosition;
    private float lastTapTime;
    private bool isDragging;
    private bool isLongPress;
    private float touchStartTime;
    private Vector2 touchStartPosition;
    
    public delegate void TouchEvent(Vector2 position);
    public static event TouchEvent OnTap;
    public static event TouchEvent OnDoubleTap;
    public static event TouchEvent OnLongPress;
    
    private void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        
        towerPlacement = FindObjectOfType<TowerPlacement>();
    }
    
    private void Update()
    {
        HandleInput();
    }
    
    private void HandleInput()
    {
        // Handle mouse input for testing in editor
        if (Application.isEditor)
        {
            HandleMouseInput();
        }
        
        // Handle touch input for mobile
        HandleTouchInput();
    }
    
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            if (!IsPointerOverUI(mousePos))
            {
                HandleTouchStart(mousePos);
            }
        }
        
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Input.mousePosition;
            HandleTouchMove(mousePos);
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 mousePos = Input.mousePosition;
            HandleTouchEnd(mousePos);
        }
        
        // Handle scroll wheel for zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f && enableCameraControl)
        {
            ZoomCamera(-scroll * zoomSpeed);
        }
    }
    
    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (!IsPointerOverUI(touch.position))
                    {
                        HandleTouchStart(touch.position);
                    }
                    break;
                    
                case TouchPhase.Moved:
                    HandleTouchMove(touch.position);
                    break;
                    
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    HandleTouchEnd(touch.position);
                    break;
            }
        }
        else if (Input.touchCount == 2 && enableCameraControl)
        {
            HandlePinchZoom();
        }
    }
    
    private void HandleTouchStart(Vector2 screenPosition)
    {
        touchStartPosition = screenPosition;
        touchStartTime = Time.time;
        lastTouchPosition = screenPosition;
        isDragging = false;
        isLongPress = false;
    }
    
    private void HandleTouchMove(Vector2 screenPosition)
    {
        Vector2 deltaPosition = screenPosition - lastTouchPosition;
        float distance = Vector2.Distance(screenPosition, touchStartPosition);
        
        if (distance > dragThreshold && !isDragging)
        {
            isDragging = true;
        }
        
        if (isDragging && enableCameraControl && !towerPlacement.IsPlacingTower())
        {
            // Move camera
            Vector3 worldDelta = mainCamera.ScreenToWorldPoint(new Vector3(deltaPosition.x, deltaPosition.y, mainCamera.nearClipPlane));
            worldDelta = worldDelta - mainCamera.ScreenToWorldPoint(Vector3.zero);
            
            mainCamera.transform.position -= worldDelta * cameraSpeed;
        }
        
        lastTouchPosition = screenPosition;
    }
    
    private void HandleTouchEnd(Vector2 screenPosition)
    {
        float touchDuration = Time.time - touchStartTime;
        float distance = Vector2.Distance(screenPosition, touchStartPosition);
        
        if (!isDragging && distance < dragThreshold)
        {
            if (touchDuration >= longPressTime)
            {
                // Long press
                isLongPress = true;
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, mainCamera.nearClipPlane));
                OnLongPress?.Invoke(worldPosition);
                HandleLongPress(worldPosition);
            }
            else
            {
                // Tap or double tap
                float timeSinceLastTap = Time.time - lastTapTime;
                
                if (timeSinceLastTap <= doubleTapTime)
                {
                    // Double tap
                    Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, mainCamera.nearClipPlane));
                    OnDoubleTap?.Invoke(worldPosition);
                    HandleDoubleTap(worldPosition);
                }
                else
                {
                    // Single tap
                    Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, mainCamera.nearClipPlane));
                    OnTap?.Invoke(worldPosition);
                    HandleTap(worldPosition);
                }
                
                lastTapTime = Time.time;
            }
        }
        
        isDragging = false;
        isLongPress = false;
    }
    
    private void HandlePinchZoom()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);
        
        Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
        Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;
        
        float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
        float touchDeltaMag = (touch1.position - touch2.position).magnitude;
        
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
        ZoomCamera(deltaMagnitudeDiff * zoomSpeed * 0.01f);
    }
    
    private void ZoomCamera(float zoomAmount)
    {
        if (mainCamera.orthographic)
        {
            mainCamera.orthographicSize += zoomAmount;
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
        }
        else
        {
            Vector3 position = mainCamera.transform.position;
            position.z += zoomAmount;
            position.z = Mathf.Clamp(position.z, -maxZoom, -minZoom);
            mainCamera.transform.position = position;
        }
    }
    
    private void HandleTap(Vector3 worldPosition)
    {
        // Handle tower placement or selection
        if (towerPlacement.IsPlacingTower())
        {
            // This will be handled by TowerPlacement script
        }
        else
        {
            // Try to select a tower
            Collider2D hit = Physics2D.OverlapPoint(worldPosition);
            if (hit != null)
            {
                Tower tower = hit.GetComponent<Tower>();
                if (tower != null)
                {
                    // Tower selection is handled by TowerPlacement script
                }
            }
        }
    }
    
    private void HandleDoubleTap(Vector3 worldPosition)
    {
        // Double tap to quickly upgrade selected tower
        Tower selectedTower = towerPlacement.GetSelectedTower();
        if (selectedTower != null)
        {
            selectedTower.Upgrade();
        }
    }
    
    private void HandleLongPress(Vector3 worldPosition)
    {
        // Long press to show tower info or context menu
        Collider2D hit = Physics2D.OverlapPoint(worldPosition);
        if (hit != null)
        {
            Tower tower = hit.GetComponent<Tower>();
            if (tower != null)
            {
                // Show tower context menu or detailed info
                ShowTowerContextMenu(tower);
            }
        }
    }
    
    private void ShowTowerContextMenu(Tower tower)
    {
        // This could show a context menu with upgrade, sell, and targeting options
        // For now, just select the tower
        // The UI system will handle showing the tower info panel
    }
    
    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
    
    public void SetCameraControlEnabled(bool enabled)
    {
        enableCameraControl = enabled;
    }
    
    public void FocusOnPosition(Vector3 worldPosition, float duration = 1f)
    {
        StartCoroutine(SmoothMoveCamera(worldPosition, duration));
    }
    
    private System.Collections.IEnumerator SmoothMoveCamera(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = mainCamera.transform.position;
        targetPosition.z = startPosition.z; // Maintain camera's Z position
        
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = Mathf.SmoothStep(0f, 1f, t); // Smooth interpolation
            
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        
        mainCamera.transform.position = targetPosition;
    }
}