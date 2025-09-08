using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("Stage Settings")]
    public int currentStage = 0; // 0-4
    public float stageChangeInterval = 60f; // Change stage every 60 seconds
    public bool enableAutoStageChange = true;
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    // Events
    public System.Action<int> OnStageChanged;
    
    // Singleton pattern for easy access
    private static StageManager _instance;
    public static StageManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<StageManager>();
            }
            return _instance;
        }
    }
    
    private float timer = 0f;
    
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
    
    private void Update()
    {
        if (enableAutoStageChange)
        {
            timer += Time.deltaTime;
            
            if (timer >= stageChangeInterval)
            {
                ChangeToNextStage();
                timer = 0f;
            }
        }
    }
    
    #region Stage Management
    
    public void ChangeToNextStage()
    {
        int nextStage = (currentStage + 1) % 5; // 0->1->2->3->4->0
        ChangeStage(nextStage);
    }
    
    public void ChangeStage(int newStage)
    {
        if (newStage < 0 || newStage > 4) return;
        
        currentStage = newStage;
        
        // Trigger event
        OnStageChanged?.Invoke(currentStage);
    }
    
    public void SetStage(int stage)
    {
        ChangeStage(stage);
    }
    
    #endregion
    
    #region Debug Methods
    
    [ContextMenu("Change to Next Stage")]
    public void ChangeToNextStageManual()
    {
        ChangeToNextStage();
    }
    
    [ContextMenu("Reset to Stage 0")]
    public void ResetToStage0()
    {
        ChangeStage(0);
    }
    
    [ContextMenu("Force Stage Change Now")]
    public void ForceStageChangeNow()
    {
        ChangeToNextStage();
        timer = 0f;
    }
    
    #endregion
}
