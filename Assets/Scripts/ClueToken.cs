using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ClueToken : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Clue Info")]
    public string clueId;
    public string clueTitle;
    public string clueDescription;
    
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI titleText;
    
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Transform originalParent;
    private bool isDraggable = true;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        
        if (canvas == null)
        {
            Debug.LogWarning($"ClueToken '{gameObject.name}': No Canvas found in parent hierarchy");
        }
    }
    
    public void SetupClue(string id, string title, string description, Sprite icon = null)
    {
        clueId = id;
        clueTitle = title;
        clueDescription = description;
        
        // Update UI
        if (titleText != null)
        {
            titleText.text = title;
        }
        
        if (iconImage != null)
        {
            if (icon != null)
            {
                iconImage.sprite = icon;
                iconImage.gameObject.SetActive(true);
            }
            else
            {
                iconImage.gameObject.SetActive(false);
            }
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        // Store original position and parent
        originalPosition = rectTransform.anchoredPosition;
        originalParent = rectTransform.parent;
        
        // Make token appear on top
        rectTransform.SetAsLastSibling();
        
        // Visual feedback while dragging
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        // Convert screen position to world position within canvas
        Vector3 worldPosition;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out worldPosition))
        {
            rectTransform.position = worldPosition;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        // Restore visual state
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        
        // Check for valid drop target
        GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;
        if (dropTarget != null)
        {
            ClueSlot slot = dropTarget.GetComponent<ClueSlot>();
            if (slot != null)
            {
                return; // Let the slot handle the drop
            }
        }
        
        // No valid drop target, return to original position
        ReturnToOriginalPosition();
    }
    
    public void ReturnToOriginalPosition()
    {
        // Return to original parent and position
        rectTransform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.SetAsFirstSibling(); // Put back in original order
    }
    
    public void SetDraggable(bool draggable)
    {
        isDraggable = draggable;
        
        // Update visual feedback
        if (canvasGroup != null)
        {
            canvasGroup.alpha = draggable ? 1f : 0.8f;
        }
    }
    
    public bool IsDraggable()
    {
        return isDraggable;
    }
}
