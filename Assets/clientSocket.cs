using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



[RequireComponent (typeof(MeshFilter))]

public class clientSocket : MonoBehaviour
{

	const int Port = 10000;
	//端口号与服务端端口对应
	private TcpClient client;
	byte[] data;

	public string UserName = "";
	//用户名
	public string message = "";
	//聊天内容
	public string sendMsg = "";
	//输入框

//	private float yaw;
//	private float pitch;
//	private float roll;

	private float w;
	private float x;
	private float y;
	private float z;

	public float frontDirection;
	private int key;

//	GameObject mobile;
//
//	//mesh 
//	Material green;
//	Mesh meshFront;
//
//	Vector3[] frontMeshVectors;
//	int[] frontMeshTriangles;
//
//	Vector3[] backMeshVectors;
//	int[] backMeshTriangles;
//
//
//	GameObject frontObj;
//
//
//	int center_x = 0;
//	int center_y = 0;
//	int center_z = 0;
//	int radius = 2;
//	float distance = 0.8f;
//	int pointNum = 30;
//
//	Vector3 circleCenter;


	void Awake(){
//		frontObj = GameObject.Find ("MeshFront");
//		mobile = GameObject.Find ("GameObject");

//		meshFront = frontObj.GetComponent<MeshFilter> ().mesh;
	}

	void OnGUI ()
	{
		UserName = GUI.TextField (new Rect (10, 10, 100, 20), UserName);
		message = GUI.TextArea (new Rect (10, 40, 300, 200), message);
		sendMsg = GUI.TextField (new Rect (10, 250, 210, 20), sendMsg);

		if (GUI.Button (new Rect (120, 10, 80, 20), "连接服务器")) {
			this.client = new TcpClient ();
			this.client.Connect ("141.76.22.29", Port);
//			this.client.Connect ("192.168.1.103", Port);
			data = new byte[this.client.ReceiveBufferSize];
			SendSocket (UserName);
			this.client.GetStream ().BeginRead (data, 0, System.Convert.ToInt32 (this.client.ReceiveBufferSize), ReceiveMessage, null);
		}
		;

		if (GUI.Button (new Rect (230, 250, 80, 20), "发送")) {
			SendSocket (sendMsg);
			sendMsg = "";
		}
		;
	}

	public void SendSocket (string message)
	{
		try {
			NetworkStream ns = this.client.GetStream ();
			byte[] data = System.Text.Encoding.ASCII.GetBytes (message);
			ns.Write (data, 0, data.Length);
			ns.Flush ();
//			ns.Close();
		} catch (Exception ex) {
			Debug.Log (ex);
		}
	}

	public void ReceiveMessage (IAsyncResult ar)
	{
		try {
			int bytesRead;
			bytesRead = this.client.GetStream ().EndRead (ar);
			if (bytesRead < 1) {
				return;
			} else {
//				float fTemp = BitConverter.ToSingle(data, 0);
//				 double fTemp = Convert.ToDouble(data);
//				int myInt = Convert.ToInt32(data);


//				message += System.Text.Encoding.ASCII.GetString(data, 0, bytesRead);
//				message += Convert.ToString(myInt);
//				Debug.Log(Convert.ToString(myInt));
//				sbyte[] signed = (sbyte[]) (Array)data; 
//				Debug.Log(data[0]+" "+data[1]+" "+data[2]+" "+data[3]);
//				uint u = (uint)(data[3] | data[2] << 8 |
//					data[1] << 16 | data[0] << 24);
//				Debug.Log(BitConverter.ToInt32(data, 0));

//				euler
//				byte[] bytesYaw = new byte[4];
//				byte[] bytesPitch = new byte[4];
//				byte[] bytesRoll = new byte[4];
//				Array.Copy (data, 0, bytesYaw, 0, 4);
//				Array.Copy (data, 4, bytesPitch, 0, 4);
//				Array.Copy (data, 8, bytesRoll, 0, 4);
//				this.yaw = (float)getUnsignedIntFromBytes (bytesYaw) / 1000;
//				this.pitch = (float)getUnsignedIntFromBytes (bytesPitch) / 1000;
//				this.roll = (float)getUnsignedIntFromBytes (bytesRoll) / 1000; 
//				Debug.Log (yaw + "  " + pitch + "  " + roll + "  ");

				byte[] type = new byte[4]; 

				byte[] wBytes = new byte[4];
				byte[] xBytes = new byte[4];
				byte[] yBytes = new byte[4];
				byte[] zBytes = new byte[4];


				byte[] key = new byte[4];
				byte[] front = new byte[4];


				Array.Copy (data, 0, type, 0, 4);
				int msgType = getIntFromBytes(type);
				if(msgType==0x1){
					//				float myw,myx,myy,myz;
					Array.Copy (data, 4, wBytes, 0, 4);
					Array.Copy (data, 8, xBytes, 0, 4);
					Array.Copy (data, 12, yBytes, 0, 4);
					Array.Copy (data, 16, zBytes, 0, 4);			
					this.w = (float)getIntFromBytes (wBytes) / 1000000; 
					this.x = (float)getIntFromBytes (xBytes) / 1000000; 
					this.y = (float)getIntFromBytes (yBytes) / 1000000; 
					this.z = (float)getIntFromBytes (zBytes) / 1000000; 
//					Debug.Log (w + "  " + x + "  " + y + "  "+" "+z);
				}
				else if(msgType==0x3){
					Array.Copy (data, 4, key, 0, 4);	
					this.key = getIntFromBytes (key); 
					Debug.Log (" key: " + this.key);
				}
				else if(msgType==0x4){
					Array.Copy (data, 4, front, 0, 4);
					this.frontDirection = (float)getIntFromBytes (front) / 1000000;
//					Debug.Log("front "+this.frontDirection);
				}
				else{
					
				}
//				quaternion


//				float myw,myx,myy,myz;
//				Array.Copy (data, 0, wBytes, 0, 4);
//				Array.Copy (data, 4, xBytes, 0, 4);
//				Array.Copy (data, 8, yBytes, 0, 4);
//				Array.Copy (data, 12, zBytes, 0, 4);
//				this.w = (float)getIntFromBytes (wBytes) / 1000000000; 
//				this.x = (float)getIntFromBytes (xBytes) / 1000000000; 
//				this.y = (float)getIntFromBytes (yBytes) / 1000000000; 
//				this.z = (float)getIntFromBytes (zBytes) / 1000000000; 

//				myw = (float)getUnsignedIntFromBytes (w) ; 
//				myx = (float)getUnsignedIntFromBytes (x) ; 
//				myy = (float)getUnsignedIntFromBytes (y) ; 
//				myz = (float)getUnsignedIntFromBytes (z) ; 



			}
			this.client.GetStream ().BeginRead (data, 0, System.Convert.ToInt32 (this.client.ReceiveBufferSize), ReceiveMessage, null);
		} catch (Exception ex) {
			Debug.Log (ex);
		}
	}

//	void CreateCircleCenter ()
//	{
//
//		circleCenter = new Vector3 (center_x, center_y, center_z);
//	}

	// Use this for initialization
	void Start ()
	{
//		green = Resources.Load("meshColor", typeof(Material)) as Material;
//
//		MakeFontOrBackMesh (true);
//
//		CreateMesh ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		this.transform.localRotation = new Quaternion(-x, -y, z, w);
//		this.transform.localRotation = Quaternion.Euler (this.pitch, this.roll, 0-this.yaw);
//		transform.rotation = Quaternion.Euler(new Vector3(0, 60, 0));
//		Debug.Log(this.frontDirection);
	}

	uint getUnsignedIntFromBytes (byte[] data)
	{
		uint u = (uint)(data [3] | data [2] << 8 |
		        data [1] << 16 | data [0] << 24);
		return u;
	}

	int getIntFromBytes(byte[] data){
		
		int u = (int)(data [3] | data [2] << 8 |
			data [1] << 16 | data [0] << 24);
		return u;
	}



	void MakeFontOrBackMesh (bool isFront)
	{
//		Vector3[] meshVectors;
//		int[] meshTriangles;
//
//		float initDegree = 69f;
//		float initRad = (float)(initDegree * Mathf.PI / 180f);
//		meshVectors = new Vector3[pointNum];
//		float length = 2f * Mathf.Cos (initRad) * radius;
//		float step = length / (pointNum - 2);
//		for (int i = 0; i < pointNum - 1; i++) {
//			//			float rad = (initDegree + i * 45 / (pointNum - 2)) * Mathf.PI / 360;
//			//			frontMeshVectors [i] = new Vector3 (centre_x - radius * Mathf.Cos (rad), centre_y + radius * Mathf.Sin (rad), 0);
//
//			float x = center_x - radius * Mathf.Cos (initRad) + i * step;
//			float y;
//			if (isFront) {
//				y = Mathf.Sqrt (Mathf.Pow (radius, 2) - Mathf.Pow (x - center_x, 2)) + center_y;
//			} else {
//				y = center_y - Mathf.Sqrt (Mathf.Pow (radius, 2) - Mathf.Pow (x - center_x, 2));
//			}
//			meshVectors [i] = new Vector3 (x, y, center_z);
//		}
//
//		meshVectors [pointNum - 1] = new Vector3 (center_x, center_y, center_z);
//
//
//		//		frontMeshVectors = new Vector3[]{new Vector3(0,0,0), new Vector3(0,2,0), new Vector3(1,0,0)};
//		//
//		//
//		//		frontMeshTriangles = new int[]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
//		meshTriangles = new int[(pointNum - 2) * 3];
//
//		for (int i = 0; i < pointNum - 2; i++) {
//			for (int j = 0; j < 3; j++) {
//				if (j == 0) {
//					meshTriangles [i * 3 + j] = i;
//				} else if (j == 1) {
//					meshTriangles [i * 3 + j] = i + 1;
//				} else if (j == 2) {
//					meshTriangles [i * 3 + j] = pointNum - 1;
//				}
//
//			}
//		}
//
//		if (isFront) {
//			frontMeshVectors = meshVectors;
//			frontMeshTriangles = meshTriangles;
//		} else {
//			backMeshVectors = meshVectors;
//			backMeshTriangles = meshTriangles;
//		}


//		Vector3[] meshVectors;
//		int[] meshTriangles;
//
//		meshVectors = new Vector3[]{new Vector3(2,0,0), new Vector3(0,0,2), new Vector3(0,0,0)};
//		meshTriangles = new int[]{ 0, 1, 2 };
//		frontMeshVectors = meshVectors;
//		frontMeshTriangles = meshTriangles;
	}

	void CreateMesh (){
//		meshFront.Clear ();
//		meshFront.vertices = frontMeshVectors;
//		meshFront.triangles = frontMeshTriangles;
//		frontObj.transform.eulerAngles = new Vector3(0, 0, 0); 
	}
}
