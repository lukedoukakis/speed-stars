using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpWindowController : MonoBehaviour
{
	[SerializeField] GameObject HelpWindow;

	public void show()
	{
		HelpWindow.SetActive(true);
	}
	public void hide()
	{
		HelpWindow.SetActive(false);
	}
}
