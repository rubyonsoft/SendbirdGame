using UnityEngine;
using SendBird;

public class SendBirdUnity : MonoBehaviour
{
	void Awake ()
	{
		SendBirdClient.SetupUnityDispatcher (gameObject);
		StartCoroutine (SendBirdClient.StartUnityDispatcher);

        SendBirdClient.Init("Your App ID"); // App ID
        SendBirdClient.LoggerLevel = 100000;
        SendBirdClient.Log += (message) =>
        {
            Debug.Log(message);
        };
    }
}
