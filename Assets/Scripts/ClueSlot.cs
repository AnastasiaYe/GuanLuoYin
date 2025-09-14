using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ClueSlot : MonoBehaviour, IDropHandler
{
    [Header("Slot Settings")]
    [Tooltip("Expected clue ID or title (system will try both)")]
    public string expectedClueId;
    [Tooltip("Expected category for this slot (used for color coding)")]
    public ClueCategory expectedCategory = ClueCategory.Object;
    public Transform snapPoint; // Optional snap point for the clue token
    public bool isFilled = false;
    
    [Header("Visual Feedback")]
    public Image slotImage;
    public Color emptyColor = Color.white;
    public Color filledColor = Color.green;
    
    [Header("Category Color Settings")]
    [Tooltip("Use category-based colors instead of default colors")]
    public bool useCategoryColors = true;
    [Tooltip("Color intensity for empty slots (0 = transparent, 1 = opaque)")]
    [Range(0f, 1f)]
    public float emptyColorIntensity = 0.3f;
    [Tooltip("Color intensity for filled slots (0 = transparent, 1 = opaque)")]
    [Range(0f, 1f)]
    public float filledColorIntensity = 0.8f;
    
    [Header("Custom Category Colors")]
    [Tooltip("Enable custom colors (overrides default category colors)")]
    public bool useCustomColors = false;
    [Tooltip("Custom color for Name category")]
    public Color customNameColor = new Color(0.3f, 0.3f, 1f, 1f);
    [Tooltip("Custom color for Location category")]
    public Color customLocationColor = new Color(0.3f, 1f, 0.3f, 1f);
    [Tooltip("Custom color for Object category")]
    public Color customObjectColor = new Color(1f, 1f, 0.3f, 1f);
    [Tooltip("Custom color for Action category")]
    public Color customActionColor = new Color(1f, 0.3f, 1f, 1f);
    
    [Header("Title Display")]
    public TextMeshProUGUI titleText;
    public bool showTitleWhenFilled = true;

    public AudioClip interactionSound;
    private AudioSource audioSource;

    private ClueToken currentToken;
    private ClueManager cachedClueManager;
    
    /// <summary>
    /// Get the color for a specific category (custom or default)
    /// </summary>
    /// <param name="category">The category to get color for</param>
    /// <returns>Color for the category</returns>
    private Color GetCategoryColor(ClueCategory category)
    {
        if (useCustomColors)
        {
            switch (category)
            {
                case ClueCategory.Name:
                    return customNameColor;
                case ClueCategory.Location:
                    return customLocationColor;
                case ClueCategory.Object:
                    return customObjectColor;
                case ClueCategory.Action:
                    return customActionColor;
                default:
                    return customObjectColor; // Fallback to Object color
            }
        }
        else
        {
            return ClueCategoryColors.GetBackgroundColor(category);
        }
    }
    
    /// <summary>
    /// Get cached ClueManager reference
    /// </summary>
    /// <returns>Cached ClueManager instance</returns>
    private ClueManager GetClueManager()
    {
        if (cachedClueManager == null)
        {
            cachedClueManager = FindObjectOfType<ClueManager>();
        }
        return cachedClueManager;
    }
    
    private void Start()
    {
        snapPoint ??= transform;
        slotImage ??= GetComponent<Image>();
        
        // Auto-find title text if not assigned
        if (titleText == null)
        {
            titleText = GetComponentInChildren<TextMeshProUGUI>();
        }
        
        UpdateVisualState();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (isFilled) return;
        
        var draggedToken = eventData.pointerDrag?.GetComponent<ClueToken>();
        if (draggedToken == null) return;

        // Check if the dropped clue matches this slot (by ID or title)
        if (IsCorrectClue(draggedToken))
        {
            SnapToken(draggedToken);
            if (interactionSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(interactionSound);
            }
        }
        else
        {
            draggedToken.AddBackToScrollContent();
            draggedToken.SetDraggable(true);
        }
    }
    
    /// <summary>
    /// Check if the given token matches this slot's expected clue
    /// </summary>
    /// <param name="token">The clue token to check</param>
    /// <returns>True if the token matches this slot</returns>
    private bool IsCorrectClue(ClueToken token)
    {
        if (string.IsNullOrEmpty(expectedClueId)) return false;
        
        // Check by ID first (exact match)
        if (token.clueId == expectedClueId)
        {
            return true;
        }
        
        // Check by title (exact match)
        if (token.clueTitle == expectedClueId)
        {
            return true;
        }
        
        // Check by effective ID (handles auto-generated IDs)
        ClueManager clueManager = FindObjectOfType<ClueManager>();
        if (clueManager != null)
        {
            ClueData expectedClue = clueManager.GetClueByTitle(expectedClueId);
            if (expectedClue != null && token.clueId == expectedClue.GetEffectiveId())
            {
                return true;
            }
        }
        
        return false;
    }
    
    private void SnapToken(ClueToken token)
    {
        isFilled = true;
        currentToken = token;
        
        // Clean up any temporary drag canvas before snapping
        Canvas dragCanvas = token.GetComponent<Canvas>();
        if (dragCanvas != null && dragCanvas.overrideSorting)
        {
            DestroyImmediate(dragCanvas);
        }
        
        // Move token to slot and make it permanent
        token.transform.SetParent(snapPoint, false);
        token.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        token.transform.localScale = Vector3.one;
        token.SetDraggable(false);
        
        // Ensure the token stays visible in the slot
        CanvasGroup tokenCanvasGroup = token.GetComponent<CanvasGroup>();
        if (tokenCanvasGroup != null)
        {
            tokenCanvasGroup.alpha = 1f;
            tokenCanvasGroup.blocksRaycasts = false; // Don't block raycasts since it's not draggable
        }
        
        // Remove from ClueManager's earned clues list since it's now permanently placed
        ClueManager clueManager = FindObjectOfType<ClueManager>();
        if (clueManager != null)
        {
            clueManager.RemoveClueToken(token.clueId);
        }
        
        UpdateVisualState();
        
        // Notify GameCompletionManager that a slot was updated
        if (GameCompletionManager.Instance != null)
        {
            GameCompletionManager.Instance.OnSlotUpdated();
        }
    }
    
    public void RemoveToken()
    {
        if (currentToken != null)
        {
            // Clean up any temporary drag canvas before moving
            Canvas dragCanvas = currentToken.GetComponent<Canvas>();
            if (dragCanvas != null && dragCanvas.overrideSorting)
            {
                DestroyImmediate(dragCanvas);
            }
            
            // Re-enable dragging and restore raycast blocking
            CanvasGroup tokenCanvasGroup = currentToken.GetComponent<CanvasGroup>();
            if (tokenCanvasGroup != null)
            {
                tokenCanvasGroup.blocksRaycasts = true;
            }
            
            currentToken.SetDraggable(true);
            currentToken.AddBackToScrollContent();
            
            // Re-add to ClueManager's earned clues list
            ClueManager clueManager = FindObjectOfType<ClueManager>();
            if (clueManager != null)
            {
                // Re-create the token in the scroll area
                clueManager.GrantClue(currentToken.clueId, false);
            }
            
            currentToken = null;
        }
        
        isFilled = false;
        UpdateVisualState();
        
        // Notify GameCompletionManager that a slot was updated
        if (GameCompletionManager.Instance != null)
        {
            GameCompletionManager.Instance.OnSlotUpdated();
        }
    }
    
    private void UpdateVisualState()
    {
        UpdateSlotColor();
        UpdateTitleDisplay();
    }
    
    /// <summary>
    /// Update slot color based on current state and settings
    /// </summary>
    private void UpdateSlotColor()
    {
        if (slotImage == null) return;
        
        if (useCategoryColors)
        {
            Color categoryColor = GetCategoryColor(expectedCategory);
            
            if (isFilled && currentToken != null)
            {
                // Use the actual clue's category color when filled
                ClueManager clueManager = GetClueManager();
                if (clueManager != null)
                {
                    ClueData clueData = clueManager.GetClue(currentToken.clueId);
                    if (clueData != null)
                    {
                        categoryColor = GetCategoryColor(clueData.category);
                    }
                }
            }
            
            float intensity = isFilled ? filledColorIntensity : emptyColorIntensity;
            slotImage.color = new Color(categoryColor.r, categoryColor.g, categoryColor.b, intensity);
        }
        else
        {
            // Use default colors
            slotImage.color = isFilled ? filledColor : emptyColor;
        }
    }
    
    /// <summary>
    /// Update title display based on current state
    /// </summary>
    private void UpdateTitleDisplay()
    {
        if (titleText == null) return;
        
        bool shouldShowTitle = isFilled && showTitleWhenFilled && currentToken != null;
        
        if (shouldShowTitle)
        {
            titleText.text = currentToken.clueTitle;
        }
        
        titleText.gameObject.SetActive(shouldShowTitle);
    }
    
    public bool IsFilled()
    {
        return isFilled;
    }
    
    public string GetExpectedClueId()
    {
        return expectedClueId;
    }
    
    /// <summary>
    /// Get the current token in this slot
    /// </summary>
    /// <returns>ClueToken if slot is filled, null otherwise</returns>
    public ClueToken GetCurrentToken()
    {
        return currentToken;
    }
    
    /// <summary>
    /// Check if this slot has the correct clue placed
    /// </summary>
    /// <returns>True if filled with correct clue, false otherwise</returns>
    public bool HasCorrectClue()
    {
        return isFilled && currentToken != null && IsCorrectClue(currentToken);
    }
    
    /// <summary>
    /// Get the current clue title displayed in this slot
    /// </summary>
    /// <returns>Clue title if filled, empty string otherwise</returns>
    public string GetDisplayedClueTitle()
    {
        return (isFilled && currentToken != null) ? currentToken.clueTitle : string.Empty;
    }
    
    /// <summary>
    /// Set whether to show the clue title when filled
    /// </summary>
    /// <param name="show">Whether to show the title</param>
    public void SetShowTitle(bool show)
    {
        showTitleWhenFilled = show;
        UpdateVisualState();
    }
    
    /// <summary>
    /// Set the expected category for this slot
    /// </summary>
    /// <param name="category">The category to expect</param>
    public void SetExpectedCategory(ClueCategory category)
    {
        expectedCategory = category;
        UpdateVisualState();
    }
    
    /// <summary>
    /// Enable or disable category-based colors
    /// </summary>
    /// <param name="useCategory">Whether to use category colors</param>
    public void SetUseCategoryColors(bool useCategory)
    {
        useCategoryColors = useCategory;
        UpdateVisualState();
    }
    
    /// <summary>
    /// Enable or disable custom colors
    /// </summary>
    /// <param name="useCustom">Whether to use custom colors</param>
    public void SetUseCustomColors(bool useCustom)
    {
        useCustomColors = useCustom;
        UpdateVisualState();
    }
    
    /// <summary>
    /// Set a custom color for a specific category
    /// </summary>
    /// <param name="category">The category to set color for</param>
    /// <param name="color">The color to use</param>
    public void SetCustomColor(ClueCategory category, Color color)
    {
        switch (category)
        {
            case ClueCategory.Name:
                customNameColor = color;
                break;
            case ClueCategory.Location:
                customLocationColor = color;
                break;
            case ClueCategory.Object:
                customObjectColor = color;
                break;
            case ClueCategory.Action:
                customActionColor = color;
                break;
        }
        UpdateVisualState();
    }
    
    /// <summary>
    /// Reset custom colors to default category colors
    /// </summary>
    [ContextMenu("Reset to Default Colors")]
    public void ResetToDefaultColors()
    {
        customNameColor = ClueCategoryColors.GetBackgroundColor(ClueCategory.Name);
        customLocationColor = ClueCategoryColors.GetBackgroundColor(ClueCategory.Location);
        customObjectColor = ClueCategoryColors.GetBackgroundColor(ClueCategory.Object);
        customActionColor = ClueCategoryColors.GetBackgroundColor(ClueCategory.Action);
        useCustomColors = false;
        UpdateVisualState();
        Debug.Log($"ClueSlot '{name}': Reset to default category colors");
    }
    
    /// <summary>
    /// Apply a bright/vibrant color scheme
    /// </summary>
    [ContextMenu("Apply Bright Color Scheme")]
    public void ApplyBrightColorScheme()
    {
        customNameColor = new Color(0f, 0.5f, 1f, 1f);      // Bright Blue
        customLocationColor = new Color(0f, 1f, 0f, 1f);   // Bright Green
        customObjectColor = new Color(1f, 1f, 0f, 1f);     // Bright Yellow
        customActionColor = new Color(1f, 0f, 1f, 1f);     // Bright Magenta
        useCustomColors = true;
        UpdateVisualState();
        Debug.Log($"ClueSlot '{name}': Applied bright color scheme");
    }
    
    /// <summary>
    /// Apply a pastel/soft color scheme
    /// </summary>
    [ContextMenu("Apply Pastel Color Scheme")]
    public void ApplyPastelColorScheme()
    {
        customNameColor = new Color(0.7f, 0.7f, 1f, 1f);     // Pastel Blue
        customLocationColor = new Color(0.7f, 1f, 0.7f, 1f); // Pastel Green
        customObjectColor = new Color(1f, 1f, 0.7f, 1f);     // Pastel Yellow
        customActionColor = new Color(1f, 0.7f, 1f, 1f);     // Pastel Magenta
        useCustomColors = true;
        UpdateVisualState();
        Debug.Log($"ClueSlot '{name}': Applied pastel color scheme");
    }
    
    /// <summary>
    /// Apply a dark color scheme
    /// </summary>
    [ContextMenu("Apply Dark Color Scheme")]
    public void ApplyDarkColorScheme()
    {
        customNameColor = new Color(0.2f, 0.2f, 0.6f, 1f);   // Dark Blue
        customLocationColor = new Color(0.2f, 0.6f, 0.2f, 1f); // Dark Green
        customObjectColor = new Color(0.6f, 0.6f, 0.2f, 1f); // Dark Yellow
        customActionColor = new Color(0.6f, 0.2f, 0.6f, 1f); // Dark Magenta
        useCustomColors = true;
        UpdateVisualState();
        Debug.Log($"ClueSlot '{name}': Applied dark color scheme");
    }
    
    /// <summary>
    /// Get the current category color for this slot
    /// </summary>
    /// <returns>Color based on current state and settings</returns>
    public Color GetCurrentSlotColor()
    {
        if (!useCategoryColors)
        {
            return isFilled ? filledColor : emptyColor;
        }
        
        Color categoryColor = GetCategoryColor(expectedCategory);
        
        if (isFilled && currentToken != null)
        {
            // Use the actual clue's category color when filled
            ClueManager clueManager = GetClueManager();
            if (clueManager != null)
            {
                ClueData clueData = clueManager.GetClue(currentToken.clueId);
                if (clueData != null)
                {
                    categoryColor = GetCategoryColor(clueData.category);
                }
            }
        }
        
        float intensity = isFilled ? filledColorIntensity : emptyColorIntensity;
        return new Color(categoryColor.r, categoryColor.g, categoryColor.b, intensity);
    }
    
    /// <summary>
    /// Debug method to check what clue this slot expects
    /// </summary>
    [ContextMenu("Debug Expected Clue")]
    private void DebugExpectedClue()
    {
        if (string.IsNullOrEmpty(expectedClueId))
        {
            Debug.Log($"ClueSlot '{name}': No expected clue set");
            return;
        }
        
        ClueManager clueManager = FindObjectOfType<ClueManager>();
        if (clueManager == null)
        {
            Debug.LogWarning($"ClueSlot '{name}': ClueManager not found!");
            return;
        }
        
        // Try to find the clue by title first
        ClueData clue = clueManager.GetClueByTitle(expectedClueId);
        if (clue != null)
        {
            Debug.Log($"ClueSlot '{name}': Expects clue by title '{expectedClueId}' -> Found: '{clue.title}' (ID: {clue.GetEffectiveId()})");
        }
        else
        {
            // Try by ID
            clue = clueManager.GetClue(expectedClueId);
            if (clue != null)
            {
                Debug.Log($"ClueSlot '{name}': Expects clue by ID '{expectedClueId}' -> Found: '{clue.title}' (ID: {clue.GetEffectiveId()})");
            }
            else
            {
                Debug.LogWarning($"ClueSlot '{name}': No clue found for '{expectedClueId}' (tried both title and ID)");
            }
        }
    }
    
    /// <summary>
    /// Debug method to test slot colors
    /// </summary>
    [ContextMenu("Debug Slot Colors")]
    private void DebugSlotColors()
    {
        Debug.Log($"=== ClueSlot '{name}' Color Debug ===");
        Debug.Log($"Use Category Colors: {useCategoryColors}");
        Debug.Log($"Use Custom Colors: {useCustomColors}");
        Debug.Log($"Expected Category: {expectedCategory}");
        Debug.Log($"Is Filled: {isFilled}");
        Debug.Log($"Empty Color Intensity: {emptyColorIntensity}");
        Debug.Log($"Filled Color Intensity: {filledColorIntensity}");
        
        if (useCategoryColors)
        {
            Color categoryColor = GetCategoryColor(expectedCategory);
            Debug.Log($"Category Color ({expectedCategory}): {categoryColor}");
            Debug.Log($"Current Slot Color: {GetCurrentSlotColor()}");
            
            if (useCustomColors)
            {
                Debug.Log($"Custom Colors - Name: {customNameColor}, Location: {customLocationColor}, Object: {customObjectColor}, Action: {customActionColor}");
            }
        }
        else
        {
            Debug.Log($"Default Empty Color: {emptyColor}");
            Debug.Log($"Default Filled Color: {filledColor}");
        }
        
        if (currentToken != null)
        {
            ClueManager clueManager = FindObjectOfType<ClueManager>();
            if (clueManager != null)
            {
                ClueData clueData = clueManager.GetClue(currentToken.clueId);
                if (clueData != null)
                {
                    Debug.Log($"Current Token Category: {clueData.category}");
                    Debug.Log($"Current Token Color: {GetCategoryColor(clueData.category)}");
                }
            }
        }
    }
}

