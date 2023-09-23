namespace PB.Book
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
