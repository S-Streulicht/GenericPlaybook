using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using PB.Attribute;

// interesting stuff about do stuff for every thing of type ... (abstract class)
// https://stackoverflow.com/questions/49329764/get-all-components-with-a-specific-interface-in-unity

namespace PB.Logic
{
  /**
  *  @brief     Interprets a command string and execute the coresponding comand at the corect place
  *  @details   the interpreter reseives all function which can be called via Serialized fields
  *             the interpreter reseives from the parser the actual comand, the objct where it is tobe executt and an rgument list
  */
  public class CommandInterpreter : MonoBehaviour
  {
    // search https://docs.unity3d.com/ScriptReference/SerializeReference.html
    [SerializeField] GameObject AvailableAttributs;  /**< A game object which contains all relevant scripts with implements IAttributeInterface*/

    private SortedDictionary<string, IAttributeInterface> AttributPair = new SortedDictionary<string, IAttributeInterface>(); /**< Connects the classname with the asress of the class such that the interfacefunction can be called*/


    /**
    * @brief   initialize the commandinterpreter
    * @details filles the Dictionarys with the availabl interfaces 
    * @return  void
    */
    void Start()
    {
      AttributPair = GetAllComponentsWithIn<IAttributeInterface>(AvailableAttributs);
    }

    /**
    * @brief   execute a command string
    * @details first pares the string then try to execute the function according to the dictionarys
    * @param   CommandString the actual string which needs to be executed
    * @return  void
    */
    public void ExecuteCommand(string CommandString)
    {
      Command seperatedCommand = Parser.Parse(CommandString);
      if (seperatedCommand.Com == CommandRef.CHANGE_ATTRIBUTE)
      {
        string classToCall = seperatedCommand.Arguments[0];
        if (AttributPair.ContainsKey(classToCall))
        {
          AttributPair[classToCall].Change(seperatedCommand.Arguments);
        }
      }
      //Debug.Log(AttributPair[seperatedCommand.Arguments[0]].Test("Change", seperatedCommand.Arguments));
    }

    /**
    * @brief   veryfies if a given condition is valide
    * @details first pares the string then try to execute the test functionlity according to the dictionarys
    * @param   CommandString the actual string which needs to be executed
    * @return  bool true if condition is valide fals elsewise
    */
    public bool TestCommand(string CommandString)
    {
      Command seperatedCommand = Parser.Parse(CommandString);
      if (seperatedCommand.Com == CommandRef.IS_ATTRIBUTE)
      {
        string classToCall = seperatedCommand.Arguments[0];
        if (AttributPair.ContainsKey(classToCall))
        {
          return AttributPair[classToCall].Is(seperatedCommand.Arguments);
        }
      }
      //Debug.Log(AttributPair[seperatedCommand.Arguments[0]].Test("Is", seperatedCommand.Arguments));
      return false;
    }

    /**
    * @brief   get all interfaces to a sortable dictionary
    * @details get all components of the object wich a certain interface
    *          get the class name of the interface scripts
    *          add iti to the output
    * @param   HeaderObject the game object containing the Interface scripts
    * @return  a SortedDictionary with Class name and adress
    */
    private SortedDictionary<string, TInterface> GetAllComponentsWithIn<TInterface>(GameObject HeaderObject)
    {
      SortedDictionary<string, TInterface> ret = new SortedDictionary<string, TInterface>();
      TInterface[] attributes = AvailableAttributs.GetComponentsInChildren<TInterface>();

      foreach (var attribut in attributes)
      {
        string pureClassname = getClassname(attribut.ToString());
        ret.Add(pureClassname, attribut);
      }
      return ret;
    }

    /**
    * @brief   get and trim the actual name of the class
    * @param   fullQualifiedName the full name of the class
    * @return  string the actual class name
    */
    private string getClassname(string fullQualifiedName)
    {
      string actualClassname = fullQualifiedName.Split('.').Last();
      return actualClassname.Trim(')');
    }
  }
}
