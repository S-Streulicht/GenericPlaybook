namespace PB.Book
{
  /**
  *  @brief     Structure to contain the links to the next test element 
  *  @details   this structure contains the leements for a jump to te next text element, the actual answer test, where to jump the condition under whicih it is valide and the comands executedwhen choosen.
  */
  [System.Serializable]
  public struct JumpTo
  {
    public string text;         /**< the answer text */
    public string referenceId;  /**< the next text element */
    public string[] conditions; /**< under which condition is the anser valide */
    public string[] comands;    /**< the comands executed when answer is choosen */
  }
}
