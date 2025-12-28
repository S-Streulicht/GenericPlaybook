using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using PB.Attribute;
using PB.ExtraUI;
using PB.Helper;
using PB.Book;

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
    /**
    * @brief a standard key value pair both as strings
    */
    [System.Serializable]
    private struct pair 
    {
      public string key;   /**< key element for a key value pair */
      public string value; /**< value of a key pair elemewnt */
    }

    // search https://docs.unity3d.com/ScriptReference/SerializeReference.html
    [SerializeField] GameObject AvailableAttributs;  /**< A game object which contains all relevant scripts with implements IAttributeInterface*/
    [SerializeField] GameObject AvailableExtraUis;   /**< A game object which contains all relevant scripts with implements IExtraUIInterface*/
    [SerializeField] GameObject AvailableBooks;      /**< A game object which contains all relevant scripts wich implements IScriptBookInterface*/
    [SerializeField] List<pair> Variables;           /**< Contains a set of variables the system can operate with*/

    private readonly ClassManager<CommandInterpreter> classManager; /**< containing acces to the Helper Class: Class Manager */

    private SortedDictionary<string, IAttributeInterface> AttributPair = new SortedDictionary<string, IAttributeInterface>(); /**< Connects the classname with the asress of the class such that the interfacefunction can be called*/
    private SortedDictionary<string, IExtraUiInterface> ExtraUIPair = new SortedDictionary<string, IExtraUiInterface>();      /**< Connects the classname with the asress of the class such that the interfacefunction can be called*/
    private SortedDictionary<string, IScriptBookInterface> BookPair = new SortedDictionary<string, IScriptBookInterface>();   /**< Connects the classname with the acess of the class such that the interfacfunction can be called*/
    /**
    * @brief   constructor initialises the helper classes 
    */
    public CommandInterpreter()
    {
      classManager = new ClassManager<CommandInterpreter>(this);
    }

    /**
    * @brief   initialize the commandinterpreter
    * @details filles the Dictionarys with the available interfaces. It is in awake to limit race conditions.
    */
    void Awake()
    {
      AttributPair = GetAllComponentsWithIn<IAttributeInterface>(AvailableAttributs);
      ExtraUIPair = GetAllComponentsWithIn<IExtraUiInterface>(AvailableExtraUis);
      BookPair = GetAllComponentsWithIn<IScriptBookInterface>(AvailableBooks);
    }

    /**
    * @brief   execute a command string
    * @details first parse the string then try to execute the function according to the dictionarys
    * @param   CommandString the actual string which needs to be executed
    */
    public void ExecuteCommand(string CommandString)
    {
      Command seperatedCommand = Parser.Parse(CommandString);
      string classToCall = seperatedCommand.Result;
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
        case CommandRef.UNSET_EXTRAUI:
          if (ExtraUIPair.ContainsKey(classToCall))
          {
            ExtraUIPair[classToCall].UnSet();
          }
          break;
        case CommandRef.LOAD:
          if (BookPair.ContainsKey(classToCall))
          {
            BookPair[classToCall].Load(seperatedCommand.Arguments);
          }
          break;
        default:
          // empty on purpose
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
      string classToCall = seperatedCommand.Result;
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
        if (AttributPair.ContainsKey(classToCall))
        {
          var answer = AttributPair[classToCall].GetInfo<int>();
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
