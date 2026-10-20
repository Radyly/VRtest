#VRtest.github.io

# AUPP ETL Metaverse V1 — Anime VR Campus Tour

**Deliverable:** real, editable, procedural 3D tour prototype, not a photograph panorama, a trained reconstruction, or an already compiled Quest APK.

## What is included

- **WebXR/index.html** — 5 connected 3D rooms, a desktop walk-through, Quest browser immersive-VR mode, thumbstick movement, snap turning, jump-to-zone controls, controller laser-selected teleport pads, and 3 alternate color themes.
- **Unity_Starter/Assets/AUPP/** — Unity C# scripts and **AUPP > 1 - Generate Anime VR World V1** menu command to create the same class of stylized world inside a newly created Unity 6 project. Unity scene source is created by the menu command. Unity project, packages, and final APK are **not included** because Unity must be installed locally and Android/OpenXR configured before building.
- **References/** — original anime environment style references generated from the submitted AUPP video, plus a schematic map of the 5 tour zones.
- **WebXR/README_WEBXR.md** — quick Meta Quest 3 browser route; needs HTTPS hosting for headset.
- **Unity_Starter/XR_SETUP.md** — editable native Quest 3 Unity APK route.

## Quickest route: try a 3D world right now

1. Unzip the archive. Open a terminal inside `WebXR/`.
2. Run `python3 -m http.server 8000`.
3. Open `http://localhost:8000/` in Chrome/Edge on your PC. You need Internet access for A-Frame library delivery.
4. Use `WASD + mouse` to explore, keys `1–5` to teleport between zones, top-left buttons to select a destination, and the style dropdown to recolor the world.
5. To use Quest 3, publish the **WebXR folder contents** on an **HTTPS** website (e.g. GitHub Pages), then open the HTTPS URL in **Meta Quest Browser** and choose `Enter VR`.

> **Important:** An HTTP `192.168.x.x` LAN URL is generally not an acceptable WebXR secure origin in Quest Browser. `http://localhost` works for desktop preview on the local computer, but not as a remote headset URL. Hosting over HTTPS is the simple reliable path.

## Tour map

```text
                WELCOME HALL
                     |
                SHOWCASE
                CORRIDOR
                  /   \
         AI & XR LAB  ROBOTICS LAB
                  \   /
                CORRIDOR
                     |
                PROJECT SPACE
```

The world is a custom anime/futuristic design inspired by AUPP ETL footage, not an accurate building floor plan or an exact 3D scan. World measurements are approximate and should not be used for facilities/safety planning.

## Scope and limitations

- Built from simple editable 3D shapes and text; all objects have genuine 3D positioning, unlike a 360-video or image-on-plane tour.
- Controller navigation is deliberately minimal in V1; XR device mappings can vary and must be verified on Quest 3. VR laser selection and input need on-device testing.
- WebXR has only **approximate area-bound movement**, not a full physics collision/navmesh engine. Some scenery is decorative.
- Unity starter creates collision-bearing primitive walls/floors and a desktop camera, then needs an XR Origin and OpenXR setup for native VR.
- No multiplayer, login, voice AI guide, automatic image-to-3D conversion, or APK is included at this stage.
- If shared publicly, check AUPP name/logo and any internal-building-use permissions first.

## Editing ideas

- **WebXR:** near the beginning of `index.html`, edit `STOPS` to move the teleport destinations. Change `buildWorld()` to move walls, rooms, robots, screens or benches. Theme colors are in `theme(name)` and `P`.
- **Unity:** edit `Assets/AUPP/Editor/AUPPWorldBuilder.cs` for objects/locations/material colors, then regenerate. **Save any manual changes to the generated scene first**—regeneration replaces that scene.
- Replace placeholder robot arms, furniture, and signage with original optimized low-poly GLB/FBX assets when available.
- Keep target device performance in mind: avoid real-time shadows, excessive geometry and very many materials/draw calls.

## Next milestone

Validate the browser experience on the actual Quest 3. If that works, use Unity to make an offline-installable native APK and add polished anime assets, interaction panels, tour narration, a guide avatar, and more accurate movement/collisions.
