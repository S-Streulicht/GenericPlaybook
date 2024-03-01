using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Security.Cryptography;
using System.Text;

public static class DiceNumbers
{
  static bool IfOnce = true;

  static int GetNumberOf(int Page)
  {
    //return UsingHash(Page);
    //return UsingRandom(Page);
    return UsingHashregions(Page,24);
  }

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

  static int UsingRandom(int Page)
  {
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
