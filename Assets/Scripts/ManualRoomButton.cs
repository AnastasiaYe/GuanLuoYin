using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManualRoomButton : MonoBehaviour
{
    public enum Direction
    {
        Left,
        Right
    }

    [Header("Button Configuration")]
    public Direction direction = Direction.Left;
    
    [Header("UI References")]
    public Button button;
    public TextMeshProUGUI buttonText;
    
    [Header("Visual Feedback")]
    public Color normalColor = Color.white;
    public Color lockedColor = Color.gray;
    public Color hoverColor = Color.yellow;
    public Color disabledColor = Color.red;
    
    private ManualRoomManager roomManager;
    private bool isLocked = false;

    private void Start()
    {
        // Get ManualRoomManager reference
        roomManager = FindObjectOfType<ManualRoomManager>();

        // Get button component if not assigned
        if (button == null)
            button = GetComponent<Button>();

        // Get text component if not assigned
        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();

        // Set up button click event
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }

        // Set button text based on direction
        if (buttonText != null)
        {
            buttonText.text = direction.ToString();
        }

        // Update button state
        UpdateButtonState();
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }

    public void OnButtonClicked()
    {
        if (roomManager == null || isLocked)
            return;

        // Call the appropriate direction method
        if (direction == Direction.Left)
        {
            roomManager.GoLeft();
        }
        else
        {
            roomManager.GoRight();
        }
    }

    public void SetDirection(Direction newDirection)
    {
        direction = newDirection;
        if (buttonText != null)
        {
            buttonText.text = direction.ToString();
        }
        UpdateButtonState();
    }

    public void SetLocked(bool locked)
    {
        isLocked = locked;
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (button == null) return;

        // Check if the target room exists and is unlocked
        bool hasValidRoom = CheckIfValidRoomInDirection();
        bool shouldBeLocked = isLocked || !hasValidRoom;
        
        // Always keep button interactable - let the manager handle the logic
        button.interactable = true;

        // Update visual appearance
        if (buttonText != null)
        {
            if (!hasValidRoom)
            {
                buttonText.color = disabledColor;
            }
            else if (shouldBeLocked)
            {
                buttonText.color = lockedColor;
            }
            else
            {
                buttonText.color = normalColor;
            }
        }

        // Update button colors
        var colors = button.colors;
        if (!hasValidRoom)
        {
            colors.normalColor = disabledColor;
            colors.highlightedColor = disabledColor;
        }
        else if (shouldBeLocked)
        {
            colors.normalColor = lockedColor;
            colors.highlightedColor = lockedColor;
        }
        else
        {
            colors.normalColor = normalColor;
            colors.highlightedColor = hoverColor;
        }
        button.colors = colors;
    }

    private bool CheckIfValidRoomInDirection()
    {
        if (roomManager == null) return false;

        var currentRoom = roomManager.GetCurrentRoom();
        if (currentRoom == null) return false;

        // Check if the target room index is valid and room is unlocked
        if (direction == Direction.Left)
        {
            int leftIndex = currentRoom.leftRoomIndex;
            if (leftIndex >= 0 && leftIndex < roomManager.rooms.Count)
            {
                return roomManager.rooms[leftIndex].isUnlocked;
            }
        }
        else
        {
            int rightIndex = currentRoom.rightRoomIndex;
            if (rightIndex >= 0 && rightIndex < roomManager.rooms.Count)
            {
                return roomManager.rooms[rightIndex].isUnlocked;
            }
        }

        return false;
    }

    // Method to refresh button state (useful when room states change)
    public void RefreshButtonState()
    {
        UpdateButtonState();
    }

    // For debugging in inspector
    [ContextMenu("Test Button Click")]
    public void TestButtonClick()
    {
        OnButtonClicked();
    }
}
