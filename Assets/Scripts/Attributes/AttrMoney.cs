using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PB.Attribute
{
  /**
  *  @brief   Handles the attribut "money" 
  *  @details Besides the state f the money the class heritates from th IAttributeInterface functionality and tets are handled by this.
  */
  public class AttrMoney : MonoBehaviour, IAttributeInterface
  {
    [SerializeField] int Money; /**< the actual state of the atribut, can beset by the GUI */

    private readonly SortedDictionary<string, Action<int>> ChangeFunction = new SortedDictionary<string, Action<int>>(); /**< groups the fubctionality (change command) set by string requets to the actual action to modify the state. */
    private readonly SortedDictionary<string, Func<int, bool>> Isfunction = new SortedDictionary<string, Func<int, bool>>(); /**< groups the fubctionality (is command) set by string requets to the actual action to modify the state. */

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
        Debug.Log("Unknown function " + arg[1] + " in " + getClassname() + " is called in Change");
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
        Debug.Log("Unknown function " + arg[1] + " in " + getClassname() + " is called in Is");
      }

      return false;
    }

  /**
  * @brief   implementation of the interface test used to tests the argument list
  * @details test theargument list of the other interfaces
  *          can be used as stand alone or as part of the other interfaces
  * parme    Interface which interface to test "change" or "is"
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
        ret &= isCorrectClass(arg[0]);
        bool isArgFunctionName = false;
        foreach (var funct in ChangeFunction)
        {
          isArgFunctionName |= funct.Key == arg[1];
        }
        ret &= isArgFunctionName;
        ret &= int.TryParse(arg[2], out _);
        break;
      case "Is":
        if (arg.Count != 3) return false;
        ret &= isCorrectClass(arg[0]);
        ret &= int.TryParse(arg[2], out _);
        break;
      default:
        Debug.Log("Wrong interface (" + Interface + ") to be testerd in AttrMonney");
        ret = false;
        break;
    }

    return ret;
  }
  #endregion

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
    string actualClassname = fullQualifiedName.Split('.')[2];
    return actualClassname.Trim(')');
  }
  #endregion
}
}
