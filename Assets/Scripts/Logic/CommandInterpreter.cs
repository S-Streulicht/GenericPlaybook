using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using PB.Attribute;
using PB.ExtraUI;
using PB.Helper;

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
    [SerializeField] GameObject AvailableExtraUis;   /**< A game object which contains all relevant scripts with implements IExtraUIInterface*/

    private readonly ClassManager<CommandInterpreter> classManager; /**< containing acces to the Helper Class: Class Manager */

    private SortedDictionary<string, IAttributeInterface> AttributPair = new SortedDictionary<string, IAttributeInterface>(); /**< Connects the classname with the asress of the class such that the interfacefunction can be called*/
    private SortedDictionary<string, IExtraUiInterface> ExtraUIPair = new SortedDictionary<string, IExtraUiInterface>();      /**< Connects the classname with the asress of the class such that the interfacefunction can be called*/

    /**
    * @brief   constructor initialises the helper classes 
    */
    public CommandInterpreter()
    {
      classManager = new ClassManager<CommandInterpreter>(this);
    }

    /**
    * @brief   initialize the commandinterpreter
    * @details filles the Dictionarys with the availabl interfaces 
    * @return  void
    */
    void Start()
    {
      AttributPair = GetAllComponentsWithIn<IAttributeInterface>(AvailableAttributs);
      ExtraUIPair = GetAllComponentsWithIn<IExtraUiInterface>(AvailableExtraUis);
    }

    /**
    * @brief   execute a command string
    * @details first parse the string then try to execute the function according to the dictionarys
    * @param   CommandString the actual string which needs to be executed
    * @return  void
    */
    public void ExecuteCommand(string CommandString)
    {
      Command seperatedCommand = Parser.Parse(CommandString);
      string classToCall = seperatedCommand.Arguments[0];
      switch (seperatedCommand.Com)
      {
        case CommandRef.CHANGE_ATTRIBUTE:
          if (AttributPair.ContainsKey(classToCall))
          {
            AttributPair[classToCall].Change(seperatedCommand.Arguments);
          }
          break;
        case CommandRef.SET_EXTRAUI:
          if (ExtraUIPair.ContainsKey(classToCall))
          {
            ExtraUIPair[classToCall].Set(seperatedCommand.Arguments);
          }
          break;
      }

      //Debug.Log(AttributPair[seperatedCommand.Arguments[0]].Test("Change", seperatedCommand.Arguments));
    }

    /**
    * @brief   all script comanfdswhcih have one feedback should be in here
    * @details first parse the string then try to execute the functionlity according to the dictionarys
    * @param   CommandString the actual string which needs to be executed
    * @return  returns the answere of the Comand
    */
    public TType ReturnCommand<TType>(string CommandString)
    {
      Command seperatedCommand = Parser.Parse(CommandString);
      string classToCall = seperatedCommand.Arguments[0];
      if (seperatedCommand.Com == CommandRef.IS_ATTRIBUTE)
      {

        if (AttributPair.ContainsKey(classToCall))
        {
          var answer = AttributPair[classToCall].Is(seperatedCommand.Arguments);
          return (TType)System.Convert.ChangeType(answer, typeof(TType));
        }
      }
      if (seperatedCommand.Com == CommandRef.GET_INFO)
      {
        if (ExtraUIPair.ContainsKey(classToCall))
        {
          var answer = ExtraUIPair[classToCall].GetInfo<string>();
          return (TType)System.Convert.ChangeType(answer, typeof(TType));
        }
      }
      //Debug.Log(AttributPair[seperatedCommand.Arguments[0]].Test("Is", seperatedCommand.Arguments));
      return default(TType);
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
      TInterface[] interfaces = HeaderObject.GetComponentsInChildren<TInterface>();

      foreach (var interfaceVar in interfaces)
      {
        string pureClassname = classManager.getClassnameByString(interfaceVar.ToString());
        ret.Add(pureClassname, interfaceVar);
      }
      return ret;
    }
  }
}
