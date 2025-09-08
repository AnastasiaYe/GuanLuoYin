using UnityEngine;

[System.Serializable]
public class CharacterData
{
    [Header("Character Info")]
    public string characterName;
    public GameObject characterPrefab;
    
    [Header("Current State")]
    public int currentState; // 0-4
    public int currentRoomIndex;
    
    // Events for this character
    public System.Action<CharacterData> OnStateChanged;
    public System.Action<CharacterData> OnMoved;
    
    // Change state
    public void ChangeState(int newState)
    {
        currentState = newState;
        
        // Trigger event
        OnStateChanged?.Invoke(this);
    }
    
    // Move to room
    public void MoveToRoom(int roomIndex)
    {
        currentRoomIndex = roomIndex;
        OnMoved?.Invoke(this);
    }
}
