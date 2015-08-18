using UnityEngine;
using System.Collections;

//Class for vehicle waypoints
public class VehicleWaypoint : MonoBehaviour
{
	public VehicleWaypoint nextPoint;
	public float radius = 10;
	public AnimationCurve speedMutliplicator = AnimationCurve.EaseInOut(0, 0, 1, 1);

	[Tooltip("Percentage of a vehicle's max speed to drive at")]
	[RangeAttribute(0, 1)]
	public float speed = 1;

	public void Initialize(){
//		speed = SetSpeed();
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
		float _calc = Mathf.Clamp01(1.0F - (_angle /90.0F));
		print (_angle);
		return speedMutliplicator.Evaluate(_calc);
	}
}
