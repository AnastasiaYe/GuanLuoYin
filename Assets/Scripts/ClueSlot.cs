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
        snapPoint ??= transform;
        slotImage ??= GetComponent<Image>();
        UpdateVisualState();
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (isFilled) return;
        
        var draggedToken = eventData.pointerDrag?.GetComponent<ClueToken>();
        if (draggedToken == null) return;
        
        if (draggedToken.clueId == expectedClueId)
        {
            SnapToken(draggedToken);
        }
        else
        {
            draggedToken.AddBackToScrollContent();
            draggedToken.SetDraggable(true);
        }
    }
    
    private void SnapToken(ClueToken token)
    {
        isFilled = true;
        currentToken = token;
        
        token.transform.SetParent(snapPoint, false);
        token.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        token.transform.localScale = Vector3.one;
        token.SetDraggable(false);
        
        UpdateVisualState();
    }
    
    public void RemoveToken()
    {
        if (currentToken != null)
        {
            currentToken.SetDraggable(true);
            currentToken.AddBackToScrollContent();
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
}

