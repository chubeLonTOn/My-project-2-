using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Assets.Scripts.Networking
{
    public class NetworkImage : MonoBehaviour
    {
        [SerializeField] private string URL;
        [SerializeField] private RawImage rawImage;
        
        public async void SetImage()
        {
            Texture2D texturestored = await ImageUploader.DownloadImageAsync(URL);
            rawImage.texture = texturestored;
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