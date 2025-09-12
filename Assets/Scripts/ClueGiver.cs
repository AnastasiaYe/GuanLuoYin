using UnityEngine;

public class ClueGiver : MonoBehaviour
{
    [Header("Clue Settings")]
    public string clueId;
    public bool openNotebookOnGrant = true;
    
    [Header("Multiple Clues")]
    public bool givesMultipleClues = false;
    public string[] clueIds = new string[0];
    
    [Header("Visual Feedback")]
    public GameObject visualFeedback; // Optional visual effect when clue is granted
    
    private bool hasGrantedClue = false;
    private ClueManager clueManager;
    
    private void Start()
    {
        clueManager = FindObjectOfType<ClueManager>();
    }
    
    /// <summary>
    /// Grant the clue (called by ItemBehaviour or other scripts)
    /// </summary>
    public void GrantClue()
    {
        if (hasGrantedClue || clueManager == null) return;
        
        if (givesMultipleClues)
        {
            bool grantedAny = false;
            foreach (string id in clueIds)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    clueManager.GrantClue(id, false);
                    grantedAny = true;
                }
            }
            
            if (grantedAny)
            {
                hasGrantedClue = true;
                if (openNotebookOnGrant)
                {
                    FindObjectOfType<NotebookController>()?.OpenNotebook();
                }
            }
        }
        else
        {
            if (string.IsNullOrEmpty(clueId)) return;
            
            clueManager.GrantClue(clueId, openNotebookOnGrant);
            hasGrantedClue = true;
        }
        
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
        givesMultipleClues = false;
    }
    
    /// <summary>
    /// Set multiple clue IDs
    /// </summary>
    /// <param name="newClueIds">Array of clue IDs to set</param>
    public void SetClueIds(string[] newClueIds)
    {
        clueIds = newClueIds;
        givesMultipleClues = true;
    }
}
