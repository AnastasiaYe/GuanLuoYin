# Stage-Based Item System Setup Guide

This guide explains how to set up and use the new stage-based item system that allows items to exist only in certain stages.

## Overview

The stage-based item system consists of:
- **ItemManager**: Manages all items and their stage availability
- **ItemData**: Data structure containing item information and stage availability
- **ItemBehaviour**: Enhanced component for individual items with stage management

## Quick Setup (3 minutes)

### 1. Setup Item Manager
- Create empty GameObject named "ItemManager"
- Add `ItemManager` component
- Enable `Show Debug Info` if you want debug logging

### 2. Setup Items
- For each item you want to manage:
  - Add `ItemBehaviour` component
  - Enable `Use Item Manager` (default: true)
  - Set `Available Stages` to specify which stages (0-4) the item should appear in
  - Configure other item properties (name, description, etc.)

### 3. Test the System
- Items will automatically show/hide based on the current stage
- Items will only be interactable in their specified stages
- Use the StageManager to change stages and see items appear/disappear

## Stage System

The game uses 5 stages (0-4):
- **Stage 0** - Default stage
- **Stage 1** - Second stage  
- **Stage 2** - Third stage
- **Stage 3** - Fourth stage
- **Stage 4** - Fifth stage

Items can be configured to appear in any combination of these stages.

## ItemManager Features

### Automatic Management
- Items with `ItemBehaviour` and `Use Item Manager = true` are automatically registered
- Items show/hide automatically when stages change
- Items are only interactable in their specified stages

### Manual Management
```csharp
// Get the item manager
ItemManager itemManager = ItemManager.Instance;

// Add an item manually
ItemData itemData = new ItemData(gameObject);
itemData.SetAvailableStages(new int[] { 0, 2, 4 });
itemManager.AddItem(itemData);

// Get items in specific stage
List<ItemData> stage2Items = itemManager.GetItemsInStage(2);

// Get all visible items
List<ItemData> visibleItems = itemManager.GetVisibleItems();
```

### Debug Methods
```csharp
// Debug all item states
itemManager.DebugItemStates();

// Show all items (for testing)
itemManager.ShowAllItems();

// Hide all items (for testing)
itemManager.HideAllItems();

// Force update items for current stage
itemManager.ForceUpdateItemsForCurrentStage();
```

## ItemBehaviour Features

### Stage Management
- **Available Stages**: List of stages (0-4) where the item should be visible
- **Use Item Manager**: Enable/disable automatic stage management
- **Stage Based Interactivity**: Items are only interactable in their specified stages

### Manual Control
```csharp
// Get item behaviour component
ItemBehaviour itemBehaviour = GetComponent<ItemBehaviour>();

// Set which stages this item should be available in
itemBehaviour.SetAvailableStages(new int[] { 1, 3 });

// Add a stage
itemBehaviour.AddAvailableStage(2);

// Remove a stage
itemBehaviour.RemoveAvailableStage(1);

// Check if item is available in current stage
bool isAvailable = itemBehaviour.IsAvailableInCurrentStage();
```

## Common Use Cases

### Item Only in Stage 0
```csharp
// In ItemBehaviour inspector, set Available Stages to: [0]
// Or in code:
itemBehaviour.SetAvailableStages(new int[] { 0 });
```

### Item in Multiple Stages
```csharp
// Item appears in stages 1, 2, and 3
itemBehaviour.SetAvailableStages(new int[] { 1, 2, 3 });
```

### Item in All Stages
```csharp
// Item appears in all stages
itemBehaviour.SetAvailableStages(new int[] { 0, 1, 2, 3, 4 });
```

### Progressive Item Unlocking
```csharp
// Start with item only in stage 0
itemBehaviour.SetAvailableStages(new int[] { 0 });

// Later, unlock for more stages
itemBehaviour.AddAvailableStage(1);
itemBehaviour.AddAvailableStage(2);
```

## Integration with Existing Systems

### ClueGiver Integration
Items with `ClueGiver` components will only grant clues when they're interactable (in their specified stages).

### ItemInteraction Integration
The existing `ItemInteraction` system works seamlessly with the new stage system. Items that aren't in the current stage won't be interactable.

## Debugging

### Enable Debug Logging
1. Set `Show Debug Info = true` on the ItemManager
2. Watch the console for item visibility changes
3. Use context menu options on ItemManager for testing

### Common Issues

#### Items Not Disappearing
If items are not disappearing when stages change:

1. **Check ItemManager exists**: Make sure you have an ItemManager GameObject in your scene
2. **Enable debug logging**: Set `Show Debug Info = true` on the ItemManager to see what's happening
3. **Verify item registration**: Use the `ItemManagerDebugger` script to check if items are being registered
4. **Check Available Stages**: Ensure items have specific stages set (not empty)
5. **Manual registration**: Right-click on ItemManager → "Register All Items in Scene"

#### Debugging Steps
1. Add `ItemManagerDebugger` script to a GameObject in your scene
2. Right-click on the ItemManagerDebugger → "Debug ItemManager Status"
3. This will show you:
   - If ItemManager and StageManager exist
   - How many items are managed
   - Current stage
   - All ItemBehaviour components and their settings

#### Common Issues
- **Item not appearing**: Check if `Use Item Manager` is enabled and `Available Stages` is set
- **Item always visible**: Check if ItemManager is missing or not working
- **Item not interactable**: Verify the item is in the current stage's available stages
- **Items not disappearing**: Use the debugging steps above to identify the issue

## Performance Notes

- The system is optimized for runtime performance
- Item visibility changes are batched during stage transitions
- No performance impact when items aren't changing stages

## Migration from Old System

If you have existing items with `ItemBehaviour`:
1. Add `ItemManager` to your scene
2. Existing items will automatically be managed (if `Use Item Manager` is true)
3. Items without `Available Stages` set will default to stage 0
4. No other changes needed - existing functionality is preserved

This system provides a clean, efficient way to manage item availability across different stages while maintaining compatibility with your existing item interaction system.
