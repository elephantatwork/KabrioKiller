﻿using UnityEngine;
using System.Collections;

//Class for setting the camera input with the input manager
public class BasicCameraInput : MonoBehaviour
{
	CameraControl cam;
	public string xInputAxis;
	public string yInputAxis;

	void Start()
	{
		//Get camera controller
		cam = GetComponent<CameraControl>();
	}

	void FixedUpdate()
	{
		//Set camera rotation input if the input axes are valid
		if (cam && !string.IsNullOrEmpty(xInputAxis) && !string.IsNullOrEmpty(yInputAxis))
		{
			cam.SetInput(Input.GetAxis(xInputAxis), Input.GetAxis(yInputAxis));
		}
	}
}
