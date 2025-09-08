# Simple Character Control System

A simplified character management system that stores characters as GameObjects with basic states and room positioning, controlled by a central StageManager.

## Quick Setup (2 minutes)

### 1. Setup Stage Manager
- Create empty GameObject named "StageManager"
- Add `StageManager` component
- Set `Stage Change Interval` to desired seconds (default: 60)
- Enable/disable `Enable Auto Stage Change` as needed
- Set `Current Stage` to starting stage (0-4)

### 2. Setup Character Manager
- Create empty GameObject named "CharacterManager"
- Add `CharacterManager` component

### 3. Create Character GameObjects
- Create GameObjects for each character
- Add `CharacterBehaviour` component to each
- Add `CharacterStatePositions` component to each
- Manually set character data in the inspector (no defaults)

### 4. Setup Characters
- Assign character prefabs in the CharacterManager
- Set up character positions for each state in the CharacterStatePositions component

### 5. Setup Item Interaction
- Create empty GameObject named "ItemInteraction"
- Add `ItemInteraction` component
- Set `Interactable Layer` to your interactable layer
- Add `ItemBehaviour` component to any interactable items
- Make sure items have colliders for raycast detection

## Stage System

5 numeric stages (0-4):
- **Stage 0** - Default stage
- **Stage 1** - Second stage
- **Stage 2** - Third stage
- **Stage 3** - Fourth stage
- **Stage 4** - Fifth stage

## Basic Usage

### Automatic Stage Changes
- Stages automatically cycle every 60 seconds (configurable)
- Stage progression: Stage0 → Stage1 → Stage2 → Stage3 → Stage4 → Stage0
- Can be enabled/disabled in the StageManager inspector
- All characters automatically change to match the current stage

### Manual Stage Control
```csharp
// Get the stage manager
StageManager stageManager = StageManager.Instance;

// Change to specific stage
stageManager.SetStage(2);

// Change to next stage
stageManager.ChangeToNextStage();

// Get current stage
int currentStage = stageManager.currentStage;
```

### Manual Character Control
```csharp
// Get the character manager
CharacterManager charManager = CharacterManager.Instance;

// Get character and change state directly
var character = charManager.GetCharacter("Alice");
if (character != null)
{
    character.ChangeState(2);
    character.MoveToRoom(1);
}
```

### Room Management
```csharp
// Move character to room
var character = charManager.GetCharacter("Alice");
if (character != null)
{
    character.MoveToRoom(1);
}

// Get characters in specific room
var charactersInRoom = charManager.GetCharactersInRoom(1);
```

## Character Data Setup

### CharacterData (in CharacterBehaviour):
- **Character Name** - Name of the character
- **Character Prefab** - Reference to the character GameObject
- **Current State** - Current state (0-4)
- **Current Room Index** - Which room the character is in

### CharacterStatePositions (on each character):
- **State 0 Position** - Where character appears in state 0
- **State 1 Position** - Where character appears in state 1
- **State 2 Position** - Where character appears in state 2
- **State 3 Position** - Where character appears in state 3
- **State 4 Position** - Where character appears in state 4

## Item Interaction

### Basic Item Interaction
- **Click to interact** - Click on items in the interactable layer
- **Debug logging** - Shows which item was clicked
- **Layer-based** - Only items on the specified layer are interactable
- **Collider-based** - Items need colliders for raycast detection
- **Cooldown system** - Prevents rapid clicking

### Setup Item Interaction
1. **Create ItemInteraction GameObject** - Add `ItemInteraction` component
2. **Set Interactable Layer** - Choose which layer contains interactable items
3. **Add ItemBehaviour to items** - Add `ItemBehaviour` component to any item you want to interact with
4. **Add colliders to items** - Items need colliders for raycast detection
5. **Set item properties** - Configure item name and description

### ItemBehaviour Properties
- **Item Name** - Name of the item
- **Item Description** - Description of the item
- **Is Interactable** - Enable/disable interaction
- **Interaction Cooldown** - Prevent rapid clicking (default: 0.5s)
- **Show Debug Info** - Enable debug logging

## Events

### Character Events
```csharp
// Subscribe to character state changes
var character = charManager.GetCharacter("Alice");
if (character != null)
{
    character.OnStateChanged += (charData) => {
        Debug.Log($"{charData.characterName} changed to {charData.currentState}");
    };
    
    character.OnMoved += (charData) => {
        Debug.Log($"{charData.characterName} moved to room {charData.currentRoomIndex}");
    };
}
```

### Item Events
```csharp
// Subscribe to item interactions
var item = GetComponent<ItemBehaviour>();
if (item != null)
{
    item.OnItemInteracted += (itemBehaviour) => {
        Debug.Log($"Item {itemBehaviour.itemName} was interacted with");
    };
}
```

## Debug Features

```csharp
// Debug all character states
charManager.DebugCharacterStates();

// Reset all characters to stage 0
charManager.ResetAllCharactersToStage0();

// Force stage change now (for testing)
stageManager.ForceStageChangeNow();
```

## StageManager Settings

- **Current Stage** - Current stage (0-4) - can be changed in editor
- **Stage Change Interval** - How often to change stages (in seconds, default: 60)
- **Enable Auto Stage Change** - Toggle automatic stage cycling on/off
- **Show Debug Info** - Enable debug logging for stage changes

## CharacterManager Settings

- **Show Debug Info** - Enable debug logging for character changes

This simplified system provides just the essential functionality you need without complex features or predefined states.
