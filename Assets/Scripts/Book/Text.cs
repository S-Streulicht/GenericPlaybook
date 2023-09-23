using UnityEngine;

namespace PB.Book
{
  [CreateAssetMenu(fileName = "NewBook", menuName = "TextCreation/Book", order = 1)]
  public class Text : ScriptableObject
  {
    [SerializeField] TextElements[] nodes;
  }
}
