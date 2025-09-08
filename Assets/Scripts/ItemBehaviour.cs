using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    [Header("Item Info")]
    public string itemName;
    public string itemDescription;
    
    [Header("Interaction")]
    public bool isInteractable = true;
    public float interactionCooldown = 0.5f; // Prevent rapid clicking
    
    private float lastInteractionTime = 0f;
    
    // Events
    public System.Action<ItemBehaviour> OnItemInteracted;
    
    private void Start()
    {
        // Set item name to GameObject name if not set
        if (string.IsNullOrEmpty(itemName))
        {
            itemName = gameObject.name;
        }
    }
    
    public void OnInteract()
    {
        // Check if item is interactable
        if (!isInteractable) return;
        
        // Check cooldown
        if (Time.time - lastInteractionTime < interactionCooldown) return;
        
        // Update last interaction time
        lastInteractionTime = Time.time;
        
        // Handle interaction
        HandleInteraction();
        
        // Trigger event
        OnItemInteracted?.Invoke(this);
    }
    
    private void HandleInteraction()
    {
        Debug.Log($"Interacted with item: {itemName}");
        
        // Check if this item has a clue giver
        var clueGiver = GetComponent<ClueGiver>();
        if (clueGiver != null)
        {
            clueGiver.GrantClue();
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
}
