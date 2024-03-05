using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PB.Logic
{
  /**
  *  @brief     Parsing the comandstring
  *  @details   Supported Syntax Comand( Arg1, Arg2, ..., ArgN )
  *             Whitespaces get removed, including the ones in the arguments 
  *             also the Ending parenthesis (depreciated: CommandLine works also without ending parenthesis)
  */
  public class Parser : MonoBehaviour
  {
    private const char Delimiter = ','; /**< delimiter of the arguments */
    private const char StartArg = '('; /**< start the arument list */
    private const char EndArg = ')'; /**< ends the argument list */

    /**
    * @brief   Parsing of the command
    * @details frist cleaning the string from whitespaces
    *          than plit the comand into command and rest
    *          than removing the parenthesis
    *          than plitting the arguments
    *          Some further things to read:
    *          https://code-maze.com/replace-whitespaces-string-csharp/
    *          https://stackoverflow.com/questions/8928601/how-can-i-split-a-string-with-a-string-delimiter
    * @param   CommandString the actual string which needs to be executed
    * @return  the splitted command
    */
    public static Command Parse(string CommandString)
    {
      Command command = new Command();
      string cleanString = RemoveWhitespaces(CommandString);
      string[] tokens = cleanString.Split(StartArg);
      command.Com = tokens[0];
      tokens[1] = RemoveStartEnd(tokens[1]);
      command.Arguments = tokens[1].Split(Delimiter).ToList();

      return command;
    }

    /**
    * @brief   Remove the whitespaces
    * @details using the Linq and the system functionality IsWhitesSpace:
    *          https://learn.microsoft.com/en-us/dotnet/api/system.char.iswhitespace?view=net-8.0
    * @param   source string which needs to be cleaned
    * @return  clean string
    */
    private static string RemoveWhitespaces(string source)
    {
      return new string(source.Where(c => !char.IsWhiteSpace(c)).ToArray());
    }

    /**
    * @brief   REmove start and end parenthesis
    * @details using the Linq and seach for all parenthesis
    * @param   source string which needs to be cleaned
    * @return  clean string
    */
    private static string RemoveStartEnd(string source)
    {
      return new string(source.Where(c => c != StartArg && c != EndArg).ToArray());
    }
  }
}
