using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PB.Book
{
  /**
  *  @brief     The actual text on each page
  *  @details   the story is toled by individuel elements presented by this class
  */
  public class TextElements : ScriptableObject, ITextNodeInterface
  {
    [SerializeField] private int textNumber = 1;                        /**< The actual number of the text  could be the page number*/
    [SerializeField] private string text;                               /**< The actual storry text */
    [SerializeField] private List<string> comands = new List<string>(); /**< comands executed when the text apreas the fist */
    [SerializeField] private List<JumpTo> jumpTos = new List<JumpTo>(); /**< potential answers */
    [SerializeField] private Rect area = new Rect(10, 50, 200, 100);    /**< just for the editor, the position and size of the element \todo guard by editor flag */

    /**
    * @brief the getter of the textNumber the setter is private
    * @details the setter is only available at the editor time, it handles the undo event and the save mechanism when changed.
    */
    public int TextNumber {
      get => textNumber;
#if UNITY_EDITOR
      set {
        Undo.RecordObject(this, "changed number of " + textNumber);
        textNumber = value;
        EditorUtility.SetDirty(this);
      }
#endif
    }

    /**
    * @brief the getter of the text the setter is private
    * @details the setter is only available at the editor time, it handles the undo event and the save mechanism when changed.
    */
    public string Text {
      get => text;
#if UNITY_EDITOR
      set {
        if( text != value)
        {
          Undo.RecordObject(this, "change text of " + textNumber);
          text = value;
          EditorUtility.SetDirty(this);
        }
      }
#endif
    }

    /**
    * @brief the getter of the comand the setter is private
    * @details the setter is only available at the editor time, it handles the undo event and the save mechanism when changed.
    */
    public List<string> Comands {
      get => comands;
#if UNITY_EDITOR
      set {
        Undo.RecordObject(this, "changed Comand of " + textNumber);
        comands = value;
        EditorUtility.SetDirty(this);
      }
#endif
    }
    /**
    * @brief the getter of the jumpTos the setter is private
    * @details the setter is only available at the editor time, it handles the undo event and the save mechanism when changed.
    */
    public List<JumpTo> JumpTos {
      get => jumpTos;
#if UNITY_EDITOR
      set {
        Undo.RecordObject( this, "changed Jump of " + textNumber);
        jumpTos = value;
        EditorUtility.SetDirty(this);
      }
#endif
    }
    
    /**
    * @brief the getter of the area the setter is private
    * @details the setter is only available at the editor time, it handles the undo event and the save mechanism when changed.
    */
    public Rect Area {
      get => area;
#if UNITY_EDITOR
      set {
        Undo.RecordObject(this, "changed ares of " + textNumber);
        area = value;
        EditorUtility.SetDirty(this);    
      }
#endif
    }

#if UNITY_EDITOR
    /**
    * @brief handles the linking of two text elements at editor time
    * @details it handles the undo event
    *          create a new jumpTo add the coresponding reference ID given by childID
    *          add the jumpTo to the lsit of JumpTos
    * @param   childID the actual ID of the linked text element 
    */
    public void CreateLink(string childID)
    {
      Undo.RecordObject(this, "create link on " + textNumber );
      JumpTo link = new JumpTo();
      link.referenceId = childID;
      JumpTos.Add(link);
      EditorUtility.SetDirty(this);
    }

    /**
    * @brief handles the unlinking of two text elements at editor time
    * @details it handles the undo event
    *          it removes the elemnt for the JummpTos
    * @param   childID the actual ID of the linked text element 
    */
    public void DeleteLink(string childID)
    {
      Undo.RecordObject(this, "delete link of " + textNumber);
      JumpTos.RemoveAll(x => x.referenceId == childID);
      EditorUtility.SetDirty(this);
    }
#endif
  }
}
