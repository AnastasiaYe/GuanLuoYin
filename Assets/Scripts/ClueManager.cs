using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ClueManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform clueScrollContent;
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
    }
    
    /// <summary>
    /// Grant a clue to the player (one time only)
    /// </summary>
    /// <param name="clueId">ID of the clue to grant</param>
    /// <param name="openNotebook">Whether to open the notebook after granting</param>
    public void GrantClue(string clueId, bool openNotebook = false)
    {
        if (string.IsNullOrEmpty(clueId))
        {
            Debug.LogWarning("ClueManager: Clue ID cannot be null or empty");
            return;
        }
        
        // Check if clue already earned
        if (earnedClues.ContainsKey(clueId))
        {
            return; // Already earned, silently ignore
        }
        
        // Check if clue exists in database
        if (!clueDatabase.TryGetValue(clueId, out ClueData clue))
        {
            Debug.LogWarning($"ClueManager: Clue '{clueId}' not found in database");
            return;
        }
        
        // Mark clue as earned and create token
        clue.isEarned = true;
        CreateClueToken(clue);
        
        // Open notebook if requested
        if (openNotebook)
        {
            var notebook = FindObjectOfType<NotebookController>();
            notebook?.OpenNotebook();
        }
        
        OnClueEarned?.Invoke(clueId);
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
        if (clueTokenPrefab == null || clueScrollContent == null)
        {
            Debug.LogWarning("ClueManager: ClueTokenPrefab or ClueScrollContent not assigned");
            return;
        }
        
        // Create new clue token
        GameObject tokenObj = Instantiate(clueTokenPrefab, clueScrollContent);
        ClueToken token = tokenObj.GetComponent<ClueToken>();
        
        if (token == null)
        {
            Debug.LogWarning("ClueManager: ClueTokenPrefab must have a ClueToken component");
            Destroy(tokenObj);
            return;
        }
        
        // Setup and store the token
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
