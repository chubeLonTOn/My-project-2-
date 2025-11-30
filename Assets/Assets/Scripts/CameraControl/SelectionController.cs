using System.Collections;
using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SelectionBox : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform selectionBoxUI;
    [SerializeField] private Image selectionBoxImage;
    [SerializeField] private RawImage outputImage;
    
    [Header("Settings")]
    [SerializeField] private Color selectionColor = new Color(0.8f, 0.8f, 1f, 0.25f);
    [SerializeField] private Color borderColor = new Color(0.8f, 0.8f, 1f, 0.8f);
    
    private Vector2 startMousePosition;
    private Vector2 currentMousePosition;
    private bool isDragging = false;
    private Rect selectionRect;
    
    private void Start()
    {
        if (selectionBoxUI == null)
        {
            CreateSelectionBoxUI();
        }
        
        selectionBoxUI.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        HandleInput();
        
        if (isDragging)
        {
            UpdateSelectionBox();
        }
    }
    
    private void HandleInput()
    {
        if (Mouse.current.leftButton.isPressed && !isDragging)
        {
            StartSelection();
        }

        if (Mouse.current.leftButton.isPressed && isDragging)
        {
            currentMousePosition = Mouse.current.position.ReadValue();
            
        }
        
        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            EndSelection();
        }
    }
    
    private void StartSelection()
    {
        startMousePosition = Mouse.current.position.ReadValue();
        currentMousePosition = startMousePosition;
        isDragging = true;
        selectionBoxUI.gameObject.SetActive(true);
    }
    
    private void UpdateSelectionBox()
    {
        float width = Mathf.Abs(currentMousePosition.x - startMousePosition.x);
        float height = Mathf.Abs(currentMousePosition.y - startMousePosition.y);
        Debug.Log("width: " + width + ", height: " + height);
        
        float left = Mathf.Min(startMousePosition.x, currentMousePosition.x);
        float bottom = Mathf.Min(startMousePosition.y, currentMousePosition.y);
        
        selectionBoxUI.anchoredPosition = new Vector2(left, bottom);
        selectionBoxUI.sizeDelta = new Vector2(width, height);
        
        selectionRect = new Rect(left, bottom, width, height);
    }
    
    private void EndSelection()
    {
        isDragging = false;
        selectionBoxUI.gameObject.SetActive(false);
        StartCoroutine(CapturedRegion());
    }
    
    IEnumerator CapturedRegion()
    {
        yield return new WaitForEndOfFrame();
        Texture2D texture = new Texture2D((int)selectionRect.width, (int)selectionRect.height, TextureFormat.RGB24, false);
        texture.ReadPixels(selectionRect, 0, 0);
        texture.Apply();
        ImageUploader.UploadImageAsync(texture , "LonTOn");
    }
    
    private void CreateSelectionBoxUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        GameObject selectionObj = new GameObject("SelectionBox");
        selectionObj.transform.SetParent(canvas.transform, false);
        
        selectionBoxUI = selectionObj.AddComponent<RectTransform>();
        selectionBoxUI.pivot = new Vector2(0, 0); 
        selectionBoxUI.anchorMin = new Vector2(0, 0);
        selectionBoxUI.anchorMax = new Vector2(0, 0);
        
        selectionBoxImage = selectionObj.AddComponent<Image>();
        selectionBoxImage.color = selectionColor;
        
        GameObject borderObj = new GameObject("Border");
        borderObj.transform.SetParent(selectionObj.transform, false);
        
        RectTransform borderRect = borderObj.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.sizeDelta = Vector2.zero;
        
        Outline outline = borderObj.AddComponent<Outline>();
        outline.effectColor = borderColor;
        outline.effectDistance = new Vector2(2, 2);
    }
}