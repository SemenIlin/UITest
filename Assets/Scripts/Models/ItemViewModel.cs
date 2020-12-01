using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemViewModel
{
    public TextMeshProUGUI ProductTitleText { get; set; }
    public TextMeshProUGUI ProductPriceText { get; set; }
    public TextMeshProUGUI ProductQuantityText { get; set; }
    public Image ProductImage { get; set; }

    public TextMeshProUGUI UserLevelText { get; set; }
    public TextMeshProUGUI UserNameText { get; set; }
    public RawImage UserImage { get; set; }

    public ItemViewModel(Transform itemViewModel)
    {
        ProductTitleText = itemViewModel.Find("ProductTitleText").GetComponent<TextMeshProUGUI>();
        ProductPriceText = itemViewModel.Find("PriceText").GetComponent<TextMeshProUGUI>();
        ProductQuantityText = itemViewModel.Find("QuantityText").GetComponent<TextMeshProUGUI>();
        ProductImage = itemViewModel.Find("ProductImage").GetComponent<Image>();

        UserLevelText = itemViewModel.Find("AvaterImage/LevelImage/UserLevelText").GetComponent<TextMeshProUGUI>();
        UserNameText = itemViewModel.Find("AvaterImage/UserNameText").GetComponent<TextMeshProUGUI>();
        UserImage = itemViewModel.Find("AvaterImage").GetComponent<RawImage>();
    }
}
