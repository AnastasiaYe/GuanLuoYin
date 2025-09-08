using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CharacterManager : MonoBehaviour
{
    [Header("Character Management")]
    public List<CharacterData> characters = new List<CharacterData>();
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    // Singleton pattern for easy access
    private static CharacterManager _instance;
    public static CharacterManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CharacterManager>();
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Subscribe to stage changes
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnStageChanged += OnStageChanged;
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from stage changes
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnStageChanged -= OnStageChanged;
        }
    }
    
    #region Character Management
    
    public void AddCharacter(CharacterData characterData)
    {
        if (characterData == null) return;
        
        characters.Add(characterData);
        
        if (showDebugInfo)
        {
            Debug.Log($"Added character: {characterData.characterName}");
        }
    }
    
    public void RemoveCharacter(string characterName)
    {
        var character = GetCharacter(characterName);
        if (character != null)
        {
            characters.Remove(character);
            
            if (showDebugInfo)
            {
                Debug.Log($"Removed character: {characterName}");
            }
        }
    }
    
    public CharacterData GetCharacter(string characterName)
    {
        return characters.FirstOrDefault(c => c.characterName == characterName);
    }
    
    public CharacterData GetCharacter(int index)
    {
        if (index >= 0 && index < characters.Count)
        {
            return characters[index];
        }
        return null;
    }
    
    public List<CharacterData> GetCharactersInRoom(int roomIndex)
    {
        return characters.Where(c => c.currentRoomIndex == roomIndex).ToList();
    }
    
    public List<CharacterData> GetCharactersInState(int state)
    {
        return characters.Where(c => c.currentState == state).ToList();
    }
    
    #endregion
    
    #region Stage Change Handler
    
    private void OnStageChanged(int newStage)
    {
        // Update all characters to match the new stage
        foreach (var character in characters)
        {
            if (character != null)
            {
                character.ChangeState(newStage);
            }
        }
    }
    
    #endregion
    
    #region Debug Methods
    
    [ContextMenu("Debug Character States")]
    public void DebugCharacterStates()
    {
        Debug.Log("=== Character States ===");
        foreach (var character in characters)
        {
            Debug.Log($"{character.characterName}: {character.currentState} in Room {character.currentRoomIndex}");
        }
    }
    
    [ContextMenu("Reset All Characters to Stage 0")]
    public void ResetAllCharactersToStage0()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.SetStage(0);
        }
    }
    
    #endregion
}