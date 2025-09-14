using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClueSlot : MonoBehaviour, IDropHandler
{
    [Header("Slot Settings")]
    [Tooltip("Expected clue ID or title (system will try both)")]
    public string expectedClueId;
    public Transform snapPoint; // Optional snap point for the clue token
    public bool isFilled = false;
    
    [Header("Visual Feedback")]
    public Image slotImage;
    public Color emptyColor = Color.white;
    public Color filledColor = Color.green;

    public AudioClip interactionSound;
    private AudioSource audioSource;

    private ClueToken currentToken;
    
    private void Start()
    {
        snapPoint ??= transform;
        slotImage ??= GetComponent<Image>();
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
    }
    
    private void UpdateVisualState()
    {
        slotImage.color = isFilled ? filledColor : emptyColor;
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
}

