using UnityEngine;

using PB.Jump;

namespace PB.TextElement
{
  [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TextElement", order = 1)]
  public class TextElements : ScriptableObject
  {
    public string text;
    public string[] comands;
    public JumpTo[] jumpTos;
  }
}
