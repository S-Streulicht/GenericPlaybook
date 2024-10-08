using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PB.UI
{
  /**
  *  @brief   Simple script to evaluate the Mous down event
  *  @details The script is providing a UnityEvent for the GUI. In it the actual function to call is set as well as the number parameter
  */
  public class OnMouseDownScript : MonoBehaviour
  {
    [SerializeField] private UnityEvent ue;  /**< GUI element to set the actual callback */
    public bool enable { get; set; } = true; /**< Enables and disables the capability of clicking */

    /**
    * @brief   entry for the callback for the clickevent on the GUI
    * @details Just invokes the function set in the GUI
    */
    void OnMouseDown()
    {
      if (enable) { ue.Invoke(); }
    }
  }
}
