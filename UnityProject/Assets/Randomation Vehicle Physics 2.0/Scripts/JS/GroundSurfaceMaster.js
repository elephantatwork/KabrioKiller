#pragma strict

//Class managing surface types
public class GroundSurfaceMaster extends MonoBehaviour
{
	var surfaceTypes : GroundSurface[];
	static var surfaceTypesStatic : GroundSurface[];

	function Start()
	{
		surfaceTypesStatic = surfaceTypes;
	}
}

//Class for individual surface types
@System.Serializable
public class GroundSurface
{
	var useColliderFriction : boolean;
	var friction : float;
	@Tooltip("Always leave tire marks")
	var alwaysScrape : boolean;
	@Tooltip("Rims leave sparks on this surface")
	var leaveSparks : boolean;
	var tireSnd : AudioClip;
	var rimSnd : AudioClip;
	var tireRimSnd : AudioClip;
}