using UnityEngine;
using System.Collections;

public class RaceManager : MonoBehaviour {

	public static RaceManager instance {get; private set;}	

	public GameObject carPrefab;

	public Transform levelObject;
	public Waypoint[] waypoints;
	public StartPosition[] startPositions;

	public GameObject[] allCars;
	public int[] lapsCompleted; // per car

	public int lapsGoal = 2;

	public int carsInGame = 2;

	public enum gameMode {

		laps
	}
	public gameMode currentGameMode;

	void Awake(){
		instance = this;
	}

	// Use this for initialization
	void Start () {

		levelObject = GameObject.Find ("Level").transform;

		print (levelObject.FindChild("Waypoints"));

		waypoints = levelObject.FindChild("Waypoints").GetComponentsInChildren<Waypoint>();

		for(int i = 0; i < waypoints.Length; i++){
			if(i == 0)
				waypoints[i].nextWaypoint = waypoints[waypoints.Length-1];
			else if(i == waypoints.Length-1)
				waypoints[i].nextWaypoint = waypoints[0];
			else
				waypoints[i].nextWaypoint = waypoints[i+1];

		}

		startPositions = levelObject.FindChild("StartPositions").GetComponentsInChildren<StartPosition>();

	

		allCars = new GameObject[carsInGame];
		for(int ii = 0; ii < carsInGame; ii++){

			SpawnPlayer(ii);

		}

		lapsCompleted = new int[carsInGame];
	}

	public void SpawnPlayer(int _playerID){

		GameObject _car = Instantiate(carPrefab) as GameObject;
		RaceStats _carStats = _car.GetComponent<RaceStats>();

		_carStats.racerID = _playerID;
		_carStats.currentWaypoint = waypoints[0];

		_car.transform.position = startPositions[_playerID].transform.position;

		if(_playerID == 0){
			Camera.main.GetComponent<CameraControl>().target = _car.transform;
			Camera.main.GetComponent<CameraControl>().Initialize();
		}

		allCars[_playerID] = _car;
	
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
