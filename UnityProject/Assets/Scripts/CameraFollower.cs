using UnityEngine;
using System.Collections;

public class CameraFollower : MonoBehaviour {
	

	public Transform localTransform;
	
	public Vector3 offset = Vector3.back * 2.0F;
	public Transform target;
	
	private Vector3 desiredPos;
	
	
	public float refXPos;
	public float refYPos;
	public float refZPos;
	
	public float smoothTime = 0.2F;
	
	
	// Use this for initialization
	void Start () {
		
		localTransform = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		
		desiredPos = target.position + Vector3.up;
		
		desiredPos.x = Mathf.SmoothDamp(localTransform.position.x, desiredPos.x,ref refXPos, smoothTime);
		desiredPos.y = Mathf.SmoothDamp(localTransform.position.y, desiredPos.y,ref refYPos, smoothTime);
		desiredPos.z = Mathf.SmoothDamp(localTransform.position.z, desiredPos.z,ref refZPos, smoothTime);
		
		localTransform.position = desiredPos;
		
//				localTransform.LookAt(Coach.currentPlayer.transform);
	}
}
