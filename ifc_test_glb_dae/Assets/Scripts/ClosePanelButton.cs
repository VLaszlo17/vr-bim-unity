using UnityEngine;

//Ui panel bezárásáért felelõs osztály
public class ClosePanelButton : MonoBehaviour
{
    public GameObject panelToClose;
    public MetadataSelector selector;

    //Ez zárja be a panelt, rátesszük Inspectorban a gombokra, ONCLICK eseméynre
    public void ClosePanel()
    {
        //Panel elrejtése
        panelToClose.SetActive(false);

        //Ha van kijelölve objektum, akkor annak kikapcsoljuk az outline-ját
        if (selector != null)
            selector.ClearOutline();
    }
}
