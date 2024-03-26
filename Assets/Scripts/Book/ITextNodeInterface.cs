using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PB.Book
{
  /**
  *  @brief   This defines the bare minumum of functions an text element should have to be usable by the Director.
  *  @details The functions are suposed to be implemented in the actual Textelements, with static content and the storyteller.
  *           The idea is: Make them exchangeable for concerning the Director. 
  */
  public interface ITextNodeInterface
  {
    int TextNumber { get; }       /**< geting the number of an node */
    string Text { get; }          /**< getting the text of an node */
    List<string> Comands { get; } /**< get a list of comand attached to the node supposed to be executedn on first aperance*/
    List<JumpTo> JumpTos { get; } /**< get a list of jumpTos attached to the node */
  }
}