using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

	public void LoadLevel(string _levelID){
		Application.LoadLevel(_levelID);
	}
}
