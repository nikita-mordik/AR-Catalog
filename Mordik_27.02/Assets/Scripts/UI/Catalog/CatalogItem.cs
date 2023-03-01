using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI.Catalog
{
    public class CatalogItem : MonoBehaviour
    {
        [SerializeField] private Button itemButton;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Image itemImage;

        private string textureUrl;
        private float objectSize;
        private Texture2D texture;

        private void Start()
        {
            itemButton.onClick.AddListener(SetUpData);
        }

        public void FillItemData(string title, Sprite sprite, string textureUrl, string size)
        {
            titleText.text = title;
            itemImage.sprite = sprite;
            this.textureUrl = textureUrl;
            objectSize = float.Parse(size, CultureInfo.InvariantCulture);
        }

        private void SetUpData()
        {
            CatalogCanvas.Instance.OpenDataPanel();
            CatalogCanvas.Instance.DefaultSize = objectSize;
            StartCoroutine(SetTexture());
        }

        private IEnumerator SetTexture()
        {
            var imageRequest = UnityWebRequestTexture.GetTexture(textureUrl);
            yield return imageRequest.SendWebRequest();
            CatalogCanvas.Instance.MaterialTexture = ((DownloadHandlerTexture) imageRequest.downloadHandler).texture;
        }
    }
}
