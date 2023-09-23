using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PB.TextElement
{
  [CreateAssetMenu(fileName = "NewBook", menuName = "TextCreation/Book", order = 1)]
  public class Text : ScriptableObject
  {
    [SerializeField] TextElements[] nodes;
  }
}
