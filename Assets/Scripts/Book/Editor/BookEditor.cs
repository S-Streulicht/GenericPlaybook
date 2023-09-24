using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;

namespace PB.Book.BookEditor
{
  public class BookEditor : EditorWindow
  {
    [MenuItem("Window/Book Editor")]
    public static void ShowEditorWindow()
    {
      GetWindow(typeof(BookEditor), false, "Book Editor");
    }

   [OnOpenAssetAttribute(1)]
   public static bool OnOpenAsset(int instanceID, int line)
   {
     Text book = EditorUtility.InstanceIDToObject(instanceID) as Text;
     if(book != null)
     {
       ShowEditorWindow();
       return true;
     }
     return false;
   }

  }
}