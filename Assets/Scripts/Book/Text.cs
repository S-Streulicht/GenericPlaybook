using System.Collections.Generic;

using System.Linq;
using UnityEngine;

namespace PB.Book
{
  [CreateAssetMenu(fileName = "NewBook", menuName = "TextCreation/Book", order = 1)]
  public class Text : ScriptableObject
  {
    [SerializeField] List<TextElements> nodes = new List<TextElements>();

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
#endif

    public IEnumerable<TextElements> GetAllNodes()
    {
      return nodes;
    }
  }
}
