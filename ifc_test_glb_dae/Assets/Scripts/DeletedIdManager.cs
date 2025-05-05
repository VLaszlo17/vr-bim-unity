using System.Collections.Generic;
using UnityEngine;
using System.IO;

// T�r�lt elemek ID-jainak kezel�s�t v�gzi JSON f�jlban.
public class DeletedIdManager : MonoBehaviour
{
    // A JSON f�jl el�r�si �tja
    private static string path => Application.persistentDataPath + "/deleted_ids.json";

    // ID-k list�j�nak t�rol�s�ra szolg�l� seg�doszt�ly
    [System.Serializable]
    private class IdList
    {
        public List<string> ids = new List<string>();
    }

    // Bet�lti az ID list�t a f�jlb�l, ha van
    private static IdList LoadIdList()
    {
        // Ha nincs m�g f�jl, visszaadunk egy �res list�t
        if (!File.Exists(path))
            return new IdList();

        // F�jl beolvas�sa sz�vegk�nt
        string json = File.ReadAllText(path);

        // JSON-b�l objektum konvert�l�sa, ha nem siker�l, �res list�t adunk vissza
        return JsonUtility.FromJson<IdList>(json) ?? new IdList();
    }

    // Elmenti az ID list�t JSON f�jlba
    private static void SaveIdList(IdList idList)
    {
        // Objektum �talak�t�sa form�zott JSON sz�vegg�
        string json = JsonUtility.ToJson(idList, true);

        // JSON sz�veg ki�r�sa f�jlba
        File.WriteAllText(path, json);
    }

    // �j t�r�lt ID hozz�ad�sa a list�hoz
    public static void AddDeletedId(string id)
    {
        // Bet�ltj�k az aktu�lis ID list�t
        var idList = LoadIdList();

        // Ha m�g nincs benne az ID, hozz�adjuk
        if (!idList.ids.Contains(id))
        {
            idList.ids.Add(id);
            SaveIdList(idList);
        }
    }

    // Az �sszes t�r�lt ID t�rl�se (f�jl t�rl�se)
    public static void ClearDeletedIds()
    {
        // Ha l�tezik a f�jl, kit�r�lj�k
        if (File.Exists(path))
            File.Delete(path);
    }

    // Az aktu�lis t�r�lt ID-k lek�rdez�se
    public static List<string> GetDeletedIds()
    {
        // Visszaadjuk a t�rolt ID list�t
        return LoadIdList().ids;
    }
}
