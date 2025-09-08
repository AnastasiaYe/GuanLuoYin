using UnityEngine;

public class ClueGiver : MonoBehaviour
{
    [Header("Clue Settings")]
    public string clueId;
    public bool openNotebookOnGrant = true;
    
    [Header("Visual Feedback")]
    public GameObject visualFeedback; // Optional visual effect when clue is granted
    
    private bool hasGrantedClue = false;
    private ClueManager clueManager;
    
    private void Start()
    {
        clueManager = FindObjectOfType<ClueManager>();
        if (clueManager == null)
        {
            Debug.LogWarning($"ClueGiver on {gameObject.name}: No ClueManager found in scene");
        }
    }
    
    /// <summary>
    /// Grant the clue (called by ItemBehaviour or other scripts)
    /// </summary>
    public void GrantClue()
    {
        if (hasGrantedClue || clueManager == null || string.IsNullOrEmpty(clueId))
        {
            return;
        }
        
        // Grant the clue
        clueManager.GrantClue(clueId, openNotebookOnGrant);
        hasGrantedClue = true;
        
        // Show visual feedback
        if (visualFeedback != null)
        {
            visualFeedback.SetActive(true);
        }
    }
    
    /// <summary>
    /// Check if this clue has been granted
    /// </summary>
    /// <returns>True if granted, false otherwise</returns>
    public bool HasGrantedClue()
    {
        return hasGrantedClue;
    }
    
    /// <summary>
    /// Set the clue ID (useful for dynamic clue assignment)
    /// </summary>
    /// <param name="newClueId">New clue ID to set</param>
    public void SetClueId(string newClueId)
    {
        clueId = newClueId;
    }
}
