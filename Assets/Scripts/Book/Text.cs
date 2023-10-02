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
        nodes.Add(new TextElements());
      }
    }
#else
    void Awake()
    {
      OnValidate();   
    }
#endif

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
