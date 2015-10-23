using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ethan.ETools.Test
{
	public class EMeshUtilTest : MonoBehaviour
	{
		public EMesh EMesh;
		public ESplineTrail ESplineTrail;

		void Start ()
		{
			EMesh.BuildMesh (GetVertexList ().Select (_ => new Vector2 (_.x, _.y)).ToList ());
			ESplineTrail.BuildPolygonMesh (GetVertexList ().ToList ());

			EMesh.GetComponent<MeshRenderer> ().sortingOrder = 2;
			ESplineTrail.GetComponent<MeshRenderer> ().sortingOrder = 4;
		}

		void OnDrawGizmos ()
		{
			List<Vector3> vertexList = GetVertexList ();
			foreach (Vector3 p in vertexList) {
				Gizmos.DrawWireCube (p, Vector3.one * .1f);
			}
		}

		List<Vector3> GetVertexList ()
		{
			List<Vector3> resultList = new List<Vector3> ();
			for (int i = 0; i < this.transform.childCount; i++) {
				resultList.Add (this.transform.GetChild (i).position);
			}
			return resultList;
		}
	}
}