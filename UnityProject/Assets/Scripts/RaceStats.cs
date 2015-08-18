using UnityEngine;
using System.Collections;

public class RaceStats : MonoBehaviour {

	public int racerID;
	public Waypoint lastWaypoint;
	public Waypoint currentWaypoint;
	public int waypointScore = 0; // race rankings
	public float positionSpeed = 0; //0.7-1.5F
	public Motor motorLink;

}
