using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class CatalogCanvas : MonoBehaviour
    {
        [Header("Load")]
        [SerializeField] private CanvasGroup fadePanel;
        [SerializeField] private GameObject loadCircle;

        [FormerlySerializedAs("scrollPanel")]
        [Header("Object Data")] 
        [SerializeField] private CanvasGroup catalogScrollPanel;
        [SerializeField] private CanvasGroup dataPanel;
        [SerializeField] private TMP_InputField widthField;
        [SerializeField] private TMP_InputField heightField;
        [SerializeField] private TMP_InputField depthField;
        [SerializeField] private Button okButton;
        [SerializeField] private GameObject cubePrefab;
        
        [Header("AR panel")]
        [SerializeField] private CanvasGroup arPanel;
        [SerializeField] private Button backButton;

        public static CatalogCanvas Instance;

        private GameObject arObject;
        
        public Texture2D MaterialTexture { get; set; }
        public float DefaultSize { get; set; }

        private void Awake()
        {
            Instance = this;
            okButton.onClick.AddListener(CloseDataPanel);
            backButton.onClick.AddListener(OpenCatalog);
        }

        public void UnFadeScreen()
        {
            StartCoroutine(UnFadeRoutine());
        }

        public void OpenDataPanel()
        {
            CanvasGroupState(ref dataPanel, true);
        }

        private IEnumerator UnFadeRoutine()
        {
            while (fadePanel.alpha > 0)
            {
                fadePanel.alpha -= 0.01f;
                yield return null;
            }

            fadePanel.interactable = false;
            fadePanel.blocksRaycasts = false;
            loadCircle.SetActive(false);
        }

        private void CloseDataPanel()
        {
            CanvasGroupState(ref dataPanel, false);
            CanvasGroupState(ref catalogScrollPanel, false);
            InstantiateARObject();
            OpenARPanel();
        }

        private void OpenCatalog()
        {
            CanvasGroupState(ref arPanel, false);
            CanvasGroupState(ref catalogScrollPanel, true);
            DestroyARObject();
        }

        private void InstantiateARObject()
        {
            arObject = Instantiate(cubePrefab);
            arObject.transform.localScale = GetSize();
            
            var material = arObject.GetComponent<MeshRenderer>().material;
            material.mainTexture = MaterialTexture;
        }

        private Vector3 GetSize()
        {
            var width = string.IsNullOrEmpty(widthField.text) ? 
                DefaultSize :
                float.Parse(widthField.text, CultureInfo.InvariantCulture) / 100f;
            var height = string.IsNullOrEmpty(heightField.text) ? 
                DefaultSize :
                float.Parse(heightField.text, CultureInfo.InvariantCulture) / 100f;
            var depth = string.IsNullOrEmpty(depthField.text) ? 
                DefaultSize :
                float.Parse(depthField.text, CultureInfo.InvariantCulture) / 100f;
            
            return new Vector3(width, height, depth);
        }

        private void OpenARPanel()
        {
            CanvasGroupState(ref arPanel, true);
        }

        private void DestroyARObject()
        {
            Destroy(arObject);
            arObject = null;
        }

        private void CanvasGroupState(ref CanvasGroup canvasGroup, bool state)
        {
            canvasGroup.alpha = state ? 1f : 0f;
            canvasGroup.interactable = state;
            canvasGroup.blocksRaycasts = state;
        }
    }
}
