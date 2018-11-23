using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

public class SpinningCube : MonoBehaviour 
{
	public float m_Speed = 200f;

	private Vector3 m_RotationDirection = Vector3.up;
	private AndroidJavaObject datachain;

	public void ToggleRotationDirection()
	{
		Debug.Log ("Toggling rotation direction");

		if (m_RotationDirection == Vector3.up) 
		{
			m_RotationDirection = Vector3.down;
		}
		else 
		{
			m_RotationDirection = Vector3.up;
		}
	}

	void Update() 
	{
		transform.Rotate(m_RotationDirection * Time.deltaTime * m_Speed);
		if (Input.GetKeyDown(KeyCode.Escape)) {
			// notify when game ends
			if(datachain!=null){
				datachain.CallStatic("applicationStop");
			}
    		Application.Quit(); 
		}
	}


	void Start(){

		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		
		// Initializing Datachain
		datachain = new AndroidJavaClass("in.datacha.classes.DataChain");

		AndroidJavaObject instance = datachain.CallStatic<AndroidJavaObject>("getInstance");
		instance = instance.Call<AndroidJavaObject>("publisherKey","PUBLISHER_KEY");
		instance = instance.Call<AndroidJavaObject>("serverUrl","SERVER_URL");
		instance = instance.Call<AndroidJavaObject>("enableLocation",true);			// enable/disable location update
		instance = instance.Call<AndroidJavaObject>("askLocationPermission", true);
		instance = instance.Call<AndroidJavaObject>("locationUpdateInterval", new object[]{12});
		instance.Call("init",activity );


		string email = "jerinamathw@gmail.com";
        string phone = "+919539741274";

		// pass hashed user email or phone number
		datachain.CallStatic("setUserEmail",md5Hash(email),sha1Hash(email),sha256Hash(email));
		datachain.CallStatic("setUserPhoneNumber",md5Hash(phone),sha1Hash(phone),sha256Hash(phone));

		// pass user interest (based on activity)
		datachain.CallStatic("setUserInterest",new object[] {"Play","Game"});

		// pass in app purchase details
		datachain.CallStatic("setUserPurchase", "USD","1.5");
				

	}

		private string md5Hash(string value){
			HashAlgorithm algorithm = new MD5CryptoServiceProvider();
			byte[] message =  ASCIIEncoding.ASCII.GetBytes(value);
            byte[] hashValue = algorithm.ComputeHash(message);
            return hashValue.Aggregate(string.Empty, (current, x) => current + string.Format("{0:x2}", x));
		}

		private string sha1Hash(string value){
			HashAlgorithm algorithm = new SHA1Managed();
			byte[] message =  ASCIIEncoding.ASCII.GetBytes(value);
            byte[] hashValue = algorithm.ComputeHash(message);
            return hashValue.Aggregate(string.Empty, (current, x) => current + string.Format("{0:x2}", x));
		}
		private string sha256Hash(string value){
			HashAlgorithm algorithm = new SHA256Managed();
			byte[] message =  ASCIIEncoding.ASCII.GetBytes(value);
            byte[] hashValue = algorithm.ComputeHash(message);
            return hashValue.Aggregate(string.Empty, (current, x) => current + string.Format("{0:x2}", x));
		}
}
