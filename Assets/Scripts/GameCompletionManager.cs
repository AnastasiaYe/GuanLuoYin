using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameCompletionManager : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Name of the end scene to load when game is completed")]
    public string endSceneName = "End Scene";
    
    [Header("Completion Settings")]
    [Tooltip("Delay before transitioning to end scene (in seconds)")]
    public float transitionDelay = 2f;
    [Tooltip("Show completion message before transitioning")]
    public bool showCompletionMessage = true;
    [Tooltip("Completion message text")]
    public string completionMessage = "Congratulations! All clues have been placed!";
    
    [Header("Debug")]
    public bool enableDebugLogging = true;
    
    // Singleton pattern for easy access
    private static GameCompletionManager _instance;
    public static GameCompletionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameCompletionManager>();
            }
            return _instance;
        }
    }
    
    // Events
    public System.Action OnGameCompleted;
    public System.Action OnAllSlotsFilled;
    
    private List<ClueSlot> allSlots = new List<ClueSlot>();
    public bool gameCompleted = false;
    private bool slotsCacheValid = false;
    
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
        RefreshSlotsCache();
        
        if (enableDebugLogging)
        {
            Debug.Log($"GameCompletionManager: Found {allSlots.Count} clue slots in scene");
        }
    }
    
    /// <summary>
    /// Find all clue slots in the scene
    /// </summary>
    private void FindAllClueSlots()
    {
        allSlots.Clear();
        ClueSlot[] foundSlots = FindObjectsOfType<ClueSlot>();
        allSlots.AddRange(foundSlots);
        
        if (enableDebugLogging)
        {
            Debug.Log($"GameCompletionManager: Found {foundSlots.Length} ClueSlot components");
            foreach (var slot in foundSlots)
            {
                Debug.Log($"  - Slot: {slot.name}, Expected: {slot.GetExpectedClueId()}, Category: {slot.expectedCategory}");
            }
        }
    }
    
    /// <summary>
    /// Refresh the slots cache (call when slots might have changed)
    /// </summary>
    private void RefreshSlotsCache()
    {
        FindAllClueSlots();
        slotsCacheValid = true;
    }
    
    /// <summary>
    /// Ensure slots cache is valid, refresh if needed
    /// </summary>
    private void EnsureSlotsCacheValid()
    {
        if (!slotsCacheValid)
        {
            RefreshSlotsCache();
        }
    }
    
    /// <summary>
    /// Check if all clue slots are correctly filled
    /// </summary>
    /// <returns>True if all slots have correct clues, false otherwise</returns>
    public bool AreAllSlotsCorrectlyFilled()
    {
        EnsureSlotsCacheValid();
        
        if (allSlots.Count == 0)
        {
            if (enableDebugLogging)
            {
                Debug.LogWarning("GameCompletionManager: No clue slots found!");
            }
            return false;
        }
        
        foreach (ClueSlot slot in allSlots)
        {
            if (slot == null) continue;
            
            if (!slot.IsFilled() || !slot.HasCorrectClue())
            {
                if (enableDebugLogging)
                {
                    Debug.Log($"GameCompletionManager: Slot '{slot.name}' is not correctly filled (Filled: {slot.IsFilled()}, Correct: {slot.HasCorrectClue()})");
                }
                return false;
            }
        }
        
        if (enableDebugLogging)
        {
            Debug.Log("GameCompletionManager: All slots are correctly filled!");
        }
        
        return true;
    }
    
    /// <summary>
    /// Check if all clue slots are filled (regardless of correctness)
    /// </summary>
    /// <returns>True if all slots are filled, false otherwise</returns>
    public bool AreAllSlotsFilled()
    {
        EnsureSlotsCacheValid();
        
        if (allSlots.Count == 0) return false;
        
        foreach (ClueSlot slot in allSlots)
        {
            if (slot == null || !slot.IsFilled()) return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Get completion statistics
    /// </summary>
    /// <returns>Completion statistics</returns>
    public (int filled, int correct, int total) GetCompletionStats()
    {
        int filled = 0;
        int correct = 0;
        int total = allSlots.Count;
        
        foreach (ClueSlot slot in allSlots)
        {
            if (slot == null) continue;
            
            if (slot.IsFilled())
            {
                filled++;
                if (slot.HasCorrectClue())
                {
                    correct++;
                }
            }
        }
        
        return (filled, correct, total);
    }
    
    /// <summary>
    /// Called when a clue slot is updated - checks for game completion
    /// </summary>
    public void OnSlotUpdated()
    {
        if (gameCompleted) return;
        
        if (enableDebugLogging)
        {
            var stats = GetCompletionStats();
            Debug.Log($"GameCompletionManager: Slot updated - Filled: {stats.filled}/{stats.total} (Correct: {stats.correct}/{stats.total})");
        }
        
        // Check if all slots are filled (which means they're correct)
        if (AreAllSlotsFilled())
        {
            OnAllSlotsFilled?.Invoke();
            CompleteGame();
        }
    }
    
    /// <summary>
    /// Complete the game and transition to end scene
    /// </summary>
    private void CompleteGame()
    {
        if (gameCompleted) return;
        
        gameCompleted = true;
        
        if (enableDebugLogging)
        {
            Debug.Log("GameCompletionManager: All slots filled! Game completed! Transitioning to end scene...");
        }
        
        // Trigger completion event
        OnGameCompleted?.Invoke();
        
        // Show completion message if enabled
        if (showCompletionMessage)
        {
            ShowCompletionMessage();
        }
        
        // Transition to end scene after delay
        StartCoroutine(TransitionToEndScene());
    }
    
    /// <summary>
    /// Show completion message
    /// </summary>
    private void ShowCompletionMessage()
    {
        // You can customize this to show a UI message, play sound, etc.
        Debug.Log(completionMessage);
        
        // Example: If you have a UI system, you could show a popup here
        // UIManager.Instance.ShowMessage(completionMessage);
    }
    
    /// <summary>
    /// Transition to end scene with delay
    /// </summary>
    private System.Collections.IEnumerator TransitionToEndScene()
    {
        yield return new WaitForSeconds(transitionDelay);
        
        if (enableDebugLogging)
        {
            Debug.Log($"GameCompletionManager: Loading end scene: {endSceneName}");
        }
        
        // Load the end scene
        SceneManager.LoadScene(endSceneName);
    }
    
    /// <summary>
    /// Manually trigger game completion (for testing)
    /// </summary>
    [ContextMenu("Test Complete Game")]
    public void TestCompleteGame()
    {
        if (enableDebugLogging)
        {
            Debug.Log("GameCompletionManager: Manually triggering game completion");
        }
        
        CompleteGame();
    }
    
    /// <summary>
    /// Reset the completion state (useful for restarting)
    /// </summary>
    public void ResetCompletionState()
    {
        gameCompleted = false;
        RefreshSlotsCache();
        
        if (enableDebugLogging)
        {
            Debug.Log("GameCompletionManager: Completion state reset");
        }
    }
    
    /// <summary>
    /// Get debug information about all slots
    /// </summary>
    [ContextMenu("Debug All Slots")]
    public void DebugAllSlots()
    {
        Debug.Log("=== GameCompletionManager Debug ===");
        Debug.Log($"Total slots: {allSlots.Count}");
        Debug.Log($"Game completed: {gameCompleted}");
        
        var stats = GetCompletionStats();
        Debug.Log($"Filled: {stats.filled}/{stats.total}");
        Debug.Log($"Correct: {stats.correct}/{stats.total}");
        Debug.Log($"All filled: {AreAllSlotsFilled()} (Since slots only accept correct clues, filled = correct)");
        
        foreach (var slot in allSlots)
        {
            if (slot == null) continue;
            
            string status = slot.IsFilled() ? (slot.HasCorrectClue() ? "CORRECT" : "INCORRECT") : "EMPTY";
            Debug.Log($"  {slot.name}: {status} (Expected: {slot.GetExpectedClueId()}, Category: {slot.expectedCategory})");
        }
    }
}
