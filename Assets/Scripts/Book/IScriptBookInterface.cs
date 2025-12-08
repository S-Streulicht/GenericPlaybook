using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PB.Book
{
  /**
  *  @brief   Interface to load a book from another book
  *  @details Enable acess to the Fightstorry teller or any other book in a book
  */
  public interface IScriptBookInterface
  {
    void Load(List<string> arg);                   /**< Loads the particular Book */
    Ttype GetInfo<Ttype>();                        /**< returns information which book is currently used*/
    bool Test(string Interface, List<string> arg); /**< the the corectness of the arguments for the given interface functions */
  }
}