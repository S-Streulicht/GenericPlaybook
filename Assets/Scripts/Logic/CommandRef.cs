﻿namespace PB.Logic
{
  /**
  *  @brief    Functionality is referenced by name, provide a common place for the references
  *  @details  Have an abstraction layer to decouple the String given in the Commands from the Refernec the Code is using
  */
  static class CommandRef
  {
    // Commands
    public const string CHANGE_ATTRIBUTE = "CHANGE"; /**< String reference of the Change comand e.g. used for attributes */
    public const string IS_ATTRIBUTE = "IS";         /**< String reference of the IS command e.g. used for the attributes */
    public const string SET_EXTRAUI = "SET";         /**< String reference of the Set command e.g. used for the Extra UIs */
    public const string GET_INFO = "GET";            /**< get the info of an attribut or other things */
    public const string UNSET_EXTRAUI = "UNSET";     /**< unset an attribut e.g. an Extra bUI element */
  }
}
