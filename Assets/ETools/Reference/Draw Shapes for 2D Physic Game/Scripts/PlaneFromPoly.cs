﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class PlaneFromPoly : MonoBehaviour
{
	
	public Material mat;
	public Vector2[] poly;  // Initialized in the inspector
	public Vector2[] polyCopy;  // Initialized in the inspector
	public List<Vector2> keyPoints;
	public Vector2 uvPosition;
	public float uvScale = 1;
	public float uvRotation;
	public MeshCollider meshCollider;
	public float colliderDepth = 1;
	public bool buildColliderEdges = true;
	public bool buildColliderFront;

	void Start ()
	{
//		Renderer rend = gameObject.AddComponent<MeshRenderer> ();
//		rend.material = mat;
	}

	void Update ()
	{
	}

	public List<Vector3> GetEdgePoints ()
	{
		//Build the point list and calculate curves
		var points = new List<Vector3> ();
		for (int i = 0; i < keyPoints.Count; i++) {
			points.Add (keyPoints [i]);
		}
		return points;
	}

	public void BuildMesh (List<Vector2> points)
	{
		Renderer rend = gameObject.GetComponent<MeshRenderer> ();
		if (rend == null) {
			rend = gameObject.AddComponent<MeshRenderer>();
		}
		rend.material = mat;

		gameObject.GetComponent<MeshRenderer> ().enabled = true;

		//	var points = GetEdgePoints();
		var vertices = new Vector3[points.Count]; 

		for (int i = 0; i < points.Count; i++) {
			vertices [i] = points [i];
		}

		//Build the index array
		var indices = new List<int> ();
		while (indices.Count < points.Count)
			indices.Add (indices.Count);
		
		//Build the triangle array
		var triangles = Triangulates.Points (points);
		
		//Build the uv array
		var scale = uvScale != 0 ? (1 / uvScale) : 0;
		var matrix = Matrix4x4.TRS (-uvPosition, Quaternion.Euler (0, 0, uvRotation), new Vector3 (scale, scale, 1));
		var uv = new Vector2[points.Count];
		for (int i = 0; i < uv.Length; i++) {
			var p = matrix.MultiplyPoint (points [i]);
			uv [i] = new Vector2 (p.x, p.y);
		}
		
		//Find the mesh (create it if it doesn't exist)

		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter> ();
		if (meshFilter == null) {

			meshFilter = gameObject.AddComponent<MeshFilter> ();

		}
		Mesh mesh = meshFilter.sharedMesh;

		if (mesh == null) {
			mesh = new Mesh ();
			mesh.name = "PolySprite_Mesh";
			meshFilter.mesh = mesh;
		}

		
		//Update the mesh
		mesh.Clear ();
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.RecalculateNormals ();
		mesh.Optimize ();
		transform.position += Vector3.forward * 0.1f;

	}


}

public static class Triangulates
{
	public static int[] Points (List<Vector2> points)
	{
		var indices = new List<int> ();
		
		int n = points.Count;
		if (n < 3)
			return indices.ToArray ();
		
		int[] V = new int[n];
		if (Area (points) > 0) {
			for (int v = 0; v < n; v++)
				V [v] = v;
		} else {
			for (int v = 0; v < n; v++)
				V [v] = (n - 1) - v;
		}
		
		int nv = n;
		int count = 2 * nv;
		for (int m = 0, v = nv - 1; nv > 2;) {
			if ((count--) <= 0)
				return indices.ToArray ();
			
			int u = v;
			if (nv <= u)
				u = 0;
			v = u + 1;
			if (nv <= v)
				v = 0;
			int w = v + 1;
			if (nv <= w)
				w = 0;
			
			if (Snip (points, u, v, w, nv, V)) {
				int a, b, c, s, t;
				a = V [u];
				b = V [v];
				c = V [w];
				indices.Add (a);
				indices.Add (b);
				indices.Add (c);
				m++;
				for (s = v, t = v + 1; t < nv; s++, t++)
					V [s] = V [t];
				nv--;
				count = 2 * nv;
			}
		}
		
		indices.Reverse ();
		return indices.ToArray ();
	}
	
	public static float Area (List<Vector2> points)
	{
		int n = points.Count;
		float A = 0.0f;
		for (int p = n - 1, q = 0; q < n; p = q++) {
			Vector3 pval = points [p];
			Vector3 qval = points [q];
			A += pval.x * qval.y - qval.x * pval.y;
		}
		return (A * 0.5f);
	}
	
	static bool Snip (List<Vector2> points, int u, int v, int w, int n, int[] V)
	{
		int p;
		Vector3 A = points [V [u]];
		Vector3 B = points [V [v]];
		Vector3 C = points [V [w]];
		if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
			return false;
		for (p = 0; p < n; p++) {
			if ((p == u) || (p == v) || (p == w))
				continue;
			Vector3 P = points [V [p]];
			if (InsideTriangle (A, B, C, P))
				return false;
		}
		return true;
	}
	
	static bool InsideTriangle (Vector2 A, Vector2 B, Vector2 C, Vector2 P)
	{
		float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
		float cCROSSap, bCROSScp, aCROSSbp;
		
		ax = C.x - B.x;
		ay = C.y - B.y;
		bx = A.x - C.x;
		by = A.y - C.y;
		cx = B.x - A.x;
		cy = B.y - A.y;
		apx = P.x - A.x;
		apy = P.y - A.y;
		bpx = P.x - B.x;
		bpy = P.y - B.y;
		cpx = P.x - C.x;
		cpy = P.y - C.y;
		
		aCROSSbp = ax * bpy - ay * bpx;
		cCROSSap = cx * apy - cy * apx;
		bCROSScp = bx * cpy - by * cpx;
		
		return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
	}
}