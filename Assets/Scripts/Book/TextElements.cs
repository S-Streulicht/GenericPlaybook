using System.Collections.Generic;
using UnityEngine;

namespace PB.Book
{
  public class TextElements : ScriptableObject
  {
    public int textNumber = 1;
    public string text;
    public List<string> comands = new List<string>();
    public List<JumpTo> jumpTos = new List<JumpTo>();
    public Rect area = new Rect(10, 50, 200, 100);
  }
}
