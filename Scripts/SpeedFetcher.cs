using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SpeedFetcher : MonoBehaviour
{
   // public int test;
    private const string url = "http://127.0.0.1:3000/speed"; 
    public float currentSpeed = 0f; // Make speed public so the waypoint system can access it

    void Start()
    {
        StartCoroutine(FetchSpeedCoroutine());
    }

    IEnumerator FetchSpeedCoroutine()
    {
        while (true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = webRequest.downloadHandler.text;
                    currentSpeed = JsonUtility.FromJson<SpeedData>(jsonResponse).speed;

                    Debug.Log("Fetched Speed: " + currentSpeed);
                }
                else
                {
                    Debug.LogError("Error fetching speed: " + webRequest.error);
                }
            }

            yield return new WaitForSeconds(1); // Fetch data every second
        }
    }

    [System.Serializable]
    private class SpeedData
    {
        public float speed;
    }
}