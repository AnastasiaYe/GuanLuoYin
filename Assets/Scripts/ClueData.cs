using UnityEngine;

public enum ClueCategory
{
    Name,       // Person names, character information
    Location,   // Places, rooms, areas
    Object,     // Physical items, evidence, objects
    Action      // Actions, events, activities
}

[System.Serializable]
public class ClueData
{
    [Header("Clue Identification")]
    [Tooltip("Unique ID for the clue (auto-generated from title if empty)")]
    public string id;
    
    [Header("Clue Information")]
    public string title;
    public string description;
    public Sprite icon;
    public ClueCategory category = ClueCategory.Object;
    
    [Header("Runtime State")]
    public bool isEarned;
    
    public ClueData(string id, string title, string description, Sprite icon = null, ClueCategory category = ClueCategory.Object)
    {
        this.id = string.IsNullOrEmpty(id) ? GenerateIdFromTitle(title) : id;
        this.title = title;
        this.description = description;
        this.icon = icon;
        this.category = category;
        this.isEarned = false;
    }
    
    /// <summary>
    /// Generate a safe ID from the title
    /// </summary>
    /// <param name="title">The title to generate ID from</param>
    /// <returns>Safe ID string</returns>
    private static string GenerateIdFromTitle(string title)
    {
        if (string.IsNullOrEmpty(title)) return "clue_unknown";
        
        // Remove spaces and special characters, make lowercase
        string safeId = title.ToLower()
            .Replace(" ", "_")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace("-", "_")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("!", "")
            .Replace("?", "");
        
        return $"clue_{safeId}";
    }
    
    /// <summary>
    /// Get the effective ID (auto-generate from title if empty)
    /// </summary>
    /// <returns>Safe ID to use for tracking</returns>
    public string GetEffectiveId()
    {
        return string.IsNullOrEmpty(id) ? GenerateIdFromTitle(title) : id;
    }
}
