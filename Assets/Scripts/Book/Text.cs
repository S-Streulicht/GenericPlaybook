using System.Collections.Generic;

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PB.Book
{
  [CreateAssetMenu(fileName = "NewBook", menuName = "TextCreation/Book", order = 1)]
  public class Text : ScriptableObject, ISerializationCallbackReceiver
  {
    [SerializeField] List<TextElements> nodes = new List<TextElements>();
    List<TextElements> reverseNodes = new List<TextElements>();

    Dictionary<string, TextElements> nodeLookup = new Dictionary<string, TextElements>();    

#if UNITY_EDITOR
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
      OnValidate();
    }
#endif

    public void CreateNode(TextElements parent)
    {
        TextElements child = CreateInstance<TextElements>();
        child.name = System.Guid.NewGuid().ToString();
        child.textNumber = getBiggestTextNum() + 1;
        if( null != parent)
        {
          Vector3 positionOffset = new Vector2( parent.area.width + 20, parent.area.height * parent.jumpTos.Count());
          child.area.x = parent.area.xMax + 100;
          child.area.y = parent.area.y + (parent.area.height + 5) * parent.jumpTos.Count();
          JumpTo newJumpPoint = new JumpTo();
          newJumpPoint.referenceId = child.name;
          parent.jumpTos.Add(newJumpPoint);
        }

        Undo.RegisterCreatedObjectUndo(child, "Created TestElement " + child.textNumber);

        nodes.Add(child);
        OnValidate();
    }

    public void DeleteNode(TextElements nodeToDelete)
    {
      nodes.Remove(nodeToDelete);
      OnValidate();
      foreach (TextElements node in GetAllNodes())
      {
        node.jumpTos.RemoveAll(x => x.referenceId == nodeToDelete.name);
      }
      Undo.DestroyObjectImmediate(nodeToDelete);
    }

    public void CreateLink(TextElements parent, string childID)
    {
      JumpTo link = new JumpTo();
      link.referenceId = childID;
      parent.jumpTos.Add(link);
    }

    public void DeleteLink(TextElements parent, string childID)
    {
      parent.jumpTos.RemoveAll(x => x.referenceId == childID);
    }

    private int getBiggestTextNum()
    {
      int ret = 0;
      foreach(TextElements node in nodes)
      {
        if(node.textNumber > ret) {ret = node.textNumber;}
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

    public IEnumerable<TextElements> GetAllNodesReverse()
    {
      return reverseNodes;
    }

    public IEnumerable<TextElements> GetAllChildren(TextElements parentNode)
    {
      foreach(JumpTo jumps in parentNode.jumpTos)
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
      if (0 == nodes.Count)
      {
        CreateNode(null);
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
    }

    public void OnAfterDeserialize() {}
  }
}
