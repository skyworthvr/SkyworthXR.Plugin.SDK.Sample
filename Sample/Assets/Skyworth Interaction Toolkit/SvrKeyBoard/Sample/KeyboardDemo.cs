using Svr.Keyboard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardDemo : MonoBehaviour {

	public SvrInputField svrInputField1;
	public SvrInputField svrInputField2;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void InputFieldValueChange(string text)
	{
		Debug.Log("InputFieldValueChange:"+text);
	}

	public void ButtonEvent()
	{
		Debug.Log("user1:"+ svrInputField1.text);
		Debug.Log("user2:"+ svrInputField2.text);
		SvrInputMethod.Instacne.HidKeyboard();
	}
}
