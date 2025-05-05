using System.Collections.Generic;
using UnityEngine;
using System.IO;

// Törölt elemek ID-jainak kezelését végzi JSON fájlban.
public class DeletedIdManager : MonoBehaviour
{
    // A JSON fájl elérési útja
    private static string path => Application.persistentDataPath + "/deleted_ids.json";

    // ID-k listájának tárolására szolgáló segédosztály
    [System.Serializable]
    private class IdList
    {
        public List<string> ids = new List<string>();
    }

    // Betölti az ID listát a fájlból, ha van
    private static IdList LoadIdList()
    {
        // Ha nincs még fájl, visszaadunk egy üres listát
        if (!File.Exists(path))
            return new IdList();

        // Fájl beolvasása szövegként
        string json = File.ReadAllText(path);

        // JSON-ból objektum konvertálása, ha nem sikerül, üres listát adunk vissza
        return JsonUtility.FromJson<IdList>(json) ?? new IdList();
    }

    // Elmenti az ID listát JSON fájlba
    private static void SaveIdList(IdList idList)
    {
        // Objektum átalakítása formázott JSON szöveggé
        string json = JsonUtility.ToJson(idList, true);

        // JSON szöveg kiírása fájlba
        File.WriteAllText(path, json);
    }

    // Új törölt ID hozzáadása a listához
    public static void AddDeletedId(string id)
    {
        // Betöltjük az aktuális ID listát
        var idList = LoadIdList();

        // Ha még nincs benne az ID, hozzáadjuk
        if (!idList.ids.Contains(id))
        {
            idList.ids.Add(id);
            SaveIdList(idList);
        }
    }

    // Az összes törölt ID törlése (fájl törlése)
    public static void ClearDeletedIds()
    {
        // Ha létezik a fájl, kitöröljük
        if (File.Exists(path))
            File.Delete(path);
    }

    // Az aktuális törölt ID-k lekérdezése
    public static List<string> GetDeletedIds()
    {
        // Visszaadjuk a tárolt ID listát
        return LoadIdList().ids;
    }
}
