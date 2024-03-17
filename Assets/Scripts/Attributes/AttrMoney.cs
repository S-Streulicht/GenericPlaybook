using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PB.Helper;

namespace PB.Attribute
{
  /**
  *  @brief   Handles the attribut "money" 
  *  @details Besides the state for the money the class heritates from the IAttributeInterface functionality and tetss are handled by this.
  */
  public class AttrMoney : MonoBehaviour, IAttributeInterface
  {
    [SerializeField] int Money; /**< the actual state of the attribut, can be set by the GUI */

    private readonly ClassManager<AttrMoney> classManager; /**< containing acces to the Helper Class: Class Manager */

    private readonly SortedDictionary<string, Action<int>> ChangeFunction = new SortedDictionary<string, Action<int>>(); /**< groups the fubctionality (change command) set by string requets to the actual action to modify the state. */
    private readonly SortedDictionary<string, Func<int, bool>> Isfunction = new SortedDictionary<string, Func<int, bool>>(); /**< groups the fubctionality (is command) set by string requets to the actual action to modify the state. */

    /**
    * @brief   constructor initialises the helper classes 
    */
    public AttrMoney()
    {
      classManager = new ClassManager<AttrMoney>(this);
    }

    /**
    * @brief   setup the instance
    * @details Start is called before the first frame update
    *          initialisation of the dictionaries
    * @return  void
    */
    void Start()
    {
      ChangeFunction.Add("ADD", x => { Money += x; });
      ChangeFunction.Add("SUB", x => { Money -= x; });
      ChangeFunction.Add("SET", x => { Money = x; if (Money < 0) { Money = 0; } });
      Isfunction.Add("<", x => { return Money < x; });
      Isfunction.Add(">", x => { return Money > x; });
      Isfunction.Add("==", x => { return Money == x; });
    }

    #region InterfaceImplementation
    /**
    * @brief   implementation of the interface Change
    * @details test if the arguments are valide
    *          parses the second argument 
    *          continue if onyl the arg[1] exists as a key in the Change dictionary
    * param    arg List: arg[0] is the classname, arg[1] is element of SET SUB or ADD, arg[2] is an integer
    * @return void
    */
    void IAttributeInterface.Change(List<string> arg)
    {
      if (!((IAttributeInterface)this).Test("Change", arg)) return;

      int val = Int32.Parse(arg[2]);
      if (ChangeFunction.ContainsKey(arg[1]))
      {
        ChangeFunction[arg[1]](val);
      }
      else
      {
        Debug.Log("Unknown function " + arg[1] + " in " + classManager.getClassname() + " is called in Change");
      }

    }

    /**
    * @brief   implementation of the interface Is
    * @details test if the arguments are valide
    *          parses the second argument
    *          continue if onyl the arg[1] exists as a key in the Is dictionary
    * param    arg List: arg[0] is the classname, arg[1] is element of <, > and == arg[2] is an integer
    * @return  true if the condition is meet, false other wise
    */
    bool IAttributeInterface.Is(List<string> arg)
    {
      if (!((IAttributeInterface)this).Test("Is", arg)) return false;

      int val = Int32.Parse(arg[2]);
      if (Isfunction.ContainsKey(arg[1]))
      {
        return Isfunction[arg[1]](val);
      }
      else
      {
        Debug.Log("Unknown function " + arg[1] + " in " + classManager.getClassname() + " is called in Is");
      }

      return false;
    }

    /**
    * @brief   implementation of the interface test used to tests the argument list
    * @details test the argument list of the other interfaces
    *          can be used as stand alone or as part of the other interfaces
    * parme    Interface which interface to test "Change" or "Is"
    * param    arg List: arg[0] is the classname, arg[1] is element of <, > and == arg[2] is an integer
    * @return  true if the args meet the actual needs.
    */
    bool IAttributeInterface.Test(string Interface, List<string> arg)
    {
      bool ret = true;

      switch (Interface)
      {
        case "Change":
          if (arg.Count != 3) return false;
          ret &= classManager.isCorrectClass(arg[0]);
          ret &= ChangeFunction.ContainsKey(arg[1]);
          ret &= int.TryParse(arg[2], out _);
          break;
        case "Is":
          if (arg.Count != 3) return false;
          ret &= classManager.isCorrectClass(arg[0]);
          ret &= Isfunction.ContainsKey(arg[1]);
          ret &= int.TryParse(arg[2], out _);
          break;
        default:
          Debug.Log("Wrong interface (" + Interface + ") to be tested in " + classManager.getClassname());
          ret = false;
          break;
      }

      return ret;
    }
    #endregion
  }
}
