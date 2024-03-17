using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PB.Helper;

namespace PB.ExtraUI
{
  /**
  *  @brief   Handles the extra UI "GameState" 
  *  @details Besides the state for the gamestate the class heritates from the IExtraUiInterface functionality and tests are handled by this.
  */
  public class ExtraUIGameState : MonoBehaviour, IExtraUiInterface
  {
    private readonly ClassManager<ExtraUIGameState> classManager; /**< containing acces to the Helper Class: Class Manager */

    readonly List<string> GameState = new List<string>() { "Default", "Win", "Lose" };  /**< the potential game state: Default, Win, Lose" */
    [SerializeField] string State = "Default"; /**< the actual game state can be set by the GUI */

    /**
    * @brief   constructor initialises the helper classes 
    */
    public ExtraUIGameState()
    {
      classManager = new ClassManager<ExtraUIGameState>(this);
    }

    /**
    * @brief   set the state to the one of the script.
    * @details the first element of the GameState member
    * @param   arg List: arg[0] is the class name and arg[1] the actual stat.
    */
    void IExtraUiInterface.Set(List<string> arg)
    {
      if (!((IExtraUiInterface)this).Test("Set", arg)) return;
      State = arg[1];
    }

    /**
    * @brief   set the state to the default
    * @details the first element of the GameState member
    */
    void IExtraUiInterface.UnSet()
    {
      State = GameState.First();
    }

    /**
    * @brief   returns the actual state of the GameState as string
    * @details if TType is a string the actual state is returned else he default of the actual type.
    * @return  the actual game state
    */
    Ttype IExtraUiInterface.GetInfo<Ttype>()
    {
      Ttype ret = default(Ttype);
      if (typeof(Ttype) == typeof(string)) return (Ttype)Convert.ChangeType(State, typeof(Ttype));
      return ret;
    }

    /**
    * @brief   implementation of the interface test used to tests the argument list
    * @details test the argument list of the other interfaces
    *          can be used as stand alone or as part of the other interfaces
    * param    Interface which interface to test "Set" or "UnSet" or GetInfo
    * param    arg List: arg[0] is the classname, arg[1] is element of <, > and == arg[2] is an integer
    * @return  true if the args meet the actual needs.
    */
    bool IExtraUiInterface.Test(string Interface, List<string> arg)
    {
      bool ret = true;

      switch (Interface)
      {
        case "Set":
          if (arg.Count != 2) return false;
          ret &= classManager.isCorrectClass(arg[0]);
          ret &= GameState.Contains(arg[1]);
          break;
        case "UnSet":
          // no parameters are needed
          break;
        case "GetInfo":
          // no parameters are needed
          break;
        default:
          Debug.Log("Wrong interface (" + Interface + ") to be tested in " + classManager.getClassname());
          ret = false;
          break;
      }

      return ret;
    }
  }
}