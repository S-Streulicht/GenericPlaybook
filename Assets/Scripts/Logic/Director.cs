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
    [SerializeField] EventAggregator Events;/**< Handles events comming from sub book handling*/
    IBookInterface _book;                   /**< internaly used to refere to the interface of the book. Unfurtunately the interface is not serializable hence it cant be used directly */

    private ITextNodeInterface CurrentNode; /**< contains the currend node of a play book*/
    private CommandInterpreter Interpreter; /**< contains the Comantlineinterpreter gest filled at Start*/

    /**
    * @brief   setup the instance get
    * @details Start is called before the first frame update
    *          sets the event for the correct handling of starting and ending a book
    *          sets the internal book variable to use only the interface
    *          Get the commandInterpreter instance
    *          test if a Book is loaded
    *          initialise the CurrnetNode to the root of the loaded node
    */
    void Start()
    {
      Events.Subscribe<ProcessingStartedEvent>(e => StartANewBook(e.book));
      Events.Subscribe<ProcessingFinishedEvent>(e => EndBook(e.succsess));

      Interpreter = GetComponent<CommandInterpreter>();
      
      _book = Book as IBookInterface;

      if (null == Book)
      {
        Debug.Log("Director: Forgot to set the book");
        return;
      }
      CurrentNode = _book.GetRootNode();

      ExecuteNodeCommands();
    }

    #region BookHandling

    /**
     * @brief   start enters a new book
     * @details gettings the new books root node and execut its comand list
     * @param   book the book to start
     */
    private void StartANewBook(IBookInterface book)
    {
       if (null == book)
       {
        Debug.Log("Director: fail to load a new book");
        return;
       }
       _book = book;
       CurrentNode = _book.GetRootNode();
       ExecuteNodeCommands();
    }

    /**
    * @brief   returns from a book to the next story
    * @param   var som handover parameter currently unused
    */
    private void EndBook(bool var)
    {
      _book = Book as IBookInterface;
      CurrentNode = _book.GetRootNode();
      // execute comant is comming from the Set return
    }

    #endregion

    #region Normal play text
    /**
    * @brief  get the text of the currend node
    * @return string of the current text
    */
    public string GetText()
    {
      return CurrentNode.Text;
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
    *          move to the new node given by the jumpTo
    *          execute the comands of the given Jumpto
    *          execute the commands of the new node
    * @param   number the actual number of the valid answers
    */
    public void SetAnswer(int number)
    {
      List<JumpTo> jumpTos = GetJumpsBasedOn(true);
      TextElements newNode = _book.GetNodeByRefId(jumpTos[number].referenceId);

      if (null != newNode)
      {
        CurrentNode = newNode; // the node can be changed by a command
 
        foreach (string commandString in jumpTos[number].comands)
        {
          Interpreter.ExecuteCommand(commandString);
        }
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
    * @details get the information by calling the interpreter requesting infor from GameState object
    * @return  The actual state
    */
    public string GetGameState()
    {
      return Interpreter.ReturnCommand<string>(Commands.GET_GAME_STATE);
    }

    /**
    * @brief   returns A texture set by a script
    * @return  Texture set by script
    */
    public Texture2D GetTexture()
    {
      return Interpreter.ReturnCommand<Texture2D>(Commands.GET_UI_PICTURE);
    }
  }
}