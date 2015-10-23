using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ethan.ETools
{
	public class ESplineTrail : SplineTrailRenderer
	{
		public void BuildPolygonMesh (List<Vector3> pointList)
		{
			SplineTrailRenderer trail = GetComponent<SplineTrailRenderer> ();

			trail.SubSegmentPerSegmentNumber = 1;

			trail.Clear ();
			trail.spline.Clear ();

		
			trail.spline.AddKnot (pointList [pointList.Count - 1]);

			foreach (Vector3 t in pointList) {
				trail.spline.AddKnot (t);
			}
		
			trail.spline.AddKnot (pointList [0]);
			trail.spline.AddKnot (pointList [1]);
			trail.spline.AddKnot (pointList [1]);
		
			trail.spline.Parametrize ();

			trail.RenderMesh ();
		}

		public void BuildSplineMesh (List<Vector3> pointList)
		{
			SplineTrailRenderer trail = GetComponent<SplineTrailRenderer> ();
		
			trail.Clear ();
			trail.spline.Clear ();
		
			foreach (Vector3 t in pointList) {
				trail.spline.AddKnot (t);
			}
		
			trail.spline.Parametrize ();
	
		}
	}
}