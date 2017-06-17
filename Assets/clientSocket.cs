using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


// xml process
using System.Xml;
using System.Xml.Serialization;
using System.IO;


[RequireComponent (typeof(MeshFilter))]

public class clientSocket : MonoBehaviour
{

	//three scope mode
	private ScopeMode scopeMode = ScopeMode.FOUR;
//	private ScopeMode scopeMode = ScopeMode.SIX;
//	private ScopeMode scopeMode = ScopeMode.EIGHT;

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


	private float w;
	private float x;
	private float y;
	private float z;

	//stick pointing direction, updated in real time
	public float pointingDirection;
	private int key;
	//current facing direction, not stick pointing direction, updated by user command 
	public float facingDirection;

	public float up;
	public float down;
	public float left;
	public float right;



	//xml process
	XmlDocument doc;
	SceneAnalyzerOutput objs;
	Hashtable focusedObstacles = new Hashtable ();
	Hashtable absoluteDirection = new Hashtable ();

	void Awake(){

	}

	void OnGUI ()
	{
		UserName = GUI.TextField (new Rect (10, 10, 100, 20), UserName);
		message = GUI.TextArea (new Rect (10, 40, 300, 200), message);
		sendMsg = GUI.TextField (new Rect (10, 250, 210, 20), sendMsg);

		if (GUI.Button (new Rect (120, 10, 80, 20), "连接服务器")) {
			this.client = new TcpClient ();
//			this.client.Connect ("141.76.21.182", Port);
			this.client.Connect ("172.26.144.63", Port);
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
					if(this.key==29){
						this.facingDirection = this.pointingDirection;

						//up scope
						this.up = this.facingDirection;
						//down scope
						float down = this.facingDirection + 180;
						this.down = down>360? down-360:down;
						//left scope
						float left = this.facingDirection + 270;
						this.left = left>360?left-360:left;
						//right scope
						float right = this.facingDirection + 90;
						this.right = right>360?right-360:right;

						Debug.Log ("hello key: " + this.key);

					}
					else if(this.key==8){

						//Debug.Log(getObjectsFromSpecificDirection(getRelativeDirection(this.facingDirection, this.pointingDirection)));
					}
					else if(this.key==9){

//						Debug.Log("jiaodu");
//						Debug.Log(getRelativeDirection2(this.facingDirection, this.pointingDirection));
					}

					// scope mode switch key value 36
					else if(this.key==36){
//						switchScopeMode();
					}
					// cross mode press upkey to get up scope 
					else if(this.key == 19){
						if(scopeMode == ScopeMode.FOUR){
							string str = getObjectsFromSpecificDirection2(this.up);	
							str = str.Replace("\r","");
							str = str.Replace("\n","");
							SendSocket(str+Environment.NewLine);
							Debug.Log(str);	
						}
					}

					// cross mode press downkey to get down scope 
					else if(this.key == 20){
						if(scopeMode == ScopeMode.FOUR){
							string str = getObjectsFromSpecificDirection2(this.down);	
							str = str.Replace("\r","");
							str = str.Replace("\n","");
							SendSocket(str+Environment.NewLine);
							Debug.Log(str);	
						}
					}


					// cross mode press leftkey to get left scope 
					else if(this.key == 21){
						if(scopeMode == ScopeMode.FOUR){
							string str = getObjectsFromSpecificDirection2(this.left);	
							str = str.Replace("\r","");
							str = str.Replace("\n","");
							SendSocket(str+Environment.NewLine);
							Debug.Log(str);	
						}
					}

					// cross mode press rightkey to get right scope 
					else if(this.key == 22){
						if(scopeMode == ScopeMode.FOUR){
							string str = getObjectsFromSpecificDirection2(this.right);	
							str = str.Replace("\r","");
							str = str.Replace("\n","");
							SendSocket(str+Environment.NewLine);
							Debug.Log(str);	
						}
					}


//					Debug.Log (" key: " + this.key);
				}
				else if(msgType==0x4){
					Array.Copy (data, 4, front, 0, 4);
					this.pointingDirection = (float)getIntFromBytes (front) / 1000000;
//					Debug.Log("front "+this.pointingDirection);
				}

				else if(msgType==0x5){
					string str = getObjectsFromSpecificDirection2(getRelativeDirection2(this.facingDirection, this.pointingDirection));
					str = str.Replace("\r","");
					str = str.Replace("\n","");
					SendSocket(str+Environment.NewLine);
					Debug.Log(str);	
				}
				else{
					
				}
//				quaternion


			}
			this.client.GetStream ().BeginRead (data, 0, System.Convert.ToInt32 (this.client.ReceiveBufferSize), ReceiveMessage, null);
		} catch (Exception ex) {
			Debug.Log (ex);
		}
	}
		

	// Use this for initialization
	void Start ()
	{
		// xml process
		doc = new XmlDocument ();
		ProcessXML ();
	}

	// xml process
	void ProcessXML ()
	{
		string path = Application.dataPath + "/xml/scene2.xml";
		//		string path = Application.dataPath + "/xml/cars.xml";

		if (File.Exists (path)) {
			Debug.Log ("file exits...");
			XmlSerializer serializer = new XmlSerializer (typeof(SceneAnalyzerOutput));

			StreamReader reader = new StreamReader (path);
			objs = (SceneAnalyzerOutput)serializer.Deserialize (reader);

//			Debug.Log (objs.AnalyzedFrame [5].Obstacle [1].centralPosition.x);

			//OutPutSVG (0);
			reader.Close ();


			//LoadResults (focusedObstacles, absoluteDirection);

		}
	}

	public sealed class Utf8StringWriter : StringWriter
	{
		public override Encoding Encoding { get { return Encoding.UTF8; } }
	}

	public string calculateDirectionInClock(float absoluteDirection){
		//difference in degree
		float difference = 0.0f;
		if (this.facingDirection < absoluteDirection) {
			difference = absoluteDirection - this.facingDirection;
		} else {
			difference = 360 + absoluteDirection - this.facingDirection;
		}

		if (difference < 15 || difference >= 345) {
			return "12 Uhr ";
		}
		else if(difference>=15 && difference <45){
			return "1 Uhr ";
		}
		else if(difference>=45 && difference <75){
			return "2 Uhr ";
		}
		else if(difference>=75 && difference <105){
			return "3 Uhr ";
		}
		else if(difference>=105 && difference <135){
			return "4 Uhr ";
		}
		else if(difference>=135 && difference <165){
			return "5 Uhr ";
		}
		else if(difference>=165 && difference <195){
			return "6 Uhr ";
		}
		else if(difference>=195 && difference <225){
			return "7 Uhr ";
		}
		else if(difference>=225 && difference <255){
			return "8 Uhr ";
		}
		else if(difference>=255 && difference <285){
			return "9 Uhr ";
		}
		else if(difference>=285 && difference <315){
			return "10 Uhr ";
		}
		else if(difference>=315 && difference <345){
			return "11 Uhr ";
		}
			
		return "falsch Richtung";
	}

	public string LoadResults2(Hashtable objects, Hashtable directions){

		List<OutputObject> objs = new List<OutputObject> ();
		foreach (DictionaryEntry de in objects) {
			int id = (int)(de.Key);
			Obstacle obstacle = (Obstacle)(de.Value);
			OutputObject outputObj = new OutputObject ();
			outputObj.ID = id;
			outputObj.Distance = obstacle.distance;
			outputObj.Direction = (float)(directions[id]);
			outputObj.Category = obstacle.category;
			outputObj.DirectionInClock = calculateDirectionInClock (outputObj.Direction);

			objs.Add (outputObj);

		}

		//序列化这个对象
		XmlSerializer serializer2 = new XmlSerializer (typeof(List<OutputObject>));

		StringWriter textWriter = new Utf8StringWriter ();

		//将对象序列化输出到控制台
		serializer2.Serialize (textWriter, objs);

		//		Debug.Log (textWriter.ToString());

		return textWriter.ToString();
	}

	public string LoadResults(Hashtable objects, Hashtable directions){

		List<OutputObject> objs = new List<OutputObject> ();
		foreach (DictionaryEntry de in objects) {
			int id = (int)(de.Key);
			Obstacle obstacle = (Obstacle)(de.Value);
			OutputObject outputObj = new OutputObject ();
			outputObj.ID = id;
			outputObj.Distance = obstacle.distance;
			outputObj.Direction = (float)(directions[id]);
			outputObj.Category = obstacle.category;

			objs.Add (outputObj);

		}

		//序列化这个对象
		XmlSerializer serializer2 = new XmlSerializer (typeof(List<OutputObject>));

		StringWriter textWriter = new Utf8StringWriter ();

		//将对象序列化输出到控制台
		serializer2.Serialize (textWriter, objs);

//		Debug.Log (textWriter.ToString());

		return textWriter.ToString();
	}

	public void OutPutSVG2(float direction){
		focusedObstacles.Clear ();
		absoluteDirection.Clear ();
		float startDegree;
		float endDegree;
		float step=0f;

		if (scopeMode == ScopeMode.FOUR) {
			step = 90.0f;
		} else if (scopeMode == ScopeMode.SIX) {
			step = 60.0f;
		} else if(scopeMode == ScopeMode.EIGHT){
			step = 45.0f;	
		}

		startDegree = direction - step / 2;
		if (startDegree < 0) {
			startDegree += 360;
		}
		endDegree = direction + step / 2;
		if(endDegree >= 360){
			endDegree -= 360;
		}

		if (objs == null) {
			return;
		} else {
			foreach (AnalyzedFrame curAnalyzedFrame in objs.AnalyzedFrame) {
				foreach (Obstacle obstacle in curAnalyzedFrame.Obstacle) {
					float absDirection = CalcuteAbsoluteYaw (curAnalyzedFrame.Orientation.yaw, obstacle.direction);
					if (isLocatedInScope (startDegree, endDegree, absDirection)) {
						focusedObstacles.Add (obstacle.id, obstacle);
						absoluteDirection.Add (obstacle.id, absDirection);
					} else {
					}


				}
			}
		}


	}

	public void OutPutSVG (float direction)
	{

		focusedObstacles.Clear ();
		absoluteDirection.Clear ();
		string htmlHead = "<!DOCTYPE html>\n<html lang=\"en\">\n\n<head>\n<meta charset=\"UTF-8\">\n<title>Document\n</title>\n<style>\n    .hover_group:hover {\n        /*opacity: 0.5;*/\n        cursor: pointer;\n    }\n    </style>\n</head>\n\n<body>";
		string htmlTail = "</body>\n\n</html>";
		string SVGHeader = "<svg width=\"100%\" height=\"500px\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\">";
		string SVGTail = " </svg>";
		float startDegree;
		float endDegree;

		if (direction < 30) {

			startDegree = 330 + direction;
			endDegree = direction + 30;

		} else if (direction > 330) {
			startDegree = direction - 30;
			endDegree = 30 - (360 - direction);
		} else {
			startDegree = direction - 30;
			endDegree = direction + 30;
		}

		if (objs == null) {
			return;
		} else {
			foreach (AnalyzedFrame curAnalyzedFrame in objs.AnalyzedFrame) {
				foreach (Obstacle obstacle in curAnalyzedFrame.Obstacle) {
					float absDirection = CalcuteAbsoluteYaw (curAnalyzedFrame.Orientation.yaw, obstacle.direction);
					if (isLocatedInScope (startDegree, endDegree, absDirection)) {
						focusedObstacles.Add (obstacle.id, obstacle);
						absoluteDirection.Add (obstacle.id, absDirection);
					} else {
					}


				}
			}

			//			DrawSVG ();

		}

		//		Debug.Log (htmlHead+SVGHeader + DrawSVG (1, 2, 3) + SVGTail+htmlTail);
//		Debug.Log (DrawSVG (500, 250, 150, direction));
//		File.WriteAllText ("./test.html", htmlHead + SVGHeader + DrawSVG (500, 250, 150, direction) + SVGTail + htmlTail);
	}


	public string getObjectsFromSpecificDirection2(float direction){
		OutPutSVG2 (direction);
		return LoadResults2 (focusedObstacles, absoluteDirection);
	}

	public float getRelativeDirection(float fDirection, float curDirection){

		float[] middleDirection = new float[8];
		PointSet[] startEndPoints = new PointSet[8];
		float ret=0;
		middleDirection [0] = fDirection;
		for (int i = 1; i < 8; i++) {
			float tmpDirection = fDirection + i * 45;
			if (tmpDirection >= 360) {
				tmpDirection = tmpDirection - 360;
			}
			middleDirection [i] = tmpDirection;
		}

		for (int i = 0; i < 8; i++) {
			PointSet tmpPoint = new PointSet ();
			float start = middleDirection [i] - 22.5f;
			if (start < 0) {
				start += 360;
			}
			float end = middleDirection [i] + 22.5f;
			if (end >= 360) {
				end -= 360;
			}
			tmpPoint.Start = start;
			tmpPoint.End = end;
			startEndPoints [i] = tmpPoint;
		}

		for (int i = 0; i < 8; i++) {
			float start = startEndPoints [i].Start;
			float end = startEndPoints [i].End;
			if (end>start) {
				if (curDirection > start && curDirection <= end) {
					ret = middleDirection [i];
				} else {
				}
			} else {
				if ((curDirection >= 0 && curDirection <= end) || (curDirection > start && curDirection < 360)) {
					ret = middleDirection [i];
				} else {
				}
			}
		}

		return ret;
			
	}


	public float getRelativeDirection2(float facingDirection, float pointingDirection){
	
		float[] middleDirection;
		PointSet[] startEndPoints;
		int num = 4;
		float step = 90.0f;

		if (scopeMode == ScopeMode.FOUR) {
			num = 4;
			step = 90.0f;
		} else if (scopeMode == ScopeMode.SIX) {
			num = 6;
			step = 60.0f;
			
		} else if (scopeMode == ScopeMode.EIGHT) {
			num = 8;
			step = 45.0f;
		} else {
			Debug.Log ("getRelativeDirection2 error");
		}
		middleDirection = new float[num];
		startEndPoints = new PointSet[num];


		float ret=0;
		middleDirection [0] = facingDirection;
		for (int i = 1; i < num; i++) {
			float tmpDirection = facingDirection + i * step;
			if (tmpDirection >= 360) {
				tmpDirection = tmpDirection - 360;
			}
			middleDirection [i] = tmpDirection;
		}

		for (int i = 0; i < num; i++) {
			PointSet tmpPoint = new PointSet ();
			float start = middleDirection [i] - step/2;
			if (start < 0) {
				start += 360;
			}
			float end = middleDirection [i] + step/2;
			if (end >= 360) {
				end -= 360;
			}
			tmpPoint.Start = start;
			tmpPoint.End = end;
			startEndPoints [i] = tmpPoint;
		}

		for (int i = 0; i < num; i++) {
			float start = startEndPoints [i].Start;
			float end = startEndPoints [i].End;
			if (end>start) {
				if (pointingDirection > start && pointingDirection <= end) {
					ret = middleDirection [i];
					return ret;
				} else {
				}
			} else {
					if ((pointingDirection >= 0 && pointingDirection <= end) || (pointingDirection > start && pointingDirection < 360)) {
						ret = middleDirection [i];
						return ret;
					} else {
					}
			}
		}

		return ret;
		
	}

	public string DrawSVG (float cx, float cy, float radius, float direction)
	{

		//		string circle = "<circle cx=\"500\" cy=\"250\" r=\"150\" stroke=\"lightgreen\" stroke-width=\"2\" fill=\"none\" stroke-dasharray=\"10 20\" />";
		string circle = "<circle cx=" + WrapString (cx.ToString ()) + " cy=" + WrapString (cy.ToString ()) +
			" r=" + WrapString (radius.ToString ()) + " stroke=" + WrapString ("lightgreen") + " stroke-width=" + WrapString ("2") + " fill=" + WrapString ("none") +
			" stroke-dasharray=" + WrapString ("10 20") + " />";
		//		string lineLeft = "<line x1=\"500\" y1=\"250\" x2=\"425\" y2=\"120\" style=\"stroke:rgb(255,0,0);stroke-width:2\" stroke-dasharray=\"10 20\" />";
		float lineLeft_x2 = cx - Mathf.Cos (Mathf.PI / 3) * radius;
		float lineLeft_y2 = cy - Mathf.Sin (Mathf.PI / 3) * radius;
		string lineLeft = "<line x1=" + WrapString (cx.ToString ()) + " y1=" + WrapString (cy.ToString ()) + " x2=" + WrapString (lineLeft_x2.ToString ()) + " y2=" +
			WrapString (lineLeft_y2.ToString ()) + " style=" + WrapString ("stroke:rgb(255,0,0);stroke-width:2") + " stroke-dasharray=" +
			WrapString ("10 20") + " />";

		float lineRight_x2 = cx + Mathf.Cos (Mathf.PI / 3) * radius;
		float lineRight_y2 = cy - Mathf.Sin (Mathf.PI / 3) * radius;

		string lineRight = "<line x1=" + WrapString (cx.ToString ()) + " y1=" + WrapString (cy.ToString ()) + " x2=" + WrapString (lineRight_x2.ToString ()) + " y2=" +
			WrapString (lineRight_y2.ToString ()) + " style=" + WrapString ("stroke:rgb(255,0,0);stroke-width:2") + " stroke-dasharray=" +
			WrapString ("10 20") + " />";

		//		string degree = "<text x=\"500\" y=\"80\" fill=\"red\">0°</text>";
		string degree = "<text x=" + WrapString (cx.ToString ()) + " y=" + WrapString ((cy - radius - 20).ToString ()) + " fill=" + WrapString ("red") + ">" +
			direction + "°" + "</text>";

		string objs = null;
		if (direction > 30 && direction < 330) {

			foreach (DictionaryEntry de in absoluteDirection) { //ht为一个Hashtable实例
				Debug.Log (de.Key);//de.Key对应于key/value键值对key
				Debug.Log (de.Value);//de.Key对应于key/value键值对value
				if (((float)(de.Value) - direction) >= 0) {

					float x = cx + Mathf.Cos ((90 + direction - (float)(de.Value)) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
					float y = cy - Mathf.Sin ((90 + direction - (float)(de.Value)) * Mathf.PI / 180) * ((Obstacle)focusedObstacles [de.Key]).distance * radius / 2;
					objs += DrawCircle (x, y);

				} else {
					float x = cx - Mathf.Cos ((((float)(de.Value) - direction) + 90) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
					float y = cy - Mathf.Sin ((((float)(de.Value) - direction) + 90) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
					objs += DrawCircle (x, y);
				}
			}
		} else if (direction >= 330) {
			foreach (DictionaryEntry de in absoluteDirection) { //ht为一个Hashtable实例
				if (((float)(de.Value) - direction) >= 0) {
					float x = cx + Mathf.Cos ((90 + direction - (float)(de.Value)) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
					float y = cy - Mathf.Sin ((90 + direction - (float)(de.Value)) * Mathf.PI / 180) * ((Obstacle)focusedObstacles [de.Key]).distance * radius / 2;
					objs += DrawCircle (x, y);
				} else {
					if (Mathf.Abs (((float)(de.Value) - direction)) > 30) {
						float x = cx + Mathf.Cos ((90 - 360 + direction - (float)(de.Value)) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
						float y = cy - Mathf.Sin ((90 - 360 + direction - (float)(de.Value)) * Mathf.PI / 180) * ((Obstacle)focusedObstacles [de.Key]).distance * radius / 2;
						objs += DrawCircle (x, y);
					} else {
						float x = cx - Mathf.Cos ((((float)(de.Value) - direction) + 90) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
						float y = cy - Mathf.Sin ((((float)(de.Value) - direction) + 90) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
						objs += DrawCircle (x, y);
					}
				}
			}	
		} else {
			foreach (DictionaryEntry de in absoluteDirection) {
				if (((float)(de.Value) - direction) <= 0) {
					float x = cx - Mathf.Cos ((((float)(de.Value) - direction) + 90) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
					float y = cy - Mathf.Sin ((((float)(de.Value) - direction) + 90) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
					objs += DrawCircle (x, y);
				} else {
					if (Mathf.Abs (((float)(de.Value) - direction)) > 30) {
						float x = cx - Mathf.Cos ((((float)(de.Value) - direction) + 90 - 360) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
						float y = cy - Mathf.Sin ((((float)(de.Value) - direction) + 90 - 360) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
						objs += DrawCircle (x, y);
					} else {
						float x = cx + Mathf.Cos ((90 + direction - (float)(de.Value)) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
						float y = cy - Mathf.Sin ((90 + direction - (float)(de.Value)) * Mathf.PI / 180) * ((Obstacle)focusedObstacles [de.Key]).distance * radius / 2;
						objs += DrawCircle (x, y);	
					}
				}
			}	
		}

		//		return circle+lineLeft+lineRight+degree+DrawCircle(500, 200)+DrawRect(520, 150, 7, 7)+DrawTriangle(10,20);
		return circle + lineLeft + lineRight + degree + objs;
	}

	public string DrawCircle (float cx, float cy)
	{

		//		string circle = "<circle cx=\"490\" cy=\"190\" r=\"5\" fill=\"blue\" />";
		string circle = "<circle cx=" + WrapString (cx.ToString ()) + " cy=" + WrapString (cy.ToString ()) + " r=" + WrapString ("5") + " fill=" + WrapString ("blue") + " />";
		return circle;
	}

	public string DrawRect (float x, float y, float w, float h)
	{

		//		string rect = "<rect x=\"520\" y=\"150\" width=\"7\" height=\"7\" style=\"fill:green\" />";
		string rect = "<rect x=" + WrapString (x.ToString ()) + " y=" + WrapString (y.ToString ()) + " width=" + WrapString (w.ToString ()) +
			" height=" + WrapString (h.ToString ()) + " style=" + WrapString ("fill:green") + " />";
		return rect;
	}

	public string DrawTriangle (float x, float y)
	{
		//		string triangle = "<polygon points=\"450,130 440,130 445,120 \" style=\"fill:red;stroke:black;stroke-width:1\" />";
		float x1 = x - 4;
		float y1 = y + 3;
		float x2 = x + 4;
		float y2 = y + 3;
		float x3 = x;
		float y3 = y - 3;
		string points = x1 + "," + y1 + " " + x2 + "," + y2 + " " + x3 + "," + y3;
		string triangle = "<polygon points=" + WrapString (points) + " style=" + WrapString ("fill:red;stroke:black;stroke-width:1") + " />";
		return triangle;
	}

	private string WrapString (string str)
	{

		return "\"" + str + "\"";
	}

	public float CalcuteAbsoluteYaw (float frameYaw, float relativeDirection)
	{
		float direction = frameYaw + relativeDirection;
		if (direction >= 360) {
			direction = direction - 360;
		} else if (direction < 0) {
			direction = 360 + direction;
		}

		return direction;
	}

	public bool isLocatedInScope (float startDegree, float endDegree, float direction)
	{

		if (startDegree > endDegree) {
			if ((direction >= startDegree && direction < 360) || (direction < endDegree)) {
				return true;
			} else {
				return false;
			}

		} else {
			if (direction >= startDegree && direction <= endDegree) {
				return true;
			} else {
				return false;
			}
		}
	}


	// Update is called once per frame
	void Update ()
	{
		this.transform.localRotation = new Quaternion(-x, -y, z, w);
//		this.transform.localRotation = Quaternion.Euler (this.pitch, this.roll, 0-this.yaw);
//		transform.rotation = Quaternion.Euler(new Vector3(0, 60, 0));
//		Debug.Log(this.facingDirection);
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



	public void switchScopeMode(){
		if (scopeMode == ScopeMode.FOUR) {
			scopeMode = ScopeMode.SIX;
			Debug.Log ("switch to Six mode");
		}
		else if(scopeMode == ScopeMode.SIX){
			scopeMode = ScopeMode.EIGHT;
			Debug.Log ("switch to Eight mode");
		}
		else if(scopeMode == ScopeMode.EIGHT){
			scopeMode = ScopeMode.FOUR;
			Debug.Log ("switch to Four mode");
		}
	}


}

public class PointSet{
	public float Start{ get; set;}
	public float End{ get; set;}
}

//xml process
[System.Xml.Serialization.XmlRoot ("SceneAnalyzerOutput", Namespace = "http://range-it.eu/xmlschemata/sceneanalyzeroutput")]
public class SceneAnalyzerOutput
{
	[XmlArray ("AnalyzedFrames")]
	[XmlArrayItem ("AnalyzedFrame", typeof(AnalyzedFrame))]
	public AnalyzedFrame[] AnalyzedFrame { get; set; }


}


public class AnalyzedFrame
{
	//
	[XmlAttribute]
	public int id { get; set; }
	//
	[XmlAttribute]
	public string timestamp { get; set; }
	//
	[XmlAttribute]
	public string mode { get; set; }
	//
	[XmlAttribute]
	public float cameraHeight { get; set; }
	//
	[XmlAttribute]
	public bool muteState { get; set; }


	[XmlElement ("Orientation", Namespace = "http://range-it.eu/xmlschemata/sceneanalyzeroutput")]
	public Orientation Orientation { get; set; }

	[XmlElement ("Saturation", Namespace = "http://range-it.eu/xmlschemata/sceneanalyzeroutput")]
	public Saturation Saturation { get; set; }


	[XmlArray ("Obstacles")]
	[XmlArrayItem ("Obstacle", typeof(Obstacle))]
	public Obstacle[] Obstacle { get; set; }
}


public class Orientation
{
	[XmlAttribute]
	public float yaw { get; set; }

	[XmlAttribute]
	public float pitch { get; set; }

	[XmlAttribute]
	public float roll { get; set; }
}

public class Saturation
{
	[XmlAttribute]
	public int rate { get; set; }

	[XmlAttribute]
	public string area { get; set; }
}

public class Obstacle
{
	[XmlAttribute]
	public int id { get; set; }

	[XmlAttribute]
	public string obstacleType { get; set; }

	[XmlAttribute]
	public float distance { get; set; }

	[XmlAttribute]
	public float direction { get; set; }

	[XmlAttribute]
	public string category { get; set; }

	[XmlElement ("Size", Namespace = "http://range-it.eu/xmlschemata/sceneanalyzeroutput")]
	public Size size { get; set; }

	[XmlElement ("CentralPosition", Namespace = "http://range-it.eu/xmlschemata/sceneanalyzeroutput")]
	public CentralPosition centralPosition { get; set; }
}

public class Size
{

	[XmlAttribute]
	public float width { get; set; }

	[XmlAttribute]
	public float height { get; set; }

	[XmlAttribute]
	public float depth { get; set; }
}

public class CentralPosition
{

	[XmlAttribute]
	public float x { get; set; }

	[XmlAttribute]
	public float y { get; set; }

	[XmlAttribute]
	public float z { get; set; }
}

[XmlRoot ("object")]
public class OutputObject
{
	//	//定义Color属性的序列化为cat节点的属性
	//	[XmlAttribute ("id")]
	//	public int ID { get; set; }

	//	//要求不序列化Speed属性
	//	[XmlIgnore]
	//	public int Speed { get; set; }

	[XmlElement ("id")]
	public int ID { get; set; }

	[XmlElement ("distance")]
	public float Distance { get; set; }

	[XmlElement ("direction")]
	public float Direction { get; set; }

	[XmlElement ("category")]
	public string Category { get; set; }

	[XmlElement ("directionInClock")]
	public string DirectionInClock { get; set; }

}

// three scope mode 
public enum ScopeMode
{
	FOUR = 0,
	SIX = 1,
	EIGHT= 2,
};