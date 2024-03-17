using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PB.Helper
{
  /**
  *  @brief   Handles Class names 
  *  @details during initalisation a class pointer of the relevant instance is stored. From this the name is derived.
  */
  public class ClassManager<TType>
  {
    private TType prtOfClass = default(TType);  /**< stors the attache class pointe "this" frome the calling class */

    /**
    * @brief   constructor of the helper class sets the name of the class
    * @details sets the name of the Class to handle#
    * @param   classPtr pointer to the calling class usually "this"
    */
    public ClassManager(TType classPtr)
    {
      prtOfClass = classPtr;
    }

    /**
    * @brief   test if the inputstring is the class name
    * @details the function is using the shortend name, not the full qualified one.
    * param    className the name to be tests
    * @return  true if the input is equal to the actual classe
    */
    public bool isCorrectClass(string className)
    {
      return getClassname() == className;
    }

    /**
    * @brief   get and trim the actual name of the class
    * @return  string the actual class name
    */
    public string getClassname()
    {
      string fullQualifiedName = prtOfClass.ToString();
      string actualClassname = fullQualifiedName.Split('.').Last();
      return actualClassname.Trim(')');
    }

    /**
    * @brief   get and trim the actual name of the class
    * @param   fullQualifiedName the full name of the class
    * @return  string the actual class name
    */
    public string getClassnameByString(string fullQualifiedName)
    {
      string actualClassname = fullQualifiedName.Split('.').Last();
      return actualClassname.Trim(')');
    }
  }
}
