using UnityEngine;
using System.Collections;

//Class for vehicle waypoints
public class VehicleWaypoint : MonoBehaviour
{
	public VehicleWaypoint nextPoint;
	public float radius = 10;

	[Tooltip("Percentage of a vehicle's max speed to drive at")]
	[RangeAttribute(0, 1)]
	public float speed = 1;

	public void Initialize(){
		speed = SetSpeed();
	}

	void OnDrawGizmos()
	{
		//Visualize waypoint
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, radius);

		//Draw line to next point
		if (nextPoint)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(transform.position, nextPoint.transform.position);
		}
	}

	private float SetSpeed(){

		float _angle = Vector3.Angle(this.transform.position, nextPoint.transform.position);

		return _angle /180.0F;
	}
}
