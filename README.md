# DAOC Navigator

**DAOC Navigator** is a cross-platform Dark Age of Camelot world navigator and model viewer,
written in C# (.NET 8) and built on **OpenTK 4.x**.

The repository contains three projects:

| Project | Description |
|---|---|
| `DAOC_Navigator_Core` | Cross-platform library: NIF parser, MPAK reader, OpenGL rendering layer, game data |
| `DAOC_Navigator_Avalonia` | Desktop GUI frontend using [Avalonia UI](https://avaloniaui.net/) |
| `DAOC_Navigator_ImGUI` | Lightweight frontend using [ImGui.NET](https://github.com/ImGuiNET/ImGui.NET) |

---

## Screenshots

Avalonia version :
<img width="1021" height="796" alt="avalonia_navigator" src="https://github.com/user-attachments/assets/76ffd18c-82cc-4391-8872-5d9abcd0d479" />

ImGUI version : 
<img width="1018" height="794" alt="imgui_navigator" src="https://github.com/user-attachments/assets/d9df5442-88c0-4942-9636-2965328d7108" />

---

## Features

- **NIF parser** – reads DAoC NIF files (NetImmerse / Gamebryo format, versions up to
  19.x) and converts the scene graph into flat vertex/index geometry.
- **MPAK / PAK reader** – decompresses `.mpk` and `.npk` archives used by DAoC to
  store textures and game data files.
- **Assimp import** – loads additional 3-D formats (OBJ, FBX, …) via AssimpNet for
  development and tooling purposes.
- **OpenGL rendering layer** – thin wrappers around OpenTK 4.x (VAO/VBO/EBO, shaders,
  texture loading from files or MPAK archives).
- **Game data** – parses `gamedata.mpk` CSV files (`monsters.csv`, `monnifs.csv`,
  `skins.csv`, …) to build a complete monster → NIF → skin mapping.
- **Two frontends** – a full desktop UI (Avalonia) and a lightweight immediate-mode UI
  (ImGui), both targeting Windows, Linux and macOS.

---

## Requirements

| Dependency        | Version | Notes                             |
|-------------------|---------|-----------------------------------|
| .NET SDK          | 8.0+    | `dotnet build`                    |
| OpenGL driver     | 3.3+    | Required for rendering            |
| DAoC installation | any     | Required at runtime for game data |

NuGet packages are declared in each `.csproj` and restored automatically by the .NET toolchain.

---

## Building

```bash
git clone <repo-url>
cd DAOC_Navigator
```

### Core library only
```bash
cd DAOC_Navigator_Core
dotnet build
```

### Avalonia frontend
```bash
cd DAOC_Navigator_Avalonia
dotnet build
dotnet run
```

### ImGUI frontend
```bash
cd DAOC_Navigator_ImGUI
dotnet build
dotnet run
```

---

## Configuration

Both frontends read a single setting from their `app.config`:

```xml
<appSettings>
  <add key="DAOCNavigator" value="/path/to/your/daoc/installation" />
</appSettings>
```

This path must point to the root of your local DAoC installation (the folder
containing `gamedata.mpk` and the `figures/` directory).  
The setting can also be changed at runtime from the Options panel in both UIs.

---

## Architecture

```
DAOC_Navigator_Core/
  NIFLib/
    NiFile.cs          – top-level parser, object graph, reference fixup
    NiHeader.cs        – file header (version, block type table)
    NiFooter.cs        – root node references
    compounds/         – shared data structures (NiRef<T>, Triangle, …)
    enums/             – NIF enumeration types
    objs/              – NiObject subclasses (NiNode, NiTriShape, …)
  Parsers/
    NIFParser.cs       – walks the NIF scene graph → List<Shape>
    AssimpParser.cs    – wraps AssimpNet → List<Shape>
    MaterialUtil.cs    – deduplication of ShapeMaterial entries
  WorldObjects/
    Shape.cs           – geometry + material data
    ShapeMaterial.cs   – texture filename + DAoC node-name category
    Model.cs           – high-level file → Shape list dispatcher
    Polygon.cs         – triangle index triple
    Camera.cs          – FPS-style OpenGL camera
  GL_Rendering/
    GL_Mesh.cs         – VAO/VBO/EBO wrapper
    GL_Model.cs        – converts Model → list of GL_Mesh
    GL_Shader.cs       – GLSL program wrapper
    GL_Texture.cs      – texture loading (file, MPK archive)
  Game/
    GameData.cs        – parses gamedata.mpk and builds Monster/Skin tables
    Monster.cs / SkinSet.cs / Skin.cs / SkinID.cs / FigurePart.cs
  PAKFile.cs           – MPAK archive reader (zlib deflate)
  FileUtil.cs          – cross-platform file-lookup utilities
  NavigatorSettings.cs – app.config wrapper

DAOC_Navigator_Avalonia/
  Views/               – Avalonia AXAML windows and controls
  ViewModels/          – MVVM view-models (ReactiveUI / CommunityToolkit)
  OpenTK/              – Avalonia ↔ OpenTK bridge (context, keyboard input)
  Shaders/             – GLSL vertex and fragment shaders

DAOC_Navigator_ImGUI/
  MainWindow.cs        – OpenTK GameWindow + ImGui render loop
  ImGuiController.cs   – ImGui.NET ↔ OpenTK integration layer
  Shaders/             – GLSL vertex and fragment shaders
```

---

## License

Copyright © DAOC Navigator contributors.  
Portions adapted from the **Dawn of Light** project (GPL v2 or later).

This program is free software: you can redistribute it and/or modify it under
the terms of the **GNU General Public License version 3** (or any later version)
as published by the Free Software Foundation.

See the [`LICENSE`](DAOC_Navigator_Core/DAOC_Navigator_Core/LICENSE) file for the full text.

*Dark Age of Camelot is a registered trademark of Electronic Arts / Broadsword Online Games.
This project is not affiliated with or endorsed by EA or Broadsword.*
