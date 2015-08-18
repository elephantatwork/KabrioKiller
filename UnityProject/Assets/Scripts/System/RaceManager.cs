using UnityEngine;
using System.Collections;

public class RaceManager : MonoBehaviour {

	public static RaceManager instance {get; private set;}	

	public GameObject carPrefab;

	public Transform levelObject;
	public Waypoint[] waypoints;
	public VehicleWaypoint[] vehicleWaypoints;
	public StartPosition[] startPositions;

	public GameObject[] allCars;
	public RaceStats[] rankings;
	public int[] lapsCompleted; // per car

	public int lapsGoal = 2;

	public int carsInGame = 2;

	public Vector2 rankingSpeedMultiplier = new Vector2(0.1F,1.0F);

	public enum gameMode {

		laps
	}
	public gameMode currentGameMode;

	void Awake(){
		instance = this;
	}

	// Use this for initialization
	public void InitiateGame () {

		levelObject = GameObject.Find ("Level").transform;

		print (levelObject.FindChild("Waypoints"));

		waypoints = levelObject.FindChild("Waypoints").GetComponentsInChildren<Waypoint>();

		for(int i = 0; i < waypoints.Length; i++){
			if(i == waypoints.Length-1)
				waypoints[i].nextWaypoint = waypoints[0];
			else
				waypoints[i].nextWaypoint = waypoints[i+1];

		}

		vehicleWaypoints = levelObject.FindChild("VehicleWaypoints").GetComponentsInChildren<VehicleWaypoint>();

		for(int i = 0; i < vehicleWaypoints.Length; i++){
			if(i == vehicleWaypoints.Length-1)
				vehicleWaypoints[i].nextPoint = vehicleWaypoints[0];
			else
				vehicleWaypoints[i].nextPoint = vehicleWaypoints[i+1];

		}
		for(int iw = 0; iw < vehicleWaypoints.Length; iw++){

//			vehicleWaypoints[iw].Initialize();

		}


		startPositions = levelObject.FindChild("StartPositions").GetComponentsInChildren<StartPosition>();

		rankings = new RaceStats[carsInGame];
		allCars = new GameObject[carsInGame];
		for(int ii = 0; ii < carsInGame; ii++){

			SpawnPlayer(ii);

		}

		lapsCompleted = new int[carsInGame];
	}

	public void SpawnPlayer(int _playerID){

		GameObject _car = Instantiate(carPrefab) as GameObject;
		RaceStats _carStats = _car.AddComponent<RaceStats>();

		_carStats.racerID = _playerID;
		_carStats.currentWaypoint = waypoints[0];

		_car.transform.position = startPositions[_playerID].transform.position;

		_carStats.motorLink = _car.GetComponentInChildren<Motor>();


		if(_playerID == 0){
//			Camera.main.GetComponent<CameraControl>().target = _car.transform;
//			Camera.main.GetComponent<CameraControl>().Initialize();
			_car.GetComponent<VehicleParent>().controlled = true;

			GameObject.Find ("CardboardMain").GetComponent<CameraControl>().target = _car.transform;
			GameObject.Find ("CardboardMain").GetComponent<CameraControl>().Initialize();
		}else{
			_car.AddComponent<FollowAI>().target = vehicleWaypoints[0].transform;
		}

		_car.name = "Car_" + _playerID;

		rankings[_playerID] = _carStats;
		allCars[_playerID] = _car;
	
	}

	public void RecalculateRankings(){

		for(int i = 0; i < allCars.Length; i++){
			for(int ii = 0; ii < allCars.Length; ii++){

				if(rankings[ii].waypointScore > rankings[i].waypointScore && i < ii){
					RaceStats _temp = rankings[i];

					rankings[i] = rankings[ii];
					rankings[ii] = _temp;

				}
			}
		}

		for(int iii = 0; iii < allCars.Length; iii++){
			if(rankings[iii].GetComponent<FollowAI>() != null)
			rankings[iii].GetComponent<FollowAI>().speed = rankingSpeedMultiplier.x + iii*((rankingSpeedMultiplier.y-rankingSpeedMultiplier.x)/allCars.Length);
		}
	}

	public void CompletedLap(int _playerID){

		if(currentGameMode == gameMode.laps){
			lapsCompleted[_playerID] ++;

			if(lapsCompleted[_playerID] >= lapsGoal){
				GameOver(_playerID);
			}
		}
	}

	private void GameOver(int _winnerID){

		GUIManager.instance.ShowMessage("Player " + _winnerID + " has won the game");

	}
}
