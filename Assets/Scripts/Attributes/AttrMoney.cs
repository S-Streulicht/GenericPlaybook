using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PB.Attribute
{
  public class AttrMoney : MonoBehaviour, IAttributeInterface
  {
    [SerializeField] int Money;

    private SortedDictionary<string, Action<string>> ChangeFunction = new SortedDictionary<string, Action<string>>() /*{ 
      { "ADD", TryAddMoney }, 
      { "SUB", TrySubMoney }, 
      { "SET", TrySetMoney } }*/;

    // Start is called before the first frame update
    void Start()
    {
      ChangeFunction.Add("ADD", TryAddMoney);
      ChangeFunction.Add("SUB", TrySubMoney);
      ChangeFunction.Add("SET", TrySetMoney);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arg"> List: arg[0] is the classname, arg[1] is element of SET SUB or ADD, arg[2] is an integer</param>
    void IAttributeInterface.Change(List<string> arg)
    {
      if (!isRightClass(arg[0])) return;

      if (ChangeFunction.ContainsKey(arg[1]))
      {
        ChangeFunction[arg[1]](arg[2]);
      }
      else
      {
        Debug.Log("Unknown function " + arg[1] + " in " + getClassname() + " is called in Change");
      }
    }

    private void TryAddMoney(string arg)
    {
      int val = 0;
      if (Int32.TryParse(arg, out val))
      {
        Money += val;
      }
      else {/*empty on purpose*/}
    }

    private void TrySubMoney(string arg)
    {
      int val = 0;
      if (Int32.TryParse(arg, out val))
      {
        Money -= val;
        if (Money <= 0) { Money = 0; }
      }
      else {/*empty on purpose*/}
    }

    private void TrySetMoney(string arg)
    {
      int val = 0;
      if (Int32.TryParse(arg, out val))
      {
        Money = val;
        if (Money <= 0) { Money = 0; }
      }
      else {/*empty on purpose*/}
    }

    bool IAttributeInterface.Is(List<string> arg)
    {
      if (!isRightClass(arg[0])) return false;


      return true;
    }

    bool IAttributeInterface.Test(string Interface, List<string> arg)
    {
      bool ret = true;

      switch (Interface)
      {
        case "Change":
          if (arg.Count != 3) return false;
          ret &= isRightClass(arg[0]);
          bool isArgFunctionName = false;
          foreach (var funct in ChangeFunction)
          {
            isArgFunctionName |= funct.Key == arg[1];
          }
          ret &= isArgFunctionName;
          ret &= arg.Count == 3;
          ret &= int.TryParse(arg[2], out _);
          break;
        case "Is":
          break;
        default:
          Debug.Log("Wrong interface (" + Interface + ") to be testerd in AttrMonney");
          ret = false;
          break;
      }

      return ret;
    }

    private bool isRightClass(string className)
    {
      return getClassname() == className;
    }

    private string getClassname()
    {
      string fullQualifiedName = this.ToString();
      string actualClassname = fullQualifiedName.Split('.')[2];
      return actualClassname.Trim(')');
    }
  }
}
