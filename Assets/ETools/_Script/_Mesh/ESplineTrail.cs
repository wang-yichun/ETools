using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ESplineTrail : SplineTrailRenderer
{
	public void BuildMesh (List<Vector3> pointList)
	{
		SplineTrailRenderer trail = GetComponent<SplineTrailRenderer> ();
		
		trail.Clear ();
		trail.spline.Clear ();
		
		foreach (Vector3 t in pointList) {
			trail.spline.knots.Add (new Knot (t));
		}
		
		trail.spline.Parametrize ();
	
	}
}
