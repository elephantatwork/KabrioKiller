using UnityEngine;
using System.Collections;

public class PolyBezier : MonoBehaviour {
	
	public BezierComplex bez = new BezierComplex();
	public float t = 0;

	public GUISkin customSkin;

	private void Start(){
		customSkin = Resources.Load("PathUI", typeof(GUISkin)) as GUISkin;
	}


	
}

