from fastapi import FastAPI, UploadFile, File, HTTPException
from pathlib import Path
import shutil
from fastapi import Path as FastAPIPath
from ifc_api.convert import convert_ifc_to_glb
from ifc_api.generate_matadata import generate_metadata_json
from fastapi.responses import FileResponse
import os

# Alapértelmezett mappák létrehozása, ha még nem léteznek
os.makedirs("uploads", exist_ok=True)
os.makedirs("outputs", exist_ok=True)

# FastAPI indítása
app = FastAPI()

# Elérési utak beállítása
BASE_DIR = Path(__file__).resolve().parent.parent
UPLOAD_DIR = BASE_DIR / "uploads"
OUTPUT_DIR = BASE_DIR / "outputs"
UPLOAD_DIR.mkdir(exist_ok=True)
OUTPUT_DIR.mkdir(exist_ok=True)
IFCCONVERT_PATH = BASE_DIR / "IfcConvert.exe"


# IFC fájl feltöltése
@app.post("/upload")
async def upload_ifc(file: UploadFile = File(...)):
    # Ellenőrizzük, hogy .ifc fájlt töltöttek-e fel
    if not file.filename.endswith(".ifc"):
        raise HTTPException(status_code=400, detail="Csak .ifc fájl engedélyezett.")

    # Feltöltési hely meghatározása
    save_path = UPLOAD_DIR / file.filename

    # Ellenőrizzük, hogy nincs-e már ilyen nevű fájl
    if save_path.exists():
        raise HTTPException(status_code=409, detail=f"Ilyen nevű .ifc fájl már létezik: {file.filename}")

    # Fájl mentése a feltöltési mappába
    with open(save_path, "wb") as buffer:
        shutil.copyfileobj(file.file, buffer)

    return {"message": "Fájl sikeresen feltöltve és elmentve.", "filename": file.filename}


# GLB fájl feltöltése
@app.post("/upload/glb")
async def upload_glb(file: UploadFile = File(...)):
    # Ellenőrizzük, hogy .glb fájlt töltöttek-e fel
    if not file.filename.endswith(".glb"):
        raise HTTPException(status_code=400, detail="Csak .glb fájl engedélyezett.")

    # Feltöltési hely meghatározása
    save_path = OUTPUT_DIR / file.filename

    # Ellenőrizzük, hogy nincs-e már ilyen nevű fájl
    if save_path.exists():
        raise HTTPException(status_code=409, detail=f"Ilyen nevű .glb fájl már létezik: {file.filename}")

    # Fájl mentése az output mappába
    with open(save_path, "wb") as buffer:
        shutil.copyfileobj(file.file, buffer)

    return {"message": "GLB fájl sikeresen feltöltve és elmentve.", "filename": file.filename}

# APK fájl feltöltése
@app.post("/upload/apk")
async def upload_apk(file: UploadFile = File(...)):
    # Ellenőrizzük, hogy .apk fájlt töltöttek-e fel
    if not file.filename.endswith(".apk"):
        raise HTTPException(status_code=400, detail="Csak .apk fájl engedélyezett.")

    # Feltöltési hely meghatározása
    save_path = OUTPUT_DIR / file.filename

    # Ellenőrizzük, hogy nincs-e már ilyen nevű fájl
    if save_path.exists():
        raise HTTPException(status_code=409, detail=f"Ilyen nevű .apk fájl már létezik: {file.filename}")

    # Fájl mentése az output mappába
    with open(save_path, "wb") as buffer:
        shutil.copyfileobj(file.file, buffer)

    return {"message": "APK fájl sikeresen feltöltve és elmentve.", "filename": file.filename}

# GLB fájlok listázása
@app.get("/files/glb")
def list_glb_files():
    files = [f.name for f in OUTPUT_DIR.glob("*.glb")]
    return {"glb_files": files}

# JSON fájlok listázása
@app.get("/files/json")
def list_json_files():
    files = [f.name for f in OUTPUT_DIR.glob("*.json")]
    return {"json_files": files}

# IFC fájlok listázása
@app.get("/files/ifc")
def list_ifc_files():
    files = [f.name for f in UPLOAD_DIR.iterdir() if f.is_file()]
    return {"files": files}

# Minden fájl listázása egyszerre
@app.get("/files/all")
def list_all_files():
    ifc_files = [f.name for f in UPLOAD_DIR.glob("*.ifc")]
    glb_files = [f.name for f in OUTPUT_DIR.glob("*.glb")]
    json_files = [f.name for f in OUTPUT_DIR.glob("*.json")]

    return {
        "ifc_files": ifc_files,
        "glb_files": glb_files,
        "json_files": json_files
    }

# APK fájlok listázása
@app.get("/files/apk")
def list_apk_files():
    files = [f.name for f in OUTPUT_DIR.glob("*.apk")]
    return {"apk_files": files}


# Fájl törlése
@app.delete("/files/{filename}")
def delete_file(filename: str = FastAPIPath(..., description="A törlendő fájl neve (pl.: .ifc, .glb vagy .json)")):
    # Ellenőrizzük a fájl kiterjesztését
    ext = Path(filename).suffix.lower()

    if ext not in [".ifc", ".glb", ".json", ".apk"]:
        raise HTTPException(status_code=400, detail="Csak .ifc, .glb, .apk és .json fájlok törölhetők.")

    # Feltöltési vagy output mappában keresünk
    if ext == ".ifc":
        file_path = UPLOAD_DIR / filename
    else:
        file_path = OUTPUT_DIR / filename

    # Ellenőrizzük, hogy a fájl létezik-e
    if not file_path.exists():
        raise HTTPException(status_code=404, detail="A megadott fájl nem található.")

    # Fájl törlése
    file_path.unlink()
    return {"message": f"{filename} törölve."}


# IFC -> GLB konvertálás
@app.post("/convert/{filename}")
def convert_ifc(filename: str = FastAPIPath(..., description="A konvertálandó fájl neve, pl. column.ifc")):
    # Ellenőrizzük, hogy .ifc fájlt akarunk-e konvertálni
    if not filename.endswith(".ifc"):
        raise HTTPException(status_code=400, detail="Csak .ifc fájl konvertálható.")

    # Betöltjük az IFC fájl elérési útját
    ifc_path = UPLOAD_DIR / filename

    # Ellenőrizzük, hogy az IFC fájl létezik-e
    if not ifc_path.exists():
        raise HTTPException(status_code=404, detail="A megadott .ifc fájl nem található.")

    # Meghatározzuk a konvertált GLB fájl nevét és helyét
    glb_name = Path(filename).with_suffix(".glb").name
    glb_path = OUTPUT_DIR / glb_name

    # Ellenőrizzük, hogy nincs-e már ilyen nevű GLB fájl
    if glb_path.exists():
        raise HTTPException(status_code=409, detail=f"Ilyen nevű .glb fájl már létezik: {glb_name}")

    # Megpróbáljuk konvertálni az IFC fájlt GLB formátumba
    try:
        glb_path = convert_ifc_to_glb(ifc_path, OUTPUT_DIR, IFCCONVERT_PATH)
    except RuntimeError as e:
        raise HTTPException(status_code=500, detail=str(e))

    return {
        "message": "Sikeres konvertálás",
        "glb_file": glb_path.name
    }

# IFC -> JSON generálás
@app.post("/generatejson/{filename}")
def generate_metadata(filename: str = FastAPIPath(..., description="Az IFC fájl neve, pl. column.ifc")):
    # Ellenőrizzük, hogy .ifc fájlt adtak-e meg
    if not filename.endswith(".ifc"):
        raise HTTPException(status_code=400, detail="Csak .ifc fájl támogatott.")

    # Betöltjük az IFC fájl elérési útját
    ifc_path = UPLOAD_DIR / filename
    # Meghatározzuk a generált JSON fájl helyét
    json_path = OUTPUT_DIR / (Path(filename).stem + ".json")

    # Ellenőrizzük, hogy az IFC fájl létezik-e
    if not ifc_path.exists():
        raise HTTPException(status_code=404, detail="A megadott .ifc fájl nem található.")

    # Ellenőrizzük, hogy nincs-e már ilyen nevű JSON fájl
    if json_path.exists():
        raise HTTPException(status_code=409, detail=f"Ilyen nevű JSON már létezik: {json_path.name}")

    # Megpróbáljuk legenerálni a JSON fájlt
    try:
        generated_path = generate_metadata_json(ifc_path, OUTPUT_DIR)
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Hiba a generálás során: {str(e)}")

    return {
        "message": "Sikeres JSON generálás",
        "json_file": generated_path.name
    }

# Teljes feldolgozás (GLB + JSON)
@app.post("/process/{filename}")
def process_ifc_file(filename: str = FastAPIPath(..., description="Az IFC fájl neve, pl. column.ifc")):
    # Ellenőrizzük, hogy .ifc fájlt adtak-e meg
    if not filename.endswith(".ifc"):
        raise HTTPException(status_code=400, detail="Csak .ifc fájl támogatott.")

    # Betöltjük az IFC fájl elérési útját
    ifc_path = UPLOAD_DIR / filename
    if not ifc_path.exists():
        raise HTTPException(status_code=404, detail="A megadott .ifc fájl nem található.")

    # Meghatározzuk a GLB és JSON fájlok elérési útját
    glb_path = OUTPUT_DIR / (Path(filename).stem + ".glb")
    json_path = OUTPUT_DIR / (Path(filename).stem + ".json")

    results = {}

    # GLB generálás, ha még nem létezik
    if not glb_path.exists():
        try:
            glb_generated = convert_ifc_to_glb(ifc_path, OUTPUT_DIR, IFCCONVERT_PATH)
            results["glb"] = f"{glb_generated.name} létrehozva"
        except Exception as e:
            raise HTTPException(status_code=500, detail=f"GLB konvertálási hiba: {str(e)}")
    else:
        results["glb"] = f"{glb_path.name} már létezik"

    # JSON generálás, ha még nem létezik
    if not json_path.exists():
        try:
            json_generated = generate_metadata_json(ifc_path, OUTPUT_DIR)
            results["json"] = f"{json_generated.name} létrehozva"
        except Exception as e:
            raise HTTPException(status_code=500, detail=f"JSON generálási hiba: {str(e)}")
    else:
        results["json"] = f"{json_path.name} már létezik"

    return {
        "message": "Feldolgozás kész",
        "details": results
    }

# Fájl letöltése
@app.get("/download/{filename}")
def download_file(filename: str = FastAPIPath(..., description="A letöltendő fájl neve (.ifc, .glb, .json)")):
     # Ellenőrizzük a fájl kiterjesztését
    ext = Path(filename).suffix.lower()

    if ext not in [".ifc", ".glb", ".json", ".apk"]:
        raise HTTPException(status_code=400, detail="Csak .ifc, .glb, .apk és .json fájlok tölthetők le.")

    # Meghatározzuk a fájl helyét
    if ext == ".ifc":
        file_path = UPLOAD_DIR / filename
    else:
        file_path = OUTPUT_DIR / filename

    # Ellenőrizzük, hogy létezik-e a fájl
    if not file_path.exists():
        raise HTTPException(status_code=404, detail="A megadott fájl nem található.")

    return FileResponse(path=file_path, filename=filename, media_type="application/octet-stream")