# Az alkalmazás beállítása

## Szerver

A hálózat beállítása (parancssorból beimportálhatóak):

**A requirements.txt tartalmazza a szükséges csomagokat**

- IfcConvert és IfcOpenshell telepítése
- Venv szerver beállítása:
  ```bash
  python -m venv venv 
  ```
  aktiválása:
  ```bash
  venv\Scripts\activate
  ```



A `halozat` mappában parancssorból lehet elindítani a szervert az alábbi paranccsal:

```bash
uvicorn ifc_api.main:app --host 0.0.0.0 --port 8000 --reload
```

Ezután az azonos hálózaton lévő eszközök elérik. Ehhez meg kell adni a számítógép IP címét, ami parancssorból az `ipconfig` parancs kiadása után a **Wireless LAN adapter Wi-Fi / IPv4 Address** mellett látható. Emellett a port számot is meg kell adni.

Példa elérési út böngészőben:

```
http://128.127.100.107:8000
```

- Ha van internet kapcsolat, akkor a `/docs` útvonalon elérhető a Swagger UI, ahol grafikusan ki lehet próbálni az útvonalakat.
- Ha nincsen, akkor manuálisan kell megadni. Például a `.glb` fájlok elérhetők itt: `http://<ip.cím>:8000/files/glb`

### Fájlok feldolgozása a szerveren

- `/upload`: `.ifc` fájlok feltöltése
- `/process/{filename}`: a `{filename}` helyére a feltöltött fájl nevét kell írni. A szerver ezután `.glb` (3D modell) és `.json` (metaadatok) fájlokat generál.
- A fájlok listázhatók, letölthetők és törölhetők. A listázást (.glb fájlok: /files/glb) és a letöltést (.glb és .json: /download/{filename}) használja a Unity.

---

## Unity

A projekt neve: **`ifc_test_glb_dae`** (Nem gondoltam az elején, hogy ezzel fogok a későbbiekben foglalkozni, de a nevet meghagytam).

A `Hierarchy` nézetben az alábbi helyen kell megadni a szerver címét:

```
---Models/ModelDownloader/Api Base Url
```

Példa URL:

```
http://127.0.0.1:8000
```

Ezután a **File > Build and Run** menüponttal indítható a tesztelés.

---

## Lehetséges problémák és megoldások

> Ezek a problémák nem mindig fordulnak elő, de érdemes tudni róluk:

- **GLTFUtility hiba (nem látja a Unity):**  
  Megoldás: A Package Managerben távolítsd el a GLTFUtility csomagot a *Remove* gombbal, majd add hozzá újra:  
  `+` → *Add package from GIT URL...* →  
  `https://github.com/Siccity/GLTFUtility.git`

- **Assets mappában nem érzékel elemet:**  
  Megoldás: Húzd át a fájlt másik mappába az `Assets`-en belül, hogy újraimportálódjon.

- **HTTP engedélyezése:**  
  Project Settings → Player → Other Settings → *Allow downloads over HTTP:* → **Always allowed**

- **Android platform beállítása:**  
  Build Settings → Android → **Switch Platform**

- **Modell nem jelenik meg az alkalmazás futása közben:**  
  Ha a modell nem jelenik meg futás közben, húzd be manuálisan a `Scene` nézetbe. Ha ott megjelenik, akkor el lehet távolítani a `Hierarchy`-ból. Ezután általában már automatikusan megjelenik.

- **Nagyobb modellek lassabban töltődnek be.**
