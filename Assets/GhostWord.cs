using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GhostWord : MonoBehaviour {
	[SerializeField]TextMesh m_text;
	[SerializeField]Rigidbody2D m_rigidbody;

	private string m_originalText;
	private int m_textIndex;
	private TextBlurb m_textBlurb;

	Vector2 m_centralPosition;

	[SerializeField]private float m_minLerpTime = 1.0f;
	[SerializeField]private float m_maxLerpTime = 5.0f;
	[SerializeField]private float m_maxDistFromCenter = 5.0f;

	// Use this for initialization
	void Start () {
		m_text.color = Color.white;
	}

	// Update is called once per frame
	void Update () {
		if (m_textBlurb.GetCurrentWord() > m_textIndex) {
			return;
		}

		if (m_textBlurb.GetCurrentWord() < m_textIndex) {
			m_text.text = m_originalText;
			return;
		}

		if (m_textBlurb.GetCurrentLetter () < 0) {
			return;
		}
		
		m_text.text = m_originalText.Substring (m_textBlurb.GetCurrentLetter(),m_originalText.Length - m_textBlurb.GetCurrentLetter());
	}

	public void FadeOut(){
		StartCoroutine (FadeOutRoutine ());
	}

	private IEnumerator FadeOutRoutine(){
		float t = 0.0f;
		float fadeOutTime = Random.Range (m_minLerpTime, m_maxLerpTime);
		Color origColor = m_text.color;
		Color invis = new Color (0.0f, 0.0f, 0.0f, 0.0f);
		while (t < fadeOutTime) {
			t += Time.deltaTime;
			m_text.color = Color.Lerp (origColor, invis, t/fadeOutTime);
			yield return new WaitForEndOfFrame ();
		}
		Destroy (this.gameObject);
	}

	public void SetWord(string newText, float opacity, TextBlurb textBlurb, int textIndex){
		m_text.text = newText;
		m_originalText = newText;
		m_textBlurb = textBlurb;
		m_textIndex = textIndex;
		this.gameObject.AddComponent <BoxCollider2D>();
		m_text.color = new Color (1.0f, 1.0f, 1.0f, opacity);
	}

	public void SetPosition(Vector2 newPosition){
		m_centralPosition = newPosition;
		transform.position = newPosition;
		StartCoroutine (InterpolatePosition ());
	}

	private IEnumerator InterpolatePosition(){
		yield return new WaitForEndOfFrame ();

		transform.position = m_centralPosition;
		while (true){
			Vector3 origPosition = new Vector3 (transform.position.x, transform.position.y,transform.position.z);
			Vector3 newPosition = new Vector3 (m_centralPosition.x + Random.Range (-m_maxDistFromCenter, m_maxDistFromCenter), m_centralPosition.y + Random.Range (-m_maxDistFromCenter, m_maxDistFromCenter),transform.position.z);
			float t = 0.0f;

			float lerpTime = Random.Range (m_minLerpTime, m_maxLerpTime);
			while (t < lerpTime) {
				t += Time.deltaTime;
				m_rigidbody.AddForce (Time.deltaTime* 1000.0f *(newPosition-transform.position));
				yield return new WaitForEndOfFrame();
			}

			yield return new WaitForEndOfFrame();
		}
	}
}
