using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using PB.Book;
using PB.Logic;

namespace PB.UI
{
  /**
  *  @brief     Fill the GUI with content
  *  @details   This is just a placeholder class providing a playable boog, but as a kind of tech demo.
  *             /todo exchange the GUI for a more advanced display
  */
  public class FilllUI : MonoBehaviour
  {
    [SerializeField] TextMeshPro MainText; /**< Place of the main text, filled from the Unity GUI */
    [SerializeField] GameObject Answer1;   /**< Place of the first answer need to contain a TextMashPro text element relevant for dis (en) abeling the the answer, filled from the Unity GUI */
    [SerializeField] GameObject Answer2;   /**< Place of the second answer need to contain a TextMashPro text element relevant for dis (en) abeling the the answer, filled from the Unity GUI */
    [SerializeField] GameObject Answer3;   /**< Place of the thired answer need to contain a TextMashPro text element relevant for dis (en) abeling the the answer, filled from the Unity GUI */
    [SerializeField] GameObject GameState; /**< The place to display if the game ended */
    private Director Director;             /**< Link to the director filled in the start by searching the game object "Director" */

    private TAnswer[] Answers = { };       /**< chached array of text components */
    private TextMeshPro StateText;         /**< the actual text of the state */

    /**
    *  @brief   provides a pair to cantain a gameobect and its coresponding text field
    */
    private struct TAnswer
    {
      /**
      * @brief  structure to combine the gameobject of the answerfield with its text element and the click ability
      * @param  obj the game object
      * @param  text the text element
      * @param  click the ability to click on the answer
      * @param  renderer the actual renderer of the object
      */
      public TAnswer(GameObject obj, TextMeshPro text, OnMouseDownScript click, Renderer renderer)
      {
        AnswerObject = obj;
        AnswerText = text;
        AnswerClick = click;
        AnswerRenderer = renderer;
      }

      public GameObject AnswerObject { get; }        /**< The game Object of the answer field */
      public TextMeshPro AnswerText { get; }         /**< The actual Textelement of the Answer field */
      public OnMouseDownScript AnswerClick { get; }  /**< The actual Mouseevent script to (dis) enable clicking */
      public Renderer AnswerRenderer { get; }        /**< The actual render of the object */
      public override string ToString() => $"({AnswerObject}: {AnswerText})"; /**< provides the ToString function */

    }

    /**
    * @brief setup the FillUI
    * @details Just initialisation no prefilling with information
    *          find the Director component
    *          fill the answers object by finding the individual component of the Answers
    *          cache the text of the state object
    * @return void
    */
    void Start()
    {
      Director = GameObject.Find("Director").GetComponent<Director>();
      Answers = new TAnswer[3];
      Answers[0] = new TAnswer(Answer1,
                               Answer1.GetComponentsInChildren<TextMeshPro>()[0],
                               Answer1.GetComponent<OnMouseDownScript>(),
                               Answer1.GetComponent<Renderer>());
      Answers[1] = new TAnswer(Answer2,
                               Answer2.GetComponentsInChildren<TextMeshPro>()[0],
                               Answer2.GetComponent<OnMouseDownScript>(),
                               Answer2.GetComponent<Renderer>());
      Answers[2] = new TAnswer(Answer3,
                               Answer3.GetComponentsInChildren<TextMeshPro>()[0],
                               Answer3.GetComponent<OnMouseDownScript>(),
                               Answer3.GetComponent<Renderer>());
      StateText = GameState.GetComponentsInChildren<TextMeshPro>()[0];
    }

    /**
    * @brief   every frame the gui element is filled with information
    * @details getting the actual main text from the director
    *          deactiavte all answers
    *          getting all valide answers
    *            setting the answer field,
    *            activate the object and display answer
    *          get all invalide answers
    *            setting the answer filed,
    *            activate the object but disable clicking
    *          get the state of the game
    *            if in default dont display the gameobject els display with actual information
    *          /todo this is highly inefficent -> move to is changed
    * @return  void
    */
    void Update()
    {
      MainText.text = Director.GetText();

      foreach (TAnswer answer in Answers)
      {
        answer.AnswerObject.SetActive(false);
        answer.AnswerText.text = "";
        answer.AnswerClick.enable = false;
        answer.AnswerRenderer.material.color = Color.green;
      }

      List<string> valideAnswers = Director.GetValideAnswers();
      if (valideAnswers != null)
      {
        for (int i = 0; i < valideAnswers.Count; i++)
        {
          Answers[i].AnswerObject.SetActive(true);
          Answers[i].AnswerText.text = valideAnswers[i];
          Answers[i].AnswerClick.enable = true;
        }
      }

      List<string> inValideAnswers = Director.GetInValideAnswers();
      if (inValideAnswers != null)
      {
        for (int i = valideAnswers.Count; i < valideAnswers.Count + inValideAnswers.Count; i++)
        {
          Answers[i].AnswerObject.SetActive(true);
          Answers[i].AnswerRenderer.material.color = Color.yellow;
          Answers[i].AnswerText.text = inValideAnswers[i - valideAnswers.Count];
        }
      }

      string state = Director.GetGameState();
      StateText.text = state;
      Debug.Log(state);
      if (state == "Default")
      {
        GameState.SetActive(false);
      }
      else
      {
        GameState.SetActive(true);
      }
    }

    /**
    * @brief   callback for the clickevent on the GUI
    * @details The callback is set in the Unit GUI by an On mouse down event script
    *          In the GUI the actual number of the answer is also set for each Gui element
    *          Base on the event the Director is called toset the answer 
    * @param   numberOfAnswer the actual number of the Answer set in the GUI
    * @return void
    */
    public void OnAnswerClickEvent(int numberOfAnswer)
    {
      Director.SetAnswer(numberOfAnswer);
    }
  }
}
