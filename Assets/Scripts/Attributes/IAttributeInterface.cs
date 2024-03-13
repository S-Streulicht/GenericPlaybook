using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PB.Attribute
{
  /**
  *  @brief   Interface for the attributes
  *  @details provides the declaration of the interface functions
  */
  public interface IAttributeInterface
  {
    void Change(List<string> arg);                 /**< If called the relevant atribute gets changed acording to the arguments */
    bool Is(List<string> arg);                     /**< If called the state of the atribute gets tested acording to the arguments */
    bool Test(string Interface, List<string> arg); /**< the the corectness of the arguments for the given interface functions */
  }
}
