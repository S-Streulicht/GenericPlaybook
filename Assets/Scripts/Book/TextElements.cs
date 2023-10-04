using System.Collections.Generic;
using UnityEngine;

namespace PB.Book
{
  //[CreateAssetMenu(fileName = "NewTextElement", menuName = "TextCreation/TextElement", order = 0)]
  [System.Serializable]
  public class TextElements
  {
    public int textNumber = 1;
    public string text;
    public List<string> comands = new List<string>();
    public List<JumpTo> jumpTos = new List<JumpTo>();
    public string uniqueID;
    public Rect area = new Rect(10, 50, 200, 100);
  }
}
