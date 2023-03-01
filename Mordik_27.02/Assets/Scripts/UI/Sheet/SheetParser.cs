using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UI.Catalog;
using UnityEngine;
using UnityEngine.Networking;

namespace UI.Sheet
{
    public class SheetParser : MonoBehaviour
    {
        [SerializeField] private CatalogItem catalogItem;
        [SerializeField] private Transform parent;
        
        private const string SheetURL = "https://sheets.googleapis.com/v4/spreadsheets/1Wj75QfY2F8PkNCTMYvOsL-FxYia2mdGvQITVti1xHMk/values/Sheet1?key=AIzaSyA7thtCrF3TDH0Rqhim5Slea1dochV3Nu4";

        private List<string> eachRow = new List<string>();
        private string rowsJson = "";
        private string[] lines;
        private string[] notes;
        private int rawNumber = 1;

        private List<CatalogItem> catalogItems = new List<CatalogItem>();

        private void Awake()
        {
            StartCoroutine(ObtainSheetData());
        }

        private void OnApplicationFocus(bool isFocus)
        {
            if (isFocus) return;
            
            Reinitialize();
        }

        public void Reinitialize()
        {
            StartCoroutine(ReinitializeDataRoutine());
        }

        private IEnumerator ReinitializeDataRoutine()
        {
            yield return GetSheetData();
            
            for (int i = 1; i < lines.Length - 1; i++)
            {
                notes = lines[i].Split(new char[] { ',' });
                var title = notes[0];
                var imageUrl = notes[1];
                var textureUrl = notes[2];
                var size = notes[3];
                
                var imageRequest = UnityWebRequestTexture.GetTexture(imageUrl);
                yield return imageRequest.SendWebRequest();
                var resultImage = ((DownloadHandlerTexture) imageRequest.downloadHandler).texture;
                var sprite = Sprite.Create(resultImage, new Rect(0, 0, resultImage.width, resultImage.height), Vector2.zero);
                
                catalogItems[i - 1].FillItemData(title, sprite, textureUrl, size);
            }
        }

        private IEnumerator ObtainSheetData()
        {
            yield return GetSheetData();

            for (int i = 1; i < lines.Length - 1; i++)
            {
                notes = lines[i].Split(new char[] { ',' });

                var title = notes[0];
                var imageUrl = notes[1];
                var textureUrl = notes[2];
                var size = notes[3];
                var item = Instantiate(catalogItem, parent);

                var imageRequest = UnityWebRequestTexture.GetTexture(imageUrl);
                yield return imageRequest.SendWebRequest();
                var resultImage = ((DownloadHandlerTexture) imageRequest.downloadHandler).texture;
                var sprite = Sprite.Create(resultImage, new Rect(0, 0, resultImage.width, resultImage.height), Vector2.zero);
                
                item.FillItemData(title, sprite, textureUrl, size);
                catalogItems.Add(item);
            }

            CatalogCanvas.Instance.UnFadeScreen();
        }

        private IEnumerator GetSheetData()
        {
            UnityWebRequest request = UnityWebRequest.Get(SheetURL);
            yield return request.SendWebRequest();

            if (!string.IsNullOrEmpty(request.error))
            {
                Debug.LogError(request.error);
                yield break;
            }

            var json = request.downloadHandler.text;
            var data = JSON.Parse(json);
        
            foreach (var item in data["values"])
            {
                var itemo = JSON.Parse(item.ToString());
                eachRow = itemo[0].AsStringList;
            
                foreach (var bro in eachRow)
                {
                    rowsJson += bro+",";
                }
                rowsJson += "\n";
            }

            lines = rowsJson.Split(new char[] { '\n' });
        }
    }
}
