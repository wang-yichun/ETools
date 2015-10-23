using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ethan.ETools.Test
{
	public class EMeshUtilTest : MonoBehaviour
	{
		public EMesh EMesh;

		void Start ()
		{
			EMesh.BuildMesh (GetVertexList ().Select (_ => new Vector2 (_.x, _.y)).ToList ());
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