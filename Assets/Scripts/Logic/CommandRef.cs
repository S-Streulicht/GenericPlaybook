namespace PB.Logic
{
  /**
  *  @brief    Functionality is referenced by name, provide a common place for the references
  *  @details  Have an abstraction layer to decouple the String given in the Commands from the Refernec the Code is using
  */
  static class CommandRef
  {
    // Commands
    public const string CHANGE_ATTRIBUTE = "CHANGE"; /**< String reference of the Change comand e.g. used for attributes */
    public const string IS_ATTRIBUTE = "IS"; /**< String reference of the IS command e.g. used for the attributes */
  }
}
