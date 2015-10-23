using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneratePath : MonoBehaviour 
{
	public List<Transform> pointList;
	
	void Start() 
	{
		SplineTrailRenderer trail = GetComponent<SplineTrailRenderer>();
		
		trail.Clear();
		trail.spline.Clear();
		
		foreach(Transform t in pointList)
		{
			trail.spline.AddKnot(t.position);
		}
		
		trail.spline.Parametrize();
	}
}
