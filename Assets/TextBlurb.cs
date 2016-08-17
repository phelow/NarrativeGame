using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TextBlurb : MonoBehaviour {

	/// <summary>
	/// Whether or not this is a "Driving" text blurb. If it's a driver it will kick off the chain of all following
	/// text objects.
	/// </summary>
	[SerializeField]private bool m_isDriver = false;
	[SerializeField]private string [] m_acceptedLocks;
	[SerializeField]private bool m_providesGenericKey = false;
	[SerializeField]private string m_key = "";

	[SerializeField]private bool m_requiresGenericLock;

    [TextArea(3,10)]
	[SerializeField]private string m_string = "";
	[SerializeField]private Text m_text;

	//todo: unserialize
	[SerializeField]private string [] m_words;

	[SerializeField]private TextBlurb [] m_nextNodes;
	[SerializeField]private float m_width = 3.0f;
	[SerializeField]private float m_height = 3.0f;

	private GhostWord [] m_ghosts;

	[SerializeField]private float m_lineHeight = 1.0f;

	private bool m_active = false;
	private int m_curWordTyping = 0;
	private int m_curLetterTyping = 0;
	private float c_textCharWidth = 10.0f;

	private float m_XStart = 0.0f;
	private int m_ghostsSpawned = 0;
	private TextBlurb m_parent;

	private bool m_deleteLastLetter = false;


	private bool m_completed = false;

	[SerializeField]private GameObject m_ghostSpawnPoint;

	[SerializeField]private GameObject m_ghostLetter;

	[SerializeField]private int m_minGhostLetters = 1;
	[SerializeField]private int m_maxGhostLetters = 10;

	int curGhosts = 0;


	//These two variables determine how many words to show that have not yet been typed.
	[SerializeField]private float m_minWordsRevealed;
	[SerializeField]private float m_maxWordsRevealed;

	[SerializeField]private float m_maxPositionalOffest = 3.0f;
	[SerializeField]private float m_ghostCharWidth = 1.0f;

	//A word that prompts the player to type it
	[SerializeField]private GameObject m_ghostWord;

	/// <summary>
	/// If the next key in the string has been pressed
	/// </summary>
	private bool m_nextKeyPressed = false;


	void Start () {
		m_XStart = m_text.transform.position.x;
		m_words = ParseText (m_string);
		m_ghosts = new GhostWord[m_words.Length];
		if (m_isDriver == true) {
			StartCoroutine (WriteWordsByInput());
		}
	}

	/// <summary>
	/// Parses the text, takes the text string and converts it into an array of words.
	/// </summary>
	/// <returns>List of the words in strings.</returns>
	/// <param name="inputText">The input string text that is to be parsed.</param>
	private static string [] ParseText (string inputText){
		char[] delimiters = { ' ' };
		return inputText.Split (delimiters);
	}

	void Update(){

		//Check to see if our next key is pressed
		if (m_completed == false && Input.anyKeyDown) {
			if (Input.GetKeyDown (NextKeyCode ())) {
				m_nextKeyPressed = true;
			} else {
				if(Input.GetKeyDown (KeyCode.Space)){
					//TODO: any sound effect for typing, only accept one space press
				}
				else if (!(Input.GetKeyDown(KeyCode.LeftArrow))&& !(Input.GetKeyDown(KeyCode.RightArrow))&& !(Input.GetKeyDown(KeyCode.UpArrow))&& !(Input.GetKeyDown(KeyCode.DownArrow))) {
					m_deleteLastLetter = true;
				}
			}
		}
	}

	private void InstantiateGhosts(){
		Debug.Log (100);
		//Instantiate the first x words
		StartCoroutine (KickoffGhosts ());

	}

	private void SpawnCurrentGhost(){
		if (m_curWordTyping < m_ghosts.Length) {
			if (m_ghosts [m_curWordTyping] != null) {
				Destroy (m_ghosts [m_curWordTyping].gameObject);
			}
			Vector3 newPos = new Vector3 (m_XStart + (m_curWordTyping%m_width * c_textCharWidth), transform.position.y - m_lineHeight *(m_curWordTyping/m_width), transform.position.z);
			GameObject gw = (GameObject)GameObject.Instantiate (m_ghostWord, newPos, transform.rotation);
			gw.GetComponent<GhostWord> ().SetPosition (newPos);

			m_ghosts [m_curWordTyping] = gw.GetComponent<GhostWord> ();
			m_ghosts [m_curWordTyping].GetComponent<GhostWord> ().SetWord (m_words [m_curWordTyping], .8f);
			curGhosts++;
		}
	}

	private IEnumerator KickoffGhosts(){
		int wordsToReveal = (int) Random.Range(m_minWordsRevealed, m_maxWordsRevealed) - curGhosts;
		Debug.Log ("KickoffGhosts: " + m_curWordTyping + " wordsToReveal: " + wordsToReveal + " curGhosts: " + curGhosts);
		float opacity = 1.0f;
		for (int i = m_curWordTyping; i < m_curWordTyping + wordsToReveal && i < m_words.Length; i++) {
			if (m_ghosts [i] == null) {
				Vector3 newPos = new Vector3 (m_XStart+ (i%m_width * c_textCharWidth), transform.position.y - m_lineHeight *(i/m_width), transform.position.z);
				Debug.Log ("newPos:" + newPos + " Mathf.Ceil(i/m_width):" + Mathf.Ceil(i/m_width) + "m_lineHeight: " + m_lineHeight);
				GameObject gw = (GameObject)GameObject.Instantiate (m_ghostWord, newPos, transform.rotation);
				yield return new WaitForSeconds (.1f);
				gw.GetComponent<GhostWord> ().SetPosition (newPos);
				yield return new WaitForEndOfFrame ();

				m_ghosts [i] = (gw.GetComponent<GhostWord> ());
				gw.GetComponent<GhostWord> ().SetWord (m_words [i], opacity);
				curGhosts++;
				opacity *= Random.Range (0.4f, 0.6f);
			}
		}
	}

	/// <summary>
	/// Main driving coroutine for this prefab. Called when this blurb can be activated. Listens to input text and shows
	/// text as it unfolds.
	/// </summary>
	private IEnumerator WriteWordsByInput(){

		//instantiate the following words
		m_active = true;
		InstantiateGhosts();
		m_completed = false;
		while (m_completed == false) {
			yield return new WaitForEndOfFrame ();

			//If next char in the word is currently pressed
			if (m_nextKeyPressed && m_curWordTyping < m_words.Length && m_completed == false) {

				int numLetters = Random.Range (m_minGhostLetters, m_maxGhostLetters);
				for(int i = 0; i < numLetters; i++){

					Vector3 SpawnPos = new Vector3 (m_ghosts [m_curWordTyping].transform.position.x + Random.Range(-m_maxPositionalOffest,m_maxPositionalOffest) + m_ghostCharWidth *m_curLetterTyping, m_ghosts [m_curWordTyping].transform.position.y + Random.Range(-m_maxPositionalOffest,m_maxPositionalOffest), m_ghosts [m_curWordTyping].transform.position.z + Random.Range(-m_maxPositionalOffest,m_maxPositionalOffest));
					((GameObject)GameObject.Instantiate(m_ghostLetter,SpawnPos,new Quaternion(0,0,0,0))).GetComponent<GhostLetter>().StartFalling(""+m_words[m_curWordTyping] [m_curLetterTyping]);
				}
				//write that char in the word	
				if (m_curLetterTyping < m_words [m_curWordTyping].Length) {
					m_text.text += "" + m_words [m_curWordTyping] [m_curLetterTyping];
				} else {
					m_text.text += " ";
				}
				//move your position forward
				m_curLetterTyping++;

				if (m_curLetterTyping >= m_words [m_curWordTyping].Length && m_curWordTyping < m_words.Length) {
					m_curLetterTyping = 0;
					GhostWord gw = m_ghosts[m_curWordTyping];
					m_curWordTyping++;
					curGhosts--;
					gw.FadeOut ();

					Debug.Log ("Fading out, curGhosts: " + curGhosts);
					SpawnCurrentGhost ();
					m_text.text += " ";
					//move onto the next word
				}

				if (m_curWordTyping >= m_words.Length) {
					ActivateNextNode ();
					m_completed = true;
				}

				m_nextKeyPressed = false;
			}else if (m_deleteLastLetter){
				m_deleteLastLetter = false;
				m_text.text = m_text.text.Substring (0, Mathf.Max (0, m_text.text.Length - 1));
				m_curLetterTyping--;

				Debug.Log ("m_curLetterTyping:" + m_curLetterTyping);
				if (m_curLetterTyping <= 0 && m_curWordTyping <= 0) {
					m_curLetterTyping = 0;
					m_curWordTyping = 0;
				}
				else if (m_curLetterTyping < 0) {
					Debug.Log ("190: calling spawn current ghost m_curLetterTyping:" + m_curLetterTyping);
					m_curWordTyping--;
					if (m_curWordTyping < 0) {
						m_curWordTyping = 0;
					}
					m_curLetterTyping = m_words [m_curWordTyping].Length;
					SpawnCurrentGhost ();
				}
			}

		}
	}

	private bool HasKeys(){
		bool hasKey = false;
		if (m_requiresGenericLock) {
			hasKey = LockManager.HasGenericKey () && LockManager.HasKey (m_acceptedLocks);
		} else {
			hasKey = LockManager.HasKey (m_acceptedLocks);
		}
		return hasKey;

	}

	public void Enable(TextBlurb parent){
		m_parent = parent;

		StartCoroutine (WaitForKey ());
	}

	private IEnumerator WaitForKey(){
		while (HasKeys () == false) {
			yield return new WaitForEndOfFrame ();
		}
		StartCoroutine (WriteWordsByInput ());
	}

	public void DisableChildren(){
		//disable each one one by one
		foreach (TextBlurb child in m_nextNodes) {
			child.DisableSelf ();
		}
	}

	public void DisableSelf(){
		//kill all ghosts
		foreach(GhostWord gw in m_ghosts){
			if (gw != null) {
				Destroy (gw.gameObject);
			}
		}

		//stop listening for input
		m_completed = true;
	}

	/// <summary>
	/// Activates the next set of nodes and disables this one.
	/// </summary>
	private void ActivateNextNode(){
		if (m_providesGenericKey) {
			LockManager.UnlockKeyGeneric ();
		}
		if (m_key != "") {
			LockManager.UnlockKeySpecific (m_key);
		}

		foreach (TextBlurb tb in m_nextNodes) {
			tb.Enable (this);
		}
		if (m_isDriver == false) {
			m_parent.DisableChildren ();
		}
		m_completed = true;
		m_active = false;
	}

	/// <summary>
	/// Gets and returns the KeyCode for the next character in the string
	/// </summary>
	/// <returns>The key code for the next character in the string.</returns>
	private string NextKeyCode(){
		//Determine the next character
		string nextChar;
		if (m_curLetterTyping < m_words [m_curWordTyping].Length) {
			nextChar = "" + m_words [m_curWordTyping] [m_curLetterTyping];
		} else {
			nextChar = "space";
		}
		//Compare the next character to all available keycodes, return that character

		return (string)nextChar.ToLower();
	}

}
