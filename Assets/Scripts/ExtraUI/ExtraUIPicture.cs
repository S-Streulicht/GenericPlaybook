using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PB.Helper;

namespace PB.ExtraUI
{
  /**
  *  @brief   Handles the extra UI "Picture" 
  *  @details Besides the picture, the class heritates from the IExtraUiInterface functionality and tests are handled by this.
  */
  public class ExtraUIPicture : MonoBehaviour, IExtraUiInterface
  {
    private readonly ClassManager<ExtraUIPicture> classManager; /**< containing acces to the Helper Class: Class Manager */

    [SerializeField] Texture2D Picture = null; /**< picture to display */

    /**
    * @brief   constructor initialises the helper classes 
    */
    public ExtraUIPicture()
    {
      classManager = new ClassManager<ExtraUIPicture>(this);
    }

    /**
    * @brief   Load a picture from the resorces
    * @details the picture is given sa second argument without an extention
    * @param   arg List: arg[0] is the class name and arg[1] the the picture referenced by name.
    */
    void IExtraUiInterface.Set(List<string> arg)
    {
      if (!((IExtraUiInterface)this).Test("Set", arg)) return;

      Picture = Resources.Load(arg[1]) as Texture2D;
    }

    /**
    * @brief   set the picture to the default
    * @details set the picture to null
    */
    void IExtraUiInterface.UnSet()
    {
      Picture = null;
    }

    /**
    * @brief   returns the actual picture as Texture2D
    * @details if TType is a Texture2D the actual Picture is returned else the default of the actual type.
    * @return  the picture to be displayed
    */
    Ttype IExtraUiInterface.GetInfo<Ttype>()
    {
      return Picture is Ttype value ? value : default(Ttype);
    }

    /**
    * @brief   implementation of the interface test used to tests the argument list
    * @details Since only Set has parameters this is the only one to test. and even there
    *          ther is not much to test since the picture is referenced by string
    * param    Interface which interface to test "Set" or "UnSet" or "GetInfo"
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
          // no further tests done since the texture is anyhow referenced by string
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