from pathlib import Path
import subprocess

# IFC fájl átkonvertálása GLB formátumba.
def convert_ifc_to_glb(ifc_path: Path, glb_output_dir: Path, ifcconvert_path: Path):
    # Ellenőrizzük, hogy létezik-e az IFC fájl.
    if not ifc_path.exists():
        raise FileNotFoundError(f"IFC fájl nem található: {ifc_path}")

    # Ha az output mappa nem létezik, létrehozzuk.
    glb_output_dir.mkdir(parents=True, exist_ok=True)

    # Meghatározzuk az output fájl nevét.
    output_path = glb_output_dir / (ifc_path.stem + ".glb")

    # Lefuttatjuk az IfcConvert parancsot, hogy átalakítsuk az IFC fájlt GLB-vé.
    result = subprocess.run(
        [str(ifcconvert_path), "--use-element-guids", str(ifc_path), str(output_path)],
        capture_output=True, text=True
    )

    if result.returncode != 0:
        raise RuntimeError(f"Hiba a konvertálás során: {result.stderr}")

    return output_path
