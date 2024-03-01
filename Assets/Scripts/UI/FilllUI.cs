using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using PB.Book;
using PB.Logic;

namespace PB.UI
{
  public class FilllUI : MonoBehaviour
  {
    [SerializeField] TextMeshPro MainText;
    [SerializeField] GameObject Answer1;
    private TextMeshPro Answer1Text;
    [SerializeField] GameObject Answer2;
    private TextMeshPro Answer2Text;
    [SerializeField] GameObject Answer3;
    private TextMeshPro Answer3Text;

    private Director Director;
    // Start is called before the first frame update
    void Start()
    {
      Director = GameObject.Find("Director").GetComponent<Director>();
      Answer1Text = Answer1.GetComponentsInChildren<TextMeshPro>()[0];
      Answer2Text = Answer2.GetComponentsInChildren<TextMeshPro>()[0];
      Answer3Text = Answer3.GetComponentsInChildren<TextMeshPro>()[0];
    }

    // Update is called once per frame
    void Update()
    {
      MainText.text = Director.GetText();
      List<string> valideAnswers = Director.GetValideAnswers();
      if (valideAnswers != null && valideAnswers.Count > 0)
      {
        Answer1.SetActive(true);
        Answer1Text.text = Director.GetValideAnswers()[0];
      }
      else
      {
        Answer1.SetActive(false);
        Answer1Text.text = "";
        ///ToDo get systemstate may be we are done
      }
      if (valideAnswers.Count > 1)
      {
        Answer2.SetActive(true);
        Answer2Text.text = Director.GetValideAnswers()[1];
      }
      else
      {
        Answer2.SetActive(false);
        Answer2Text.text = "";
      }
      if (valideAnswers.Count > 2)
      {
        Answer3.SetActive(true);
        Answer3Text.text = Director.GetValideAnswers()[2];
      }
      else
      {
        Answer3.SetActive(false);
        Answer3Text.text = "";
      }
    }

    public void OnAnswerClickEvent(int numberOfAnswer)
    {
      Director.SetAnswer(numberOfAnswer);
    }
  }
}
