using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PB.Logic
{
  /**
  *  @brief     Evales a patter in a text
  *  @details   This class is used to provide find the pattern, See "Patter" evaluates its content with the ComandlineInterpreter and exchanges the text. 
  */
  public class TextEval : MonoBehaviour
  {
    //private const string Pattern = "{{(\\w+?)}}"; /**< Regular expression wich finds "{{OneWord and a Character}}" Example: "{{gg }}" */
    //private const string Pattern = "{{(\\w+\\(\\w+\\))}}"; /**< Regular expression which finds "{{OneWord(OneOtherword)}}" Example "GET(Something)"*/
    private const string Pattern = "{{(\\w+\\(.+\\))}}"; /**< Regular expression which finds "{{OneWord(Anything with at least one cchar)}}" Example "GET(Something, Some other thing)"*/
    //private const string Pattern = "{{().+)}}"; /**< Regular expression which finds "{{Anything with any length above one caracter}} Example "{{ {{Bla}} }}" is found as one reference */
    private CommandInterpreter Interpreter; /**< contains the Comantlineinterpreter get filled at Start*/

    /**
    * @brief   setup the instance
    * @details Start is called before the first frame update
    *          Get the commandInterpreter instance
    */
    void Start()
    {
      Interpreter = GetComponent<CommandInterpreter>();
    }

    /**
    * @brief   Evaluate the matched regular expression
    * @details hand over the match as a string to the Interpreter.
    *          If it cant interpreted a "??" is returned.
    * @param   m the actual match of a regular expression
    * @return  Either the evaled result of the match or "??"
    */
    private string Eval(Match m)
    {
      string ret = Interpreter.ReturnCommand<string>(m.Groups[1].Value.ToString());
      if(null != ret) {return ret;}
      else return "??";
    }

    /**
    * @brief   Find and replace the Pattern
    * @details Find the Pattern replace its content and returns the result.
    *          The function makes sure that at least the input is returned
    *          in case something went wrong in the evaluation.
    * @param   input the text input
    * @return  The original Text but the fields {{*}} are replaced with its evaluated values.
    */
    public string Replace(string input)
    {
      string ret = Regex.Replace( input, Pattern, new MatchEvaluator(Eval) );
      if(null != ret) {return ret;}
      else return input;
    }
  }
}