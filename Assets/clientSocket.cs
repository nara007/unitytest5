using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

	private float frontDirection;
	private int key;

	void OnGUI ()
	{
		UserName = GUI.TextField (new Rect (10, 10, 100, 20), UserName);
		message = GUI.TextArea (new Rect (10, 40, 300, 200), message);
		sendMsg = GUI.TextField (new Rect (10, 250, 210, 20), sendMsg);

		if (GUI.Button (new Rect (120, 10, 80, 20), "连接服务器")) {
			this.client = new TcpClient ();
			this.client.Connect ("141.76.21.206", Port);
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

				Array.Copy (data, 0, type, 0, 4);
				int msgType = getIntFromBytes(type);
				if(msgType==0x1){
					//				float myw,myx,myy,myz;
					Array.Copy (data, 4, wBytes, 0, 4);
					Array.Copy (data, 8, xBytes, 0, 4);
					Array.Copy (data, 12, yBytes, 0, 4);
					Array.Copy (data, 16, zBytes, 0, 4);			
					this.w = (float)getIntFromBytes (wBytes) / 1000000000; 
					this.x = (float)getIntFromBytes (xBytes) / 1000000000; 
					this.y = (float)getIntFromBytes (yBytes) / 1000000000; 
					this.z = (float)getIntFromBytes (zBytes) / 1000000000; 
//					Debug.Log (w + "  " + x + "  " + y + "  "+" "+z);
				}
				else if(msgType==0x3){
					Array.Copy (data, 4, key, 0, 4);	
					this.key = getIntFromBytes (key); 
					Debug.Log (" key: " + this.key);
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


	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		this.transform.localRotation = new Quaternion(-x, -y, z, w);
//		this.transform.localRotation = Quaternion.Euler (this.pitch, this.roll, 0-this.yaw);
//		transform.rotation = Quaternion.Euler(new Vector3(0, 60, 0));
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
}
