using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ethan.ETools
{
	public class ESolidPolygon : MonoBehaviour
	{
		public EMesh eMesh;
		public ESplineTrail eSplineTrail;

		// Use this for initialization
		void Start ()
		{
			List<Vector2> pointsList = new List<Vector2> (GetComponent<PolygonCollider2D> ().points);
			List<Vector3> worldList = pointsList.Select (_ => transform.TransformPoint (new Vector3 (_.x, _.y))).ToList ();

			eMesh.BuildMesh (pointsList.Select (_ => new Vector2 (_.x, _.y)).ToList ());
			eSplineTrail.BuildPolygonMesh (worldList.Select (_ => new Vector3 (_.x, _.y, 0f)).ToList ());
		
			eMesh.GetComponent<MeshRenderer> ().sortingOrder = 2;
			eSplineTrail.GetComponent<MeshRenderer> ().sortingOrder = 4;

			eSplineTrail.enabled = false;
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
	}
}