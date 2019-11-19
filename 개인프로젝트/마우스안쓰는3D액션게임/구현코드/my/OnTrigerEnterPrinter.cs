using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTrigerEnterPrinter : MonoBehaviour {
	public string print_message = "OnTrigerEnterPrinter";
	private void OnTriggerEnter(Collider other)
	{
		print(print_message);
	}
}
