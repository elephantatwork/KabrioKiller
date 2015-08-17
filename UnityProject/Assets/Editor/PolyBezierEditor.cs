

using UnityEditor;
using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

// Thanks to Dantus from Edelweiss Interactive for this trick
public class DefaultHandles
{
	public static bool Hidden
	{
		get
		{
			Type type = typeof(Tools);
			FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
			return ((bool)field.GetValue(null));
		}
		set
		{
			Type type = typeof(Tools);
			FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
			field.SetValue(null, value);
		}
	}
}

[CustomEditor(typeof(PolyBezier))]
public class PolyBezierEditor : Editor {
	
	private enum cpte {Node, HandleA, HandleB}; // current point type enum
	private cpte curPointType = cpte.Node;
	int curPointIndex = -1;
	bool hideDefaultHandle = true;
	GameObject testPath;

	void OnEnable()    { DefaultHandles.Hidden = hideDefaultHandle; }
	void OnDisable()    { DefaultHandles.Hidden = false;}
	
	void MoveNode(PolyBezier polyBezier, Vector3 localPos, int i) {
		polyBezier.bez.MoveNode(localPos, i);
		EditorUtility.SetDirty(target);
	}
	
	void MoveHandle(PolyBezier polyBezier, Vector3 localPos, int i, bool handleA){
		polyBezier.bez.MoveHandle(localPos, i, handleA);
		EditorUtility.SetDirty(target);
	}
	
	void RotateHandles(PolyBezier polyBezier, Quaternion qDelta, int i){
		Vector3 p = polyBezier.bez.nodes[i];
		Vector3 ha = polyBezier.bez.handlesA[i];
		Vector3 hb = polyBezier.bez.handlesB[i];
		
		polyBezier.bez.handlesA[i] = p + qDelta * (ha-p);
		polyBezier.bez.handlesB[i] = p + qDelta * (hb-p);
		EditorUtility.SetDirty(target);
	}
	
	void DeleteNode(PolyBezier polyBezier, int index)
	{
		int nl = polyBezier.bez.nodes.Length;
		if (index < 0 || index >= nl)
			return;
		
		polyBezier.bez.DeleteNode(index);
		
		if (nl == 0)
		{
			curPointIndex = -1;
			RectifyCurrentPointEnum(polyBezier);
		}
		else
		{
			if (index != 0)
			{
				curPointIndex -= 1;
				RectifyCurrentPointEnum(polyBezier);
			}
		}
		EditorUtility.SetDirty(target);
	}
	
	void AddNode(PolyBezier polyBezier, int position)
	{
		// before = position
		// after = position + 1
		int nl = polyBezier.bez.nodes.Length;
		Vector3 p;
		Vector3 h;
		Vector3 p2;
		Vector3[] nodes = polyBezier.bez.nodes;
		
		if (nl == 0)
		{
			p = Vector3.zero;
			h = Vector3.right * 10;
			polyBezier.bez.InsertNode(0, p, p+h, p-h);
		}
		else if (nl == 1)
		{
			if (position == 0)
			{
				p = Vector3.zero;
				h = Vector3.right * 10;
				polyBezier.bez.InsertNode(0, p, p+h, p-h);
			}
			else
			{
				p = nodes[0] * 2;
				p2 = p - nodes[0];
				
				polyBezier.bez.InsertNode(    1, p,
				                          polyBezier.bez.handlesA[0] + p2,
				                          polyBezier.bez.handlesB[0] + p2);
			}
		}
		else
		{
			
			if (position == -1 ) return; //position = nl ; // last
			
			if (position == 0)
			{
				p = nodes[position];
				p2 = p - nodes[position + 1];
				
				polyBezier.bez.InsertNode(    position,     p + p2  * 2/4,
				                          p + p2  * 3/4, // ext
				                          p + p2  * 1/4 ); // int
			}
			else if ( position == nl)
			{
				p = nodes[position - 1];
				p2 = p - nodes[position - 2];
				
				polyBezier.bez.InsertNode(    position,     p + p2  * 2/4,
				                          p + p2  * 1/4, // int
				                          p + p2  * 3/4 ); // ext
			}
			else
			{
				p = 0.5f * (nodes[position - 1] + nodes[position]); // between point before and after
				Vector3 pMid = 0.5f * (p - polyBezier.bez.nodes[position -  1]); // this + p = between p and point after, p - this = between point
				
				polyBezier.bez.InsertNode(    position,     p,
				                          p - pMid,
				                          p + pMid);
			}
		}
		EditorUtility.SetDirty(target);
	}
	
	void ResetHandles(PolyBezier polyBezier, int i) {
		Vector3 p = polyBezier.bez.nodes[i];
		polyBezier.bez.handlesA[i] = p - Vector3.right * 4;
		polyBezier.bez.handlesB[i] = p + Vector3.right * 4;
		EditorUtility.SetDirty(target);
	}
	
	void ClearAll(PolyBezier polyBezier) {
		polyBezier.bez.DeleteAllNodes();
	}
	
	void insideSceneGUI(PolyBezier polyBezier)
	{
		GUI.skin = polyBezier.customSkin;

		Rect size = new Rect(0, 0, Screen.width*0.3F, Screen.width*0.35F);
		float heightButton = 20;
		Handles.BeginGUI();
		//  Handles.BeginGUI(new Rect(Screen.width - size.width - 10, Screen.height - size.height - 10, size.width, size.height));
		
		GUI.BeginGroup(new Rect(Screen.width - size.width - 10, Screen.height - size.height*0.8F, size.width, size.height/2));		
		
		Rect rc = new Rect(0, 15, heightButton*2, heightButton);



//		GUI.contentColor = Color.black;
//		GUI.Label(rc, "Clic on Circles to select a point");
//		GUI.contentColor = Color.white
		rc.y = 100;
		if (curPointIndex != -1)
		{
//			GUI.contentColor = Color.black;
//			GUI.Label(rc, "Current Point " + curPointIndex);
//			GUI.contentColor = Color.white;

//			rc.y -= 25; // back up
			rc.x = 0;
			rc.width = size.width;



			if (GUI.Button(rc, "Deselect"))
			{
				curPointIndex = -1;
				RectifyCurrentPointEnum(polyBezier);
			}
//			rc.y += 25; // back down
			rc.x = 0;
			
			
			
			
			
			rc.y -= rc.height*1.1F;
			rc.width = size.width / 2;
			
			if (GUI.Button(rc, "Create Before"))
			{
				AddNode(polyBezier, curPointIndex);
			}
			rc.x += rc.width;
			if (GUI.Button(rc, "Create After"))
			{
				AddNode(polyBezier, curPointIndex + 1);
				curPointIndex++;
				RectifyCurrentPointEnum(polyBezier);
			}

			rc.y -= rc.height*1.1F;
			rc.x = rc.width/2;
			
			if (GUI.Button(rc, "Delete", polyBezier.customSkin.customStyles[0])) DeleteNode(polyBezier, curPointIndex);


			rc.x = 0;
			rc.y -= 10;
			rc.y -= heightButton;
			if (GUI.Button(rc, "Reset Handles"))
			{
				ResetHandles(polyBezier, curPointIndex);
			}
			
			
		}
		else
		{
			if (polyBezier.bez.nodes.Length == 0)
			{
				if (GUI.Button(rc, "Insert")) AddNode(polyBezier, 0);
				rc.y -= heightButton;
			}
			else
			{



				rc.width = size.width / 2;
				
				if (GUI.Button(rc, "Insert First"))
				{
					AddNode(polyBezier, 0);
					curPointIndex = 0;
					RectifyCurrentPointEnum(polyBezier);
//					DestroyImmediate(testPath);
				}
				rc.x += rc.width;
				if (GUI.Button(rc, "Insert Last"))
				{
					AddNode(polyBezier, polyBezier.bez.nodes.Length);
					curPointIndex = polyBezier.bez.nodes.Length - 1;
					RectifyCurrentPointEnum(polyBezier);
				}
				rc.y -= heightButton;
			}
		}
		
		if (polyBezier.bez.nodes.Length > 0)
		{
			//rc.y += sizeButton;
			rc.width = size.width / 2;
			rc.x = 0;
//			rc.y += heightButton+10;
			if (GUI.Button(rc, "Clear All"))
			{
				ClearAll(polyBezier);
//				DestroyImmediate(testPath);

				curPointIndex = -1;
				RectifyCurrentPointEnum(polyBezier);
			}
			rc.x = rc.width;
			if (GUI.Button(rc, "Render Mesh"))
			{
				testPath = GameObject.Find("PreviewPath");

				if(testPath != null)
					DestroyImmediate(testPath);
				
				testPath = new GameObject();
				testPath.name = "PreviewPath";

				testPath.AddComponent<PolygonTester>().linkedCurve = polyBezier;
				testPath.GetComponent<PolygonTester>().TestMesh();


				
				//				PolygonTester.TestMesh();
			}
		}
		
//		rc.x -= rc.width;
//		if (hideDefaultHandle)
//		{
//			if (GUI.Button(rc, "Show Main"))
//			{
//				hideDefaultHandle = false;
//				DefaultHandles.Hidden = hideDefaultHandle;
//			}
//		}
//		else
//		{
//			if (GUI.Button(rc, "Hide Main"))
//			{
//				hideDefaultHandle = true;
//				DefaultHandles.Hidden = hideDefaultHandle;
//			}
//		}

		GUI.Box(size, "PolyBezier Tool Bar", polyBezier.customSkin.label);
		
		GUI.EndGroup();
		Handles.EndGUI();
	}
	
	void OnSceneGUI ()
	{
		PolyBezier polyBezier = (PolyBezier) target;
		DisplayPointsAndLines(polyBezier);
		DisplayBezier(polyBezier);
		DisplayProgression(polyBezier);
		
		SceneView.RepaintAll(); // repaint to display progression
	}
	
	void DisplayBezier(PolyBezier polyBezier) {
		
		int l = polyBezier.bez.GetNumberOfSegment(); // segments
		if (l == 0) return;
		Transform tr = polyBezier.transform;

		//Super Smooth
		Vector3[,] h = polyBezier.bez.GetBezierInHandlesFormat(); //Vector3[segments, 4(p0,h0,p1,h1)];
		for (int i = 0; i < l; i++)
		{
			for (int j = 0; j < 4; j++) h[i, j] = tr.TransformPoint(h[i, j]);
			Handles.DrawBezier(h[i,0], h[i,1], h[i,2], h[i,3], Color.red, null, 2);
		}
		
		//*
		Handles.color = Color.yellow;
		float tt = 0;
		float step = 0.0001f;
		
		Vector3 pb;
		Vector3 pa;
		for (float t = step; t <= 1; t += step)
		{
			pb = polyBezier.bez.GetPointAtTime(tt);
			pb = tr.TransformPoint(pb);
			pa = polyBezier.bez.GetPointAtTime(t);
			pa = tr.TransformPoint(pa);
			Handles.DrawLine(pb, pa);
			tt = t;
		}
		//*/
	}
	
	
	//private Quaternion qRec = Quaternion.identity;
	private Vector3 nodeRec = Vector3.zero;
	private Vector3 hanARec = Vector3.zero;
	private Vector3 hanBRec = Vector3.zero;
	
	void DisplayPointsAndLines(PolyBezier polyBezier) {
		
		int someHashCode = GetHashCode();
		
		Transform tr = polyBezier.transform;
		int nl = polyBezier.bez.nodes.Length;
		
		Vector3[] worldNodes = new Vector3[nl];
		Vector3[] worldHandlesA = new Vector3[nl];
		Vector3[] worldHandlesB = new Vector3[nl];
		
		for (int i = 0; i < nl; i++)
		{  
			Vector3 node = tr.TransformPoint(polyBezier.bez.nodes[i]);
			Vector3 hanA = tr.TransformPoint(polyBezier.bez.handlesA[i]);
			Vector3 hanB = tr.TransformPoint(polyBezier.bez.handlesB[i]);
			
			//Handles.color = Color.yellow;
			//Handles.DrawLine(hanA, node);
			//Handles.DrawLine(node, hanB);
			//if (i != nl-1) Handles.DrawLine(hanB, tr.TransformPoint(polyBezier.bez.handlesA[i+1]));
			
			Handles.color = Color.white;
			Handles.Label(node, "   " + i);
			float size;
			
			// Display Node and check if it has the focus
			//
			size = HandleUtility.GetHandleSize(node);
			SetControlIDState(someHashCode);
			Handles.ScaleValueHandle(0, node, Quaternion.identity, size, Handles.SphereCap, 0); // display cap
			if (GetControlIDState(someHashCode))
			{
				//Debug.Log(i);
				curPointIndex = i;
				curPointType = cpte.Node;
				RectifyCurrentPointEnum(polyBezier);
			}
			
			Handles.color = Color.blue;
			// Display First Handle and check if it has the focus
			//
			if (i != 0)
			{
				size = HandleUtility.GetHandleSize(hanA);
				SetControlIDState(someHashCode);
				Handles.ScaleValueHandle(0, hanA, Quaternion.identity, size, Handles.SphereCap, 0);
				if (GetControlIDState(someHashCode))
				{
					//Debug.Log(i);
					curPointIndex = i;
					curPointType = cpte.HandleA;
					RectifyCurrentPointEnum(polyBezier);
				}
				Handles.DrawLine(hanA, node);
			}
			
			// Display Last Handle and check if it has the focus
			//
			if (i != nl-1)
			{
				size = HandleUtility.GetHandleSize(hanB);
				SetControlIDState(someHashCode);
				Handles.ScaleValueHandle(0, hanB, Quaternion.identity, size, Handles.SphereCap, 0);
				if (GetControlIDState(someHashCode))
				{
					//Debug.Log(i);
					curPointIndex = i;
					curPointType = cpte.HandleB;
					RectifyCurrentPointEnum(polyBezier);
				}
				Handles.DrawLine(hanB, node);
			}
			
			// Move points for selected object
			//
			if (curPointIndex == i)
			{
				/*
                    Quaternion q = Handles.RotationHandle(Quaternion.identity, node);
                    if (q != qRec) // do not call EditorUtility.Set Dirty() if nothing is moved
                    {
                        q = q * Quaternion.Inverse(qRec);
                        qRec = q;
                        //RotateHandles(polyBezier, q, curPointIndex); // display rotation gizmo
                    }
                    //*/
				
				if (curPointType == cpte.Node)
				{
					node = Handles.PositionHandle(node, Quaternion.identity); // display position gizmo
					if (node != nodeRec) // do not call EditorUtility.Set Dirty() if nothing is moved
					{
						MoveNode(polyBezier, tr.InverseTransformPoint(node), i);
						nodeRec = node;
					}
				}
				else if (curPointType == cpte.HandleA)
				{
					hanA = Handles.PositionHandle(hanA, Quaternion.identity);
					if (hanA != hanARec) // do not call EditorUtility.Set Dirty() if nothing is moved
					{
						MoveHandle(polyBezier, tr.InverseTransformPoint(hanA), i, true);
						hanARec = hanA;
					}
				}
				else
				{
					hanB = Handles.PositionHandle(hanB, Quaternion.identity);
					if (hanB != hanBRec) // do not call EditorUtility.Set Dirty() if nothing is moved
					{
						MoveHandle(polyBezier, tr.InverseTransformPoint(hanB), i, false);
						hanBRec = hanB;
					}
				}
			}
			
			// Store nodes to display lines
			//
			worldNodes[i] = node;
			worldHandlesA[i] = hanA;
			worldHandlesB[i] = hanB;
			
		}
		insideSceneGUI(polyBezier);
		
		//Handles.color = Color.red;
		//Handles.DrawPolyLine(worldNodes);
		
	}
	
	int controlIDBeforeHandle;
	bool isEventUsedBeforeHandle;
	
	private void SetControlIDState(int hashCode) {
		// Get the needed data before the handle (selected SphereCap ID)
		controlIDBeforeHandle = GUIUtility.GetControlID(hashCode, FocusType.Passive);
		isEventUsedBeforeHandle = (Event.current.type == EventType.used);
	}
	
	private bool GetControlIDState(int hashCode) {
		// Get the needed data after the handle
		int controlIDAfterHandle = GUIUtility.GetControlID(hashCode, FocusType.Passive);
		bool isEventUsedByHandle = false;//!isEventUsedBeforeHandle  (Event.current.type == EventType.used);
		
		if ( (controlIDBeforeHandle < GUIUtility.hotControl &&  GUIUtility.hotControl < controlIDAfterHandle) ||  isEventUsedByHandle) return true;
		return false;
	}
	
	void DisplayProgression(PolyBezier polyBezier) {
		float t = polyBezier.t;
		t = Mathf.Clamp01(t);
		polyBezier.t = t;
		if (t == 0)
		{
			float tAuto = (Time.realtimeSinceStartup/polyBezier.bez.nodes.Length) % 1;
			t = tAuto;
		}
		
		Transform tr = polyBezier.transform;
		Vector3 p = polyBezier.bez.GetPointAtTime(t);
		p = tr.TransformPoint(p);
		Vector3 tan = polyBezier.bez.GetTangentAtTime(t);
		tan = tr.TransformDirection(tan);
		
		Handles.color = Color.green;
		Handles.DrawWireDisc(p, tan, 1);
		Handles.DrawLine(p, p + tan * 1);
		
		float tRect = t;
		int segment = polyBezier.bez.GetSegmentAtTime(ref tRect);
		Handles.Label(p,     "     " + Math.Round(t,2) + "\n" +
		              "segment : " + segment + "\n" +
		              "local t : " + Math.Round(tRect,2));
		
	}
	
	override public void  OnInspectorGUI()
	{
		base.OnInspectorGUI();
	}
	
	private void RectifyCurrentPointEnum(PolyBezier polyBezier) {
		DestroyImmediate(testPath);
		if (curPointIndex == -1) curPointType = cpte.Node;
		if (curPointIndex == 0 && curPointType == cpte.HandleA) curPointType = cpte.HandleB;
		if (curPointIndex == polyBezier.bez.nodes.Length && curPointType == cpte.HandleB) curPointType = cpte.HandleA;
	}
	
}































