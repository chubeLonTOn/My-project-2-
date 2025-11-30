using Assets.Singleton;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Assets.Scripts.Networking
{
    public class NetworkImage : MonoBehaviour
    {
        [SerializeField] private string URL;
        [SerializeField] private RawImage rawImage;
        
        public void SetImage()
        {
             CoroutineRunnerSingleton.Instance.StartCoroutine(AsyncHelper.WrapTask<Texture2D>(ImageUploader.DownloadImageAsync(URL), (success, texture) =>
             {
                 if(!success)
                 {
                     Debug.LogError("Failed to download image");
                     return;
                 }                
                 Texture2D texturestored = texture;
                 rawImage.texture = texturestored;
            }));
        }

        void Start()
        {
            SetImage();
        }

        void Update()
        {
            if (Keyboard.current.digit0Key.wasPressedThisFrame)
            {
                SetImage();
            }
        }
    }
}