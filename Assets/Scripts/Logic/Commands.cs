namespace PB.Logic
{
  /**
  *  @brief    Sets ingame comands
  *  @details  Have an abstraction layer to decouple the String describing a Command from the Refernec the Code is using
  */
  static class Commands
  {
    // Commands
    public const string GET_GAME_STATE = "GET() -> ExtraUIGameState"; /**< String reference to get the game state */
    public const string GET_UI_PICTURE = "GET() -> ExtraUIPicture";   /**< String reference to get the Picture wich is displayed */
  }
}
