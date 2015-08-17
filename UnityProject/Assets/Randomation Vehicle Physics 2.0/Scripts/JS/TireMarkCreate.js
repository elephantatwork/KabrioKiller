#pragma strict
@script RequireComponent(Wheel)

//Class for creating tire marks
public class TireMarkCreate extends MonoBehaviour
{
	private var tr : Transform;
	private var w : Wheel;
	private var mesh : Mesh;
	private var tris : int[];
	private var verts : Vector3[];
	private var uvs : Vector2[];
	private var colors : Color[];

	private var leftPoint : Vector3;
	private var rightPoint : Vector3;
	private var leftPointPrev : Vector3;
	private var rightPointPrev : Vector3;

	private var creatingMark : boolean;
	private var continueMark : boolean;//Continue making mark after current one ends
	private var curMark : GameObject;//Current mark
	private var curEdge : int;
	private var gapDelay : float;//Gap between segments

	private var curSurface : int = -1;//Current surface type
	private var prevSurface : int = -1;//Previous surface type
	private var curSurfaceInstance : GroundSurfaceInstance;

	@Tooltip("How much the tire must slip before marks are created")
	var slipThreshold : float;
	private var alwaysScrape : float;

	@Tooltip("Materials in array correspond to indices in surface types in GroundSurfaceMaster")
	var tireMarkMaterials : Material[];

	@Tooltip("Materials in array correspond to indices in surface types in GroundSurfaceMaster")
	var rimMarkMaterials : Material[];
	
	@Tooltip("Particles in array correspond to indices in surface types in GroundSurfaceMaster")
	var debrisParticles : ParticleSystem[];
	var sparks : ParticleSystem;
	private var initialEmissionRates : float[];

	function Start()
	{
		tr = transform;
		w = GetComponent(Wheel);
		
		initialEmissionRates = new float[debrisParticles.Length + 1];
		for (var i : int = 0; i < debrisParticles.Length; i++)
		{
			initialEmissionRates[i] = debrisParticles[i].emissionRate;
		}

		if (sparks)
		{
			initialEmissionRates[debrisParticles.Length] = sparks.emissionRate;
		}
	}

	function Update()
	{
		//Check for continuous marking
		if (w.grounded)
		{
			curSurfaceInstance = w.contactPoint.col.GetComponent(GroundSurfaceInstance);
			if (curSurfaceInstance)
			{
				alwaysScrape = GroundSurfaceMaster.surfaceTypesStatic[curSurfaceInstance.surfaceType].alwaysScrape ? slipThreshold + Mathf.Min(0.5f, Mathf.Abs(w.rawRPM * 0.001f)) : 0;
			}
			else
			{
				alwaysScrape = 0;
			}
		}

		//Create mark
		if (w.grounded && (Mathf.Abs(F.MaxAbs(w.sidewaysSlip, w.forwardSlip)) > slipThreshold || alwaysScrape > 0) && w.connected)
		{
			prevSurface = curSurface;
			curSurface = curSurfaceInstance ? w.contactPoint.surfaceType : -1;

			if (!creatingMark)
			{
				prevSurface = curSurface;
				StartMark();
			}
			else if (curSurface != prevSurface)
			{
				EndMark();
			}

			//Calculate segment points
			if (curMark)
			{
				var pointDir : Vector3 = Quaternion.AngleAxis(90, w.contactPoint.normal) * tr.right * (w.popped ? w.rimWidth : w.tireWidth);
				leftPoint = curMark.transform.InverseTransformPoint(w.contactPoint.point + pointDir * w.suspensionParent.flippedSideFactor * Mathf.Sign(w.rawRPM) + w.contactPoint.normal * GlobalControl.tireMarkHeightStatic);
				rightPoint = curMark.transform.InverseTransformPoint(w.contactPoint.point - pointDir * w.suspensionParent.flippedSideFactor * Mathf.Sign(w.rawRPM) + w.contactPoint.normal * GlobalControl.tireMarkHeightStatic);
			}
		}
		else if (creatingMark)
		{
			EndMark();
		}

		//Update mark if it's short enough, otherwise end it
		if (curEdge < GlobalControl.tireMarkLengthStatic && creatingMark)
		{
			UpdateMark();
		}
		else if (creatingMark)
		{
			EndMark();
		}
		
		for (var i : int = 0; i < debrisParticles.Length; i++)
		{
			if (w.connected)
			{
				if (i == w.contactPoint.surfaceType)
				{
					if (GroundSurfaceMaster.surfaceTypesStatic[w.contactPoint.surfaceType].leaveSparks && w.popped)
					{
						debrisParticles[i].emissionRate = 0;

						if (sparks)
						{
							sparks.emissionRate = initialEmissionRates[debrisParticles.Length] * Mathf.Clamp01(Mathf.Abs(F.MaxAbs(w.sidewaysSlip, w.forwardSlip, alwaysScrape)) - slipThreshold);
						}
					}
					else
					{
						debrisParticles[i].emissionRate = initialEmissionRates[i] * Mathf.Clamp01(Mathf.Abs(F.MaxAbs(w.sidewaysSlip, w.forwardSlip, alwaysScrape)) - slipThreshold);

						if (sparks)
						{
							sparks.emissionRate = 0;
						}
					}
				}
				else
				{
					debrisParticles[i].emissionRate = 0;
				}
			}
			else
			{
				debrisParticles[i].emissionRate = 0;

				if (sparks)
				{
					sparks.emissionRate = 0;
				}
			}
		}
	}

	function StartMark()
	{
		creatingMark = true;
		curMark = new GameObject("Tire Mark");
		curMark.transform.parent = w.contactPoint.col.transform;
		curMark.AddComponent(TireMark);
		var tempRend : MeshRenderer = curMark.AddComponent(MeshRenderer);

		//Set material based on whether the tire is popped
		if (w.popped)
		{
			tempRend.material = rimMarkMaterials[Mathf.Min(w.contactPoint.surfaceType, rimMarkMaterials.Length - 1)];
		}
		else
		{
			tempRend.material = tireMarkMaterials[Mathf.Min(w.contactPoint.surfaceType, tireMarkMaterials.Length - 1)];
		}

		tempRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		mesh = curMark.AddComponent(MeshFilter).mesh;
		verts = new Vector3[GlobalControl.tireMarkLengthStatic * 2];
		tris = new int[GlobalControl.tireMarkLengthStatic * 3];

		if (continueMark)
		{
			verts[0] = leftPointPrev;
			verts[1] = rightPointPrev;

			tris[0] = 0;
			tris[1] = 3;
			tris[2] = 1;
			tris[3] = 0;
			tris[4] = 2;
			tris[5] = 3;
		}

		uvs = new Vector2[verts.Length];
		uvs[0] = new Vector2(0, 0);
		uvs[1] = new Vector2(1, 0);
		uvs[2] = new Vector2(0, 1);
		uvs[3] = new Vector2(1, 1);

		colors = new Color[verts.Length];
		colors[0].a = 0;
		colors[1].a = 0;

		curEdge = 2;
		gapDelay = GlobalControl.tireMarkGapStatic;
	}

	function UpdateMark()
	{
		if (gapDelay == 0)
		{
			var alpha : float = (curEdge < GlobalControl.tireMarkLengthStatic - 2 && curEdge > 5 ? 1 : 0) * Random.Range(Mathf.Clamp01(Mathf.Abs(F.MaxAbs(w.sidewaysSlip, w.forwardSlip, alwaysScrape)) - slipThreshold) * 0.9f, Mathf.Clamp01(Mathf.Abs(F.MaxAbs(w.sidewaysSlip, w.forwardSlip, alwaysScrape)) - slipThreshold));
			gapDelay = GlobalControl.tireMarkGapStatic;
			curEdge += 2;

			verts[curEdge] = leftPoint;
			verts[curEdge + 1] = rightPoint;

			for (var i : int = curEdge + 2; i < verts.Length; i++)
			{
				verts[i] = Mathf.Approximately(i * 0.5f, Mathf.Round(i * 0.5f)) ? leftPoint : rightPoint;
				colors[i].a = 0;
			}

			tris[curEdge * 3 - 3] = curEdge;
			tris[curEdge * 3 - 2] = curEdge + 3;
			tris[curEdge * 3 - 1] = curEdge + 1;
			tris[Mathf.Min(curEdge * 3,tris.Length - 1)] = curEdge;
			tris[Mathf.Min(curEdge * 3 + 1,tris.Length - 1)] = curEdge + 2;
			tris[Mathf.Min(curEdge * 3 + 2,tris.Length - 1)] = curEdge + 3;

			uvs[curEdge] = new Vector2(0, curEdge * 0.5f);
			uvs[curEdge + 1] = new Vector2(1, curEdge * 0.5f);

			colors[curEdge] = new Color(1, 1, 1, alpha);
			colors[curEdge + 1] = colors[curEdge];

			mesh.vertices = verts;
			mesh.triangles = tris;
			mesh.uv = uvs;
			mesh.colors = colors;
			mesh.RecalculateBounds();
		}
		else
		{
			gapDelay = Mathf.Max(0, gapDelay - Time.deltaTime);
			verts[curEdge] = leftPoint;
			verts[curEdge + 1] = rightPoint;

			for (var j : int = curEdge + 2; j < verts.Length; j++)
			{
				verts[j] = Mathf.Approximately(j * 0.5f, Mathf.Round(j * 0.5f)) ? leftPoint : rightPoint;
				colors[j].a = 0;
			}

			mesh.vertices = verts;	
			mesh.RecalculateBounds();
		}
	}

	function EndMark()
	{
		creatingMark = false;
		leftPointPrev = verts[Mathf.RoundToInt(verts.Length * 0.5f)];
		rightPointPrev = verts[Mathf.RoundToInt(verts.Length * 0.5f + 1)];
		continueMark = w.grounded;

		curMark.GetComponent(TireMark).fadeTime = GlobalControl.tireFadeTimeStatic;
		curMark.GetComponent(TireMark).mesh = mesh;
		curMark.GetComponent(TireMark).colors = colors;
		curMark = null;
		mesh = null;
	}
	
	function OnDestroy()
	{
		if (creatingMark && curMark)
		{
			EndMark();
		}
	}
}

//Class for tire mark instances
public class TireMark extends MonoBehaviour
{
	@System.NonSerialized
	var fadeTime : float = -1;
	private var fading : boolean;
	private var alpha : float = 1;
	@System.NonSerialized
	var mesh : Mesh;
	@System.NonSerialized
	var colors : Color[];

	//Fade the tire mark and then destroy it
	function Update()
	{
		if (fading)
		{
			if (alpha <= 0)
			{
				Destroy(gameObject);
			}
			else
			{
				alpha -= Time.deltaTime;

				for (var i : int = 0; i < colors.Length; i++)
				{
					colors[i].a -= Time.deltaTime;
				}

				mesh.colors = colors;
			}
		}
		else
		{
			if (fadeTime > 0)
			{
				fadeTime = Mathf.Max(0, fadeTime - Time.deltaTime);
			}
			else if (fadeTime == 0)
			{
				fading = true;
			}
		}
	}
}