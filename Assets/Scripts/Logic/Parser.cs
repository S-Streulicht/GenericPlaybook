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
    private const string Target = "->"; /**< starting of the comand result*/

    /**
    * @brief   Parsing of the command
    * @details frist cleaning the string from whitespaces
    *          than split the comand into command, arguments and target
    *          than removing the parenthesis from the argument token
    *          than splitting the arguments
    *          remove the Target indicator string from the target token.
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
      char[] splitPoints = {StartArg, EndArg};
      string[] tokens = cleanString.Split(splitPoints);

      command.Com = tokens[0];
      
      tokens[1] = RemoveStartEnd(tokens[1]);
      command.Arguments = tokens[1].Split(Delimiter).ToList();
      
      if(tokens.Count() > 2) {command.Result = RemoveTargetIndicator(tokens[2]);}
      
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
    * @brief   Remove the target indicator
    * @details Rreplace the indicator given with "Target" with an empty string
    * @param   source the string with needs to be striped
    * @return  the striped string
    */
    private static string RemoveTargetIndicator(string source)
    {
      return new string(source.Replace(Target, ""));
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
