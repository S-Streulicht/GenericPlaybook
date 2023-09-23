using PB.TextElement;

namespace PB.Jump
{
  [System.Serializable]
  public struct JumpTo
  {
    public string text;
    public string referenceId;
    public string[] conditions;
    public string[] comands;
  }
}
