using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PB.ExtraUI
{
  /**
  *  @brief   Interface for th exra UIs
  *  @details provides the declaration of the interface functions
  */
  public interface IExtraUiInterface
  {
    void Set(List<string> arg);  /**< set the interface component */
    void UnSet();                /**< set the state to the default */
    Ttype GetInfo<Ttype>();      /**< returns information from the interface object \todo how does nything now which type it should return? */ 
    bool Test(string Interface, List<string> arg); /**< the the corectness of the arguments for the given interface functions */
  }
}