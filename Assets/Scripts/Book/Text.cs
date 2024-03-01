using System.Collections.Generic;

using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PB.Book
{
  [CreateAssetMenu(fileName = "NewBook", menuName = "TextCreation/Book", order = 1)]
  public class Text : ScriptableObject, ISerializationCallbackReceiver
  {
    [SerializeField] List<TextElements> nodes = new List<TextElements>();
    List<TextElements> reverseNodes = new List<TextElements>();

    Dictionary<string, TextElements> nodeLookup = new Dictionary<string, TextElements>();
    static bool onSerialiseIsActive = false;    

#if UNITY_EDITOR
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
    }
#else
    private void Awake()
    {
      OnValidate();
    }
#endif

#region  EditorRelated stuff
#if UNITY_EDITOR
    public void CreateNode(TextElements parent)
    {
      TextElements child = MakeNode(parent);

      Undo.RegisterCreatedObjectUndo(child, "Created Node " + child.TextNumber);
      Undo.RecordObject(this, "Added Node to " + child.TextNumber);

      AddNode(child);
    }

    private TextElements MakeNode(TextElements parent)
    {
      TextElements child = CreateInstance<TextElements>();
      child.name = System.Guid.NewGuid().ToString();
      child.TextNumber = getBiggestTextNum() + 1;

      if (null != parent)
      {
        Rect newArea = child.Area;
        newArea.x = parent.Area.xMax + 100;
        newArea.y = parent.Area.y + (parent.Area.height + 5) * parent.JumpTos.Count();
        child.Area = newArea;

        parent.CreateLink(child.name);
      }

      return child;
    }

    private void AddNode(TextElements child)
    {
      nodes.Add(child);
      OnValidate();
    }

    public void DeleteNode(TextElements nodeToDelete)
    {
      Undo.RecordObject(this, "Remove Node " + nodeToDelete.TextNumber);
      nodes.Remove(nodeToDelete);
      OnValidate();
      foreach (TextElements node in GetAllNodes())
      {
        node.DeleteLink(nodeToDelete.name);
      }
      Undo.DestroyObjectImmediate(nodeToDelete);
    }
#endif
#endregion

    private int getBiggestTextNum()
    {
      int ret = 0;
      foreach(TextElements node in nodes)
      {
        if(node.TextNumber > ret) {ret = node.TextNumber;}
      }
      return ret;
    }

    /// <summary>
    /// Called when the script is loaded or a value is changed in the
    /// inspector (Called in the editor only).
    /// </summary>
    private void OnValidate()
    {
      reverseNodes.Clear();
      reverseNodes = new List<TextElements>(nodes);
      reverseNodes.Reverse();

      nodeLookup.Clear();
      foreach (TextElements node in GetAllNodes())
      {
        nodeLookup[node.name] = node;
      }
    }

    public IEnumerable<TextElements> GetAllNodes()
    {
      return nodes;
    }

    public TextElements GetRootNode()
    {
      return nodes.First();
    }

    public IEnumerable<TextElements> GetAllNodesReverse()
    {
      return reverseNodes;
    }

    public IEnumerable<TextElements> GetAllChildren(TextElements parentNode)
    {
      foreach(JumpTo jumps in parentNode.JumpTos)
      {
        //yield return nodes.Find( x => jumps.referenceId == x.uniqueID);
        if( nodeLookup.ContainsKey(jumps.referenceId) )
        {
          yield return nodeLookup[jumps.referenceId];
        }
      }
    }

    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
      if(onSerialiseIsActive == false)
      {
        onSerialiseIsActive = true;
        if (0 == nodes.Count())
        {
          TextElements child = MakeNode(null);
          AddNode(child);
        }

        if (AssetDatabase.GetAssetPath(this) != "" )
        {
          foreach (TextElements node in GetAllNodes())
          {
            if (AssetDatabase.GetAssetPath(node) == "")
            {
              AssetDatabase.AddObjectToAsset(node, this);
            }
          }
        }
        onSerialiseIsActive = false;
      }
#endif
    }

    public void OnAfterDeserialize() {}
  }
}
