namespace PB.Book
{
  //[CreateAssetMenu(fileName = "NewTextElement", menuName = "TextCreation/TextElement", order = 0)]
  [System.Serializable]
  public class TextElements
  {
    public string uniqueID;
    public string text;
    public string[] comands;
    public JumpTo[] jumpTos;
  }
}
