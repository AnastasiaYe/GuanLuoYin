using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    [Header("Character Reference")]
    public CharacterData characterData;
    
    private CharacterManager characterManager;
    private CharacterStatePositions statePositions;
    
    private void Start()
    {
        // Get references
        characterManager = CharacterManager.Instance;
        statePositions = GetComponent<CharacterStatePositions>();
        
        // Initialize character data if not assigned
        if (characterData == null)
        {
            characterData = new CharacterData();
            characterData.characterName = gameObject.name;
        }
        
        // Set character prefab reference
        characterData.characterPrefab = gameObject;
        
        // Register with character manager
        if (characterManager != null)
        {
            characterManager.AddCharacter(characterData);
        }
        
        // Subscribe to character events
        characterData.OnStateChanged += OnCharacterStateChanged;
        characterData.OnMoved += OnCharacterMoved;
        
        // Set initial position based on current state
        if (statePositions != null)
        {
            transform.position = statePositions.GetPositionForState(characterData.currentState);
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from character events
        if (characterData != null)
        {
            characterData.OnStateChanged -= OnCharacterStateChanged;
            characterData.OnMoved -= OnCharacterMoved;
        }
    }
    
    
    #region Event Handlers
    
    private void OnCharacterStateChanged(CharacterData character)
    {
        if (character == characterData && statePositions != null)
        {
            // Update position based on new state
            Vector3 newPosition = statePositions.GetPositionForState(character.currentState);
            transform.position = newPosition;
        }
    }
    
    private void OnCharacterMoved(CharacterData character)
    {
        if (character == characterData && statePositions != null)
        {
            // Update position if needed
            transform.position = statePositions.GetPositionForState(character.currentState);
        }
    }
    
    #endregion
    
}