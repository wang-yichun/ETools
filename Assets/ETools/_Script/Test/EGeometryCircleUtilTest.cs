using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ethan.ETools.Algorithm;

namespace Ethan.ETools.Test
{
	public class EGeometryCircleUtilTest : MonoBehaviour
	{
		public Transform CentreTransform;
		public Transform TargetTransform;
		public float Radius;
		public float EdgeLengthMax;
		public GameObject Prefab;

		void Start ()
		{
			InstantiateSphere ();
		}

		void InstantiateSphere ()
		{
			List<Vector3> resultList = EGeometryCircleUtil.CreateCircleVertexesList (
				radius: Radius,
				edgeLengthMaximum: EdgeLengthMax,
				centre: CentreTransform.position,
				target: TargetTransform.position
			);

			foreach (Vector3 p in resultList) {
				GameObject gameObject = Instantiate<GameObject> (Prefab);
				gameObject.transform.position = p;
			}
		}
	}
}