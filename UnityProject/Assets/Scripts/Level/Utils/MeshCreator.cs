using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshCreator : MonoBehaviour {

	public PolyBezier assignedCurve;
	public MeshFilter linkedMeshFilter;

	// Use this for initialization
	void Start () {
	
//		assignedCurve.bez.GetPointAtTime(0)

		linkedMeshFilter = this.GetComponent<MeshFilter>();

		GenerateMesh();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void GenerateMesh(){

		float start_time = Time.time;
		
		List<Vector3[]> verts = new List<Vector3[]>();
		List<int> tris = new List<int>();
		List<Vector2> uvs = new List<Vector2>();

		int width = assignedCurve.bez.nodes.Length;

		//Raw only nodes
		for(int i = 0; i < width; i++){

			Vector3 current_point = new Vector3(assignedCurve.bez.nodes[i].x,
			                                    assignedCurve.bez.nodes[i].y,
			                                    assignedCurve.bez.nodes[i].z);

			tris.Add(i);
			tris.Add(i+1);
			tris.Add(i+2);
		}

		// Generate everything.
//		for (int z = 0; z < width; z++)
//		{
//			verts.Add(new Vector3[width]);
//			for (int x = 0; x < width; x++)
//			{
//				Vector3 current_point = new Vector3();
//				current_point.x = x * spacing;
//				current_point.z = z * spacing; // TODO this makes right triangles, fix it to be equilateral
//				
//				current_point.y = GetHeight(current_point.x, current_point.z);
//				
//				verts[z][x] = current_point;
//				uvs.Add(new Vector2(x,z)); // TODO Add a variable to scale UVs.
//			}
//		}
//		
//		// Only generate one triangle.
//		// TODO Generate a grid of triangles.
//		tris.Add(0);
//		tris.Add(1);
//		tris.Add(width);
		
//		 Unfold the 2d array of verticies into a 1d array.
		Vector3[] unfolded_verts = new Vector3[width*width];
		int ii = 0;
		foreach (Vector3[] v in verts)
		{
			v.CopyTo(unfolded_verts, ii * width);
			ii++;
		}
		
		// Generate the mesh object.
		Mesh ret = new Mesh();
		ret.vertices = unfolded_verts;
		ret.triangles = tris.ToArray();
//		ret.uv = uvs.ToArray();
		
		// Assign the mesh object and update it.
		ret.RecalculateBounds();
//		ret.RecalculateNormals();
		linkedMeshFilter.mesh = ret;
		
		float diff = Time.time - start_time;
		Debug.Log("ProceduralTerrain was generated in " + diff + " seconds.");
	}
}
