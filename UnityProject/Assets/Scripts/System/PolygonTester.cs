using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PolygonTester : MonoBehaviour {

	public PolyBezier linkedCurve;
	private BezierComplex curve;
	public Vector2[] vertices2D;
//	public PolygonTester self = this;
	private float smoothStep = 0.04F;

	LinkedList<Vertex> leftVertices = new LinkedList<Vertex>();
	LinkedList<Vertex> rightVertices = new LinkedList<Vertex>();
	

	void Start(){

	

		TestMesh ();
	}

	public void TestMesh () {

		curve = linkedCurve.bez;

		print (curve.nodes.Length);

		if(curve.nodes.Length < 2)
		{
			Debug.LogError("Path too short");
			return;
		}


		//Methode with smooth Bezier steps
		for(float i = 0.0F; i < curve.nodes.Length; i += smoothStep){

			float tAuto = (i/curve.nodes.Length) % 1;

			print (tAuto);


			//calculate the normalized direction from the 1) most recent position of vertex creation to the 2) current position
//			Vector3 dirToCurrentPos = (trans.position - centerPositions.First.Value).normalized; 
			Vector3 dirToCurrentPos = (curve.GetPointAtTime(tAuto-smoothStep) - curve.GetPointAtTime(tAuto)).normalized;
//			
//			//calculate the positions of the left and right vertices --> they are perpendicular to 'dirToCurrentPos' and 'renderDirection'
//			Vector3 cross = Vector3.Cross(renderDirection, dirToCurrentPos);
//			Vector3 leftPos = trans.position + (cross * -widthStart * 0.5f);
//			Vector3 rightPos = trans.position + (cross * widthStart * 0.5f);
			Vector3 cross = Vector3.Cross(new Vector3(0, -1, 0), dirToCurrentPos);
			Vector3 leftPos = curve.GetPointAtTime(tAuto) + (cross * -20.0F * 0.5F);
			Vector3 rightPos = curve.GetPointAtTime(tAuto) + (cross * 20.0F * 0.5F);


			leftVertices.AddFirst(new Vertex(leftPos));
			rightVertices.AddFirst(new Vertex(rightPos ));
//			leftVertices.Add(new Vector2(leftPos.x, leftPos.z));
//			rightVertices.Add(new Vector2(rightPos.x, rightPos.z));

			//create two new vertices at the calculated positions
//			leftVertices.AddFirst(new Vertex(leftPos, trans.position, (leftPos - trans.position).normalized) );
//			rightVertices.AddFirst(new Vertex(rightPos, trans.position, (rightPos - trans.position).normalized) );
		}

		SetMesh();
	}

	/// <summary>
	/// Sets the mesh and the polygon collider of the mesh.
	/// </summary>
	private void SetMesh() {

//		//only continue if there are at least two center positions in the collection
		if (curve.nodes.Length < 2) {
			return;
		}
//		
		//create an array for the 1) trail vertices, 2) trail uvs, 3) trail triangles, and 4) vertices on the collider path
		Vector3[] vertices = new Vector3[leftVertices.Count * 2];
		Vector2[] uvs = new Vector2[leftVertices.Count * 2];
		int[] triangles = new int[ (leftVertices.Count - 1) * 6];
//		Vector2[] colliderPath = new Vector2[ (centerPositions.Count - 1) * 2];
		
		LinkedListNode<Vertex> leftVertNode = leftVertices.First;
		LinkedListNode<Vertex> rightVertNode = rightVertices.First;
		
		//get the change in time between the first and last pair of vertices
//		float timeDelta = leftVertices.Last.Value.TimeAlive - leftVertices.First.Value.TimeAlive;
		
		//iterate through all the pairs of vertices (left + right)
		for (int i = 0; i < leftVertices.Count; i++) {
			Vertex leftVert = leftVertNode.Value;
			Vertex rightVert = rightVertNode.Value;
			
			//trail vertices
			int vertIndex = i * 2;
			vertices[vertIndex] = leftVert.Position;
			vertices[vertIndex + 1] = rightVert.Position;
			
//			//collider vertices 
//			colliderPath[i] = leftVert.Position;
//			colliderPath[colliderPath.Length - (i + 1) ] = rightVert.Position;
			
			//trail uvs
//			float uvValue = leftVert.TimeAlive / timeDelta;
//			uvs[vertIndex] = new Vector2(uvValue, 0);
//			uvs[vertIndex + 1] = new Vector2(uvValue, 1);
//			
			//trail triangles
			if (i > 0) {
				int triIndex = (i - 1) * 6;
				triangles[triIndex] = vertIndex -2;
				triangles[triIndex + 1] = vertIndex - 1;
				triangles[triIndex + 2] = vertIndex + 1;
				triangles[triIndex + 3] = vertIndex - 2;
				triangles[triIndex + 4] = vertIndex + 1;
				triangles[triIndex + 5] = vertIndex;
			}
			
//			//increment the left and right vertex nodes
			leftVertNode = leftVertNode.Next;
			rightVertNode = rightVertNode.Next; 
		}

		Mesh msh = new Mesh();


		this.gameObject.AddComponent(typeof(MeshRenderer));
		MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
		filter.mesh = msh;

		msh.vertices = vertices;
		msh.triangles = triangles;
		//		msh.RecalculateNormals();
		msh.RecalculateBounds();

		//Custom
		this.GetComponent<Renderer>().material = (Material)Resources.Load("DefaultColor_Gray", typeof(Material));

		this.gameObject.AddComponent<MeshCollider>();

	}

	//************
	//
	// Private Classes
	//
	//************
	
	private class Vertex {
//		private Vector3 centerPosition; //the center position in the trail that this vertex was derived from
		private Vector3 derivedDirection; //the direction from the 1) center position to the 2) position of this vertex
//		private float creationTime;
		
		public Vector3 Position { get; private set; }
//		public float TimeAlive { get { return Time.time - creationTime; } }
		
		public Vertex(Vector3 position) {
			this.Position = position;
//			this.centerPosition = centerPosition;
//			this.derivedDirection = derivedDirection;
//			creationTime = Time.time;
		}
	}
}