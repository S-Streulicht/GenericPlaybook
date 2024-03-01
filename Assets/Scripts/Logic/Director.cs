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
  public class Director : MonoBehaviour
  {
    [SerializeField] Text Book;

    private TextElements CurrentNode;
    private CommandInterpreter Interpreter;

    // Start is called before the first frame update
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

    public string GetText()
    {
      return CurrentNode.Text;
    }

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

    public void SetAnswer(int number)
    {
      List<JumpTo> jumpTos = GetValidJumps();
      var allChildren = Book.GetAllChildren(CurrentNode);
      foreach (TextElements element in allChildren)
      {
        if (element.name == jumpTos[number].referenceId)
        {
          ///ToDo evaluate the comand of the Jumpto and the Node
          CurrentNode = element;
          break;
        }
      }
      ///ToDo Execute Command of new Text
      foreach (string commandString in CurrentNode.Comands)
      {
        Interpreter.ExecuteCommand(commandString);
      }
    }

    private List<JumpTo> GetValidJumps()
    {
      List<JumpTo> ret = new List<JumpTo>();
      List<JumpTo> jumpTos = CurrentNode.JumpTos;
      foreach (JumpTo jump in jumpTos)
      {
        ///ToDo test the condition and only than add
        if (jump.conditions.Count() > 0)
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
        else
        {
          ret.Add(jump);
        }
      }
      return ret;
    }
  }
}