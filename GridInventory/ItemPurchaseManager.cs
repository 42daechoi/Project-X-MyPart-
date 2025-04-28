using UnityEngine;
using UnityEngine.UI;

public class ItemPurchaseManager : MonoBehaviour
{
    public GameObject popupPanel; // ���� CHEKCBOX POPUP
    private GameObject currentItemPanel; // Ŭ�� ������ ���

    public void OnItemPanelClicked(GameObject panel)
    {
        currentItemPanel = panel;
        popupPanel.SetActive(true); 
    }

    // POPUP - ����
    public void OnConfirmPurchase()
    {
        Debug.Log(currentItemPanel.name + " ���� �Ϸ�!");
        popupPanel.SetActive(false); // �˾� �ݱ�
    }

    // POPUP - ���
    public void OnCancelPurchase()
    {
        popupPanel.SetActive(false); // �˾� �ݱ�
    }
}
