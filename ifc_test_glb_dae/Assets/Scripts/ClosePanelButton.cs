using UnityEngine;

//Ui panel bez�r�s��rt felel�s oszt�ly
public class ClosePanelButton : MonoBehaviour
{
    public GameObject panelToClose;
    public MetadataSelector selector;

    //Ez z�rja be a panelt, r�tessz�k Inspectorban a gombokra, ONCLICK esem�ynre
    public void ClosePanel()
    {
        //Panel elrejt�se
        panelToClose.SetActive(false);

        //Ha van kijel�lve objektum, akkor annak kikapcsoljuk az outline-j�t
        if (selector != null)
            selector.ClearOutline();
    }
}
