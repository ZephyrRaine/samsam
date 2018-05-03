using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class blinker : MonoBehaviour {

	TMP_Text text;
	// Use this for initialization
	void Start () {
		text = GetComponent<TMP_Text>();
	}
	
	float timer;
	public float _speed;
	// Update is called once per frame
	void Update () 
	{
		timer += Time.deltaTime * _speed;;
		Color c = text.color;
		c.a = Mathf.Sin(timer) * 0.5f + 0.5f;
		text.color = c;
	}
}
