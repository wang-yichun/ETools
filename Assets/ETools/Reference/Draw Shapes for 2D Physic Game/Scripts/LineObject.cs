using UnityEngine;
using System.Collections;
using Vectrosity;

public class LineObject : MonoBehaviour {
	LineRenderer line;
	public Material mat;
	public Vector3 firstObject;
	public GameObject nextObject;
 	private	VectorLine myLine;
	public float radius;

	// Use this for initialization
	void Start () {

		if(!name.Contains("Circle")){
			myLine = new VectorLine("myLine",new Vector3[]{ transform.position, nextObject.transform.position}, mat, 5f); 
			myLine.vectorObject.transform.parent = transform.parent.parent;
			myLine.SetColor( GameObject.Find("Drawer").GetComponent<Renderer>().material.color);
		//	DrawLine.Instance.SetLineCollider(transform.position, nextObject.transform.position, gameObject);
		}
		else{
			Vector3[] linePoints = new Vector3[60+1];
			nextObject = gameObject;
			myLine = new VectorLine("Circle", linePoints, null, 5f, LineType.Continuous, Joins.Weld);
			myLine.vectorObject.transform.parent = transform.parent;
			myLine.SetColor( GameObject.Find("Drawer").GetComponent<Renderer>().material.color);
			transform.localScale = Vector3.one * radius*2;
		}

	}
	
	
	// Update is called once per frame
	void Update () {
		UpdateLine();

	}

	void UpdateLine()
	{
		if(!name.Contains("Circle")){
			myLine.points3[0] = transform.position;
			myLine.points3[1] = nextObject.transform.position;
		}
		else{
			myLine.MakeEllipse(transform.position, Vector3.forward, radius, radius, 60, 0);
		}
		myLine.Draw();
	}

}
