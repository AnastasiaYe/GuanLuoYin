using UnityEngine;
using UnityEngine.UI;

public class ClueColorDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool enableDebugLogging = true;
    
    private ClueManager clueManager;
    
    private void Start()
    {
        clueManager = FindObjectOfType<ClueManager>();
    }
    
    [ContextMenu("Debug All Clue Tokens")]
    public void DebugAllClueTokens()
    {
        Debug.Log("=== Clue Color Debug Report ===");
        
        // Find all ClueToken components in the scene
        ClueToken[] allTokens = FindObjectsOfType<ClueToken>();
        Debug.Log($"Found {allTokens.Length} ClueToken components in scene");
        
        foreach (ClueToken token in allTokens)
        {
            Debug.Log($"--- Clue Token: {token.clueTitle} ---");
            Debug.Log($"  Clue ID: {token.clueId}");
            
            // Check if ClueManager has this clue data
            if (clueManager != null)
            {
                ClueData clueData = clueManager.GetClue(token.clueId);
                if (clueData != null)
                {
                    Debug.Log($"  Category: {clueData.category}");
                    Debug.Log($"  Expected Background: {ClueCategoryColors.GetBackgroundColor(clueData.category)}");
                }
                else
                {
                    Debug.LogWarning($"  ERROR: No ClueData found for ID '{token.clueId}'");
                }
            }
            else
            {
                Debug.LogError("  ERROR: ClueManager not found!");
            }
            
            // Check UI components
            Image backgroundImage = token.GetComponent<Image>();
            if (backgroundImage != null)
            {
                Debug.Log($"  Background Image Found: {backgroundImage.name}");
                Debug.Log($"  Current Background Color: {backgroundImage.color}");
            }
            else
            {
                Debug.LogWarning("  ERROR: No Image component found on ClueToken!");
            }
            
            // Check if backgroundImage is assigned in ClueToken component
            var backgroundImageField = token.GetType().GetField("backgroundImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (backgroundImageField != null)
            {
                Image assignedBackground = backgroundImageField.GetValue(token) as Image;
                if (assignedBackground != null)
                {
                    Debug.Log($"  Assigned Background Image: {assignedBackground.name}");
                }
                else
                {
                    Debug.LogWarning("  ERROR: Background Image not assigned in ClueToken component!");
                }
            }
        }
    }
    
    [ContextMenu("Test Color Application")]
    public void TestColorApplication()
    {
        Debug.Log("=== Testing Color Application ===");
        
        ClueToken[] allTokens = FindObjectsOfType<ClueToken>();
        
        foreach (ClueToken token in allTokens)
        {
            Debug.Log($"Testing color application on: {token.clueTitle}");
            
            // Try to manually apply colors
            token.ApplyCategoryColors(ClueCategory.Name); // Blue
            Debug.Log($"Applied blue color to {token.clueTitle}");
            
            // Wait a frame then apply green
            StartCoroutine(ApplyColorAfterDelay(token, ClueCategory.Location, 1f));
        }
    }
    
    private System.Collections.IEnumerator ApplyColorAfterDelay(ClueToken token, ClueCategory category, float delay)
    {
        yield return new WaitForSeconds(delay);
        token.ApplyCategoryColors(category);
        Debug.Log($"Applied {category} color to {token.clueTitle}");
    }
    
    [ContextMenu("Create Test Clue")]
    public void CreateTestClue()
    {
        if (clueManager == null)
        {
            Debug.LogError("ClueManager not found!");
            return;
        }
        
        // Create a test clue with Name category
        ClueData testClue = new ClueData("test_clue", "Test Clue", "This is a test clue", null, ClueCategory.Name);
        clueManager.GrantClue("test_clue", false);
        
        Debug.Log("Created test clue with Name category (should be blue)");
    }
    
    [ContextMenu("Check ClueManager Setup")]
    public void CheckClueManagerSetup()
    {
        Debug.Log("=== ClueManager Setup Check ===");
        
        if (clueManager == null)
        {
            Debug.LogError("ClueManager not found in scene!");
            return;
        }
        
        Debug.Log($"ClueManager found: {clueManager.name}");
        Debug.Log($"Enable Auto Sorting: {clueManager.enableAutoSorting}");
        Debug.Log($"Sorting Method: {clueManager.sortingMethod}");
        Debug.Log($"Clue Scroll Content: {(clueManager.clueScrollContent != null ? "Assigned" : "NOT ASSIGNED")}");
        Debug.Log($"Clue Token Prefab: {(clueManager.clueTokenPrefab != null ? "Assigned" : "NOT ASSIGNED")}");
        Debug.Log($"Total clues in database: {clueManager.allClues.Count}");
        
        foreach (var clue in clueManager.allClues)
        {
            Debug.Log($"  Clue: {clue.title} (ID: {clue.id}, Category: {clue.category})");
        }
    }
    
    [ContextMenu("Re-enable All Background Images")]
    public void ReEnableAllBackgroundImages()
    {
        Debug.Log("=== Re-enabling All Background Images ===");
        
        ClueToken[] allTokens = FindObjectsOfType<ClueToken>();
        Debug.Log($"Found {allTokens.Length} ClueToken components");
        
        foreach (ClueToken token in allTokens)
        {
            token.ReEnableBackgroundImage();
            
            // Also try to re-apply category colors
            if (clueManager != null)
            {
                ClueData clueData = clueManager.GetClue(token.clueId);
                if (clueData != null)
                {
                    token.ApplyCategoryColors(clueData.category);
                }
            }
        }
        
        Debug.Log("All background images re-enabled and colors reapplied!");
    }
}
