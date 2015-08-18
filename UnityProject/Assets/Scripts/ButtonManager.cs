using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {

	public GameObject nonVRUI;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ToggleVR(){

		Cardboard.SDK.VRModeEnabled = !Cardboard.SDK.VRModeEnabled;

		nonVRUI.SetActive(!Cardboard.SDK.VRModeEnabled);


	}

	public void DrivingControl(float _value){

//		RaceManager.instance.allCars[0].GetComponent<VehicleParent>().SetSteer(_value);
	}
}
