using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private GameObject openButton;
    [SerializeField] private GameObject UI;

    public void CloseUI() 
    {
        UI.SetActive(false);
        openButton.SetActive(true);
    }

    public void OpenUI()
    {
        UI.SetActive(true);
        openButton.SetActive(false);
    }
}
