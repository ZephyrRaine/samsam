using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TextScroller : MonoBehaviour {

	public GameObject prefab;
	public GameObject prefabImage;
	public TextAsset[] textes;
	public Sprite[] images;

	public Gradient gradient;
	List<string>[] strings;
	// Use this for initialization
	int objectCount;
	int part;
	void Start () 
	{
		Debug.Assert(textes.Length == images.Length);
		strings = new List<string>[textes.Length];
		for(int i = 0; i < textes.Length; i++)
		{
			int startIndex = 0;
			int offset = 1000;
			TextAsset texte = textes[i];
			strings[i] = new List<string>();
			do
			{
				int lastIndex = startIndex + offset;
				if(lastIndex > texte.text.Length)
					lastIndex = texte.text.Length;
				strings[i].Add(texte.text.Substring(startIndex, offset));
				startIndex+=offset;
			} while(startIndex + offset < texte.text.Length);
		}
		StartCoroutine(InstantiateTextObjects(10));
	}

	IEnumerator InstantiateTextObjects(int nb = -1)
	{
		if(nb == -1)
		{
			nb = 0;
			for(int i = 0; i < strings.Length; i++)
			{
				nb += strings[i].Count;
			}
		}
		for(int i = 0; i < nb; i++)
		{
			GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity,transform);
			go.GetComponent<TMP_Text>().color = gradient.Evaluate(objectCount/strings[part].Count);
			go.GetComponent<TMP_Text>().text = strings[part][objectCount++];

			Debug.Log("Part: " + part + " - Object: " + objectCount);
			yield return new WaitForEndOfFrame(); 		
			go.GetComponent<RectTransform>().sizeDelta = (Vector2)(go.GetComponent<TMP_Text>().bounds.size);
			if(objectCount >= strings[part].Count)
			{
				go = Instantiate(prefabImage, Vector3.zero, Quaternion.identity, transform);
				go.GetComponent<Image>().sprite = images[part];
				objectCount = 0;
				part++;
				if(part >= strings.Length)
					part = 0;
				yield return new WaitForEndOfFrame();
			}
		}
		ready = true;
	}	
	bool ready;
	// Update is called once per frame
	public float speed;
	void Update () 
	{
		if(ready)
		{
			RectTransform rtt = GetComponent<RectTransform>();
			GameObject toDestroy = null;
			foreach(Transform t in transform)
			{
				RectTransform rt = t.GetComponent<RectTransform>();
				rt.anchoredPosition += Vector2.left * speed * Time.deltaTime;
				// Debug.Log(rt.rect);
				if(!GetWorldSapceRect(rt).Overlaps(GetWorldSapceRect(rtt)) && rt.position.x < rtt.position.x)
				{
					toDestroy = t.gameObject;
				}
			}

			if(toDestroy != null)
			{
				Destroy(toDestroy);
				StartCoroutine(InstantiateTextObjects(1));
			}
		}
	}

	 Rect GetWorldSapceRect(RectTransform rt)
 {
     var r = rt.rect;
     r.center = rt.TransformPoint(r.center);
     r.size = rt.TransformVector(r.size);
     return r;
 }
}
