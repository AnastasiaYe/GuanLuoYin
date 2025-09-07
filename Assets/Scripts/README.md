# Rusty Lake Room Management System

This system provides a simple and effective room management solution for your Rusty Lake escape room game. It handles room switching by activating/deactivating GameObjects with manual left/right room assignment.

## Scripts Overview

### 1. ManualRoomManager.cs ⭐ (Main Controller)
The main controller that manages room switching with manual left/right assignment.

**Key Features:**
- Simple GameObject activation/deactivation
- Manual left/right room assignment
- Room locking/unlocking system
- Event system for room changes
- Support for both room indices and names

**Setup:**
1. Add the ManualRoomManager component to a GameObject in your scene
2. Configure rooms in the inspector with their GameObjects
3. Set room names and unlock status
4. Assign which room is "left" and which is "right"

### 2. ManualRoomButton.cs ⭐ (UI Buttons)
Handles simple left/right UI button interactions.

**Key Features:**
- Simple left/right direction buttons
- Automatic button state management (locked/unlocked)
- Visual feedback for different states
- Works with ManualRoomManager

**Setup:**
1. Add the ManualRoomButton component to your UI buttons
2. Set direction to "Left" or "Right"
3. The button will automatically work with ManualRoomManager

## Quick Start Guide

### Basic Setup (2 minutes) ⭐

1. **Create Room GameObjects:**
   ```
   - Create empty GameObjects for each room (e.g., "Room1", "Room2", "Room3")
   - Add all your room content as children of these GameObjects
   - Position them anywhere in the scene
   ```

2. **Add Cameras to Each Room:**
   ```
   - Add Camera component to each room GameObject (or as child)
   - Position cameras in scene view as desired
   - Each room now has its own camera
   ```

3. **Add ManualRoomManager:**
   ```
   - Create empty GameObject named "RoomManager"
   - Add ManualRoomManager component
   - Drag your room GameObjects into the "Rooms" list
   - Set room names and unlock status
   - Assign each room's camera to "Room Camera" field
   - Set Left Room Index and Right Room Index for each room
   ```

4. **Setup UI Buttons:**
   ```
   - Add ManualRoomButton component to your UI buttons
   - Set direction to "Left" or "Right"
   - Buttons will automatically work with ManualRoomManager
   ```

### Key Features

- **Room-specific cameras**: Each room has its own camera that activates/deactivates
- **Manual room assignment**: You control which room is left/right for each room
- **Simple setup**: Just position cameras in scene view like normal objects

## Usage Examples

### Change Room via Script
```csharp
// Get ManualRoomManager reference
ManualRoomManager roomManager = FindObjectOfType<ManualRoomManager>();

// Go to left room
roomManager.GoLeft();

// Go to right room
roomManager.GoRight();

// Change to specific room
roomManager.ChangeRoom(1);
roomManager.ChangeRoom("Room2");

// Unlock a room
roomManager.UnlockRoom("Secret Room");
```

### Listen to Room Events
```csharp
void Start()
{
    ManualRoomManager roomManager = FindObjectOfType<ManualRoomManager>();
    roomManager.OnRoomChanged += OnRoomChanged;
    roomManager.OnRoomChangedByName += OnRoomChangedByName;
}

void OnRoomChanged(int roomIndex)
{
    Debug.Log($"Changed to room {roomIndex}");
}

void OnRoomChangedByName(string roomName)
{
    Debug.Log($"Changed to room {roomName}");
}
```

### Custom Button Behavior
```csharp
public class CustomRoomButton : MonoBehaviour
{
    public ManualRoomManager roomManager;
    
    public void OnCustomButtonClick()
    {
        // Check if player has required items
        if (HasRequiredItems())
        {
            roomManager.GoLeft(); // or GoRight()
        }
        else
        {
            ShowMessage("You need a key to enter this room!");
        }
    }
}
```

## Integration with Existing UI

The system is designed to work with your existing UI setup. Based on your scene, you have:

- **Left Room Button** - Can be connected to ManualRoomButton component (Direction: Left)
- **Right Room Button** - Can be connected to ManualRoomButton component (Direction: Right)
- **Room 1 & Room 2 Objects** - Can be assigned as room objects in ManualRoomManager

## Tips for Rusty Lake Style

1. **Manual Control:** You decide which room is left/right - perfect for escape rooms
2. **Room Locking:** Lock rooms until puzzles are solved
3. **Simple UI:** Just two buttons - Left and Right
4. **Performance:** Only active room objects are rendered, great for complex scenes
5. **Flexible:** Change left/right assignment anytime

## Troubleshooting

**Room buttons not working:**
- Check if ManualRoomManager is in the scene
- Verify left/right room indices are set correctly
- Ensure target room is unlocked

**Room not switching:**
- Check if room GameObject is assigned in ManualRoomManager
- Verify left/right room indices are valid (not out of range)
- Check console for error messages

**Button not responding:**
- Ensure ManualRoomButton component is attached
- Check if button direction is set correctly (Left/Right)
- Verify target room is unlocked

## Support

For questions or issues, check the Unity Console for error messages and ensure all components are properly connected.

## Quick Reference

**Simple Setup:**
1. Create room GameObjects with your content
2. Add Camera component to each room
3. Position cameras in scene view
4. Add ManualRoomManager to a GameObject
5. Assign room GameObjects and cameras to the manager
6. Set Left/Right Room Index for each room
7. Add ManualRoomButton to your UI buttons
8. Set direction to Left/Right and you're done!

**Key Scripts:**
- `ManualRoomManager.cs` - Main room controller with room-specific cameras
- `ManualRoomButton.cs` - Simple left/right button handler
- `ManualRoomSetup.md` - Detailed setup guide
