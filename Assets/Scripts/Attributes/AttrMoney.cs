using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PB.Attribute
{
  public class AttrMoney : MonoBehaviour, IAttributeInterface
  {
    [SerializeField] int Money;

    private readonly SortedDictionary<string, Action<string>> ChangeFunction = new SortedDictionary<string, Action<string>>();
    private readonly SortedDictionary<string, Func<int, bool>> Isfunction = new SortedDictionary<string, Func<int, bool>>();

    // Start is called before the first frame update
    void Start()
    {
      ChangeFunction.Add("ADD", TryAddMoney);
      ChangeFunction.Add("SUB", TrySubMoney);
      ChangeFunction.Add("SET", TrySetMoney);
      Isfunction.Add("<", x => { return Money < x; });
      Isfunction.Add(">", x => { return Money > x; });
      Isfunction.Add("==", x => { return Money == x; });
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
      if (!((IAttributeInterface)this).Test("Is", arg)) return false;
      int val = 0;
      if (Int32.TryParse(arg[2], out val))
      {
        return Isfunction[arg[1]](val);
      }

      return false;
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
          ret &= int.TryParse(arg[2], out _);
          break;
        case "Is":
          if (arg.Count != 3) return false;
          ret &= isRightClass(arg[0]);
          ret &= int.TryParse(arg[2], out _);
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
