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
        
        originalPosition = rectTransform.anchoredPosition;
        originalParent = rectTransform.parent;
        rectTransform.SetAsLastSibling();
        
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
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
        if (originalParent != null)
        {
            rectTransform.SetParent(originalParent, false);
            rectTransform.anchoredPosition = originalPosition;
            rectTransform.SetAsLastSibling();
        }
    }
    
    public void AddBackToScrollContent()
    {
        if (clueManager?.clueScrollContent != null)
        {
            rectTransform.SetParent(clueManager.clueScrollContent, false);
            rectTransform.SetAsLastSibling();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            StartCoroutine(ForceLayoutUpdate());
        }
        else
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
