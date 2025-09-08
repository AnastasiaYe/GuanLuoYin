using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClueSlot : MonoBehaviour, IDropHandler
{
    [Header("Slot Settings")]
    public string expectedClueId;
    public Transform snapPoint; // Optional snap point for the clue token
    public bool isFilled = false;
    
    [Header("Visual Feedback")]
    public Image slotImage;
    public Color emptyColor = Color.white;
    public Color filledColor = Color.green;
    
    private ClueToken currentToken;
    
    private void Start()
    {
        // If no snap point specified, use this transform
        if (snapPoint == null)
        {
            snapPoint = transform;
        }
        
        // Setup visual feedback
        if (slotImage == null)
        {
            slotImage = GetComponent<Image>();
        }
        
        UpdateVisualState();
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (isFilled) return;
        
        var draggedToken = eventData.pointerDrag?.GetComponent<ClueToken>();
        if (draggedToken == null) return;
        
        // Check if the clue matches this slot
        if (draggedToken.clueId == expectedClueId)
        {
            SnapToken(draggedToken);
        }
        else
        {
            draggedToken.ReturnToOriginalPosition();
        }
    }
    
    private void SnapToken(ClueToken token)
    {
        isFilled = true;
        currentToken = token;
        
        // Move token to snap point
        token.transform.SetParent(snapPoint);
        token.transform.localPosition = Vector3.zero;
        token.transform.localScale = Vector3.one;
        
        // Disable dragging for this token
        token.SetDraggable(false);
        
        UpdateVisualState();
    }
    
    public void RemoveToken()
    {
        if (currentToken != null)
        {
            // Re-enable dragging
            currentToken.SetDraggable(true);
            currentToken.ReturnToOriginalPosition();
            currentToken = null;
        }
        
        isFilled = false;
        UpdateVisualState();
    }
    
    private void UpdateVisualState()
    {
        if (slotImage != null)
        {
            slotImage.color = isFilled ? filledColor : emptyColor;
        }
    }
    
    public bool IsFilled()
    {
        return isFilled;
    }
    
    public string GetExpectedClueId()
    {
        return expectedClueId;
    }
}
