using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class ClueToken : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Clue Info")]
    public string clueId;
    public string clueTitle;
    public string clueDescription;
    
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Transform originalParent;
    private bool isDraggable = true;
    private ClueManager clueManager;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        clueManager = FindObjectOfType<ClueManager>();
        
        // Canvas will be found when needed during drag operations
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
        
        if (descriptionText != null)
        {
            descriptionText.text = description;
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
        
        // Store original state
        originalPosition = rectTransform.anchoredPosition;
        originalParent = rectTransform.parent;
        
        // Move to top-level canvas to ensure it's above everything
        Canvas topCanvas = GetTopCanvas();
        if (topCanvas != null)
        {
            rectTransform.SetParent(topCanvas.transform, false);
            // Ensure it's at the very top of the hierarchy
            rectTransform.SetAsLastSibling();
            
            // Set a high sorting order to ensure it's above other UI elements
            Canvas dragCanvas = rectTransform.GetComponent<Canvas>();
            if (dragCanvas == null)
            {
                dragCanvas = rectTransform.gameObject.AddComponent<Canvas>();
                dragCanvas.overrideSorting = true;
            }
            else if (!dragCanvas.overrideSorting)
            {
                dragCanvas.overrideSorting = true;
            }
            dragCanvas.sortingOrder = 1000; // Very high sorting order
        }
        else
        {
            rectTransform.SetAsLastSibling();
        }
        
        // Visual feedback
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        // Get the current canvas (might be the top canvas now)
        Canvas currentCanvas = rectTransform.GetComponentInParent<Canvas>();
        if (currentCanvas == null) currentCanvas = canvas;
        
        // Convert screen position to world position within canvas
        Vector3 worldPosition;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            currentCanvas.transform as RectTransform,
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
         
        // Clean up drag canvas
        Canvas dragCanvas = rectTransform.GetComponent<Canvas>();
        if (dragCanvas != null && dragCanvas.overrideSorting)
        {
            DestroyImmediate(dragCanvas);
        }
         
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
         
         // Check for valid drop target
         GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;
         if (dropTarget != null)
         {
             ClueSlot slot = dropTarget.GetComponent<ClueSlot>();
             if (slot != null)
             {
                 // Let the slot handle the drop logic
                 return;
             }
         }
         
        // No valid drop target, add back to scrollable content
        AddBackToScrollContent();
        SetDraggable(true);
     }
    
    public void ReturnToOriginalPosition()
    {
        // Clean up any temporary drag canvas
        Canvas dragCanvas = rectTransform.GetComponent<Canvas>();
        if (dragCanvas != null && dragCanvas.overrideSorting)
        {
            DestroyImmediate(dragCanvas);
        }
        
        if (originalParent != null)
        {
            rectTransform.SetParent(originalParent, false);
            rectTransform.anchoredPosition = originalPosition;
            rectTransform.SetAsLastSibling();
        }
    }
    
    /// <summary>
    /// Find the top-level canvas in the hierarchy
    /// </summary>
    private Canvas GetTopCanvas()
    {
        Canvas currentCanvas = canvas;
        Canvas topCanvas = currentCanvas;
        
        // Walk up the hierarchy to find the root canvas
        while (currentCanvas != null)
        {
            topCanvas = currentCanvas;
            currentCanvas = currentCanvas.transform.parent?.GetComponent<Canvas>();
        }
        
        return topCanvas;
    }
    
    public void AddBackToScrollContent()
    {
        // Clean up any temporary drag canvas
        Canvas dragCanvas = rectTransform.GetComponent<Canvas>();
        if (dragCanvas != null && dragCanvas.overrideSorting)
        {
            DestroyImmediate(dragCanvas);
        }
        
        if (clueManager?.clueScrollContent != null)
        {
            rectTransform.SetParent(clueManager.clueScrollContent, false);
            rectTransform.SetAsLastSibling();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            StartCoroutine(ForceLayoutUpdate());
        }
        else if (originalParent != null)
        {
            ReturnToOriginalPosition();
        }
    }
    
    private IEnumerator ForceLayoutUpdate()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(clueManager.clueScrollContent);
    }
    
    public void SetDraggable(bool draggable)
    {
        isDraggable = draggable;
        
        canvasGroup.alpha = draggable ? 1f : 0.8f;
    }
    
    public bool IsDraggable()
    {
        return isDraggable;
    }
}
