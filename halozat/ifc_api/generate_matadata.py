import ifcopenshell
import ifcopenshell.util.element as Element
import json
from pathlib import Path

# FC fájlból az adott típusú (class_type) objektumok lekérdezése és adatok összegyújtése egy listába.
def get_objects_data_by_class(file, class_type):
    objects_data = []
    objects = file.by_type(class_type) # Lekérdezzük az összes objektumot a megadott típusban.

    #Végigmegyünk az összes lekért objektumon
    for object in objects:
        objects_data.append({
            "ExpressID": object.id(),
            "GlobalId": object.GlobalId,
            "Class": object.is_a(),
            "PredefinedType": Element.get_predefined_type(object),
            "Name": object.Name,
            "Level": Element.get_container(object).Name if Element.get_container(object) else "",
            "ObjectType": Element.get_type(object).Name if Element.get_type(object) else "",
            "QuantitySets": Element.get_psets(object, qtos_only=True), # Mennyiségi adatok lekérdezése
            "PropertySets": Element.get_psets(object, psets_only=True), # Tulajdonságok lekérdezése
            
        })

    return objects_data

#betölti az IFC fájlt, legenerálja belőle a metaadatokat JSON-ba.
def generate_metadata_json(ifc_path: Path, output_dir: Path) -> Path:
    #IFC fájl ellenőrzése
    if not ifc_path.exists():
        raise FileNotFoundError(f"IFC fájl nem található: {ifc_path}")
    
    file = ifcopenshell.open(str(ifc_path))
    data = get_objects_data_by_class(file, "IfcBuildingElement")

    output_dir.mkdir(parents=True, exist_ok=True) # Ha az output mappa nem létezik, létrehozzuk
    json_path = output_dir / (ifc_path.stem + ".json") # Megadjuk, hogy a JSON fájl neve ugyanaz legyen, mint az IFC-nek.

    # Megnyitjuk a JSON fájlt írásra és beleírjuk a kiexportált adatokat.
    with open(json_path, "w", encoding="utf-8") as f:
        json.dump(data, f, indent=2)

    return json_path
