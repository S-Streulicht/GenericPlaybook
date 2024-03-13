using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PB.ExtraUI
{
  /**
*  @brief   Handles the extra UI "GameState" 
*  @details Besides the state for the gamestate the class heritates from the IExtraUiInterface functionality and tests are handled by this.
*/
  public class ExtraUIGameState : MonoBehaviour, IExtraUiInterface
  {
    [SerializeField] string State; /**< the actual game state can be set by the GUI */

    readonly List<string> GameState = new List<string>() { "Default", "Win", "Lose" };

    void IExtraUiInterface.Set(List<string> arg)
    {
      if (!((IExtraUiInterface)this).Test("Set", arg)) return;
      State = arg[1];
    }

    void IExtraUiInterface.UnSet()
    {
      State = GameState.First();
    }

    Ttype IExtraUiInterface.GetInfo<Ttype>()
    {
      /// \todo fill the GetInfo
      Ttype ret = default(Ttype);
      string bla = "ff";
      if (typeof(Ttype) == typeof(string)) return (Ttype)Convert.ChangeType(bla, typeof(Ttype));
      return ret;
    }

    /**
    * @brief   implementation of the interface test used to tests the argument list
    * @details test the argument list of the other interfaces
    *          can be used as stand alone or as part of the other interfaces
    * parme    Interface which interface to test "Set" or "UnSet" or GetInfo
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
          ret &= isCorrectClass(arg[0]);
          ret &= GameState.Contains(arg[1]);
          break;
        case "UnSet":
          // no parameters are needed
          break;
        case "GetInfo":
          /// \todo fill the test
          break;
        default:
          Debug.Log("Wrong interface (" + Interface + ") to be tested in " + getClassname());
          ret = false;
          break;
      }

      return ret;
    }

    #region HelperClass
    /**
    * @brief   test if the inputstring is the class name
    * @details the function is using the shortend name, not the full qualified one.
    * parme    className the name to be tests
    * @return  true if the input is equal to the actual classe
    */
    private bool isCorrectClass(string className)
    {
      return getClassname() == className;
    }

    /**
    * @brief   get and trim the actual name of the class
    * @return  string the actual class name
    */
    private string getClassname()
    {
      string fullQualifiedName = this.ToString();
      string actualClassname = fullQualifiedName.Split('.').Last();
      return actualClassname.Trim(')');
    }
    #endregion
  }
}