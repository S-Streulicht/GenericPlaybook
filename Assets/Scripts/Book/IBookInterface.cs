using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PB.Book
{
  /**
  *  @brief   This defines the bare minumum of functions used to get through a book.
  *  @details The functions are suposed to be implemented in the actual Book, with static content and the storyteller.
  *           The idea is: Make them exchangeable for concerning the Director. 
  */
  public interface IBookInterface
  {
    TextElements GetRootNode();                      /**< returns the first node */
    TextElements GetNodeByRefId(string referenceId); /**< returns a node with a specific ID */
  }
}

