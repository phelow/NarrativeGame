using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LockManager : MonoBehaviour {

	private enum Key{
		Unacquired,
		Acquired,
		Used
	}

	private static LockManager ms_instance;
	[SerializeField]private string[] m_keys;
	private static int m_genericKeys = 0;
	private static Dictionary<string,Key> m_specificDictionaries;
	// Use this for initialization
	void Start () {
		ms_instance = this;
		m_specificDictionaries = new Dictionary<string, Key> ();

		foreach (string key in m_keys) {
			m_specificDictionaries.Add (key, Key.Unacquired);
		}
	}

	public static void UnlockKeyGeneric(){
		m_genericKeys++;
	}

	public static void UnlockKeySpecific(string key){
		m_specificDictionaries [key] = Key.Acquired;
	}

	public static bool HasKey(string [] locks){

		if (locks.Length == 0) {
			return true;
		}

		foreach (string l in locks) {
			if (m_specificDictionaries [l] == Key.Acquired) {
				return true;
			}
		}


		return false;
	}

	public static bool HasGenericKey(){
		return m_genericKeys > 0;
	}

	public static void ConsumeKey(string key){
		m_specificDictionaries [key] = Key.Used;
	}

	public static void ConsumeGenericKey(){
		m_genericKeys--;
	}
}
