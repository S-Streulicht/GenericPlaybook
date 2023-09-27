using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;

namespace PB.Book.BookEditor
{
  public class BookEditor : EditorWindow
  {
    #region private Variables
    private Text selectedBook = null;
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
        foreach(TextElements node in selectedBook.GetAllNodes())
        {
          EditorGUILayout.LabelField("Node: " + node.uniqueID);
          EditorGUI.BeginChangeCheck();
          var newText = EditorGUILayout.TextField(node.text);
          if(EditorGUI.EndChangeCheck())
          {
            Undo.RecordObject(selectedBook, "Update of Texxt in Node " + node.uniqueID);
            node.text = newText;
          }
        }
      }
    }
    
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
      Selection.selectionChanged += OnSelectionChanges;
      OnSelectionChanges();
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