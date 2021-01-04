﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue = null;
        [NonSerialized]
        GUIStyle nodeStyle;
        [NonSerialized]
        DialogueNode draggingNode = null;
        [NonSerialized]
        Vector2 draggingOffset;
        [NonSerialized]
        DialogueNode creatingNode = null;
        [NonSerialized]
        DialogueNode deletingNode = null;
        [NonSerialized]
        DialogueNode linkingParentNode = null;
        Vector2 scrollPosition;
        [NonSerialized]
        bool draggingCanvas = false;
        [NonSerialized]
        Vector2 draggingCanvasOffset;

        const float canvasSize = 4000;
        const float backgroundSize = 50;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue != null)
            {
                ShowEditorWindow();
                return true;
            }
            return false;
        }

        private void OnEnable() 
        {
            Selection.selectionChanged += OnSelectionChanged;   // C# event. No () or fn called immediately

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D; //blue nodes
            //nodeStyle.normal.textColor = Color.magenta;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChanged() 
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI() 
        {
            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue selected");
            }
            else
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);
                Texture2D backgroundTex = Resources.Load("background") as Texture2D;
                Rect texCoords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, texCoords);

                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }
                EditorGUILayout.EndScrollView();

                if (creatingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Added Dialogue Node");
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }
                if (deletingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Deleted Dialogue Node");
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if (draggingNode != null)
                {
                    draggingOffset = draggingNode.rect.position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                Undo.RecordObject(selectedDialogue, "Move Dialogue Node");
                draggingNode.rect.position = Event.current.mousePosition + draggingOffset;

                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;

                GUI.changed = true;
            }

            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.rect, nodeStyle);
            EditorGUI.BeginChangeCheck();

            string newText = EditorGUILayout.TextField(node.text);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Update Dialogue Text");
                node.text = newText;
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("-"))
            {
                deletingNode = node;
            }

            DrawLinkButtons(node);

            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    linkingParentNode = node;
                }
            }
            else if (linkingParentNode == node)
            {
                if (GUILayout.Button("cancel"))
                {
                    linkingParentNode = null;
                }
            }
            else if (linkingParentNode.children.Contains(node.name))
            {
                if (GUILayout.Button("unlink"))
                {
                    Undo.RecordObject(selectedDialogue, "Remove Dialogue Link");
                    linkingParentNode.children.Remove(node.name);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("child"))
                {
                    Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                    linkingParentNode.children.Add(node.name);
                    linkingParentNode = null;
                }
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.rect.xMax, node.rect.center.y);
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector2(childNode.rect.xMin, childNode.rect.center.y);
                Vector3 controlPointOffset = endPosition - startPosition; //curve depends on proximity
                controlPointOffset.y = 0;                                 //no y component to curve
                controlPointOffset.x *= 0.5f;                             //50% of x separation
                Handles.DrawBezier(
                    startPosition, endPosition, 
                    startPosition + controlPointOffset, 
                    endPosition - controlPointOffset, 
                    Color.white, null, 4f);
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode currentNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.rect.Contains(point))
                {
                    currentNode = node;             // selects the top most node
                }
            }
            return currentNode;
        }
    }
}