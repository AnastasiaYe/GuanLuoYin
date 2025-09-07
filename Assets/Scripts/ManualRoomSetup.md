# Manual Room Management System

This system allows each room to store its own left and right room references. When you hit a button, it goes to the left or right room of the current room.

## Quick Setup (2 minutes)

### 1. Create Room GameObjects
- Create empty GameObjects for each room (e.g., "Room1", "Room2", "Room3")
- Add all your room content as children of these GameObjects
- Position them anywhere in the scene

### 2. Add Cameras to Each Room
- Add a Camera component to each room GameObject (or as a child)
- Position and rotate each camera as desired for that room
- Each room now has its own camera

### 3. Add ManualRoomManager
- Create empty GameObject named "RoomManager"
- Add `ManualRoomManager` component
- Drag your room GameObjects into the "Rooms" list
- Set room names and unlock status
- For each room, assign its camera to the "Room Camera" field

### 4. Set Left/Right Room Assignment for Each Room
- For each room in the list, set:
  - **Left Room Index**: Which room index should be to the left of this room (-1 for none)
  - **Right Room Index**: Which room index should be to the right of this room (-1 for none)

### 5. Setup UI Buttons
- Add `ManualRoomButton` component to your UI buttons
- Set direction to "Left" or "Right"
- Buttons will automatically work with the manager

## Example Setup

```
Scene Hierarchy:
├── RoomManager (ManualRoomManager)
│   └── Rooms List:
│       ├── [0] Room1 (GameObject)
│       │   ├── Left Room Index: 2
│       │   └── Right Room Index: 1
│       ├── [1] Room2 (GameObject)
│       │   ├── Left Room Index: 0
│       │   └── Right Room Index: 2
│       └── [2] Room3 (GameObject)
│           ├── Left Room Index: 1
│           └── Right Room Index: 0
├── Room1 (GameObject - your first room content)
├── Room2 (GameObject - your second room content)
├── Room3 (GameObject - your third room content)
└── UI Canvas
    ├── Left Button (ManualRoomButton → Direction: Left)
    └── Right Button (ManualRoomButton → Direction: Right)
```

## Room Camera Setup

**Simple Method:**
1. Add a Camera component to each room GameObject (or as a child)
2. Position and rotate each camera in the scene view
3. Assign each camera to the "Room Camera" field in ManualRoomManager
4. The system will automatically activate/deactivate the correct camera

**Benefits:**
- Each room can have different camera settings
- No complex camera movement calculations
- Easy to position cameras in scene view
- Each room can have unique visual effects

## ManualRoomManager Configuration

In the inspector:
- **Rooms**: List of all your room GameObjects
- For each room, set:
  - **Left Room Index**: Which room is to the left (-1 for none)
  - **Right Room Index**: Which room is to the right (-1 for none)
  - **Room Camera**: Assign the camera for this specific room

## ManualRoomButton Configuration

In the inspector:
- **Direction**: Choose "Left" or "Right"
- **Button Text**: Will automatically show "Left" or "Right"
- **Visual Feedback**: Shows disabled state if target room is locked

## Usage

### Change Room via Script
```csharp
ManualRoomManager roomManager = FindObjectOfType<ManualRoomManager>();

// Go to left room
roomManager.GoLeft();

// Go to right room
roomManager.GoRight();

// Change to specific room
roomManager.ChangeRoom(1);
roomManager.ChangeRoom("Room2");
```

### Listen to Room Changes
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

## Key Features

- ✅ **Manual Assignment** - You choose which room is left/right
- ✅ **Simple UI** - Just two buttons: Left and Right
- ✅ **Visual Feedback** - Buttons show disabled state when rooms are locked
- ✅ **Easy Setup** - Just assign room indices in inspector
- ✅ **Flexible** - Can change left/right assignment anytime

## Tips

1. **Room Order**: The room indices (0, 1, 2...) don't have to match left/right order
2. **Multiple Rooms**: You can have more than 2 rooms, but only 2 are assigned as left/right
3. **Dynamic Assignment**: You can change left/right room assignment at runtime
4. **Locking**: Lock rooms to prevent access until conditions are met

## Troubleshooting

**Button not working:**
- Check if ManualRoomManager is in the scene
- Verify left/right room indices are set correctly
- Ensure target room is unlocked

**Wrong room switching:**
- Check left/right room index assignments in inspector
- Verify room indices match your room list order

**Button shows disabled:**
- Check if target room is unlocked
- Verify room index is valid (not out of range)
