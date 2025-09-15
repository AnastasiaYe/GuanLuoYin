using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;



public enum ClueSortingMethod
{
    ByCategory,     // Sort by category (Name, Location, Object)
    ByTitle,        // Sort alphabetically by title
    ByEarnedTime    // Sort by when clues were earned (newest first)
}

public class ClueManager : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform clueScrollContent;
    public GameObject clueTokenPrefab;
    
    [Header("Clue Database")]
    public List<ClueData> allClues = new List<ClueData>();
    
    [Header("Sorting Settings")]
    public bool enableAutoSorting = true;
    public ClueSortingMethod sortingMethod = ClueSortingMethod.ByCategory;
    
    [Header("Color Settings")]
    public ClueCategoryColorSettings colorSettings = new ClueCategoryColorSettings();

    // Runtime data
    private Dictionary<string, ClueData> clueDatabase = new Dictionary<string, ClueData>();
    private Dictionary<string, ClueToken> earnedClues = new Dictionary<string, ClueToken>();
    
    // Events
    public System.Action<string> OnClueEarned;
    public System.Action<string> OnCluePlaced;

    private void Start()
    {
        // Set the color settings for the static class
        ClueCategoryColors.SetColorSettings(colorSettings);
        
        // Initialize clue database
        foreach (var clue in allClues)
        {
            clueDatabase[clue.GetEffectiveId()] = clue;
        }
        
        // Validate scroll content setup
        ValidateScrollContent();
    }
    
    private void ValidateScrollContent()
    {
        if (clueScrollContent == null)
        {
            Debug.LogError("ClueManager: clueScrollContent is not assigned!");
            return;
        }
        
        if (clueScrollContent.GetComponentInParent<ScrollRect>() == null)
        {
            Debug.LogError("ClueManager: clueScrollContent should be inside a ScrollRect!");
        }
    }
    
    /// <summary>
    /// Grant a clue to the player (one time only)
    /// </summary>
    /// <param name="clueId">ID of the clue to grant</param>
    /// <param name="openNotebook">Whether to open the notebook after granting</param>
    public void GrantClue(string clueId, bool openNotebook = false)
    {
        if (string.IsNullOrEmpty(clueId) || earnedClues.ContainsKey(clueId) || !clueDatabase.TryGetValue(clueId, out ClueData clue))
            return;
        
        clue.isEarned = true;
        CreateClueToken(clue);
        OnClueEarned?.Invoke(clueId);
        
        if (openNotebook)
        {
            FindObjectOfType<NotebookController>()?.OpenNotebook();
        }
    }
    
    /// <summary>
    /// Check if a clue has been earned
    /// </summary>
    /// <param name="clueId">ID of the clue to check</param>
    /// <returns>True if earned, false otherwise</returns>
    public bool HasClue(string clueId)
    {
        return earnedClues.ContainsKey(clueId);
    }
    
    /// <summary>
    /// Get a clue by ID
    /// </summary>
    /// <param name="clueId">ID of the clue</param>
    /// <returns>ClueData if found, null otherwise</returns>
    public ClueData GetClue(string clueId)
    {
        return clueDatabase.ContainsKey(clueId) ? clueDatabase[clueId] : null;
    }
    
    /// <summary>
    /// Get a clue by title (simpler for users)
    /// </summary>
    /// <param name="title">Title of the clue</param>
    /// <returns>ClueData if found, null otherwise</returns>
    public ClueData GetClueByTitle(string title)
    {
        foreach (var clue in allClues)
        {
            if (clue.title == title)
            {
                return clue;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Grant a clue by title (simpler for users)
    /// </summary>
    /// <param name="title">Title of the clue to grant</param>
    /// <param name="openNotebook">Whether to open the notebook after granting</param>
    public void GrantClueByTitle(string title, bool openNotebook = false)
    {
        ClueData clue = GetClueByTitle(title);
        if (clue != null)
        {
            GrantClue(clue.GetEffectiveId(), openNotebook);
        }
        else
        {
            Debug.LogWarning($"ClueManager: No clue found with title '{title}'");
        }
    }
    
    /// <summary>
    /// Get all earned clues
    /// </summary>
    /// <returns>List of earned clue IDs</returns>
    public List<string> GetEarnedClues()
    {
        return earnedClues.Keys.ToList();
    }
    
    private void CreateClueToken(ClueData clue)
    {
        if (clueTokenPrefab == null || clueScrollContent == null) return;
        
        SetupScrollViewLayout();
        
        GameObject tokenObj = Instantiate(clueTokenPrefab, clueScrollContent);
        ClueToken token = tokenObj.GetComponent<ClueToken>();
        
        if (token == null)
        {
            Destroy(tokenObj);
            return;
        }
        
        token.SetupClue(clue.GetEffectiveId(), clue.title, clue.description, clue.icon, clue.category);
        earnedClues[clue.GetEffectiveId()] = token;
        
        // Ensure image stays enabled after setup (delayed to run after any other setup code)
        StartCoroutine(EnsureImageEnabledDelayed(token));
        
        // Sort clues if auto-sorting is enabled
        if (enableAutoSorting)
        {
            SortClueTokens();
        }
    }
    
    /// <summary>
    /// Remove a clue token (when placed in slot)
    /// </summary>
    /// <param name="clueId">ID of the clue to remove</param>
    public void RemoveClueToken(string clueId)
    {
        if (earnedClues.ContainsKey(clueId))
        {
            ClueToken token = earnedClues[clueId];
            if (token != null)
            {
                Destroy(token.gameObject);
            }
            earnedClues.Remove(clueId);
        }
    }
    
    /// <summary>
    /// Return a clue token to the scroll area
    /// </summary>
    /// <param name="clueId">ID of the clue to return</param>
    public void ReturnClueToken(string clueId)
    {
        if (earnedClues.ContainsKey(clueId))
        {
            ClueToken token = earnedClues[clueId];
            if (token != null)
            {
                token.ReturnToOriginalPosition();
                token.SetDraggable(true);
            }
        }
    }
    
    /// <summary>
    /// Setup ScrollView layout components for proper vertical scrolling
    /// </summary>
    private void SetupScrollViewLayout()
    {
        // Set RectTransform anchor to top for proper scrolling (preserve original width)
        RectTransform contentRect = clueScrollContent.GetComponent<RectTransform>();
        if (contentRect != null)
        {
            // Only set vertical anchors, preserve horizontal positioning
            contentRect.anchorMin = new Vector2(contentRect.anchorMin.x, 1);
            contentRect.anchorMax = new Vector2(contentRect.anchorMax.x, 1);
            contentRect.pivot = new Vector2(contentRect.pivot.x, 1);
        }
        
        var layoutGroup = clueScrollContent.GetComponent<VerticalLayoutGroup>() ?? clueScrollContent.gameObject.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 10f;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        
        var sizeFitter = clueScrollContent.GetComponent<ContentSizeFitter>() ?? clueScrollContent.gameObject.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }
    
    /// <summary>
    /// Test method to grant the first clue in the database
    /// </summary>
    [ContextMenu("Test Grant First Clue")]
    public void TestGrantFirstClue()
    {
        if (clueDatabase.Count > 0)
        {
            var firstClue = clueDatabase.Values.First();
            GrantClue(firstClue.id, true);
        }
    }
    
    #region Sorting Methods
    
    /// <summary>
    /// Sort all clue tokens according to the current sorting method
    /// </summary>
    public void SortClueTokens()
    {
        if (clueScrollContent == null) return;
        
        // Get all clue tokens in the scroll content
        List<ClueToken> tokens = new List<ClueToken>();
        foreach (Transform child in clueScrollContent)
        {
            ClueToken token = child.GetComponent<ClueToken>();
            if (token != null)
            {
                tokens.Add(token);
            }
        }
        
        // Sort tokens based on the sorting method
        switch (sortingMethod)
        {
            case ClueSortingMethod.ByCategory:
                tokens = tokens.OrderBy(t => GetCategoryOrder(t.clueId)).ThenBy(t => t.clueTitle).ToList();
                break;
            case ClueSortingMethod.ByTitle:
                tokens = tokens.OrderBy(t => t.clueTitle).ToList();
                break;
            case ClueSortingMethod.ByEarnedTime:
                // Keep current order (newest first) - tokens are already in earned order
                break;
        }
        
        // Reorder tokens in the UI
        for (int i = 0; i < tokens.Count; i++)
        {
            tokens[i].transform.SetSiblingIndex(i);
        }
        
        // Force layout update
        StartCoroutine(ForceLayoutUpdate());
    }
    
    /// <summary>
    /// Get the sort order for a category (lower number = appears first)
    /// </summary>
    /// <param name="clueId">The clue ID</param>
    /// <returns>Sort order number</returns>
    private int GetCategoryOrder(string clueId)
    {
        if (!clueDatabase.TryGetValue(clueId, out ClueData clue)) return 999;
        
        switch (clue.category)
        {
            case ClueCategory.Name:
                return 0;
            case ClueCategory.Location:
                return 1;
            case ClueCategory.Object:
                return 2;
            case ClueCategory.Action:
                return 3;
            default:
                return 999;
        }
    }
    
    private System.Collections.IEnumerator ForceLayoutUpdate()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(clueScrollContent);
    }
    
    private System.Collections.IEnumerator EnsureImageEnabledDelayed(ClueToken token)
    {
        // Wait a few frames to ensure all setup is complete
        yield return null;
        yield return null;
        yield return null;
        
        // Re-enable the background image and reapply colors
        if (token != null)
        {
            token.ForceReEnableImageAndColors();
        }
    }
    
    /// <summary>
    /// Change the sorting method and re-sort all clues
    /// </summary>
    /// <param name="newMethod">The new sorting method</param>
    public void SetSortingMethod(ClueSortingMethod newMethod)
    {
        sortingMethod = newMethod;
        SortClueTokens();
    }
    
    /// <summary>
    /// Get all clues grouped by category
    /// </summary>
    /// <returns>Dictionary of clues grouped by category</returns>
    public Dictionary<ClueCategory, List<ClueData>> GetCluesByCategory()
    {
        Dictionary<ClueCategory, List<ClueData>> categorizedClues = new Dictionary<ClueCategory, List<ClueData>>();
        
        foreach (var clue in allClues)
        {
            if (!categorizedClues.ContainsKey(clue.category))
            {
                categorizedClues[clue.category] = new List<ClueData>();
            }
            categorizedClues[clue.category].Add(clue);
        }
        
        return categorizedClues;
    }
    
    /// <summary>
    /// Refresh colors for all existing clue tokens (call this when color settings change)
    /// </summary>
    [ContextMenu("Refresh All Clue Colors")]
    public void RefreshAllClueColors()
    {
        // Update the color settings
        ClueCategoryColors.SetColorSettings(colorSettings);
        
        // Refresh all existing clue tokens
        foreach (var token in earnedClues.Values)
        {
            if (token != null)
            {
                ClueData clueData = GetClue(token.clueId);
                if (clueData != null)
                {
                    token.ApplyCategoryColors(clueData.category);
                }
            }
        }
        
        Debug.Log("ClueManager: Refreshed colors for all clue tokens");
    }
    
    #endregion
}


