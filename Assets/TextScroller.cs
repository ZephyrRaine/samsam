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
				string toAdd = texte.text.Substring(startIndex, offset);
				toAdd = toAdd.Replace(' ', '$');
				strings[i].Add(toAdd);
				startIndex+=offset;
			} while(startIndex + offset < texte.text.Length);
		}
		StartCoroutine(InstantiateTextObjects(10));
	}

	private void OnDrawGizmos()
	{
		Rect mine = GetWorldSapceRect(GetComponent<RectTransform>());
		Gizmos.color = Color.blue;
		Gizmos.DrawCube(mine.center, mine.size);
		foreach(Transform t in transform)
		{
			RectTransform tr = t.GetComponent<RectTransform>();
			Rect rect = GetWorldSapceRect(tr); 
			Gizmos.color = Color.Lerp(Color.black, Color.white, t.GetSiblingIndex()/transform.childCount);
			Gizmos.DrawCube(rect.center, rect.size);
		}
	}

	IEnumerator InstantiateTextObjects(int nb = -1, GameObject toDestroy = null)
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
			TMP_Text textComp = go.GetComponent<TMP_Text>();
			textComp.color = new Color(0f,0f,0f,0f);
			textComp.text = strings[part][objectCount++];

			Debug.Log("Part: " + part + " - Object: " + objectCount);
			yield return new WaitForEndOfFrame(); 
			
			textComp.color = gradient.Evaluate(objectCount/strings[part].Count);
			RectTransform tr = go.GetComponent<RectTransform>();		
			tr.sizeDelta = new Vector2(textComp.bounds.size.x, tr.sizeDelta.y);
			textComp.text = textComp.text.Replace('$', ' ');
			int sib = go.transform.GetSiblingIndex(); 
			if(sib != 0)
			{
				RectTransform previous = transform.GetChild(sib-1).GetComponent<RectTransform>(); 
				tr.anchoredPosition = previous.anchoredPosition + Vector2.right * previous.sizeDelta.x; 
			}
			else
			{
				tr.anchoredPosition = Vector2.zero;	
			}
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
		
		if(toDestroy != null)
		{
			Destroy(toDestroy);
			yield return new WaitForEndOfFrame();
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
				if(rt.anchoredPosition.x < -rt.sizeDelta.x)
				{
					toDestroy = t.gameObject;
				}
			}

			Debug.Log(toDestroy);

			if(toDestroy != null)
			{
				StartCoroutine(InstantiateTextObjects(1, toDestroy));
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
