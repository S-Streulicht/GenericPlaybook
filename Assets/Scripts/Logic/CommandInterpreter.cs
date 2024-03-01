using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PB.Attribute;

// interesting stuff about do stuff for every thing of type ... (abstract class)
// https://stackoverflow.com/questions/49329764/get-all-components-with-a-specific-interface-in-unity

namespace PB.Logic
{
  public class CommandInterpreter : MonoBehaviour
  {
    // search https://docs.unity3d.com/ScriptReference/SerializeReference.html
    [SerializeField] GameObject AvailableAttributs;

    private SortedDictionary<string, IAttributeInterface> AttributPair = new SortedDictionary<string, IAttributeInterface>();

    //private const SortedDictionary<string, string> AttributComand = new SortedDictionary<string, string>() { { "CHANGE", "CHANGE" }, { "IS", "IS" } };


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
      GetAllComponentsWithIAttributeInterface();
    }

    public void ExecuteCommand(string CommandString)
    {
      Command seperatedCommand = Parser.Parse(CommandString);
      // ToDo stuff
      AttributPair[seperatedCommand.Arguments[0]].Change(seperatedCommand.Arguments);
      //Debug.Log(AttributPair[seperatedCommand.Arguments[0]].Test("Change", seperatedCommand.Arguments));
    }

    private void GetAllComponentsWithIAttributeInterface()
    {
      IAttributeInterface[] attributes = AvailableAttributs.GetComponentsInChildren<IAttributeInterface>();

      foreach (var attribut in attributes)
      {
        string fullQualifiedName = attribut.ToString();
        string actualClassname = fullQualifiedName.Split('.')[2];
        string pureClassname = actualClassname.Trim(')');
        AttributPair.Add(pureClassname, attribut);
      }
    }
  }
}
