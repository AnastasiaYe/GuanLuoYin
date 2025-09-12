using UnityEngine;
using UnityEngine.InputSystem;

public class ItemInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public LayerMask interactableLayer = 8; // Layer 3 (2^3 = 8)
    
    [Header("Camera Settings")]
    public Camera playerCamera; // Assign your camera here
    
    private Mouse mouse;
    
    private void Start()
    {
        // Get mouse reference
        mouse = Mouse.current;
    }
    
    private void Update()
    {
        // Check for mouse click using new Input System
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            HandleMouseClick();
        }
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
                return;
            }
        }
    }
    
    
    // Method to manually set the camera
    public void SetCamera(Camera camera)
    {
        playerCamera = camera;
    }
    
}
