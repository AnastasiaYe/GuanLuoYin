using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class ItemInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public LayerMask interactableLayer = 8; // Layer 3 (2^3 = 8)
    
    [Header("Camera Settings")]
    public Camera playerCamera; // Assign your camera here
    
    [Header("Stage-Based Item Management")]
    public bool enableStageBasedItems = true;
    public bool showDebugInfo = false;
    
    private Mouse mouse;
    private List<ItemBehaviour> managedItems = new List<ItemBehaviour>();
    
    // Hover tracking
    private ItemBehaviour currentlyHoveredItem = null;
    private ItemBehaviour lastHoveredItem = null;
    
    // Singleton pattern for easy access
    private static ItemInteraction _instance;
    public static ItemInteraction Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ItemInteraction>();
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Get mouse reference
        mouse = Mouse.current;
        
        // Subscribe to stage changes if stage-based items are enabled
        if (enableStageBasedItems && StageManager.Instance != null)
        {
            StageManager.Instance.OnStageChanged += OnStageChanged;
            RegisterAllItems();
            UpdateItemsForCurrentStage();
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from stage changes
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnStageChanged -= OnStageChanged;
        }
    }
    
    private void Update()
    {
        // Check for mouse click using new Input System
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            HandleMouseClick();
        }
        
        // Handle mouse hover
        HandleMouseHover();
    }
    
    private void HandleMouseClick()
    {
        if (playerCamera == null || mouse == null) return;
        
        // Get mouse position and convert to world position
        Vector2 mousePos = mouse.position.ReadValue();
        Vector3 worldPos = playerCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, playerCamera.nearClipPlane));
        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);
        
        // Find all colliders at the mouse position
        Collider2D[] colliders = Physics2D.OverlapPointAll(worldPos2D, interactableLayer);
        
        foreach (Collider2D collider in colliders)
        {
            var itemBehaviour = collider?.GetComponent<ItemBehaviour>();
            if (itemBehaviour != null)
            {
                itemBehaviour.OnInteract();
                
                // Mark this item as clicked (disable hover effect)
                itemBehaviour.MarkAsClicked();
                return;
            }
        }
    }
    
    private void HandleMouseHover()
    {
        if (playerCamera == null || mouse == null) return;
        
        // Get mouse position and convert to world position
        Vector2 mousePos = mouse.position.ReadValue();
        Vector3 worldPos = playerCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, playerCamera.nearClipPlane));
        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);
        
        // Find all colliders at the mouse position
        Collider2D[] colliders = Physics2D.OverlapPointAll(worldPos2D, interactableLayer);
        
        // Find the first valid item under the mouse
        ItemBehaviour hoveredItem = null;
        foreach (Collider2D collider in colliders)
        {
            var itemBehaviour = collider?.GetComponent<ItemBehaviour>();
            if (itemBehaviour != null && itemBehaviour.IsInteractableInCurrentStage())
            {
                hoveredItem = itemBehaviour;
                break;
            }
        }
        
        // Update hover state
        if (hoveredItem != currentlyHoveredItem)
        {
            // Stop hovering previous item
            if (currentlyHoveredItem != null)
            {
                currentlyHoveredItem.OnMouseExit();
            }
            
            // Start hovering new item
            currentlyHoveredItem = hoveredItem;
            if (currentlyHoveredItem != null)
            {
                currentlyHoveredItem.OnMouseEnter();
            }
        }
    }
    
    
    // Method to manually set the camera
    public void SetCamera(Camera camera)
    {
        playerCamera = camera;
    }
    
    #region Stage-Based Item Management
    
    private void RegisterAllItems()
    {
        // Find all ItemBehaviour components in the scene
        ItemBehaviour[] allItems = FindObjectsOfType<ItemBehaviour>();
        
        if (showDebugInfo)
        {
            Debug.Log($"ItemInteraction: Found {allItems.Length} ItemBehaviour components in scene");
        }
        
        managedItems.Clear();
        
        foreach (var item in allItems)
        {
            if (item.useItemManager && item.availableStages.Count > 0)
            {
                managedItems.Add(item);
                
                if (showDebugInfo)
                {
                    Debug.Log($"ItemInteraction: Registered item {item.name} for stages [{string.Join(", ", item.availableStages)}]");
                }
            }
        }
    }
    
    private void OnStageChanged(int newStage)
    {
        if (showDebugInfo)
        {
            Debug.Log($"ItemInteraction: Stage changed to {newStage}, updating item visibility");
        }
        
        UpdateItemsForCurrentStage();
    }
    
    private void UpdateItemsForCurrentStage()
    {
        if (StageManager.Instance == null) return;
        
        int currentStage = StageManager.Instance.currentStage;
        
        foreach (var item in managedItems)
        {
            UpdateItemVisibility(item, currentStage);
        }
    }
    
    private void UpdateItemVisibility(ItemBehaviour item, int stage)
    {
        if (item == null || item.gameObject == null) return;
        
        bool shouldBeVisible = item.availableStages.Contains(stage);
        bool wasActive = item.gameObject.activeInHierarchy;
        
        item.gameObject.SetActive(shouldBeVisible);
        
        if (showDebugInfo)
        {
            Debug.Log($"ItemInteraction: Item {item.name}: Available stages [{string.Join(", ", item.availableStages)}], Current stage: {stage}, Should be visible: {shouldBeVisible}, Was active: {wasActive}, Now active: {item.gameObject.activeInHierarchy}");
        }
    }
    
    #endregion
    
    #region Public Methods for Manual Control
    
    [ContextMenu("Register All Items")]
    public void RegisterAllItemsManually()
    {
        RegisterAllItems();
        UpdateItemsForCurrentStage();
    }
    
    [ContextMenu("Debug Item States")]
    public void DebugItemStates()
    {
        Debug.Log("=== ItemInteraction Item States ===");
        Debug.Log($"Managed items count: {managedItems.Count}");
        
        if (StageManager.Instance != null)
        {
            Debug.Log($"Current stage: {StageManager.Instance.currentStage}");
        }
        
        foreach (var item in managedItems)
        {
            string stages = string.Join(", ", item.availableStages);
            Debug.Log($"Item: {item.name}, Stages [{stages}], Active: {item.gameObject.activeInHierarchy}");
        }
    }
    
    [ContextMenu("Update Items for Current Stage")]
    public void ForceUpdateItemsForCurrentStage()
    {
        UpdateItemsForCurrentStage();
    }
    
    #endregion
    
}
