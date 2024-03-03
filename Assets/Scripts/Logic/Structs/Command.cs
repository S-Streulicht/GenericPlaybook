using System.Collections.Generic;

namespace PB.Logic
{
  /**
  *  @brief    structure how the comands gets parsed
  *  @details  Splitthe comand string in different parts
  *            the actual comand and its arguments
  *            typically the first argument contains the class to call
  */
  public struct Command
  {
    public string Com;             /**< the Commmand to call*/
    public List<string> Arguments; /**< the arguments of the command*/
  }
}
