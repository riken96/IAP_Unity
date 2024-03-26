using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PurchaseValidator : MonoBehaviour
{
    private const string SERVER_URL = "https://goldygamestudio.com//IAPValidation/IAPValidator.php"; // Replace with your server URL

    public enum PlatformType
    {
        Android,
        iOS
    }

    [Serializable]
    public class PurchaseData
    {
        public string platform;
        public string receipt;
    }

    [Serializable]
    public class ValidationResponse
    {
        public bool valid;
        public string error;
    }

    public void ValidatePurchase(PlatformType platform, string receipt, Action<bool, string> callback)
    {
        PurchaseData data = new PurchaseData
        {
            platform = platform.ToString().ToLower(),
            receipt = receipt
        };

        string jsonData = JsonUtility.ToJson(data);

        UnityWebRequest request = UnityWebRequest.PostWwwForm(SERVER_URL, jsonData);
        request.SetRequestHeader("Content-Type", "application/json");

        StartCoroutine(SendRequest(request, callback));
    }

    private IEnumerator SendRequest(UnityWebRequest request, Action<bool, string> callback)
    {
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ValidationResponse response = JsonUtility.FromJson<ValidationResponse>(request.downloadHandler.text);
            callback(response.valid, response.error);
        }
        else
        {
            Debug.LogError("Error validating purchase: " + request.error);
            callback(false, "Error validating purchase: " + request.error);
        }
    }
}
