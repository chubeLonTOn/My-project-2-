using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    public static class ImageUploader
    {
        const string API_URL = "https://uploads.marcwerk.dk/api";
        public static async Task UploadImageAsync(Texture2D texture , string name)
        {
            string URL = $"{API_URL}/upload/raw" + 
                         "?userId=LonTOn" +
                         "&subfolder=Test" +
                         $"&filename={name}";
            byte[] bytes = texture.EncodeToPNG();
            using UnityWebRequest request = new UnityWebRequest(URL, UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "image/png");
            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                return;
            }
            Debug.Log(request.downloadHandler.text);
        }

        public static async Task<Texture2D> DownloadImageAsync(string url)
        {
            using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                return null;
            }
            Texture2D storedTexture = DownloadHandlerTexture.GetContent(request);
            storedTexture.name = Path.GetFileNameWithoutExtension(url);
            return storedTexture;
        }
    }
}