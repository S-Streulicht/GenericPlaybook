using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using PlasticGui.WorkspaceWindow;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace PB.Book.BookEditor
{
  public class BookEditor : EditorWindow
  {
    #region private Variables
    private Text selectedBook = null;
    private GUIStyle nodeStyle;
    private TextElements dragginNode = null;
    private Vector2 dragStart;
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
        EditorGUILayout.LabelField("");
        ProcessEvents();
        foreach(TextElements node in selectedBook.GetAllNodesReverse()) // TODO Reverse is ininefficent
        {
          OnGuiNode(node);
        }
      }
    }

    private void ProcessEvents()
    {
      if ((EventType.MouseDown == Event.current.type) && (null == dragginNode))
      {
        dragginNode = GetNodeAtPointer(Event.current.mousePosition);
        dragStart = Event.current.mousePosition - dragginNode.area.position;
      }
      else if ((EventType.MouseDrag == Event.current.type) && (null != dragginNode))
      {
        Undo.RecordObject(selectedBook, "Move Text of Node " + dragginNode.uniqueID);
        dragginNode.area.position = Event.current.mousePosition - dragStart;
        GUI.changed = true;
      }
      else if ((EventType.MouseUp == Event.current.type) && (null != dragginNode))
      {
        dragginNode = null;
      }
    }

    private void OnGuiNode(TextElements node)
    {
      GUILayout.BeginArea(node.area, nodeStyle);
      EditorGUILayout.LabelField("Node: " + node.uniqueID);
      EditorGUI.BeginChangeCheck();
      var newText = EditorGUILayout.TextField(node.text);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(selectedBook, "Update of Text in Node " + node.uniqueID);
        node.text = newText;
      }

      foreach(TextElements childNode in selectedBook.GetAllChildren(node))
      {
        EditorGUILayout.LabelField(childNode.uniqueID);
      }
      GUILayout.EndArea();
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