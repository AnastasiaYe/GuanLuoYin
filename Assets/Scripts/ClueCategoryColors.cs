using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class ClueCategoryColorSettings
{
    [Header("Background Colors")]
    [Tooltip("Background color for Name category clues")]
    public Color nameBackgroundColor = new Color(1f, 1f, 1f, 1f);      // Light Blue background
    
    [Tooltip("Background color for Location category clues")]
    public Color locationBackgroundColor = new Color(0.99f, 0.99f, 0.99f, 1f);      // Light Green background
    
    [Tooltip("Background color for Object category clues")]
    public Color objectBackgroundColor = new Color(0.98f, 0.98f, 0.98f, 1f);        // Light Yellow background
    
    [Tooltip("Background color for Action category clues")]
    public Color actionBackgroundColor = new Color(0.97f, 0.97f, 0.97f, 1f);        // Light Magenta background
}

public static class ClueCategoryColors
{
    // Static reference to color settings (will be set by ClueManager)
    private static ClueCategoryColorSettings colorSettings;
    
    /// <summary>
    /// Set the color settings (called by ClueManager)
    /// </summary>
    /// <param name="settings">The color settings to use</param>
    public static void SetColorSettings(ClueCategoryColorSettings settings)
    {
        colorSettings = settings;
    }
    
    /// <summary>
    /// Get the current color settings
    /// </summary>
    /// <returns>Current color settings, or default if none set</returns>
    public static ClueCategoryColorSettings GetColorSettings()
    {
        if (colorSettings == null)
        {
            colorSettings = new ClueCategoryColorSettings();
        }
        return colorSettings;
    }
    
    
    /// <summary>
    /// Get the background color for a clue category
    /// </summary>
    /// <param name="category">The clue category</param>
    /// <returns>Background color for the category</returns>
    public static Color GetBackgroundColor(ClueCategory category)
    {
        ClueCategoryColorSettings settings = GetColorSettings();
        
        switch (category)
        {
            case ClueCategory.Name:
                return new Color(1f, 1f, 1f, 1f);
            case ClueCategory.Location:
                return new Color(0.99f, 0.99f, 0.99f, 1f);
            case ClueCategory.Object:
                return new Color(0.98f, 0.98f, 0.98f, 1f);
            case ClueCategory.Action:
                return new Color(0.97f, 0.97f, 0.97f, 1f);
            default:
                return new Color(1f, 1f, 1f, 0.5f);          // Light white background
        }
    }

    /// <summary>
    /// Get the category name as a string
    /// </summary>
    /// <param name="category">The clue category</param>
    /// <returns>Category name string</returns>
    public static string GetCategoryName(ClueCategory category)
    {
        switch (category)
        {
            case ClueCategory.Name:
                return "Name";
            case ClueCategory.Location:
                return "Location";
            case ClueCategory.Object:
                return "Object";
            case ClueCategory.Action:
                return "Action";
            default:
                return "Unknown";
        }
    }
}
