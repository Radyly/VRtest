# Unity 6 + Meta Quest 3 Native App — Setup

This folder is an **Assets overlay**, not a full Unity project or a prebuilt APK. The Editor builder will create a new scene on your machine using procedural walls, floors, exhibits and basic collider shapes.

## 1. Create a Unity 6 project

1. Install Unity Hub and a supported **Unity 6** editor with **Android Build Support**, **SDK & NDK Tools**, and **OpenJDK** modules.
2. In Unity Hub create a new **Universal 3D (URP)** project called `AUPP_ETL_Metaverse_V1`.
3. Copy the `Assets/AUPP` directory from this starter into your new project's own `Assets/` directory.
4. Open Unity and allow scripts to compile.
5. **Edit > Project Settings > Player > Active Input Handling:** choose **Both**. The desktop preview script uses legacy `Input` (`Input.GetAxis`, keys/mouse).
6. Click top menu **AUPP > 1 - Generate Anime VR World V1**. The generator writes `Assets/AUPP/Scenes/AUPP_ETL_Metaverse_V1.unity` and editable material assets.
7. Press **Play**. Click to look, use WASD to walk, keys 1–5 to move between rooms; Esc to unlock mouse. This confirms the procedural 3D scene works before attempting XR.

## 2. Configure Quest build and OpenXR

1. In Unity **Package Manager**, install XR Plug-in Management, OpenXR Plugin, XR Interaction Toolkit and its required **Input System** dependencies (follow the compatibility warnings for your Unity editor version). Import **XR Interaction Toolkit > Samples > Starter Assets**.
2. In **Edit > Project Settings > XR Plug-in Management**, enable **OpenXR** on the Quest/Android build target.
3. Under **XR Plug-in Management > OpenXR > Android**, add **Oculus Touch Controller Profile** and enable **Meta Quest Support** feature. Run XR Project Validation and fix warnings/errors.
4. In **File > Build Profiles**, select **Meta Quest** (if available) or Android, then Switch Platform. If available, use Unity's Meta Quest build profile. Use the Android build modules installed in step 1.

The OpenXR route is recommended in Meta's October 2026 manual setup guidance. Avoid the deprecated Oculus XR provider for a new project.

## 3. Replace desktop camera with an XR Origin

1. In the generated scene, **disable** the GameObject named `DESKTOP CAMERA — disable for Quest XR`.
2. Use **GameObject > XR > XR Origin (VR)**, or drag in the XR Origin prefab from **XR Interaction Toolkit Starter Assets**. Ensure an action-based **Tracked Pose Driver** is active on the VR Main Camera, with center-eye pose input actions, and that one AudioListener remains enabled.
3. **Choose one locomotion system**:
   - **Recommended:** keep the Starter Assets' configured continuous movement, snap turn and teleportation, and use the generated world as the environment. Disable or omit `QuestTourMovement` to prevent duplicated movement.
   - **Minimal custom:** disable any XRI movement/snap/teleport providers in the XR Origin, add `QuestTourMovement.cs` to its root, set `head` to its Main Camera, and set `tour` to the generated `TOUR MANAGER` object. Then left stick walks, right stick snap turns, right A jumps forward and right B jumps back between destinations. This basic script reads `UnityEngine.XR.InputDevices`; device mappings still need a Quest test.
4. Move XR Origin to the Welcome Hall `(0, 0, 2)` and verify the headset camera is at proper floor-relative height. Do not leave both the desktop Main Camera and XR Main Camera active.
5. Add an XR-compatible CharacterController (if needed) to the rig, aligned to the player's physical head position. The primitive-generated environment already has collider geometry. Proper XR capsule alignment is an important follow-up task.

## 4. Quest APK build

1. Enable Developer Mode for your headset and authorize USB debugging if prompted.
2. Connect Quest 3 by USB-C. Confirm `adb devices` detects it (Android platform tools).
3. Set a unique Android package identifier such as `edu.aupp.etl.metaverse` in **Player Settings**.
4. In **Build Profiles**, add `Assets/AUPP/Scenes/AUPP_ETL_Metaverse_V1.unity`, then **Build And Run** on the connected Quest 3.
5. Test camera height, controls, scene orientation, collisions and framerate directly on the headset.

No APK was built here; only Unity on a computer with Android/OpenXR toolchains can compile and validate the standalone Quest application.

## 5. Edit the world

Edit the generator script at `Assets/AUPP/Editor/AUPPWorldBuilder.cs`, including:

- `Decor()` — room/corridor geometry, exhibits, signs, and lighting.
- `Robot()`, `Plant()`, `Bench()` — procedural props.
- `WallX()` and `WallZ()` — rectangular room boundaries.
- `Stop()` and the `manager.stops` array — tour teleport destinations.

**Note:** Running the generator creates a new scene and overwrites the generated scene path. Save any custom hand edits separately before rerunning it.

## Caveats

- The C# assets cannot be compiled or device-tested in this chat environment because Unity and Quest hardware are not available.
- This is a stylized placeholder layout with real editable 3D shapes and basic collisions, not a photogrammetric reproduction of the filmed building.
- To make it feel like the supplied anime concept art, replace placeholder geometry with optimized custom FBX/GLB assets, materials, HDR/color grading, and lighting, then profile on Quest.
- Shared/multiplayer virtual world functionality is not provided in V1; it can be added using Unity networking after the native single-user tour works.

## Official setup references

- Meta manual Unity setup (updated Oct. 1, 2026): https://developers.meta.com/vr/documentation/unity/unity-tutorial-hello-world-manual-steps/
- Unity XR Interaction Toolkit setup: https://docs.unity.cn/Packages/com.unity.xr.interaction.toolkit%403.1/manual/general-setup.html
- Unity Quest XR workflow: https://docs.unity.com/en-us/engine/6000.0/manual/xr/configuring-project-for/meta-quest-develop
