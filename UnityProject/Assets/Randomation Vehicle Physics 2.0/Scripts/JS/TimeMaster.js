#pragma strict
import UnityEngine.Audio;

//Class for managing time
public class TimeMaster extends MonoBehaviour
{
	private var initialFixedTime : float;//Intial Time.fixedDeltaTime

	@Tooltip("Master audio mixer")
	var masterMixer : AudioMixer;
	var destroyOnLoad : boolean;

	function Awake()
	{
		initialFixedTime = Time.fixedDeltaTime;
		
		if (!destroyOnLoad)
		{
			DontDestroyOnLoad(gameObject);
		}
	}

	function Update()
	{
		//Set the pitch of all audio to the time scale
		if (masterMixer)
		{
			masterMixer.SetFloat("MasterPitch", Time.timeScale);
		}
	}

	function FixedUpdate()
	{
		//Set the fixed update rate based on time scale
		Time.fixedDeltaTime = Time.timeScale * initialFixedTime;
	}
}
