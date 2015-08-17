using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

	public static GUIManager instance {get; private set;}	

	public GameObject popUpMessage;
	public Text popUpMessageText;

	void Awake(){
		instance = this;
	}

	private void Start(){
		GameObject _ui = GameObject.Find("UI");

		popUpMessage = _ui.transform.FindChild("Message").gameObject;
		popUpMessageText = popUpMessage.GetComponent<Text>();
		StopMessage();
	}

	public void ShowMessage(string _content){

		popUpMessageText.text = _content;

		popUpMessage.SetActive(true);

		StartCoroutine(DelayHideMessage(5.0F));
	}

	private void HideMessage(){

		popUpMessage.SetActive(false);
	}

	private IEnumerator DelayHideMessage(float _delay){

		yield return new WaitForSeconds(_delay);

		HideMessage();

	}

	public void StopMessage(){

		HideMessage();
	}
}
