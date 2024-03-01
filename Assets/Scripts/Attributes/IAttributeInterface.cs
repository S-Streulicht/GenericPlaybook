using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PB.Attribute
{
  public interface IAttributeInterface
  {
    void Change(List<string> arg);
    bool Is(List<string> arg);
    bool Test(string Interface, List<string> arg);
  }
}
