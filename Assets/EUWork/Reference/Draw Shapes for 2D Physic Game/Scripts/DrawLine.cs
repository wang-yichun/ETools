using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(LineRenderer))]
public class DrawLine : MonoBehaviour {
	public static DrawLine Instance;

	List<Vector2> linePoints = new List<Vector2>();
	Vector2[] linePointsVector2d;
	LineRenderer lineRenderer;
	public float startWidth = 1.0f;
	public float endWidth = 1.0f;
	public float threshold = 0.001f;
	Camera thisCamera;
	int lineCount = 0;
	bool StartDraw;
	GameObject point;
	PolygonCollider2D polyCollider;
	EdgeCollider2D edgeCollider;
	GameObject previousObj;
	LineRenderer line;
	Vector3 lastPos = Vector3.one * float.MaxValue;
	public Material mat;
	GameObject startPoint;
	Vector3 startPos;
	public PhysicsMaterial2D bounce;
	float DoubleClickTimer;
	public Material lineMaterial;
	float dist = 0;
	float distanceMeter = 0;
	GameObject scorePopup;
	EdgeCollider2D lineCollider;
	List<Bounds> bounds;

	void Awake()
	{
		Instance = this;
		thisCamera = Camera.main;
		lineRenderer = GetComponent<LineRenderer>();
		point = Resources.Load("Point") as GameObject;
		lineCollider = GetComponent<EdgeCollider2D>();
	}
	
	void Update()
	{
		if(Input.GetMouseButtonDown(0)){

			Collider2D col = Physics2D.OverlapCircle(thisCamera.ScreenToWorldPoint(Input.mousePosition), 0.1f);
			if( col == null){
				StartDraw = true;
				dist = 0;
				distanceMeter = 0f;
				startPos = Input.mousePosition;
				bounds = new List<Bounds>();
			}else{
				if(col.name == "polyObj" || col.name.Contains("Circle")){
					if(DoubleClickTimer+0.3f >= Time.time){ 
						DestroyShape(col.gameObject.transform.parent.gameObject, thisCamera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward*1 + Vector3.up * 2, -10);}
					else DoubleClickTimer = Time.time;
				}else if(col.name.Contains("Point")){
					if(DoubleClickTimer+0.3f >= Time.time){ 
						DestroyShape(col.gameObject.transform.parent.parent.gameObject, thisCamera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward*1 + Vector3.up * 2, -10);}
					else DoubleClickTimer = Time.time;
				}
			}
		}

		else if ( Input.GetMouseButton(0) )	{
			if(StartDraw){
				if(distanceMeter == Mathf.Infinity) distanceMeter = 0;
			}
		}
		else if(Input.GetMouseButtonUp(0)){
			if(StartDraw)
				StopDraw();
		}

		if(StartDraw ){
			Vector2 mousePos = Input.mousePosition;
			Vector2 mouseWorld = thisCamera.ScreenToWorldPoint(mousePos);
			mouseWorld = new Vector2(mouseWorld.x, mouseWorld.y);
			dist = Vector2.Distance(lastPos, mouseWorld);
			
			if(dist <= threshold)
				return;
			
			lastPos = mouseWorld;
			if(linePoints == null)
				linePoints = new List<Vector2>();

			if( Physics2D.OverlapCircle(mouseWorld, 0.1f) == null){
				linePoints.Add(mouseWorld);
				if(linePoints.Count> 1){
					distanceMeter += Vector2.Distance(mouseWorld, linePoints[linePoints.Count-2]);
				}
			}

		}
		UpdateLine();
	}

	public void StopDraw(){
		StartDraw = false;
		previousObj = null;
		startPoint = null;
		if(linePoints.Count<2){ linePoints.Clear(); return;}

		#region getting circle shape

		Hashtable table = new Hashtable();
		foreach (Vector3 item in linePoints) {    
			float radius = (float) System.Math.Round( Vector2.Distance( item,  compute2DPolygonCentroid(linePoints.ToArray(), linePoints.Count)), 2);
			if(!table.ContainsKey( radius)) table.Add(radius, 0);
			table[radius] = (int) table[radius] + 1;
		}
		GameObject parentObj = new GameObject("lineObject");
		parentObj.transform.parent = transform;

		foreach (DictionaryEntry  item in table) {    
			if((int)item.Value >= 10 && table.Count > 6) {
				CreateCircle(compute2DPolygonCentroid(linePoints.ToArray(), linePoints.Count), ""+item.Key, parentObj);
				linePoints.Clear();

				return;
			}
		}

		#endregion
		
		int ii = 0;
		GameObject gm = null;
		GameObject polyObj = new GameObject("polyObj");
		polyObj.transform.parent = parentObj.transform;
		PlaneFromPoly scr = polyObj.AddComponent<PlaneFromPoly>();
		
		foreach (Vector3 item in linePoints) {                                  //create points
			gm = Instantiate(point, item, transform.rotation) as GameObject;
			if(startPoint == null) startPoint = gm;
			gm.transform.parent = polyObj.transform;
			gm.GetComponent<LineObject>().firstObject = item;
			gm.GetComponent<LineObject>().nextObject = gm;
			if(previousObj != null){
				gm.GetComponent<LineObject>().nextObject = previousObj;
			}
			previousObj = gm;
			ii++;
		}
		scr.mat = mat;
		scr.poly = linePoints.ToArray();
		scr.keyPoints = linePoints;
		
		int scoreMinus = -80;
		
		if(Vector3.Distance( startPos, Input.mousePosition) < 10f){     // if shape is rounded
			scoreMinus = -50;
			scr.BuildMesh(linePoints);
			gm.GetComponent<LineObject>().nextObject = startPoint;
			polyCollider = polyObj.AddComponent<PolygonCollider2D>();
			polyCollider.sharedMaterial = bounce;
			polyCollider.CreatePrimitive(linePoints.Count);
			polyCollider.SetPath(0, linePoints.ToArray() );
		}
		else{
            edgeCollider = polyObj.AddComponent<EdgeCollider2D>();
            edgeCollider.sharedMaterial = bounce;

//            polyCollider = polyObj.AddComponent<PolygonCollider2D>();
//            polyCollider.sharedMaterial = bounce;
//            polyCollider.CreatePrimitive( linePoints.Count * 2 );

			Vector2[] listV2 = new Vector2[linePoints.Count*2];
			for (int i = 0; i < linePoints.Count; i++) {
				listV2[i] = linePoints[i];
			}
			int j =0;
			for (int i = linePoints.Count - 1; i >= 0; i--) {
				listV2[j+linePoints.Count] = linePoints[i]+Vector2.up*0.05f;
				j++;
            }

			edgeCollider.points = linePoints.ToArray(); //listV2;

//			polyCollider.SetPath(0, listV2);
		}


		previousObj = null;
		startPoint = null;

		Rigidbody2D r = polyObj.AddComponent<Rigidbody2D>();
		r.mass = 0.2f * linePoints.Count;
//		r.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		linePoints.Clear();
	}



	void CreateCircle(Vector2 pos, string radius, GameObject parent ){
		GameObject gm = Instantiate(Resources.Load("Circle"), pos, transform.rotation) as GameObject;
		gm.GetComponent<LineObject>().radius = float.Parse( radius);
		gm.transform.parent = parent.transform;
		Rigidbody2D r = gm.AddComponent<Rigidbody2D>();
		r.mass = 10f;
	}
	
	void UpdateLine()
	{
		lineRenderer.SetWidth(startWidth, endWidth);
		lineRenderer.SetVertexCount(linePoints.Count);
		
		for(int i = lineCount; i < linePoints.Count; i++)
		{
			lineRenderer.SetPosition(i, linePoints[i]);
		}
		lineCount = linePoints.Count;
	}

	public void DestroyShape(GameObject gm, Vector3 pos, int scoreMinus, bool fall = false){
		Destroy( gm);
	}

	Vector2 compute2DPolygonCentroid( Vector2[] vertices, int vertexCount)
	{
		Vector2 centroid = new Vector2(0, 0);
		float signedArea = 0.0f;
		float x0 = 0.0f; // Current vertex X
		float y0 = 0.0f; // Current vertex Y
		float x1 = 0.0f; // Next vertex X
		float y1 = 0.0f; // Next vertex Y
		float a = 0.0f;  // Partial signed area
		
		// For all vertices except last
		int i=0;
		for (i=0; i<vertexCount-1; ++i)
		{
			x0 = vertices[i].x;
			y0 = vertices[i].y;
			x1 = vertices[i+1].x;
			y1 = vertices[i+1].y;
			a = x0*y1 - x1*y0;
			signedArea += a;
			centroid.x += (x0 + x1)*a;
			centroid.y += (y0 + y1)*a;
		}
		
		// Do last vertex
		x0 = vertices[i].x;
		y0 = vertices[i].y;
		x1 = vertices[0].x;
		y1 = vertices[0].y;
		a = x0*y1 - x1*y0;
		signedArea += a;
		centroid.x += (x0 + x1)*a;
		centroid.y += (y0 + y1)*a;
		
		signedArea *= 0.5f;
		centroid.x /= (6.0f*signedArea);
		centroid.y /= (6.0f*signedArea);
		
		return centroid;
	}
}
