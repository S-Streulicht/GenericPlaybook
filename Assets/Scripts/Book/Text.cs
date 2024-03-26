using System.Collections.Generic;

using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PB.Book
{
  [CreateAssetMenu(fileName = "NewBook", menuName = "TextCreation/Book", order = 1)]
  /**
  *  @brief   This class handles the actual book
  *  @details The book is containing a series of Text Elements.
  *           Besides the actual text part it provides some information for the Book editod:
  *           when to store hand a (revers) lsit of the nodes.
  */
  public class Text : ScriptableObject, ISerializationCallbackReceiver
  {
    [SerializeField] List<TextElements> nodes = new List<TextElements>();                 /**< containing the individual pages of the book */
    List<TextElements> reverseNodes = new List<TextElements>();                           /**< inverse list for faster usage */

    Dictionary<string, TextElements> nodeLookup = new Dictionary<string, TextElements>(); /**< provides fast acces of the node list by node.name */
    static bool onSerialiseIsActive = false;                                              /**< some how a singelton pattern to prefent multiple calls of the serialisation */

#if UNITY_EDITOR
    /**
    *  @brief   enables the OnValidate
    *  @details Awake is called when the script instance is being loaded.
    *           The function is empty in editor.
    *           At run time it calles the OnValidate function which is other wise not called. eventualyl it calculated the reverse list.
    */
    private void Awake()
    {
    }
#else
    /**
    *  @brief   enables the OnValidate
    *  @details Awake is called when the script instance is being loaded.
    *           The function is empty in editor.
    *           At run time it calles the OnValidate function which is other wise not called. eventualyl it calculated the reverse list.
    */
    private void Awake()
    {
      OnValidate();
    }
#endif

    #region  EditorRelated stuff
#if UNITY_EDITOR
    /**
    *  @brief   creation of a new TextElemen as a node of the book
    *  @details additionally an undo enty in the node editor is created
    *           the node is added to a given parent node by the function makeNode
    *           sets the context of the newly crated node
    *           only available in editor
    *  @param   parent the parent of the node to create
    */
    public void CreateNode(TextElements parent)
    {
      TextElements child = MakeNode(parent);

      Undo.RegisterCreatedObjectUndo(child, "Created Node " + child.TextNumber);
      Undo.RecordObject(this, "Added Node to " + child.TextNumber);

      AddNode(child);
    }

    /**
    *  @brief   generates a new node and sets the base settings
    *  @details creation of node and setting of the base paramenters, such as name.
    *           it also sets the area wher the node is drawn in the editor next to its parent node
    *           only available in editor
    *  @param   parent the parent of the node to create
    */
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

    /**
    *  @brief   adding a newly created node to the nodelists
    *  @details calls the OnValidate to setup the reversed node list.
    *           only available in editor
    *  @param   child the actual node to create
    */
    private void AddNode(TextElements child)
    {
      nodes.Add(child);
      OnValidate();
    }

    /**
    *  @brief   removing of a node
    *  @details handles the environment of the node deletion
    *           set up an undo record remoces from the node lsit an remove all dangling links
    *           only available in editor
    *  @param   nodeToDelete the actual node to be deleted
    */
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

    /**
    *  @brief   gets the biggest number of the text elements
    */
    private int getBiggestTextNum()
    {
      int ret = 0;
      foreach (TextElements node in nodes)
      {
        if (node.TextNumber > ret) { ret = node.TextNumber; }
      }
      return ret;
    }

    /**
    *  @brief   updates the reverse node list and the nodeLookup table
    *  @details Called when the script is loaded or a value is changed in the
    *           inspector (Called automaticallyin the editor only) + when it is manualyl called
    */
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

    /**
    *  @brief   returns the node list
    *  @return  all nodes
    */
    public IEnumerable<TextElements> GetAllNodes()
    {
      return nodes;
    }

    /**
    *  @brief   returns the first node
    *  @return  only the first node
    */
    public TextElements GetRootNode()
    {
      return nodes.First();
    }

    /**
    *  @brief   returns the reverse node list
    *  @return  all nodes in revers order
    */
    public IEnumerable<TextElements> GetAllNodesReverse()
    {
      return reverseNodes;
    }

    /**
    *  @brief   get all linked children of a node
    *  @details loops through all jumpTos of a given node and add the linget node to the yield return.
    *           for performance reasons the look up is used.
    *  @param   parentNode the node from wich the linked nodes are needed
    *  @return  all nodes linked from the given node
    */
    public IEnumerable<TextElements> GetAllChildren(TextElements parentNode)
    {
      foreach (JumpTo jumps in parentNode.JumpTos)
      {
        //yield return nodes.Find( x => jumps.referenceId == x.uniqueID);
        if (nodeLookup.ContainsKey(jumps.referenceId))
        {
          yield return nodeLookup[jumps.referenceId];
        }
      }
    }

    /**
    *  @brief   get the node with the described by the reference ID
    *  @details find the node in the lookuptable and sets return it
    *  @param   referenceId the reference ID of the node to find
    *  @return  node withthe coresponding ID or null
    */
    public TextElements GetNodeByRefId(string referenceId)
    {
      if (nodeLookup.ContainsKey(referenceId))
      {
        return nodeLookup[referenceId];
      }
      return null;
    }

    /**
    *  @brief   handles the storage of the book.
    *  @details only used in the Editor. But since it is an interface ISerializationCallbackReceiver of the classe the function needs to be provided
    *           make sure only one instance of the serialisation is running
    *           if the book is empty create a new root node
    *           if an AssetPath is set store all nodes as an ased if not already present.
    */
    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
      if(onSerialiseIsActive == false)
      {
        onSerialiseIsActive = true;
        CreateRootIfEmpty();

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

    /**
    *  @brief   unused need only by interface ISerializationCallbackReceiver
    */
    public void OnAfterDeserialize() { }

    /**
    *  @brief  if the book is empty, create a root node
    */
    private void CreateRootIfEmpty()
    {
      if (0 == nodes.Count())
      {
        TextElements child = MakeNode(null);
        AddNode(child);
      }
    }
  }
}
