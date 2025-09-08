using UnityEngine;

[System.Serializable]
public class ClueData
{
    public string id;
    public string title;
    public string description;
    public Sprite icon;
    public bool isEarned;
    
    public ClueData(string id, string title, string description, Sprite icon = null)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.icon = icon;
        this.isEarned = false;
    }
}
