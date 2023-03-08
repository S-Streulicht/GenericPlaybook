using PB.TextElement;

namespace PB.Jump
{
  [System.Serializable]
  public struct JumpTo
  {
    public string text;
    public TextElements reference;
    public string[] conditions;
    public string[] comands;
  }
}
