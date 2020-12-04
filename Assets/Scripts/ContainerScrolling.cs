using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerScrolling : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{    
    [SerializeField] private GameObject containerPrefab;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private float DeltaSwipe;

    [SerializeField] private Texture2D[] usersAvatar;

    [SerializeField] private int totalQuantityItems;

    private WWW www;
    private UnityEngine.Object[] products;

    private int quantityItemsInRow;
    private int quantityItemsRow;

    private RectTransform rectTransformOfItem;
    private RectTransform rectTransformOfContainerForItems;

    private Vector2 tapPoint;
    private Vector2 endPoint;

    private int selectedID;
    private bool isMobilePlatform;

    private GameObject[] instPans;
    private Vector2[] panPos;

    private RectTransform contentRect;

    private string path;

    private void Awake()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
             isMobilePlatform = false;
        #else
             isMobilePlatform = true;
        #endif
    }
    private void Start()
    {
        products = Resources.LoadAll("",typeof(Sprite));        

        TotalQuantityItems = totalQuantityItems;
        rectTransformOfItem = itemPrefab.GetComponent<RectTransform>();
        rectTransformOfContainerForItems = containerPrefab.GetComponent<RectTransform>();

        quantityItemsInRow = (int)(rectTransformOfContainerForItems.sizeDelta.x / rectTransformOfItem.sizeDelta.x);
        quantityItemsRow = (int)(rectTransformOfContainerForItems.sizeDelta.y / rectTransformOfItem.sizeDelta.y);

        CountItemsInContainer = quantityItemsInRow * quantityItemsRow;
        QuaintityPages = (int)Math.Ceiling((double)totalQuantityItems / (double)CountItemsInContainer);

        contentRect = GetComponent<RectTransform>();
        path = @"E:\source\C#\mobile\UIForTest\data.txt";

        UpdateItems();
    }
    public int CountItemsInContainer { get; private set; }
    public int QuaintityPages { get; private set; }
    public int TotalQuantityItems { get; private set; }

    public void UpdateItems()
    {
        string url = "url";
        www = new WWW(url); 
        StartCoroutine(GetItems(www, results => OnReceivedModels(results)));
    }

    public void GetNextPage()
    {
        if (selectedID == QuaintityPages - 1)
            return;

        selectedID++;
        contentRect.anchoredPosition = (Vector2)panPos[selectedID];
    }

    public void GetPreviousPage()
    {
        if (selectedID == 0)
            return;

        selectedID--;
        contentRect.anchoredPosition = (Vector2)panPos[selectedID];
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        tapPoint = (Vector2)Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (tapPoint.x - endPoint.x >= DeltaSwipe)
        {
            GetNextPage();
        }
        else if (endPoint.x - tapPoint.x >= DeltaSwipe)
        {
            GetPreviousPage();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        endPoint = (Vector2)Input.mousePosition;
    }

    private void OnReceivedModels(Item[] items)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        instPans = new GameObject[QuaintityPages];
        panPos = new Vector2[QuaintityPages];
        selectedID = 0;

        for (var i = 0; i < QuaintityPages; i++)
        {
            instPans[i] = Instantiate(containerPrefab, transform, false);

            for (var j = 0; j < CountItemsInContainer; j++)
            {
                if (i * CountItemsInContainer + j == TotalQuantityItems)
                    break;

                var item = Instantiate(itemPrefab, instPans[i].transform, false);
                InitializeItemView(item, items[i * CountItemsInContainer + j]);
            }

            if (i == 0) continue;
            instPans[i].transform.localPosition = new Vector2(instPans[i - 1].transform.localPosition.x + containerPrefab.GetComponent<RectTransform>().sizeDelta.x,
                                                              instPans[i].transform.localPosition.y);
            panPos[i] = -instPans[i].transform.localPosition;
        }
    }

    private void InitializeItemView(GameObject viewGameObject, Item model)
    {
        ItemViewModel view = new ItemViewModel(viewGameObject.transform);
        if (view == null)
            return;

        view.ProductPriceText.SetText(model.ProductPrice);
        view.ProductQuantityText.SetText(model.ProductQuantity);
        view.ProductTitleText.SetText(model.ProductTitle);
        view.ProductImage.sprite = (Sprite)products[UnityEngine.Random.Range(0, products.Length)];

        view.UserImage.texture = www.error == null ? www.texture : usersAvatar[UnityEngine.Random.Range(0, usersAvatar.Length)];
        view.UserLevelText.SetText(model.UserLevel.ToString());
        view.UserNameText.SetText(model.UserName);
    }

    private IEnumerator GetItems(WWW www, System.Action<Item[]> callback)
    {
        yield return www;
        Item[] items = null;
        if (isMobilePlatform)
        {
            items = JsonHelper.ReadFromJsonServer<Item>(JSONFile);
        }
        else
        {
            items = www.error == null ? JsonHelper.ReadFromJsonServer<Item>(www.text) :
                                               JsonHelper.ReadFromJsonFile<Item>(path); 
        }
       
        callback(items);
    }

    private string JSONFile =
    "{\"array\":[{\"ProductTitle\":\"fess0\",\"ProductPrice\":\"0\",\"ProductQuantity\":\"11\",\"ProductImage\":[],\"UserLevel\":0,\"UserName\":\"user0\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess1\",\"ProductPrice\":\"5\",\"ProductQuantity\":\"12\",\"ProductImage\":[],\"UserLevel\":7,\"UserName\":\"user1\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess2\",\"ProductPrice\":\"10\",\"ProductQuantity\":\"13\",\"ProductImage\":[],\"UserLevel\":14,\"UserName\":\"user2\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess3\",\"ProductPrice\":\"15\",\"ProductQuantity\":\"14\",\"ProductImage\":[],\"UserLevel\":21,\"UserName\":\"user3\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess4\",\"ProductPrice\":\"20\",\"ProductQuantity\":\"15\",\"ProductImage\":[],\"UserLevel\":28,\"UserName\":\"user4\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess5\",\"ProductPrice\":\"25\",\"ProductQuantity\":\"16\",\"ProductImage\":[],\"UserLevel\":35,\"UserName\":\"user5\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess6\",\"ProductPrice\":\"30\",\"ProductQuantity\":\"17\",\"ProductImage\":[],\"UserLevel\":42,\"UserName\":\"user6\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess7\",\"ProductPrice\":\"35\",\"ProductQuantity\":\"18\",\"ProductImage\":[],\"UserLevel\":49,\"UserName\":\"user7\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess8\",\"ProductPrice\":\"40\",\"ProductQuantity\":\"19\",\"ProductImage\":[],\"UserLevel\":56,\"UserName\":\"user8\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess9\",\"ProductPrice\":\"45\",\"ProductQuantity\":\"20\",\"ProductImage\":[],\"UserLevel\":63,\"UserName\":\"user9\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess10\",\"ProductPrice\":\"50\",\"ProductQuantity\":\"21\",\"ProductImage\":[],\"UserLevel\":70,\"UserName\":\"user10\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess11\",\"ProductPrice\":\"55\",\"ProductQuantity\":\"22\",\"ProductImage\":[],\"UserLevel\":77,\"UserName\":\"user11\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess12\",\"ProductPrice\":\"60\",\"ProductQuantity\":\"23\",\"ProductImage\":[],\"UserLevel\":84,\"UserName\":\"user12\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess13\",\"ProductPrice\":\"65\",\"ProductQuantity\":\"24\",\"ProductImage\":[],\"UserLevel\":91,\"UserName\":\"user13\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess14\",\"ProductPrice\":\"70\",\"ProductQuantity\":\"25\",\"ProductImage\":[],\"UserLevel\":98,\"UserName\":\"user14\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess15\",\"ProductPrice\":\"75\",\"ProductQuantity\":\"26\",\"ProductImage\":[],\"UserLevel\":105,\"UserName\":\"user15\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess16\",\"ProductPrice\":\"80\",\"ProductQuantity\":\"27\",\"ProductImage\":[],\"UserLevel\":112,\"UserName\":\"user16\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess13\",\"ProductPrice\":\"65\",\"ProductQuantity\":\"24\",\"ProductImage\":[],\"UserLevel\":91,\"UserName\":\"user13\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess14\",\"ProductPrice\":\"70\",\"ProductQuantity\":\"25\",\"ProductImage\":[],\"UserLevel\":98,\"UserName\":\"user14\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess15\",\"ProductPrice\":\"75\",\"ProductQuantity\":\"26\",\"ProductImage\":[],\"UserLevel\":105,\"UserName\":\"user15\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess0\",\"ProductPrice\":\"0\",\"ProductQuantity\":\"11\",\"ProductImage\":[],\"UserLevel\":0,\"UserName\":\"user0\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess1\",\"ProductPrice\":\"5\",\"ProductQuantity\":\"12\",\"ProductImage\":[],\"UserLevel\":7,\"UserName\":\"user1\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess2\",\"ProductPrice\":\"10\",\"ProductQuantity\":\"13\",\"ProductImage\":[],\"UserLevel\":14,\"UserName\":\"user2\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess3\",\"ProductPrice\":\"15\",\"ProductQuantity\":\"14\",\"ProductImage\":[],\"UserLevel\":21,\"UserName\":\"user3\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess4\",\"ProductPrice\":\"20\",\"ProductQuantity\":\"15\",\"ProductImage\":[],\"UserLevel\":28,\"UserName\":\"user4\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess5\",\"ProductPrice\":\"25\",\"ProductQuantity\":\"16\",\"ProductImage\":[],\"UserLevel\":35,\"UserName\":\"user5\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess6\",\"ProductPrice\":\"30\",\"ProductQuantity\":\"17\",\"ProductImage\":[],\"UserLevel\":42,\"UserName\":\"user6\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess7\",\"ProductPrice\":\"35\",\"ProductQuantity\":\"18\",\"ProductImage\":[],\"UserLevel\":49,\"UserName\":\"user7\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess8\",\"ProductPrice\":\"40\",\"ProductQuantity\":\"19\",\"ProductImage\":[],\"UserLevel\":56,\"UserName\":\"user8\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess9\",\"ProductPrice\":\"45\",\"ProductQuantity\":\"20\",\"ProductImage\":[],\"UserLevel\":63,\"UserName\":\"user9\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess10\",\"ProductPrice\":\"50\",\"ProductQuantity\":\"21\",\"ProductImage\":[],\"UserLevel\":70,\"UserName\":\"user10\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess11\",\"ProductPrice\":\"55\",\"ProductQuantity\":\"22\",\"ProductImage\":[],\"UserLevel\":77,\"UserName\":\"user11\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess12\",\"ProductPrice\":\"60\",\"ProductQuantity\":\"23\",\"ProductImage\":[],\"UserLevel\":84,\"UserName\":\"user12\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess13\",\"ProductPrice\":\"65\",\"ProductQuantity\":\"24\",\"ProductImage\":[],\"UserLevel\":91,\"UserName\":\"user13\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess14\",\"ProductPrice\":\"70\",\"ProductQuantity\":\"25\",\"ProductImage\":[],\"UserLevel\":98,\"UserName\":\"user14\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess15\",\"ProductPrice\":\"75\",\"ProductQuantity\":\"26\",\"ProductImage\":[],\"UserLevel\":105,\"UserName\":\"user15\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess16\",\"ProductPrice\":\"80\",\"ProductQuantity\":\"27\",\"ProductImage\":[],\"UserLevel\":112,\"UserName\":\"user16\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess13\",\"ProductPrice\":\"65\",\"ProductQuantity\":\"24\",\"ProductImage\":[],\"UserLevel\":91,\"UserName\":\"user13\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess14\",\"ProductPrice\":\"70\",\"ProductQuantity\":\"25\",\"ProductImage\":[],\"UserLevel\":98,\"UserName\":\"user14\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess15\",\"ProductPrice\":\"75\",\"ProductQuantity\":\"26\",\"ProductImage\":[],\"UserLevel\":105,\"UserName\":\"user15\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess0\",\"ProductPrice\":\"0\",\"ProductQuantity\":\"11\",\"ProductImage\":[],\"UserLevel\":0,\"UserName\":\"user0\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess1\",\"ProductPrice\":\"5\",\"ProductQuantity\":\"12\",\"ProductImage\":[],\"UserLevel\":7,\"UserName\":\"user1\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess2\",\"ProductPrice\":\"10\",\"ProductQuantity\":\"13\",\"ProductImage\":[],\"UserLevel\":14,\"UserName\":\"user2\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess3\",\"ProductPrice\":\"15\",\"ProductQuantity\":\"14\",\"ProductImage\":[],\"UserLevel\":21,\"UserName\":\"user3\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess4\",\"ProductPrice\":\"20\",\"ProductQuantity\":\"15\",\"ProductImage\":[],\"UserLevel\":28,\"UserName\":\"user4\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess5\",\"ProductPrice\":\"25\",\"ProductQuantity\":\"16\",\"ProductImage\":[],\"UserLevel\":35,\"UserName\":\"user5\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess6\",\"ProductPrice\":\"30\",\"ProductQuantity\":\"17\",\"ProductImage\":[],\"UserLevel\":42,\"UserName\":\"user6\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess7\",\"ProductPrice\":\"35\",\"ProductQuantity\":\"18\",\"ProductImage\":[],\"UserLevel\":49,\"UserName\":\"user7\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess8\",\"ProductPrice\":\"40\",\"ProductQuantity\":\"19\",\"ProductImage\":[],\"UserLevel\":56,\"UserName\":\"user8\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess9\",\"ProductPrice\":\"45\",\"ProductQuantity\":\"20\",\"ProductImage\":[],\"UserLevel\":63,\"UserName\":\"user9\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess10\",\"ProductPrice\":\"50\",\"ProductQuantity\":\"21\",\"ProductImage\":[],\"UserLevel\":70,\"UserName\":\"user10\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess11\",\"ProductPrice\":\"55\",\"ProductQuantity\":\"22\",\"ProductImage\":[],\"UserLevel\":77,\"UserName\":\"user11\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess12\",\"ProductPrice\":\"60\",\"ProductQuantity\":\"23\",\"ProductImage\":[],\"UserLevel\":84,\"UserName\":\"user12\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess13\",\"ProductPrice\":\"65\",\"ProductQuantity\":\"24\",\"ProductImage\":[],\"UserLevel\":91,\"UserName\":\"user13\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess14\",\"ProductPrice\":\"70\",\"ProductQuantity\":\"25\",\"ProductImage\":[],\"UserLevel\":98,\"UserName\":\"user14\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess15\",\"ProductPrice\":\"75\",\"ProductQuantity\":\"26\",\"ProductImage\":[],\"UserLevel\":105,\"UserName\":\"user15\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess16\",\"ProductPrice\":\"80\",\"ProductQuantity\":\"27\",\"ProductImage\":[],\"UserLevel\":112,\"UserName\":\"user16\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess13\",\"ProductPrice\":\"65\",\"ProductQuantity\":\"24\",\"ProductImage\":[],\"UserLevel\":91,\"UserName\":\"user13\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess14\",\"ProductPrice\":\"70\",\"ProductQuantity\":\"25\",\"ProductImage\":[],\"UserLevel\":98,\"UserName\":\"user14\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess15\",\"ProductPrice\":\"75\",\"ProductQuantity\":\"26\",\"ProductImage\":[],\"UserLevel\":105,\"UserName\":\"user15\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess0\",\"ProductPrice\":\"0\",\"ProductQuantity\":\"11\",\"ProductImage\":[],\"UserLevel\":0,\"UserName\":\"user0\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess1\",\"ProductPrice\":\"5\",\"ProductQuantity\":\"12\",\"ProductImage\":[],\"UserLevel\":7,\"UserName\":\"user1\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess2\",\"ProductPrice\":\"10\",\"ProductQuantity\":\"13\",\"ProductImage\":[],\"UserLevel\":14,\"UserName\":\"user2\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess3\",\"ProductPrice\":\"15\",\"ProductQuantity\":\"14\",\"ProductImage\":[],\"UserLevel\":21,\"UserName\":\"user3\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess4\",\"ProductPrice\":\"20\",\"ProductQuantity\":\"15\",\"ProductImage\":[],\"UserLevel\":28,\"UserName\":\"user4\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess5\",\"ProductPrice\":\"25\",\"ProductQuantity\":\"16\",\"ProductImage\":[],\"UserLevel\":35,\"UserName\":\"user5\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess6\",\"ProductPrice\":\"30\",\"ProductQuantity\":\"17\",\"ProductImage\":[],\"UserLevel\":42,\"UserName\":\"user6\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess7\",\"ProductPrice\":\"35\",\"ProductQuantity\":\"18\",\"ProductImage\":[],\"UserLevel\":49,\"UserName\":\"user7\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess8\",\"ProductPrice\":\"40\",\"ProductQuantity\":\"19\",\"ProductImage\":[],\"UserLevel\":56,\"UserName\":\"user8\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess9\",\"ProductPrice\":\"45\",\"ProductQuantity\":\"20\",\"ProductImage\":[],\"UserLevel\":63,\"UserName\":\"user9\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess10\",\"ProductPrice\":\"50\",\"ProductQuantity\":\"21\",\"ProductImage\":[],\"UserLevel\":70,\"UserName\":\"user10\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess11\",\"ProductPrice\":\"55\",\"ProductQuantity\":\"22\",\"ProductImage\":[],\"UserLevel\":77,\"UserName\":\"user11\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess12\",\"ProductPrice\":\"60\",\"ProductQuantity\":\"23\",\"ProductImage\":[],\"UserLevel\":84,\"UserName\":\"user12\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess13\",\"ProductPrice\":\"65\",\"ProductQuantity\":\"24\",\"ProductImage\":[],\"UserLevel\":91,\"UserName\":\"user13\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess14\",\"ProductPrice\":\"70\",\"ProductQuantity\":\"25\",\"ProductImage\":[],\"UserLevel\":98,\"UserName\":\"user14\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess15\",\"ProductPrice\":\"75\",\"ProductQuantity\":\"26\",\"ProductImage\":[],\"UserLevel\":105,\"UserName\":\"user15\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess16\",\"ProductPrice\":\"80\",\"ProductQuantity\":\"27\",\"ProductImage\":[],\"UserLevel\":112,\"UserName\":\"user16\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess13\",\"ProductPrice\":\"65\",\"ProductQuantity\":\"24\",\"ProductImage\":[],\"UserLevel\":91,\"UserName\":\"user13\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess14\",\"ProductPrice\":\"70\",\"ProductQuantity\":\"25\",\"ProductImage\":[],\"UserLevel\":98,\"UserName\":\"user14\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess15\",\"ProductPrice\":\"75\",\"ProductQuantity\":\"26\",\"ProductImage\":[],\"UserLevel\":105,\"UserName\":\"user15\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess0\",\"ProductPrice\":\"0\",\"ProductQuantity\":\"11\",\"ProductImage\":[],\"UserLevel\":0,\"UserName\":\"user0\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess1\",\"ProductPrice\":\"5\",\"ProductQuantity\":\"12\",\"ProductImage\":[],\"UserLevel\":7,\"UserName\":\"user1\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess2\",\"ProductPrice\":\"10\",\"ProductQuantity\":\"13\",\"ProductImage\":[],\"UserLevel\":14,\"UserName\":\"user2\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess3\",\"ProductPrice\":\"15\",\"ProductQuantity\":\"14\",\"ProductImage\":[],\"UserLevel\":21,\"UserName\":\"user3\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess4\",\"ProductPrice\":\"20\",\"ProductQuantity\":\"15\",\"ProductImage\":[],\"UserLevel\":28,\"UserName\":\"user4\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess5\",\"ProductPrice\":\"25\",\"ProductQuantity\":\"16\",\"ProductImage\":[],\"UserLevel\":35,\"UserName\":\"user5\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess6\",\"ProductPrice\":\"30\",\"ProductQuantity\":\"17\",\"ProductImage\":[],\"UserLevel\":42,\"UserName\":\"user6\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess7\",\"ProductPrice\":\"35\",\"ProductQuantity\":\"18\",\"ProductImage\":[],\"UserLevel\":49,\"UserName\":\"user7\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess8\",\"ProductPrice\":\"40\",\"ProductQuantity\":\"19\",\"ProductImage\":[],\"UserLevel\":56,\"UserName\":\"user8\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess9\",\"ProductPrice\":\"45\",\"ProductQuantity\":\"20\",\"ProductImage\":[],\"UserLevel\":63,\"UserName\":\"user9\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess10\",\"ProductPrice\":\"50\",\"ProductQuantity\":\"21\",\"ProductImage\":[],\"UserLevel\":70,\"UserName\":\"user10\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess11\",\"ProductPrice\":\"55\",\"ProductQuantity\":\"22\",\"ProductImage\":[],\"UserLevel\":77,\"UserName\":\"user11\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess12\",\"ProductPrice\":\"60\",\"ProductQuantity\":\"23\",\"ProductImage\":[],\"UserLevel\":84,\"UserName\":\"user12\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess13\",\"ProductPrice\":\"65\",\"ProductQuantity\":\"24\",\"ProductImage\":[],\"UserLevel\":91,\"UserName\":\"user13\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess14\",\"ProductPrice\":\"70\",\"ProductQuantity\":\"25\",\"ProductImage\":[],\"UserLevel\":98,\"UserName\":\"user14\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess15\",\"ProductPrice\":\"75\",\"ProductQuantity\":\"26\",\"ProductImage\":[],\"UserLevel\":105,\"UserName\":\"user15\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess16\",\"ProductPrice\":\"80\",\"ProductQuantity\":\"27\",\"ProductImage\":[],\"UserLevel\":112,\"UserName\":\"user16\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess13\",\"ProductPrice\":\"65\",\"ProductQuantity\":\"24\",\"ProductImage\":[],\"UserLevel\":91,\"UserName\":\"user13\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess14\",\"ProductPrice\":\"70\",\"ProductQuantity\":\"25\",\"ProductImage\":[],\"UserLevel\":98,\"UserName\":\"user14\",\"UserImage\":[]}," +
    "{ \"ProductTitle\":\"fess15\",\"ProductPrice\":\"75\",\"ProductQuantity\":\"26\",\"ProductImage\":[],\"UserLevel\":105,\"UserName\":\"user15\",\"UserImage\":[]}]}";

}
