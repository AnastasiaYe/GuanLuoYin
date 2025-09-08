using UnityEngine;
using System.Collections.Generic;

public class ManualRoomManager : MonoBehaviour
{
    [System.Serializable]
    public class Room
    {
        public string roomName;
        public GameObject roomObject;
        public bool isUnlocked = true;
        public int leftRoomIndex = -1; // Index of the room to the left
        public int rightRoomIndex = -1; // Index of the room to the right
        
        [Header("Room Camera")]
        public Camera roomCamera; // Camera for this specific room
    }

    [Header("Room Configuration")]
    public List<Room> rooms = new List<Room>();

    [Header("Camera Settings")]
    public Camera mainCamera; // Single camera that moves between rooms

    [Header("Current State")]
    public int currentRoomIndex = 0;

    // Events
    public System.Action<int> OnRoomChanged;
    public System.Action<string> OnRoomChangedByName;

    // Reference to buttons for refreshing their state
    private ManualRoomButton[] roomButtons;

    private void Start()
    {
        // Find all room buttons
        roomButtons = FindObjectsOfType<ManualRoomButton>();
        
        // Setup camera
        SetupCamera();
        
        // Initialize the first room
        if (rooms.Count > 0)
        {
            SetRoomActive(0);
        }
    }
    
    private void SetupCamera()
    {
        // Camera must be assigned in inspector
    }

    public void GoLeft()
    {
        if (currentRoomIndex < 0 || currentRoomIndex >= rooms.Count)
        {
            return;
        }

        var currentRoom = rooms[currentRoomIndex];
        int leftIndex = currentRoom.leftRoomIndex;

        if (leftIndex >= 0 && leftIndex < rooms.Count)
        {
            ChangeRoom(leftIndex);
        }
    }

    public void GoRight()
    {
        if (currentRoomIndex < 0 || currentRoomIndex >= rooms.Count)
        {
            return;
        }

        var currentRoom = rooms[currentRoomIndex];
        int rightIndex = currentRoom.rightRoomIndex;

        if (rightIndex >= 0 && rightIndex < rooms.Count)
        {
            ChangeRoom(rightIndex);
        }
    }

    public void ChangeRoom(int roomIndex)
    {
        if (roomIndex < 0 || roomIndex >= rooms.Count)
            return;

        if (!rooms[roomIndex].isUnlocked)
            return;

        // Simply activate/deactivate rooms
        SetRoomActive(roomIndex);
        currentRoomIndex = roomIndex;

        OnRoomChanged?.Invoke(roomIndex);
        OnRoomChangedByName?.Invoke(rooms[roomIndex].roomName);
        
        // Refresh button states
        RefreshButtonStates();
    }

    public void ChangeRoom(string roomName)
    {
        int roomIndex = rooms.FindIndex(r => r.roomName == roomName);
        if (roomIndex != -1)
        {
            ChangeRoom(roomIndex);
        }
    }

    private void SetRoomActive(int roomIndex)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            bool shouldBeActive = (i == roomIndex);
            
            // Activate/deactivate room object
            if (rooms[i].roomObject != null)
            {
                rooms[i].roomObject.SetActive(shouldBeActive);
            }
            
            // Deactivate all room cameras (we use the main camera instead)
            if (rooms[i].roomCamera != null)
            {
                rooms[i].roomCamera.gameObject.SetActive(false);
            }
        }
        
        // Move main camera to match the active room's camera position
        if (mainCamera != null && rooms[roomIndex].roomCamera != null)
        {
            Transform roomCameraTransform = rooms[roomIndex].roomCamera.transform;
            mainCamera.transform.position = roomCameraTransform.position;
            mainCamera.transform.rotation = roomCameraTransform.rotation;
            
            // Copy camera settings
            mainCamera.fieldOfView = rooms[roomIndex].roomCamera.fieldOfView;
            mainCamera.nearClipPlane = rooms[roomIndex].roomCamera.nearClipPlane;
            mainCamera.farClipPlane = rooms[roomIndex].roomCamera.farClipPlane;
        }
    }


    public void UnlockRoom(int roomIndex)
    {
        if (roomIndex >= 0 && roomIndex < rooms.Count)
        {
            rooms[roomIndex].isUnlocked = true;
        }
    }

    public void UnlockRoom(string roomName)
    {
        int roomIndex = rooms.FindIndex(r => r.roomName == roomName);
        if (roomIndex != -1)
        {
            UnlockRoom(roomIndex);
        }
    }

    public void LockRoom(int roomIndex)
    {
        if (roomIndex >= 0 && roomIndex < rooms.Count)
        {
            rooms[roomIndex].isUnlocked = false;
        }
    }

    public Room GetCurrentRoom()
    {
        if (currentRoomIndex >= 0 && currentRoomIndex < rooms.Count)
        {
            return rooms[currentRoomIndex];
        }
        return null;
    }

    public Room GetRoom(int index)
    {
        if (index >= 0 && index < rooms.Count)
        {
            return rooms[index];
        }
        return null;
    }

    public Room GetRoom(string roomName)
    {
        return rooms.Find(r => r.roomName == roomName);
    }

    // For debugging in inspector
    [ContextMenu("Go Left")]
    public void GoLeftManual()
    {
        GoLeft();
    }

    [ContextMenu("Go Right")]
    public void GoRightManual()
    {
        GoRight();
    }

    private void RefreshButtonStates()
    {
        if (roomButtons != null)
        {
            foreach (var button in roomButtons)
            {
                if (button != null)
                {
                    button.RefreshButtonState();
                }
            }
        }
    }

    // Method to manually set the camera
    public void SetCamera(Camera camera)
    {
        mainCamera = camera;
    }
    
    // Method to get the current camera
    public Camera GetCurrentCamera()
    {
        return mainCamera;
    }

    // Validation
    private void OnValidate()
    {
        // Ensure room indices are valid
        foreach (var room in rooms)
        {
            if (room.leftRoomIndex >= rooms.Count)
                room.leftRoomIndex = -1;
            if (room.rightRoomIndex >= rooms.Count)
                room.rightRoomIndex = -1;
            if (room.leftRoomIndex < -1)
                room.leftRoomIndex = -1;
            if (room.rightRoomIndex < -1)
                room.rightRoomIndex = -1;
        }
    }
}
