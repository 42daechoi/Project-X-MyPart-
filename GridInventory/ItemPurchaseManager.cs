using UnityEngine;
using UnityEngine.UI;

public class ItemPurchaseManager : MonoBehaviour
{
    public GameObject popupPanel; // 구매 CHEKCBOX POPUP
    private GameObject currentItemPanel; // 클릭 아이템 기억

    public void OnItemPanelClicked(GameObject panel)
    {
        currentItemPanel = panel;
        popupPanel.SetActive(true); 
    }

    // POPUP - 구매
    public void OnConfirmPurchase()
    {
        Debug.Log(currentItemPanel.name + " 구매 완료!");
        popupPanel.SetActive(false); // 팝업 닫기
    }

    // POPUP - 취소
    public void OnCancelPurchase()
    {
        popupPanel.SetActive(false); // 팝업 닫기
    }
}
