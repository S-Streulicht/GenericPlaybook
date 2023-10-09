using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using PlasticGui.WorkspaceWindow;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.VersionControl;
using UnityEngine;

namespace PB.Book.BookEditor
{
  public class BookEditor : EditorWindow
  {
    #region Variables
                    Text selectedBook = null;
    [NonSerialized] private GUIStyle nodeStyle;
    [NonSerialized] private TextElements dragginNode = null;
    [NonSerialized] private Vector2 dragStart;
    [NonSerialized] private TextElements creationNode = null;
    [NonSerialized] private TextElements deletionNode = null;
    [NonSerialized] private TextElements linkingParentNode = null;

                    Vector2 scrollPosition;
    [NonSerialized] private Vector2 draggingCanvasOffset;
    [NonSerialized] private bool draggingCanvas = false;
    [NonSerialized] private Vector2 CanvasSize = new Vector2(4000, 4000);

    #endregion
    
    #region Editor Handels    
    [MenuItem("Window/Book Editor")]
    public static void ShowEditorWindow()
    {
      GetWindow(typeof(BookEditor), false, "Book Editor");
    }

    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    private void OnGUI()
    {
      if (null == selectedBook)
      {
        EditorGUILayout.LabelField("No Book selected.");
      }
      else
      {
        EditorGUILayout.LabelField(selectedBook.name);
        if (GUILayout.Button("Save"))
        {
          EditorUtility.SetDirty(selectedBook);
          AssetDatabase.SaveAssets();
          AssetDatabase.Refresh();
          Debug.Log("Create nw node");
        }
        ProcessEvents();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        Rect canvas = GUILayoutUtility.GetRect(CanvasSize.x, CanvasSize.y);
        Texture2D backGroundTextur = Resources.Load("background") as Texture2D;
        Rect textureCoordinates = new Rect(0, 0, CanvasSize.x / backGroundTextur.width, CanvasSize.y / backGroundTextur.height);
        GUI.DrawTextureWithTexCoords(canvas, backGroundTextur, textureCoordinates);

        foreach(TextElements node in selectedBook.GetAllNodesReverse())
        {
          DrawConnections(node);
        }
        foreach(TextElements node in selectedBook.GetAllNodesReverse())
        {
          DrawNode(node);
        }

        EditorGUILayout.EndScrollView();

        if (null != creationNode)
        {
          Undo.RecordObject(selectedBook, "Added Node to " + creationNode.textNumber);
          selectedBook.CreateNode(creationNode);
          creationNode = null;
        }
        if (null != deletionNode)
        {
          Undo.RecordObject(selectedBook, "Remove Node " + deletionNode.textNumber);
          selectedBook.DeleteNode(deletionNode);
          deletionNode = null;
        }
      }
    }

    private void ProcessEvents()
    {
      if ((EventType.MouseDown == Event.current.type) && (null == dragginNode))
      {
        dragginNode = GetNodeAtPointer(Event.current.mousePosition + scrollPosition);
        if(null != dragginNode)
        {
          dragStart = Event.current.mousePosition - dragginNode.area.position;
        }
        else
        {
          draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
          draggingCanvas = true;
        }
      }
      else if ((EventType.MouseDrag == Event.current.type) && (true == draggingCanvas))
      {
        scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
        GUI.changed = true;
      }
      else if ((EventType.MouseDrag == Event.current.type) && (null != dragginNode))
      {
        Undo.RecordObject(selectedBook, "Move Text of Node " + dragginNode.textNumber);
        dragginNode.area.position = Event.current.mousePosition - dragStart;
        GUI.changed = true;
      }
      else if ((EventType.MouseUp == Event.current.type) && (null != dragginNode))
      {
        dragginNode = null;
      }
      else if ((EventType.MouseUp == Event.current.type) && (true == draggingCanvas))
      {
        draggingCanvas = false;
      }
    }

    private void DrawNode(TextElements node)
    {
      GUILayout.BeginArea(node.area, nodeStyle);
      EditorGUILayout.LabelField("Node: " + node.textNumber);
      EditorGUI.BeginChangeCheck();
      var newText = EditorGUILayout.TextField(node.text);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(selectedBook, "Update of Text in Node " + node.textNumber);
        node.text = newText;
      }

      GUILayout.BeginHorizontal();

      if (GUILayout.Button("x"))
      {
        deletionNode = node;
      }
      DrawLinkButtons(node);

      if (GUILayout.Button("+"))
      {
        creationNode = node;
      }

      GUILayout.EndHorizontal();

      GUILayout.EndArea();
    }

    private void DrawLinkButtons(TextElements node)
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
      else
      {
        if (-1 < linkingParentNode.jumpTos.FindIndex(x => x.referenceId == node.uniqueID))
        {
          if (GUILayout.Button("unlink"))
          {
            Undo.RecordObject(selectedBook, "delete link between " + linkingParentNode.textNumber + " and " + node.textNumber);
            selectedBook.DeleteLink(linkingParentNode, node.uniqueID);
            linkingParentNode = null;
          }
        }
        else
        {
          if (GUILayout.Button("child"))
          {
            Undo.RecordObject(selectedBook, "create link between " + linkingParentNode.textNumber + " and " + node.textNumber);
            selectedBook.CreateLink(linkingParentNode, node.uniqueID);
            linkingParentNode = null;
          }
        }
      }
    }

    private void DrawConnections(TextElements parent)
    {
      Vector3 startPosition = new Vector2(parent.area.xMax, parent.area.center.y);
      foreach (TextElements childeNode in selectedBook.GetAllChildren(parent))
      {
        Vector3 endPosition = new Vector2(childeNode.area.xMin, childeNode.area.center.y);
        float distance = Vector3.Distance(endPosition, startPosition);
        Vector3 tangentOffset = new Vector2(distance * 0.5f, 0.0f);
        Handles.DrawBezier(startPosition, endPosition,
          startPosition + tangentOffset, endPosition - tangentOffset,
          Color.white, null, 4f);
      }
    }

    private TextElements GetNodeAtPointer(Vector2 point)
    {
      foreach (TextElements node in selectedBook.GetAllNodes())
      {
        if (node.area.Contains(point))
        {
          return node;
        }
      }
      return null;
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
      SetNodeStyle();

      Selection.selectionChanged += OnSelectionChanges;
      OnSelectionChanges();
    }

    private void SetNodeStyle()
    {
      nodeStyle = new GUIStyle();
      nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
      nodeStyle.padding = new RectOffset(10, 10, 10, 10);
      nodeStyle.border = new RectOffset(12, 12, 12, 12);
    }
    #endregion

    #region Callbacks 
    [OnOpenAssetAttribute(1)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
      Text book = EditorUtility.InstanceIDToObject(instanceID) as Text;
      if(null != book)
      {
        ShowEditorWindow();
        return true;
      }
      return false;
    }

    private void OnSelectionChanges()
    {
      Text newBook = Selection.activeObject as Text;
      if(null != newBook)
      {
        selectedBook = newBook;
        Repaint();
      }
    }
    #endregion
  }
}