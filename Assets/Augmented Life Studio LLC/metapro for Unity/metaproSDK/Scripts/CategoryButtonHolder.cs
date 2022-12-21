using metaproSDK.Scripts.Controllers;
using TMPro;
using UnityEngine;

namespace metaproSDK.Scripts
{
    public class CategoryButtonHolder : MonoBehaviour
    {
        [SerializeField] private AssetsWindowController assetsWindowController;
        [SerializeField] private TextMeshProUGUI categoryNameText;
        [SerializeField] private string categoryName;
        
        public void SetupCategoryButton(string category)
        {
            categoryNameText.text = category;
        }

        public void HandleClick()
        {
            assetsWindowController.FilterAssetList(categoryName);
        }
    }
}