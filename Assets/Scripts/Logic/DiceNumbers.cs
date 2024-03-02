using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Security.Cryptography;
using System.Text;

namespace PB.Logic
{
  /**
  *  @brief     Provides a mapping between a (page) number and a pseudo random dice number (reproducable)
  *  @details   given a page number the class return a number 1 to 6
  *             several different algorithmn are implemented with individual (dis) advantages
  */
  public static class DiceNumbers
  {
    static bool IfOnce = true; /**< relevant for the initialisation of the standar Random implementation /todo get it in the constructor*/

    /**
    * @brief get the pseudo dice number of a (page) number
    * @details return the result of the individual implementations
    * @param Page positive integer number
    * @return number 1 to 6 depending on the Page
    */
    static int GetNumberOf(int Page)
    {
      //return UsingHash(Page);
      //return UsingRandom(Page);
      return UsingHashregions(Page, 24);
    }

    /**
    * @brief get the pseudo dice number of a (page) number using a Hash algorithm
    * @details calculates te SHA256 hash of the pagnumber
    *          get the fist 8 charater
    *          interpret them as a number and returns it modulu 6 + 1
    *          Drawback it is puryly deterministic random, which means the first few hunderet numbers dosent have a equal distribution of each of the outputs
    *          We have a loaded dice
    *          Advantage it it a direct on to one translation
    * @param Page positive integer number
    * @return number 1 to 6 depending on the Page
    */
    static int UsingHash(int Page)
    {
      //int[] ans = new int[6];
      using (HashAlgorithm algorithm = SHA256.Create())
      {
        //for (int i = 1; i < 20000; i++)
        {

          byte[] b = algorithm.ComputeHash(Encoding.UTF8.GetBytes(Page.ToString()));
          string oString = "";
          foreach (var by in b)
          {
            oString += by.ToString("X2");
          }
          // For statistic reason
          //Debug.Log(Convert.ToInt64(oString.Substring(0,8).ToLower(),16) % 6 + 1);
          //ans[Convert.ToInt64(oString.Substring(0,8).ToLower(),16) % 6 ]++;
          return (int)(Convert.ToInt64(oString.Substring(0, 8).ToLower(), 16) % 6 + 1);
        }
      }
    }

    /**
    * @brief get the pseudo dice number of a (page) number using a standard random algorithm
    * @details initialise the Random function Unity
    *          get a random number in the range of 1 to 6
    *          Drawback it is puryly deterministic random, which means the first few hunderet numbers dosent have a equal distribution of each of the outputs
    *          We have a loaded dice
    *          Advantage it it a direct on to one translation
    * @param Page positive integer number
    * @return number 1 to 6 depending on the Page
    */
    static int UsingRandom(int Page)
    {
      // /todo get the IfOnce into the constructor
      if (IfOnce)
      {
        UnityEngine.Random.InitState(42);
        IfOnce = false;
      }
      return UnityEngine.Random.Range(1, 7);
      /*for (int i = 1; i < 20000; i++)
      {
        ans[UnityEngine.Random.Range(0, 6)]++;
      }
      foreach (var a in ans)
        Debug.Log(a / 3333.0f);
      */
    }

    /**
    *  @brief     provides a pair to cantain a number and a hash (string coded)
    *  @details   an integger number and and a hash in the format byte[] needs to be provided
    *             the has gets stored as a string
    *             the class implements IComparable which means an array of it can be sorted
    */
    private struct NumHash : IComparable
    {
      public NumHash(int num, byte[] hash)
      {
        Num = num;
        Hash = "";
        ConvertToString(hash);
      }

      public int Num { get; }
      public string Hash { get; private set; }
      public override string ToString() => $"({Num}, {Hash})";

      private void ConvertToString(byte[] hash)
      {
        foreach (var b in hash)
        {
          Hash += b.ToString("X2");
        }
      }

      public int CompareTo(object obj)
      {
        if (this.GetType() != obj.GetType())
        {
          throw (new ArgumentException(
                 "Both objects being compared must be of type NumHash."));
        }
        else
        {
          NumHash numHash2 = (NumHash)obj;
          return String.Compare(numHash2.Hash, this.Hash);
        }
      }
    }

    /**
    * @brief get the pseudo dice number of a (page) number using a goupt of Range numbers sorted by hash
    * @details calculate the hash values of all number within the range between floor and ceil ofthe number mod Range
    *          sort the number acording to their hashs
    *          output the number at the original position of the page modulu 6 + 1
    *          Drawback it is not directly a one to one translation for page to dice number
    *          THe dice is only slightly loaded for the pages above number of total pages mod Range
    *          Advantage: the dice is onyl slightly loaded.
    * @param Page positive integer number
    * @param Range positive integer of the range should be an multiple of 6 should not be too high default is 24
    * @return number 1 to 6 depending on the Page
    */
    static int UsingHashregions(int Page, int Range = 24)
    {
      using (HashAlgorithm algorithm = SHA256.Create())
      {
        int LowerLimit = (int)(Math.Floor(((float)Page + 0.5f) / (float)Range) * (float)Range);
        int UpperLimit = (int)(Math.Ceiling(((float)Page + 0.5f) / (float)Range) * (float)Range);
        NumHash[] numHash = new NumHash[Range];
        for (int i = LowerLimit; i < UpperLimit; i++)
        {
          numHash[i % Range] = new NumHash(i, algorithm.ComputeHash(Encoding.UTF8.GetBytes(i.ToString())));
          //Debug.Log(numHash[i % Range]);
        }
        Array.Sort(numHash);
        /*for (int i = 0; i < Range; i++)
        {
          Debug.Log(numHash[i].Num % 6 + 1);
        }*/
        return numHash[Page % Range].Num % 6 + 1;
      }

    }
  }
}