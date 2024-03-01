using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PB.Logic
{
  public class Parser : MonoBehaviour
  {
    private const char Delimiter = ',';
    private const char StartArg = '(';
    private const char EndArg = ')';
    private const char Whitspace = ' ';

    // function works with following Structure
    // Comand( Arg1, Arg2, ..., ArgN )
    // Whitespaces are removed 
    // such as the Ending parenthesis (depreciated: CommandLine works also without ending parenthesis)
    public static Command Parse(string CommandString)
    {
      Command command = new Command();
      // https://code-maze.com/replace-whitespaces-string-csharp/
      string cleanString = RemoveWhitespaces(CommandString);
      /*Debug.Log(cleanString);*/
      // Split Comand from Arguments StartingArg is removed
      // https://stackoverflow.com/questions/8928601/how-can-i-split-a-string-with-a-string-delimiter
      string[] tokens = cleanString.Split(StartArg);
      command.Com = tokens[0];
      /*Debug.Log(command.Com);*/
      tokens[1] = RemoveStartEnd(tokens[1]);
      /*Debug.Log(tokens[1]);*/
      // Split the argumnents
      command.Arguments = tokens[1].Split(Delimiter).ToList();

      /*foreach(string arg in command.Arguments)
      {
        Debug.Log(arg);
      }*/
      return command;
    }

    private static string RemoveWhitespaces(string source)
    {
      return new string(source.Where(c => !char.IsWhiteSpace(c)).ToArray());
    }

    private static string RemoveStartEnd(string source)
    {
      return new string(source.Where(c => c != StartArg && c != EndArg).ToArray());
    }
  }
}
