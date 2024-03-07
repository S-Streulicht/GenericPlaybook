using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PB.Book;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;


namespace PB.Logic
{
  /**
  *  @brief     Provides content for the GUI 
  *  @details   This class is used to provide generic ports like GetText, SetAnswer ... which eventuelly gets displayed
  */
  public class Director : MonoBehaviour
  {
    [SerializeField] Text Book; /**< contains the actual book to be played \todo better way of setting the book */

    private TextElements CurrentNode; /**< contains the currend node of a play book*/
    private CommandInterpreter Interpreter; /**< contains the Comantlineinterpreter gest filled at Start*/

    /**
    * @brief setup the instance get
    * @details Start is called before the first frame update
    *          Get the commandInterpreter instance
    *          test if a Book is loaded
    *          initialise the CurrnetNode to the root of the loaded node
    * @return void
    */
    void Start()
    {
      Interpreter = GetComponent<CommandInterpreter>();
      if (null == Book)
      {
        Debug.Log("Forgot to set the book");
        return;
      }
      CurrentNode = Book.GetRootNode();
    }

    /**
    * @brief get the text of the currend node
    * @return void
    */
    public string GetText()
    {
      return CurrentNode.Text;
    }

    /**
    * @brief get only valid answers of the set of all answers of the current node
    * @details exctract all jumpToos which are valide
    *          get the text of all the jumpToos
    *          return the text
    * @return returns a lit of answers. Remak the order matters in the feedback to choos the answwers
    */
    public List<String> GetValideAnswers()
    {
      List<string> ret = new List<string>();
      List<JumpTo> jumpTos = GetValidJumps();
      foreach (JumpTo jump in jumpTos)
      {
        ret.Add(jump.text);
      }
      return ret;
    }

    /**
    * @brief Move to the linked text element
    * @details choose the numbers element of all valid answers
    *          execute the Comands of the given Jumpto
    *          Move to the new node given by the jumpTo
    *          execute the commands of the new node
    * @param number the actual number of the valid answers
    * @return void
    */
    public void SetAnswer(int number)
    {
      List<JumpTo> jumpTos = GetValidJumps();
      var allChildren = Book.GetAllChildren(CurrentNode);
      foreach (TextElements element in allChildren)
      {
        if (element.name == jumpTos[number].referenceId)
        {
          foreach(string commandString in jumpTos[number].comands)
          {
            Interpreter.ExecuteCommand(commandString);
          }
          
          CurrentNode = element;
          break;
        }
      }

      foreach (string commandString in CurrentNode.Comands)
      {
        Interpreter.ExecuteCommand(commandString);
      }
    }

    /**
    * @brief get only valide JumpTos
    * @details get all Jumptoos of the curend node
    *          test each condition of the jump to
    *          if fullfileld add it to the list of valit jumpTos
    *          redturn the list
    * @param number the actual number of the valid answers
    * @return void
    */
    private List<JumpTo> GetValidJumps()
    {
      List<JumpTo> ret = new List<JumpTo>();
      List<JumpTo> jumpTos = CurrentNode.JumpTos;
      foreach (JumpTo jump in jumpTos)
      {
        bool cond = true;
        foreach (string condition in jump.conditions)
        {
          cond &= Interpreter.TestCommand(condition);
        }
        if (cond == true)
        {
          ret.Add(jump);
        }
      }
      return ret;
    }
  }
}