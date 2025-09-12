using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ClueManager : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform clueScrollContent;
    public GameObject clueTokenPrefab;
    
    [Header("Clue Database")]
    public List<ClueData> allClues = new List<ClueData>();
    
    // Runtime data
    private Dictionary<string, ClueData> clueDatabase = new Dictionary<string, ClueData>();
    private Dictionary<string, ClueToken> earnedClues = new Dictionary<string, ClueToken>();
    
    // Events
    public System.Action<string> OnClueEarned;
    public System.Action<string> OnCluePlaced;
    
    private void Start()
    {
        // Initialize clue database
        foreach (var clue in allClues)
        {
            clueDatabase[clue.id] = clue;
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
        
        token.SetupClue(clue.id, clue.title, clue.description, clue.icon);
        earnedClues[clue.id] = token;
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
}
