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
  /**
  *  @brief     THe editor to modify a book
  *  @details   only used in the editor time. by the placement in the Editor folder it does not get compiled into the final game. 
  */
  public class BookEditor : EditorWindow
  {
    #region Variables
                    Text selectedBook = null;                              /**< stores the reference to the book wich needs to be modified */
    [NonSerialized] private GUIStyle nodeStyle;                            /**< the style the nodes are looking */
    [NonSerialized] private TextElements dragginNode = null;               /**< information if the editor is in dragging mode */
    [NonSerialized] private Vector2 dragStart;                             /**< the starting point of an dragg event */
    [NonSerialized] private TextElements creationNode = null;              /**< the mode to be created */
    [NonSerialized] private TextElements deletionNode = null;              /**< the node to be deleted */
    [NonSerialized] private TextElements linkingParentNode = null;         /**< to whcih parent does the link process link */

                    Vector2 scrollPosition;                                /**< scroll position of the canvas, not the zoom level */
    [NonSerialized] private Vector2 draggingCanvasOffset;                  /**< from wich position is the canvas moved */
    [NonSerialized] private bool draggingCanvas = false;                   /**< is the canvas currently in the draggin state */
    [NonSerialized] private Vector2 CanvasSize = new Vector2(4000, 4000);  /**< total size of the canvas */

    #endregion
    
    #region Editor Handels    
    [MenuItem("Window/Book Editor")]
    /**
    *  @brief   opens the book editor
    *  @details set also an entry in the menue
    */
    public static void ShowEditorWindow()
    {
      GetWindow(typeof(BookEditor), false, "Book Editor");

    }

    /**
    *  @brief   sets the GIU to be flaged as changes.
    *  @details used as an callback
    */
    static void UndoRedoAction()
    {
      GUI.changed = true;
    }

    /**
    *  @brief   Defines what to do if the GUI is changed
    *  @details OnGUI is called for rendering and handling GUI events. \n 
    *           This function can be called multiple times per frame (one call per event). \n 
    *           If no book is conected -> mentione it else \n 
    *           Andle the same event \n 
    *           Draw the elements of the Book, \n 
    *           The nodes (text elements) and the links \n
    *           create / delete an node after it is previously seleted
    */
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
          selectedBook.CreateNode(creationNode);
          creationNode = null;
        }
        if (null != deletionNode)
        {
          selectedBook.DeleteNode(deletionNode);
          deletionNode = null;
        }
      }
    }

    /**
    *  @brief   handles events from the GUI interaction
    *  @details enables the undo action \n 
    *           5 cases with 2 supcases hare handled \n 
    *           If mous is down and a node is draged -> 2 cases if click is on node emter dragmode if not enter scrolling mode \n 
    *           if in dragging canvas and mose drag event -> update canvas position \n 
    *           If in draggin node and mause drag event update node position \n 
    *           If mouse is released and in node drag -> leave mode \n 
    *           If mouse is released and in canvas drag -> leave mode
    */
    private void ProcessEvents()
    {
      Undo.undoRedoPerformed += UndoRedoAction;
      if ((EventType.MouseDown == Event.current.type) && (null == dragginNode))
      {
        dragginNode = GetNodeAtPointer(Event.current.mousePosition + scrollPosition);
        if(null != dragginNode)
        {
          dragStart = Event.current.mousePosition - dragginNode.Area.position;
          Selection.activeObject = dragginNode;
        }
        else
        {
          draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
          draggingCanvas = true;
          Selection.activeObject = selectedBook;
        }
      }
      else if ((EventType.MouseDrag == Event.current.type) && (true == draggingCanvas))
      {
        scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
        GUI.changed = true;
      }
      else if ((EventType.MouseDrag == Event.current.type) && (null != dragginNode))
      {
        Rect newArea = dragginNode.Area;
        newArea.position = Event.current.mousePosition - dragStart;
        dragginNode.Area = newArea;
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

    /**
    *  @brief   drawing a node in the canvas
    *  @details using the nodeStyle layout: Plot the number and the text.
    *           Get a horizontal layout for node deletion linkt button and node creation
    *  @param   node the node to be drawn
    */
    private void DrawNode(TextElements node)
    {
      GUILayout.BeginArea(node.Area, nodeStyle);
      EditorGUILayout.LabelField("Node: " + node.TextNumber);

      node.Text = EditorGUILayout.TextField(node.Text);

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

    /**
    *  @brief   draw the button of a node element
    *  @details The button has differnd stated and hence different lables. The states can be entered by kliking on the the button: \n
    *           default lable link: ready to link something. \n 
    *           when clicked on the buton the current node gets the cancel lable while th others get
    *           unlink if it islinked to the node and child if the node is not already linked.
    *  @param   node the node to be handled
    */
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
        if (-1 < linkingParentNode.JumpTos.FindIndex(x => x.referenceId == node.name))
        {
          if (GUILayout.Button("unlink"))
          {
            linkingParentNode.DeleteLink(node.name);
            linkingParentNode = null;
          }
        }
        else
        {
          if (GUILayout.Button("child"))
          {
            linkingParentNode.CreateLink(node.name);
            linkingParentNode = null;
          }
        }
      }
    }

    /**
    *  @brief   draw the conections between between the nodes
    *  @details for each linked element, defined by the getAllchildren function of the book
    *           a bezier cover if is dran from the right centero to the let center of the child
    *  @param   parent the node from which the link emerge
    */
    private void DrawConnections(TextElements parent)
    {
      Vector3 startPosition = new Vector2(parent.Area.xMax, parent.Area.center.y);
      foreach (TextElements childeNode in selectedBook.GetAllChildren(parent))
      {
        Vector3 endPosition = new Vector2(childeNode.Area.xMin, childeNode.Area.center.y);
        float distance = Vector3.Distance(endPosition, startPosition);
        Vector3 tangentOffset = new Vector2(distance * 0.5f, 0.0f);
        Handles.DrawBezier(startPosition, endPosition,
          startPosition + tangentOffset, endPosition - tangentOffset,
          Color.white, null, 4f);
      }
    }

    /**
    *  @brief   helper function to figure out if an event is on a node or not.
    *  @details for each node test if te point is within the area of the node
    *  @param   point the 2D point in cancas coordinates (including the current scroll position)
    *  @return  if the position is on a node return the node if not return null
    */
    private TextElements GetNodeAtPointer(Vector2 point)
    {
      foreach (TextElements node in selectedBook.GetAllNodes())
      {
        if (node.Area.Contains(point))
        {
          return node;
        }
      }
      return null;
    }

    /**
    *  @brief   set the node style and link the OnSelectionChange (differnd book loaded) callback
    *  @details this function is called when the object becomes enabled and active.
    */
    private void OnEnable()
    {
      SetNodeStyle();

      Selection.selectionChanged += OnSelectionChanges;
      OnSelectionChanges();
    }

    /**
    *  @brief   Sets the global node style
    *  @details some setting which could be optimised, see code
    */
    private void SetNodeStyle()
    {
      nodeStyle = new GUIStyle();
      nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
      nodeStyle.padding = new RectOffset(10, 10, 10, 10);
      nodeStyle.border = new RectOffset(12, 12, 12, 12);
    }
    #endregion

    #region Callbacks 
    /**
    *  @brief   first time a book is loaded
    *  @details if an book can be instanciated the editor is shown
    */
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

    /**
    *  @brief   callback when a new book is selected to be edited
    *  @details if the selection can be linked to an book than update the global ariable seleced Book and repaing the editor
    */
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