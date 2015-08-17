using UnityEngine;
using System.Collections;

public class CameraControlls : MonoBehaviour {

	public Transform cameraTarget;
	private Transform localTransform;

	public Vector3 targetOffset;
	public Vector3 targetPosition;
	public Vector3 smoothPosition;

	private Vector3 smoothVelocity;
	public float smoothTime;

	// Use this for initialization
	void Start () {
	
		localTransform = this.transform;
	}
	
	// Update is called once per frame
	void Update () {

		if(cameraTarget){
			targetPosition = cameraTarget.transform.position + targetOffset;

			smoothPosition.x = Mathf.SmoothDamp(localTransform.position.x, targetPosition.x, ref smoothVelocity.x, smoothTime);
			smoothPosition.y = Mathf.SmoothDamp(localTransform.position.y, targetPosition.y, ref smoothVelocity.y, smoothTime);
			smoothPosition.z = Mathf.SmoothDamp(localTransform.position.z, targetPosition.z, ref smoothVelocity.z, smoothTime);

			localTransform.position = smoothPosition;

			localTransform.LookAt(cameraTarget);
		}
	}
}
