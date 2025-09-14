using UnityEngine;

public class SimpleItemTest : MonoBehaviour
{
    [Header("Test Settings")]
    public GameObject testItem;
    public int[] testStages = { 0 }; // Only show in stage 0 by default
    
    private void Start()
    {
        // Add ItemBehaviour to test item if it doesn't have one
        if (testItem != null)
        {
            ItemBehaviour itemBehaviour = testItem.GetComponent<ItemBehaviour>();
            if (itemBehaviour == null)
            {
                itemBehaviour = testItem.AddComponent<ItemBehaviour>();
                Debug.Log("SimpleItemTest: Added ItemBehaviour to test item");
            }
            
            // Configure the item
            itemBehaviour.useItemManager = true;
            itemBehaviour.availableStages.Clear();
            foreach (int stage in testStages)
            {
                itemBehaviour.availableStages.Add(stage);
            }
            
            Debug.Log($"SimpleItemTest: Configured test item '{testItem.name}' to appear in stages: [{string.Join(", ", testStages)}]");
        }
        else
        {
            Debug.LogError("SimpleItemTest: No test item assigned!");
        }
    }
    
    [ContextMenu("Test Stage 0")]
    public void TestStage0()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.SetStage(0);
            Debug.Log("SimpleItemTest: Set to stage 0");
        }
        else
        {
            Debug.LogError("SimpleItemTest: StageManager not found!");
        }
    }
    
    [ContextMenu("Test Stage 1")]
    public void TestStage1()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.SetStage(1);
            Debug.Log("SimpleItemTest: Set to stage 1");
        }
        else
        {
            Debug.LogError("SimpleItemTest: StageManager not found!");
        }
    }
    
    [ContextMenu("Check Item Status")]
    public void CheckItemStatus()
    {
        if (testItem == null)
        {
            Debug.LogError("SimpleItemTest: No test item assigned!");
            return;
        }
        
        ItemBehaviour itemBehaviour = testItem.GetComponent<ItemBehaviour>();
        if (itemBehaviour == null)
        {
            Debug.LogError("SimpleItemTest: Test item has no ItemBehaviour component!");
            return;
        }
        
        Debug.Log($"SimpleItemTest: Item '{testItem.name}' Status:");
        Debug.Log($"  - Active in hierarchy: {testItem.activeInHierarchy}");
        Debug.Log($"  - Use Item Manager: {itemBehaviour.useItemManager}");
        Debug.Log($"  - Available Stages: [{string.Join(", ", itemBehaviour.availableStages)}]");
        
        if (StageManager.Instance != null)
        {
            Debug.Log($"  - Current Stage: {StageManager.Instance.currentStage}");
            Debug.Log($"  - Should be visible: {itemBehaviour.availableStages.Contains(StageManager.Instance.currentStage)}");
        }
        else
        {
            Debug.LogError("  - StageManager not found!");
        }
        
        if (ItemInteraction.Instance != null)
        {
            Debug.Log($"  - ItemInteraction found: Yes");
            Debug.Log($"  - Enable Stage Based Items: {ItemInteraction.Instance.enableStageBasedItems}");
        }
        else
        {
            Debug.LogError("  - ItemInteraction not found!");
        }
    }
}
