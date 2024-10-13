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
    [SerializeField] Text Book;             /**< contains the actual book to be played \todo better way of setting the book */
    IBookInterface _book;                   /**< internaly used to refere to the interface of the book. Unfurtunately the interface is not serializable hence it cant be used directly */

    private ITextNodeInterface CurrentNode; /**< contains the currend node of a play book*/
    private CommandInterpreter Interpreter; /**< contains the Comantlineinterpreter get filled at Start*/
    private TextEval           textEval;    /**< contains the Textecvaluation component. Get filled during initalisation*/

    /**
    * @brief   setup the instance get
    * @details Start is called before the first frame update
    *          sets the internal book variable to use only the interface
    *          Get the commandInterpreter instance
    *          Get the textEval instance
    *          test if a Book is loaded
    *          initialise the CurrnetNode to the root of the loaded node
    */
    void Start()
    {
      _book = Book as IBookInterface;
      Interpreter = GetComponent<CommandInterpreter>();
      textEval = GetComponent<TextEval>();
      if (null == Book)
      {
        Debug.Log("Forgot to set the book");
        return;
      }
      CurrentNode = _book.GetRootNode();

      ExecuteNodeCommands();
    }

    #region Normal play text
    /**
    * @brief  get the text of the currend node befor returning all Comands are evaluated.
    * @return string of the current text
    */
    public string GetText()
    {
      return textEval.Replace(CurrentNode.Text);
    }

    /**
    * @brief   get only valid answers of the set of all answers of the current node
    * @details exctract all jumpToos which are valide
    *          get the text of all the jumpTos
    *          return the text
    * @return  returns a list of answers. Remak the order matters in the feedback to choose the answwers
    */
    public List<String> GetValideAnswers()
    {
      List<string> ret = new List<string>();
      List<JumpTo> jumpTos = GetJumpsBasedOn(true);
      foreach (JumpTo jump in jumpTos)
      {
        ret.Add(jump.text);
      }
      return ret;
    }

    /**
    * @brief   get only invalid answers of the set of all answers of the current node
    * @details exctract all jumpToos which are invalide
    *          get the text of all the jumpTos
    *          return the text
    * @return  returns a list of answers. Remak invalide answers can't be choosen
    */
    public List<String> GetInValideAnswers()
    {
      List<string> ret = new List<string>();
      List<JumpTo> jumpTos = GetJumpsBasedOn(false);
      foreach (JumpTo jump in jumpTos)
      {
        ret.Add(jump.text);
      }
      return ret;
    }

    /**
    * @brief   Move to the linked text element
    * @details choose the numbers element of all valid answers
    *          execute the comands of the given Jumpto
    *          Move to the new node given by the jumpTo
    *          execute the commands of the new node
    * @param   number the actual number of the valid answers
    */
    public void SetAnswer(int number)
    {
      List<JumpTo> jumpTos = GetJumpsBasedOn(true);
      TextElements newNode = _book.GetNodeByRefId(jumpTos[number].referenceId);

      if (null != newNode)
      {
        foreach (string commandString in jumpTos[number].comands)
        {
          Interpreter.ExecuteCommand(commandString);
        }

        CurrentNode = newNode;
      }

      ExecuteNodeCommands();
    }

    /**
    * @brief   execute the comands of the text element
    * @details loop through all comands and call the interpreter
    */
    private void ExecuteNodeCommands()
    {
      foreach (string commandString in CurrentNode.Comands)
      {
        Interpreter.ExecuteCommand(commandString);
      }
    }

    /**
    * @brief   get JumpTos wich are either valide or not valide
    * @details get all JumpTos of the curend node
    *          test each condition of the jump to
    *          if the sum of the conditons corresponds to the isValide param add it to the returned jumpTos
    *          return the list
    * @param   isValide if true return only valide jumpTos if false return only invalide jumpTos
    * @return  The rist of jumptos based on the isValide parameter
    */
    private List<JumpTo> GetJumpsBasedOn(bool isValide)
    {
      List<JumpTo> ret = new List<JumpTo>();
      List<JumpTo> jumpTos = CurrentNode.JumpTos;
      foreach (JumpTo jump in jumpTos)
      {
        bool cond = true;
        foreach (string condition in jump.conditions)
        {
          cond &= Interpreter.ReturnCommand<bool>(condition);
        }
        if (cond == isValide)
        {
          ret.Add(jump);
        }
      }
      return ret;
    }

    #endregion

    /**
    * @brief   returns the game state
    * @details get the information by calling the interpreter which is capsulating the state commmand
    * @return  The actual state
    */
    public string GetGameState()
    {
      return Interpreter.GetGameState();
    }

    /**
    * @brief   returns a texture set by a script
    * @details get the information by calling the interpreter which is capsulating the state commmand
    * @return  Texture set by script
    */
    public Texture2D GetTexture()
    {
      return Interpreter.GetTexture();
    }
  }
}