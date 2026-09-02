# urender

**urender** est un convertisseur d'images Windows simple, rapide et moderne.
Il permet de convertir plusieurs fichiers a la fois et d'enregistrer automatiquement
les resultats sur le Bureau.

## Fonctionnalites

- Interface Windows moderne en C# et XAML (WPF).
- Import de plusieurs fichiers.
- Apercu des images importees.
- Choix du format de sortie pour chaque fichier.
- Conversion reelle avec [Magick.NET](https://github.com/dlemstra/Magick.NET).
- Orientation automatique des images.
- Nommage automatique : `nom_original_converted.format`.
- Enregistrement automatique dans le dossier Bureau.
- Application autonome Windows x64 : .NET n'est pas necessaire sur le PC cible.
- Logo urender integre aux ressources de l'application.

## Formats

urender prend en charge de nombreux formats image fournis par ImageMagick,
notamment :

`PNG`, `JPG`, `JPEG`, `WEBP`, `GIF`, `BMP`, `TIFF`, `SVG`, `ICO`, `PSD`,
`AVIF`, `HEIC`, `HEIF`, `DDS`, `EXR`, `TGA`, `JP2` et bien d'autres.

Les extensions `MP4`, `WEBM`, `MOV` et `AVI` apparaissent dans le selecteur,
mais la conversion video n'est pas encore implementee. Une prochaine version
pourra utiliser FFmpeg pour ajouter cette fonctionnalite.

## Configuration requise

### Pour utiliser l'application

- Windows 10 ou Windows 11 64 bits.
- Aucun runtime .NET requis pour l'executable publie.

### Pour compiler le projet

- Windows.
- .NET SDK 10.0 ou une version plus recente compatible.
- Une connexion Internet lors de la premiere restauration des dependances.

## Lancer depuis les sources

Dans un terminal ouvert a la racine du projet :

```powershell
dotnet restore
dotnet run --project .\urender.csproj
```

## Compiler une version Windows autonome

```powershell
dotnet publish .\urender.csproj `
  --configuration Release `
  --runtime win-x64 `
  --self-contained true `
  --output .\publish\urender-win-x64
```

L'executable sera disponible ici :

```text
publish\urender-win-x64\urender.exe
```

Il suffit de lancer `urender.exe` par double-clic.

## Utilisation

1. Cliquez sur le bouton d'ajout.
2. Selectionnez une ou plusieurs images.
3. Choisissez le format de sortie pour chaque fichier.
4. Cliquez sur **Convertir**.
5. Retrouvez les fichiers convertis dans le dossier **Bureau**.

Les fichiers crees suivent ce format :

```text
mon-image_converted.png
```

## Publier sur GitHub

1. Creez un nouveau depot GitHub nomme `urender`.
2. Ajoutez les fichiers du projet, notamment `README.md`.
3. N'ajoutez pas les dossiers de compilation `bin` et `obj`.
4. Publiez une version compilee dans l'onglet **Releases**.
5. Ajoutez une capture d'ecran et decrivez les changements de la version.

Exemple de commandes :

```powershell
git init
git add .
git commit -m "Initial release of urender"
git branch -M main
git remote add origin https://github.com/VOTRE-COMPTE/urender.git
git push -u origin main
```

## Licence

Le projet n'a pas encore de licence definie. Pour autoriser clairement les
autres utilisateurs a reutiliser et modifier le code, ajoutez un fichier
`LICENSE` avec une licence open source, par exemple MIT.

## Contribuer

Les contributions sont bienvenues :

1. Ouvrez une issue pour signaler un bug ou proposer une idee.
2. Creez une branche pour votre modification.
3. Testez l'application avant de proposer vos changements.
4. Ouvrez une pull request avec une description claire.

## Remerciements

- [Magick.NET](https://github.com/dlemstra/Magick.NET) pour le moteur de
  conversion d'images.
- [ImageMagick](https://imagemagick.org/) pour la prise en charge de nombreux
  formats.
