using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Ethan.EUWork.Algorithm
{
	public class EGeometryCircleUtil
	{
		/// <summary>
		/// 生成多边形的顶点列表,生成的多边形边长应不超过边长最大限,且尽可能大(边数尽可能小)
		/// </summary>
		/// <returns>圆形的顶点列表.</returns>
		/// <param name="radius">半径.</param>
		/// <param name="edgeLengthMaximum">边长最大限.</param>
		/// <param name="centre">Centre.</param>
		public static List<Vector2> CreateCircleVertexesList (float radius, float edgeLengthMaximum, Vector2 centre)
		{
			if (radius <= 0.0f) {
				Debug.LogWarning ("半径需要大于0");
				return null;
			}
			if (edgeLengthMaximum <= 0.0f || edgeLengthMaximum > radius * 2) {
				Debug.LogWarning ("边长需要大于零且小于半径的2倍");
				return null;
			}

			// 先以最大边数求出边数浮点数
			double n = Math.PI / Math.Asin (edgeLengthMaximum / (2 * radius));

			int N = (int)Math.Ceiling (n);

			Debug.Log (string.Format ("n:{0}, N:{1}", n, N));

			float td = Mathf.PI * 2 / N;

			List<Vector2> resultList = new List<Vector2> ();

			for (int i = 0; i < N; i++) {
				float t = td * i;

				Vector2 p = new Vector2 ();
				p.x = Mathf.Cos (t) * radius;
				p.y = Mathf.Sin (t) * radius;

				resultList.Add (p + centre);
			}

			return resultList;
		}

		public static List<Vector3> CreateCircleVertexesList (float radius, float edgeLengthMaximum, Vector3 centre, Vector3 target)
		{
			List<Vector2> resultListV2 = CreateCircleVertexesList (radius, edgeLengthMaximum, Vector2.zero);

			Vector3 directionVec = target - centre;

			List<Vector3> resultList = resultListV2.Select (_ => {
				return Quaternion.FromToRotation (Vector3.back, directionVec) * (new Vector3 (_.x, _.y, 0)) + centre;
			}).ToList ();

			return resultList;
		}
	}
}

