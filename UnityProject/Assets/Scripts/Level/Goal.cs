using UnityEngine;
using System.Collections;

public class Goal : Waypoint {	

	public override void EnterAction (RaceStats _stats)
	{
		base.EnterAction (_stats);

		if(_stats.lastWaypoint == this){
			RaceManager.instance.CompletedLap(_stats.racerID);
			_stats.lastWaypoint = null;
		}
	}
}
