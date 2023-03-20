using UnityEngine;
using SendBird;

public class SendBirdUnity : MonoBehaviour
{
	void Awake ()
	{
		SendBirdClient.SetupUnityDispatcher (gameObject);
		StartCoroutine (SendBirdClient.StartUnityDispatcher);

        SendBirdClient.Init("45832989-C4A3-4103-A316-9354AAC1DB0B"); // App ID
        SendBirdClient.LoggerLevel = 100000;
        SendBirdClient.Log += (message) =>
        {
            Debug.Log(message);
        };
    }
}
