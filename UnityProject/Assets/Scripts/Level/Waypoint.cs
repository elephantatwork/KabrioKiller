﻿using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {

	public Waypoint nextWaypoint;

	// Update is called once per frame
	public void OnTriggerEnter (Collider _other) {

		if(_other.tag == "Car")
			EnterAction(_other.transform.parent.parent.parent.GetComponent<RaceStats>());
	}

	public virtual void EnterAction(RaceStats _stats){

		if(_stats != null){

			if(_stats.currentWaypoint == this){
				_stats.lastWaypoint = _stats.currentWaypoint;
				_stats.currentWaypoint = nextWaypoint;

			}
		}else{
			Debug.LogWarning("No RaceStats!");
		}
	}
}