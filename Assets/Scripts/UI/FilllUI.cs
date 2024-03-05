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
    private TextMeshPro Answer1Text;       /**< actual element containign the text of the first answer, autofilled from the corespondign GameObject */
    [SerializeField] GameObject Answer2;   /**< Place of the second answer need to contain a TextMashPro text element relevant for dis (en) abeling the the answer, filled from the Unity GUI */
    private TextMeshPro Answer2Text;       /**< actual element containign the text of the second answer, autofilled from the corespondign GameObject */
    [SerializeField] GameObject Answer3;   /**< Place of the thired answer need to contain a TextMashPro text element relevant for dis (en) abeling the the answer, filled from the Unity GUI */
    private TextMeshPro Answer3Text;       /**< actual element containign the text of the thired answer, autofilled from the corespondign GameObject */

    private Director Director;             /**< Link to the director filled in the start by searching the game object "Director" */

    /**
    * @brief setup the FillUI
    * @details Just initialisation no prefilling with information
    *          find the Director component
    *          get the actual text component of the Gameobhect Answer[x]
    * @return void
    */
    void Start()
    {
      Director = GameObject.Find("Director").GetComponent<Director>();
      Answer1Text = Answer1.GetComponentsInChildren<TextMeshPro>()[0];
      Answer2Text = Answer2.GetComponentsInChildren<TextMeshPro>()[0];
      Answer3Text = Answer3.GetComponentsInChildren<TextMeshPro>()[0];
    }

    /**
    * @brief   every frame the gui element is fileld with information
    * @details getting the actual main text from the director
    *          getting the all valide answers
    *          setting the answerfiled, if answer is not valid : deactivate the game object
    *          if valide: activate the object and display answer
    *          /todo this is highly inefficent -> move to is changed
    * @return  void
    */
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
