using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class PositionNextEntries : MonoBehaviour {
	[SerializeField]private TextBlurb m_blurb;
	// Update is called once per frame
	void Update () {
		if (m_blurb != null) {
			TextBlurb[] nextBlurbs = m_blurb.GetNextNodes ();
			Rect pixelRect = m_blurb.GetComponentInChildren<Canvas> ().pixelRect;
			for(int i = 0; i < nextBlurbs.Length; i++){
				TextBlurb nextBlurb = nextBlurbs [i];

				if (nextBlurb == null) {
					continue;
				}

				nextBlurb.transform.position = m_blurb.transform.position + new Vector3(m_blurb.GetComponentInChildren<Canvas>().pixelRect.width/10,(i+.5f)*(pixelRect.height/nextBlurbs.Length)/10 - pixelRect.height/20,0);
				Debug.Log (m_blurb.GetComponentInChildren<Canvas>().pixelRect.size);
				nextBlurb.GetComponent<PositionNextEntries> ().Update ();
			}
		}
	
	}
}
