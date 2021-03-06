# RPG

A 3D RPG game with combat, inventories and dialogue.

Following the Udemy/GameDev.tv course 'Unity RPG Series'. (Split into 3 courses: Core Combat, Dialogue & Quests, Inventory Systems)
## Changes - Dialogue
* Initial Commit - including Dialogue scriptable objects and start of Dialogue Editor
* Showing a custom editor window, and opening an asset callback
* Drawing on GUI
* Callback mechanisms
* Adding in default node
* Editing dynamic data
* Implementing Undo
* Styling Nodes
* Dragging Nodes (1) - only `root' node to begin with
* Dragging Nodes (2) - actual node clicked, with offset for mouse posn in node
* Dictionaries for Lookups - and yield return. Putting child node labels on root node
* Drawing Bezier Curves
* Adding Nodes - using buttons on existing nodes & [NonSerialize] to stop odd nodes appearing
* Deleting Nodes - and arranging buttons horizontally
* Linking Nodes - with link button changing to child/cancel/unlink as needed
* Scroll Views - and detecting scrolled mouse positions
* Drag to Scroll
* Canvas Background - and how to tile an image onto the background
* Dialogue Node Scriptable Objects - plus fixing Undo & destroying nodes/links
* Asset Database - and fixing root node creation/Asset Database entry
* Setters, Getters & Undo - restoring Undo function after change to ScriptableObjects + removing bug
* Worked around Sub Asset / Undo / SetDirty bug
* Alternating Speakers - and offsetting new child nodes to right of parent
* Atomic Design - adding Dialogue window to UI
* Mockup to Auto Layout
* Styling UI
* Dialogue Architecture - linking UI to dialogue nodes via PlayerConversant.cs
* Nested ScriptableObject Rename Bug - a work around
* Choosing a Random Response
* Choice UI
* Building a Choice List - with correct version of deleting previous choices
* Choosing State
* Choosing a Node
* Starting a Dialogue
* Quitting a Dialogue - and updated Unity to 2020.2.2f1
* IRaycastable Dialogue - dialogue only starts when suitable NPC clicked on
* Triggering Dialogue Actions
* Dialogue Trigger Components
* Enemy Aggro Groups - and updating Unity to 2020.2.3f1
* Conversant Names
* Quest List UI Outline - error with scrolling correctly to be resolved
* Quest Tooltup Outline - and updating Unity to 2020.2.6f1
