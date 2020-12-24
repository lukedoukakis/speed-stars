using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitsLineController : MonoBehaviour
{

	[SerializeField] SplitsController sc;

	void OnTriggerEnter(Collider col)
	{
		GameObject g = col.gameObject;
		if (g.tag == "Chest")
		{
			GameObject racer = g.transform.parent.parent.parent.parent.parent.parent.parent.gameObject;
			sc.register(racer);
		}
	}
}
