using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnMouseDownScript : MonoBehaviour
{
  [SerializeField] private UnityEvent ue;

	void OnMouseDown()
	{
    ue.Invoke();
	}
}
