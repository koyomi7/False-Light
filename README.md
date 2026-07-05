# False Light
False Light is a short horror game created with Unity 3D.
Current Progress

![snapshot3-ezgif com-resize](https://github.com/user-attachments/assets/178412c2-3401-4245-a01d-8c5363f2711c)
![snapshot4-ezgif com-resize](https://github.com/user-attachments/assets/96d205ef-5f65-4185-8417-57b84facdafb)
![snapshot1-ezgif com-resize](https://github.com/user-attachments/assets/17a36b83-df8a-4692-bc63-9415961b50dc)
![snapshot2-ezgif com-resize](https://github.com/user-attachments/assets/6754a413-8aa0-4356-a42c-97223ea9df29)

## Premise
Schizophrenia.

## Features
Some scares.

## Assets

### Unity Asset Store
- [AllSky Free - 10 Sky / Skybox Set](https://assetstore.unity.com/packages/p/allsky-free-10-sky-skybox-set-146014)
  - Only import Cold Night
- [Blood splatter decal package](https://assetstore.unity.com/packages/2d/textures-materials/blood-splatter-decal-package-7518)
- [Desk Table](https://assetstore.unity.com/packages/3d/props/furniture/desk-table-96582)
- [House Interior - Free](https://assetstore.unity.com/packages/3d/props/interior/house-interior-free-258782)
- [Key and Lock](https://assetstore.unity.com/packages/3d/props/furniture/key-and-lock-193317)
- [Kitchen Set - Interior](https://assetstore.unity.com/packages/3d/props/furniture/kitchen-set-interior-263284)
- [Low Poly Chess Pieces](https://assetstore.unity.com/packages/3d/props/low-poly-chess-pieces-310624)
- [Modular House Pack 1](https://assetstore.unity.com/packages/3d/environments/urban/modular-house-pack-1-236466)
- [Modular Lowpoly Streets (Free)](https://assetstore.unity.com/packages/3d/environments/urban/modular-lowpoly-streets-free-192094)
- [Office Pack - Free](https://assetstore.unity.com/packages/3d/props/interior/office-pack-free-258600)

### 3rd-party Imports
- [crazy ex GF game mesh](https://web.archive.org/web/20251017021512/https://sketchfab.com/3d-models/crazy-ex-gf-game-mesh-fdecbc5840eb4d2da3a871fb49fed9bd)
  - Used for ghost model
  - Is currently unlisted
- [Grey Bricks](https://poly.pizza/m/nTCBOYDl5Z)
  - Used for lantern model
- [Pills](https://poly.pizza/m/8XEcolZDyt5)
  - Used for pill models

### Built-in
- TextMeshPro
- Universal RP

## Setup
1. **Clone the repository**
   ```bash
   git clone https://github.com/koyomi7/False-Light.git
   ```
   - Alternatively, you can fork this repository and clone your fork

2. **Download the external assets**
   - *Note: Large asset files are excluded from Git*
   - Download external assets from [some URL]
   - Extract the contents directly into the assets folder of this project so the file structure looks like this:
     ```text
     False-Light/
     ├── Assets/
         ├── ...
         ├── 4-Animations/
         ├── 5-Sounds/
         ├── 6-Import/
         └── ...
     ```

3. **Open the project**
   - Open Unity Hub, click **Add**, and select the root `False-Light` folder
   - Let Unity load the project (this may take a few minutes)

4. **Open the main scene**
   - Navigate to `False-Light/Assets/Scenes/GameplayScene.unity`
   - Open it in your Unity project

5. **Import Asset Store packages**
   - Go to https://assetstore.unity.com and sign in
   - Subscribe to the **Unity Asset Store** assets that are listed above
   - Navigate to `Window` -> `Package Manager`
   - Download and import the packages that this project requires

6. **Upgrade to URP (Universal Render Pipeline)**
   - Navigate to `Window` -> `Rendering` -> `Render Pipeline Converter`
   - Select **Built-in to URP** and **Material Upgrade** then click `Initialize And Convert`
   - *Note: You can safely ignore the warning about 9 materials failing to upgrade; this is expected behavior for this project*

7. **Import TextMeshPro**
   - Go to `Window` -> `TextMeshPro` -> `Import TMP Essentials Resources`
   - Click `Import` on the popup window

8. **Play the game**
   - Press the **Play** button

## Credits
Alex Akoopie

Henry Nan
