using System;
using System.Collections.Generic;

using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace PB.Book
{
  [CreateAssetMenu(fileName = "NewBook", menuName = "TextCreation/Book", order = 1)]
  public class Text : ScriptableObject
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
      if (0 == nodes.Count)
      {
        TextElements rootNode = new TextElements();
        rootNode.uniqueID = System.Guid.NewGuid().ToString();
        nodes.Add(rootNode);
      }
      OnValidate();
    }
#endif

    public void CreateNode(TextElements parent)
    {
        TextElements child = new TextElements();
        child.uniqueID = System.Guid.NewGuid().ToString();
        child.textNumber = getBiggestTextNum() + 1;
        Vector3 positionOffset = new Vector2( parent.area.width + 20, parent.area.height * parent.jumpTos.Count());
        child.area.x = parent.area.xMax + 100;
        child.area.y = parent.area.y + (parent.area.height + 5) * parent.jumpTos.Count();
        JumpTo newJumpPoint = new JumpTo();
        newJumpPoint.referenceId = child.uniqueID;
        parent.jumpTos.Add(newJumpPoint);
        nodes.Add(child);
        OnValidate();
    }

    public void DeleteNode(TextElements nodeToDelete)
    {
      nodes.Remove(nodeToDelete);
      OnValidate();
      foreach (TextElements node in GetAllNodes())
      {
        node.jumpTos.RemoveAll(x => x.referenceId == nodeToDelete.uniqueID);
      }
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
        nodeLookup[node.uniqueID] = node;
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
  }
}
