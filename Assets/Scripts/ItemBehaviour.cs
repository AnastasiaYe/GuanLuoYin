using UnityEngine;
using System.Collections.Generic;

public class ItemBehaviour : MonoBehaviour
{
    [Header("Item Info")]
    public string itemName;
    public string itemDescription;
    
    [Header("Interaction")]
    public bool isInteractable = true;
    public float interactionCooldown = 0.5f; // Prevent rapid clicking
    
    [Header("Stage Management")]
    [Tooltip("Which stages (0-4) this item should be visible in. Leave empty to use ItemManager.")]
    public List<int> availableStages = new List<int>();
    
    [Tooltip("If true, this item will be managed by ItemManager automatically")]
    public bool useItemManager = true;
    
    [Header("Hover Effects")]
    [Tooltip("Enable glowing effect on hover (only for unclicked items)")]
    public bool enableHoverGlow = true;
    
    [Tooltip("Glow color when hovering")]
    public Color glowColor = Color.yellow;
    
    [Tooltip("Glow intensity multiplier")]
    public float glowIntensity = 2f;
    
    [Tooltip("Glow animation speed")]
    public float glowSpeed = 2f;
    
    private float lastInteractionTime = 0f;
    private bool hasBeenClicked = false;
    private bool isHovered = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float glowTimer = 0f;
    
    // Events
    public System.Action<ItemBehaviour> OnItemInteracted;

    public AudioClip interactionSound;
    private AudioSource audioSource;

    private void Start()
    {
        // Set item name to GameObject name if not set
        if (string.IsNullOrEmpty(itemName))
        {
            itemName = gameObject.name;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        
        // Initialize sprite renderer for glow effects
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        // Note: ItemInteraction will automatically find and register this item if enableStageBasedItems is true
        // No need to register here to avoid duplicate registration
    }
    
    private void Update()
    {
        // Handle glow animation when hovered
        if (isHovered && enableHoverGlow && !hasBeenClicked && spriteRenderer != null)
        {
            glowTimer += Time.deltaTime * glowSpeed;
            float glowAmount = (Mathf.Sin(glowTimer) + 1f) * 0.5f; // 0 to 1
            Color currentGlow = Color.Lerp(originalColor, glowColor * glowIntensity, glowAmount * 0.5f);
            spriteRenderer.color = currentGlow;
        }
    }
    

    public void OnInteract()
    {
        // Check if item is interactable
        if (!isInteractable) return;
        
        // Check if item should be interactable in current stage
        if (useItemManager && !IsInteractableInCurrentStage()) return;

        // Check cooldown
        if (Time.time - lastInteractionTime < interactionCooldown) return;

        // Update last interaction time
        lastInteractionTime = Time.time;

        // Handle interaction
        HandleInteraction();

        // Trigger event
        OnItemInteracted?.Invoke(this);

        if (interactionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(interactionSound);
        }
    }
    
    public bool IsInteractableInCurrentStage()
    {
        if (!useItemManager || StageManager.Instance == null) return true;
        
        int currentStage = StageManager.Instance.currentStage;
        return availableStages.Contains(currentStage);
    }
    
    private void HandleInteraction()
    {
        GetComponent<ClueGiver>()?.GrantClue();
    }
    
    /// <summary>
    /// Called when mouse enters this item
    /// </summary>
    public void OnMouseEnter()
    {
        if (hasBeenClicked || !enableHoverGlow) return;
        
        isHovered = true;
        glowTimer = 0f; // Reset glow animation
    }
    
    /// <summary>
    /// Called when mouse exits this item
    /// </summary>
    public void OnMouseExit()
    {
        isHovered = false;
        
        // Restore original color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
    
    /// <summary>
    /// Mark this item as clicked (disables hover effects)
    /// </summary>
    public void MarkAsClicked()
    {
        hasBeenClicked = true;
        isHovered = false;
        
        // Restore original color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
    
    /// <summary>
    /// Check if this item has been clicked
    /// </summary>
    /// <returns>True if clicked, false otherwise</returns>
    public bool HasBeenClicked()
    {
        return hasBeenClicked;
    }
    
    /// <summary>
    /// Reset the clicked state (useful for testing or resetting)
    /// </summary>
    public void ResetClickedState()
    {
        hasBeenClicked = false;
        isHovered = false;
        
        // Restore original color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
    
    // Public method to get item info
    public string GetItemInfo()
    {
        return $"Item: {itemName}\nDescription: {itemDescription}";
    }
    
    // Public method to set item info
    public void SetItemInfo(string name, string description)
    {
        itemName = name;
        itemDescription = description;
    }
    
    /// <summary>
    /// Set which stages this item should be available in
    /// </summary>
    /// <param name="stages">Array of stages (0-4)</param>
    public void SetAvailableStages(int[] stages)
    {
        availableStages.Clear();
        foreach (int stage in stages)
        {
            if (stage >= 0 && stage <= 4)
            {
                availableStages.Add(stage);
            }
        }
        
        // Update ItemInteraction if we're using it
        if (useItemManager && ItemInteraction.Instance != null)
        {
            ItemInteraction.Instance.RegisterAllItemsManually();
        }
    }
    
    /// <summary>
    /// Add a stage where this item should be available
    /// </summary>
    /// <param name="stage">Stage to add (0-4)</param>
    public void AddAvailableStage(int stage)
    {
        if (stage >= 0 && stage <= 4 && !availableStages.Contains(stage))
        {
            availableStages.Add(stage);
            
            // Update ItemInteraction if we're using it
            if (useItemManager && ItemInteraction.Instance != null)
            {
                ItemInteraction.Instance.RegisterAllItemsManually();
            }
        }
    }
    
    /// <summary>
    /// Remove a stage from available stages
    /// </summary>
    /// <param name="stage">Stage to remove (0-4)</param>
    public void RemoveAvailableStage(int stage)
    {
        availableStages.Remove(stage);
        
        // Update ItemInteraction if we're using it
        if (useItemManager && ItemInteraction.Instance != null)
        {
            ItemInteraction.Instance.RegisterAllItemsManually();
        }
    }
    
    /// <summary>
    /// Check if this item should be available in the current stage
    /// </summary>
    /// <returns>True if available in current stage</returns>
    public bool IsAvailableInCurrentStage()
    {
        if (StageManager.Instance == null) return true;
        return availableStages.Contains(StageManager.Instance.currentStage);
    }
    
    /// <summary>
    /// Manually register this item with the ItemInteraction
    /// </summary>
    public void RegisterWithItemManager()
    {
        if (useItemManager && ItemInteraction.Instance != null)
        {
            ItemInteraction.Instance.RegisterAllItemsManually();
        }
    }
    
    /// <summary>
    /// Debug method to test hover effects
    /// </summary>
    [ContextMenu("Test Hover Effect")]
    private void TestHoverEffect()
    {
        if (!hasBeenClicked)
        {
            OnMouseEnter();
            Debug.Log($"Testing hover effect on {itemName}");
        }
        else
        {
            Debug.Log($"{itemName} has already been clicked - hover effect disabled");
        }
    }
    
    /// <summary>
    /// Debug method to reset clicked state
    /// </summary>
    [ContextMenu("Reset Clicked State")]
    private void ResetClickedStateDebug()
    {
        ResetClickedState();
        Debug.Log($"Reset clicked state for {itemName}");
    }
}
