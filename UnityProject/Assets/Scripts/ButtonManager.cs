using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {

	public GameObject nonVRUI;
	public GameObject preGameChoice;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	public void StartGame (bool _vr) {
	
		RaceManager.instance.InitiateGame();

		Cardboard.SDK.VRModeEnabled = _vr;
		
		nonVRUI.SetActive(!_vr);

		GameObject.Find("CardboardMain").GetComponent<Cardboard>().enabled = _vr;

		preGameChoice.SetActive(false);
	}

	public void ToggleVR(){

		GameObject.Find("CardboardMain").GetComponent<Cardboard>().enabled = false;

		Cardboard.SDK.VRModeEnabled = !Cardboard.SDK.VRModeEnabled;

		nonVRUI.SetActive(!Cardboard.SDK.VRModeEnabled);


	}

	public void DrivingControl(float _value){

		RaceManager.instance.allCars[0].GetComponent<VehicleParent>().SetSteer(_value);
	}
}
