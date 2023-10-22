using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PB.Book
{
  public class TextElements : ScriptableObject
  {
    [SerializeField] private int textNumber = 1;
    [SerializeField] private string text;
    [SerializeField] private List<string> comands = new List<string>();
    [SerializeField] private List<JumpTo> jumpTos = new List<JumpTo>();
    [SerializeField] private Rect area = new Rect(10, 50, 200, 100);

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
    public void CreateLink(string childID)
    {
      Undo.RecordObject(this, "create link on " + textNumber );
      JumpTo link = new JumpTo();
      link.referenceId = childID;
      JumpTos.Add(link);
      EditorUtility.SetDirty(this);
    }

    public void DeleteLink(string childID)
    {
      Undo.RecordObject(this, "delete link of " + textNumber);
      JumpTos.RemoveAll(x => x.referenceId == childID);
      EditorUtility.SetDirty(this);
    }
#endif
  }
}
