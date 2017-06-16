using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mesh : MonoBehaviour
{



//	GameObject mobileScript;
	public clientSocket mobileScript;
//	Material red;
//	Material green;
	Material newMat;
	Material oldMat;

	//mesh
	Mesh meshFront;
	Mesh meshBack;
	Mesh meshLeftUp;
	Mesh meshLeftDown;
	Mesh meshLeft;
	Mesh meshRight;
	Mesh meshRightUp;
	Mesh meshRightDown;

	GameObject[] meshObjs;

	
	Vector3[] frontMeshVectors;
	int[] frontMeshTriangles;
	
	Vector3[] backMeshVectors;
	int[] backMeshTriangles;

	Vector3[] leftUpMeshVectors;
	int[] leftUpMeshTriangles;

	Vector3[] leftDownMeshVectors;
	int[] leftDownMeshTriangles;

	Vector3[] leftMeshVectors;
	int[] leftMeshTriangles;

	Vector3[] rightMeshVectors;
	int[] rightMeshTriangles;

	Vector3[] rightUpMeshVectors;
	int[] rightUpMeshTriangles;

	Vector3[] rightDownMeshVectors;
	int[] rightDownMeshTriangles;
	
	
	GameObject frontObj;
	GameObject backObj;
	GameObject leftUpObj;
	GameObject leftDownObj;
	GameObject leftObj;
	GameObject rightObj;
	GameObject rightUpObj;
	GameObject rightDownObj;

	GameObject text_0_degree;
	GameObject text_45_degree;
	GameObject text_90_degree;
	GameObject text_135_degree;
	GameObject text_180_degree;
	GameObject text_225_degree;
	GameObject text_270_degree;
	GameObject text_315_degree;
	
	
	int center_x = 25;
	int center_y = 0;
	int center_z = -5;
	int radius = 3;
	float distance = 1.5f;
	int pointNum = 30;


	int meshIndex;


	private int currentDirection = 0;
	
	Vector3 circleCenter;

	void Awake ()
	{
		frontObj = GameObject.Find ("MeshFront");
		backObj = GameObject.Find ("MeshBack");
		leftUpObj = GameObject.Find ("MeshLeftUp");
		leftDownObj = GameObject.Find ("MeshLeftDown");
		leftObj = GameObject.Find ("MeshLeft");
		rightObj = GameObject.Find ("MeshRight");
		rightUpObj = GameObject.Find ("MeshRightUp");
		rightDownObj = GameObject.Find ("MeshRightDown");
		organizeMeshs ();

//		mobileScript = (clientSocket.cs)GameObject.GetComponent<clientSocket.cs>();




		text_0_degree = GameObject.Find ("text_0_degree");
		text_45_degree = GameObject.Find ("text_45_degree");
		text_90_degree = GameObject.Find ("text_90_degree");
		text_135_degree = GameObject.Find ("text_135_degree");
		text_180_degree = GameObject.Find ("text_180_degree");
		text_225_degree = GameObject.Find ("text_225_degree");
		text_270_degree = GameObject.Find ("text_270_degree");
		text_315_degree = GameObject.Find ("text_315_degree");

		meshFront = frontObj.GetComponent<MeshFilter> ().mesh;
		meshBack = backObj.GetComponent<MeshFilter> ().mesh;
		meshLeftUp = leftUpObj.GetComponent<MeshFilter> ().mesh;
		meshLeftDown = leftDownObj.GetComponent<MeshFilter> ().mesh;
		meshLeft = leftObj.GetComponent<MeshFilter> ().mesh;
		meshRight = rightObj.GetComponent<MeshFilter> ().mesh;
		meshRightUp = rightUpObj.GetComponent<MeshFilter> ().mesh;
		meshRightDown = rightDownObj.GetComponent<MeshFilter> ().mesh;
	}

	void organizeMeshs(){
		meshObjs = new GameObject[8];
		meshObjs[0] = frontObj;
		meshObjs[1] = rightUpObj;
		meshObjs[2] = rightObj;
		meshObjs[3] = rightDownObj;
		meshObjs[4] = backObj;
		meshObjs[5] = leftDownObj;
		meshObjs[6] = leftObj;
		meshObjs[7] = leftUpObj;

	}

	// Use this for initialization
	void Start ()
	{
		oldMat = Resources.Load("meshColor", typeof(Material)) as Material;
		newMat = Resources.Load("meshColorRed", typeof(Material)) as Material;

		MakeFontOrBackMesh (true);
		MakeFontOrBackMesh (false);
		MakeLeftUpOrLeftDownMesh (true);
		MakeLeftUpOrLeftDownMesh (false);
		MakeLeftOrRightMesh (true);
		MakeLeftOrRightMesh (false);
		MakeRightUpOrRightDownMesh (true);
		MakeRightUpOrRightDownMesh (false);
		CreateMesh ();
		setTextsPosition ();
	}

	// Update is called once per frame
	void Update ()
	{
		ShowCurrentDirection (mobileScript.pointingDirection);
//		if (Input.GetKeyDown (KeyCode.A)) {
//			Debug.Log ("您按下了A键");
//			showNextMesh ();
//		} 
	}

	void showNextMesh(){

		int currentMesh = meshIndex;
		if (meshIndex == 7) {
			meshIndex = 0;

		} else {
			meshIndex++;
		}
		meshObjs [meshIndex].GetComponent<Renderer> ().material = newMat;
		meshObjs [currentMesh].GetComponent<Renderer> ().material = oldMat;

	}

	int getDirectionIndexFromDirection(float direction){

		if (direction >= 337.5 || direction < 22.5) {
			return 0;
		} else if (direction >= 22.5 && direction < 67.5) {

			return 1;
		} else if (direction >= 67.5 && direction < 112.5) {

			return 2;
		} else if (direction >= 112.5 && direction < 157.5) {

			return 3;
		} else if (direction >= 157.5 && direction < 202.5) {

			return 4;
		} else if (direction >= 202.5 && direction < 247.5) {

			return 5;
		} else if (direction >= 247.5 && direction < 292.5) {

			return 6;
		} else if (direction >= 292.5 && direction < 337.5) {

			return 7;
		} else {
			return 100;
		}
	}
	void ShowCurrentDirection(float direction){

//		int currentDireIndex = getDirectionIndexFromDirection (currentDirection);
		int newDirection = getDirectionIndexFromDirection (direction);
		if (newDirection == currentDirection) {

		} else {
			meshObjs [currentDirection].GetComponent<Renderer> ().material = oldMat;
			currentDirection = newDirection;
			meshObjs [currentDirection].GetComponent<Renderer> ().material = newMat;
		}


//		Debug.Log ("currentDireIndex:" +currentDireIndex+" newDireIndex: "+newDireIndex);

	}

	void MakeFontOrBackMesh (bool isFront)
	{
		Vector3[] meshVectors;
		int[] meshTriangles;
		
		float initDegree = 69f;
		float initRad = (float)(initDegree * Mathf.PI / 180f);
		meshVectors = new Vector3[pointNum];
		float length = 2f * Mathf.Cos (initRad) * radius;
		float step = length / (pointNum - 2);
		for (int i = 0; i < pointNum - 1; i++) {
			//			float rad = (initDegree + i * 45 / (pointNum - 2)) * Mathf.PI / 360;
			//			frontMeshVectors [i] = new Vector3 (centre_x - radius * Mathf.Cos (rad), centre_y + radius * Mathf.Sin (rad), 0);
		
			float x = center_x - radius * Mathf.Cos (initRad) + i * step;
			float z;
			if (isFront) {
				z = -Mathf.Sqrt (Mathf.Pow (radius, 2) - Mathf.Pow (x - center_x, 2)) + center_z;
			} else {
				z = center_z + Mathf.Sqrt (Mathf.Pow (radius, 2) - Mathf.Pow (x - center_x, 2));
			}
			meshVectors [i] = new Vector3 (x, center_y, z);
		}
		
		meshVectors [pointNum - 1] = new Vector3 (center_x, center_y, center_z);
		
		
		//		frontMeshVectors = new Vector3[]{new Vector3(0,0,0), new Vector3(0,2,0), new Vector3(1,0,0)};
		//
		//
		//		frontMeshTriangles = new int[]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
		meshTriangles = new int[(pointNum - 2) * 3];
		
		for (int i = 0; i < pointNum - 2; i++) {
			for (int j = 0; j < 3; j++) {
				if (j == 0) {
					meshTriangles [i * 3 + j] = i;
				} else if (j == 1) {
					meshTriangles [i * 3 + j] = i + 1;
				} else if (j == 2) {
					meshTriangles [i * 3 + j] = pointNum - 1;
				}
		
			}
		}
		
		if (isFront) {
			frontMeshVectors = meshVectors;
			frontMeshTriangles = meshTriangles;
		} else {
			backMeshVectors = meshVectors;
			backMeshTriangles = meshTriangles;
		}


//		Vector3[] meshVectors;
//		int[] meshTriangles;
//		
//		meshVectors = new Vector3[]{ new Vector3 (2, 0, 0), new Vector3 (0, 0, 2), new Vector3 (0, 0, 0) };
//		meshTriangles = new int[]{ 0, 1, 2 };
//		frontMeshVectors = meshVectors;
//		frontMeshTriangles = meshTriangles;
	}


	void MakeLeftUpOrLeftDownMesh (bool isLeftUp)
	{

		Vector3[] meshVectors;
		int[] meshTriangles;

		float initDegree = 24f;
		float initRad = (float)(initDegree * Mathf.PI / 180f);
		float endDegree = 66f;
		float endRad = (float)(endDegree * Mathf.PI / 180f);
		meshVectors = new Vector3[pointNum];
		float length = (Mathf.Cos (initRad) - Mathf.Cos (endRad)) * radius;
		float step = length / (pointNum - 2);
		for (int i = 0; i < pointNum - 1; i++) {
			//			float rad = (initDegree + i * 45 / (pointNum - 2)) * Mathf.PI / 360;
			//			frontMeshVectors [i] = new Vector3 (centre_x - radius * Mathf.Cos (rad), centre_y + radius * Mathf.Sin (rad), 0);

			float x = center_x - radius * Mathf.Cos (initRad) + i * step;
			float z;
			if (isLeftUp) {
				z = -Mathf.Sqrt (Mathf.Pow (radius, 2) - Mathf.Pow (x - center_x, 2)) + center_z;
			} else {
				z = center_z + Mathf.Sqrt (Mathf.Pow (radius, 2) - Mathf.Pow (x - center_x, 2));
			}
			meshVectors [i] = new Vector3 (x, center_y, z);
		}

		meshVectors [pointNum - 1] = new Vector3 (center_x, center_y, center_z);


		//		frontMeshVectors = new Vector3[]{new Vector3(0,0,0), new Vector3(0,2,0), new Vector3(1,0,0)};
		//
		//
		//		frontMeshTriangles = new int[]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
		meshTriangles = new int[(pointNum - 2) * 3];

		for (int i = 0; i < pointNum - 2; i++) {
			for (int j = 0; j < 3; j++) {
				if (j == 0) {
					meshTriangles [i * 3 + j] = i;
				} else if (j == 1) {
					meshTriangles [i * 3 + j] = i + 1;
				} else if (j == 2) {
					meshTriangles [i * 3 + j] = pointNum - 1;
				}

			}
		}

		if (isLeftUp) {
			leftUpMeshVectors = meshVectors;
			leftUpMeshTriangles = meshTriangles;
		} else {
			leftDownMeshVectors = meshVectors;
			leftDownMeshTriangles = meshTriangles;
		}
	}


	void MakeLeftOrRightMesh (bool isLeft)
	{

		Vector3[] meshVectors;
		int[] meshTriangles;
		float initDegree = 21f;
		float initRad = (float)(initDegree * Mathf.PI / 180f);

		meshVectors = new Vector3[pointNum];
		float length = 2 * radius * Mathf.Sin (initRad);
		float step = length / (pointNum - 2);



		for (int i = 0; i < pointNum - 1; i++) {
			//			float rad = (initDegree + i * 45 / (pointNum - 2)) * Mathf.PI / 360;
			//			frontMeshVectors [i] = new Vector3 (centre_x - radius * Mathf.Cos (rad), centre_y + radius * Mathf.Sin (rad), 0);

			float z = center_z - radius * Mathf.Sin (initRad) + i * step;
			float x;
			if (isLeft) {
				x = -Mathf.Sqrt (Mathf.Pow (radius, 2) - Mathf.Pow (z - center_z, 2)) + center_x;
			} else {
				x = center_x + Mathf.Sqrt (Mathf.Pow (radius, 2) - Mathf.Pow (z - center_z, 2));
			}
			meshVectors [i] = new Vector3 (x, center_y, z);
		}


		meshVectors [pointNum - 1] = new Vector3 (center_x, center_y, center_z);

		meshTriangles = new int[(pointNum - 2) * 3];

		for (int i = 0; i < pointNum - 2; i++) {
			for (int j = 0; j < 3; j++) {
				if (j == 0) {
					meshTriangles [i * 3 + j] = i;
				} else if (j == 1) {
					meshTriangles [i * 3 + j] = i + 1;
				} else if (j == 2) {
					meshTriangles [i * 3 + j] = pointNum - 1;
				}

			}
		}

		if (isLeft) {
			leftMeshVectors = meshVectors;
			leftMeshTriangles = meshTriangles;
		} else {
			rightMeshVectors = meshVectors;
			rightMeshTriangles = meshTriangles;
		}

	}

	void MakeRightUpOrRightDownMesh (bool isUp)
	{

		Vector3[] meshVectors;
		int[] meshTriangles;
		float initDegree = 66f;
		float initRad = (float)(initDegree * Mathf.PI / 180f);
		float endDegree = 24f;
		float endRad = (float)(endDegree * Mathf.PI / 180f);

		meshVectors = new Vector3[pointNum];
		float length = radius * Mathf.Cos (endRad) - radius * Mathf.Cos (initRad);
		float step = length / (pointNum - 2);


		for (int i = 0; i < pointNum - 1; i++) {
			float x = center_x + radius * Mathf.Cos (initRad) + i * step;
			float z;
			if (isUp) {
				z = -Mathf.Sqrt (Mathf.Pow (radius, 2) - Mathf.Pow (x - center_x, 2)) + center_z;
			} else {
				z = center_z + Mathf.Sqrt (Mathf.Pow (radius, 2) - Mathf.Pow (x - center_x, 2));
			}
			meshVectors [i] = new Vector3 (x, center_y, z);
		}

		meshVectors [pointNum - 1] = new Vector3 (center_x, center_y, center_z);

		meshTriangles = new int[(pointNum - 2) * 3];

		for (int i = 0; i < pointNum - 2; i++) {
			for (int j = 0; j < 3; j++) {
				if (j == 0) {
					meshTriangles [i * 3 + j] = i;
				} else if (j == 1) {
					meshTriangles [i * 3 + j] = i + 1;
				} else if (j == 2) {
					meshTriangles [i * 3 + j] = pointNum - 1;
				}

			}
		}

		if (isUp) {
			rightUpMeshVectors = meshVectors;
			rightUpMeshTriangles = meshTriangles;
		} else {
			rightDownMeshVectors = meshVectors;
			rightDownMeshTriangles = meshTriangles;
		}
	}

	void CreateMesh ()
	{
		meshFront.Clear ();
		meshFront.vertices = frontMeshVectors;
		meshFront.triangles = frontMeshTriangles;

		meshBack.Clear ();
		meshBack.vertices = backMeshVectors;
		meshBack.triangles = backMeshTriangles;

		meshLeftUp.Clear ();
		meshLeftUp.vertices = leftUpMeshVectors;
		meshLeftUp.triangles = leftUpMeshTriangles;

		meshLeftDown.Clear ();
		meshLeftDown.vertices = leftDownMeshVectors;
		meshLeftDown.triangles = leftDownMeshTriangles;

		meshLeft.Clear ();
		meshLeft.vertices = leftMeshVectors;
		meshLeft.triangles = leftMeshTriangles;

		meshRight.Clear ();
		meshRight.vertices = rightMeshVectors;
		meshRight.triangles = rightMeshTriangles;

		meshRightUp.Clear ();
		meshRightUp.vertices = rightUpMeshVectors;
		meshRightUp.triangles = rightUpMeshTriangles;

		meshRightDown.Clear ();
		meshRightDown.vertices = rightDownMeshVectors;
		meshRightDown.triangles = rightDownMeshTriangles;
		//		frontObj.transform.eulerAngles = new Vector3(0, 0, 0); 
	}


	void setTextsPosition(){

		text_0_degree.transform.position = new Vector3 (center_x, center_y, center_z-radius-distance);
		text_45_degree.transform.position = new Vector3 (center_x+(radius+distance)*Mathf.Cos(45*Mathf.PI/180), center_y, center_z-(radius+distance)*Mathf.Sin(45*Mathf.PI/180));
		text_90_degree.transform.position = new Vector3 (center_x+(radius+distance), center_y, center_z);
		text_135_degree.transform.position = new Vector3 (center_x+(radius+distance)*Mathf.Cos(45*Mathf.PI/180), center_y, center_z+(radius+distance)*Mathf.Sin(45*Mathf.PI/180));
		text_180_degree.transform.position = new Vector3 (center_x-0.2f, center_y, center_z+(radius+distance));
		text_225_degree.transform.position = new Vector3 (center_x-(radius+distance)*Mathf.Cos(45*Mathf.PI/180), center_y, center_z+(radius+distance)*Mathf.Sin(45*Mathf.PI/180));
		text_270_degree.transform.position = new Vector3 (center_x-(radius+distance), center_y, center_z);
		text_315_degree.transform.position = new Vector3 (center_x-(radius+distance)*Mathf.Cos(45*Mathf.PI/180), center_y, center_z-(radius+distance)*Mathf.Cos(45*Mathf.PI/180));
	}
}
