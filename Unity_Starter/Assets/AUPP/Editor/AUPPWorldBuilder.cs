#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using AUPP.Metaverse;

namespace AUPP.Metaverse.Editor
{
    /// <summary>Procedural five-zone stylized demo. Menu: AUPP > Generate Anime VR World V1.</summary>
    public static class AUPPWorldBuilder
    {
        const string RootPath = "Assets/AUPP";
        static readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();
        static Transform world;
        static readonly Color Wall = ColorFromHex("#F0EEE9");
        static readonly Color Floor = ColorFromHex("#C99E72");
        static readonly Color Trim = ColorFromHex("#192C40");
        static readonly Color Blue = ColorFromHex("#2777B4");
        static readonly Color Gold = ColorFromHex("#FFBE64");
        static readonly Color LightBlue = ColorFromHex("#64D9FF");
        static readonly Color Green = ColorFromHex("#407E56");

        static Color ColorFromHex(string s) { ColorUtility.TryParseHtmlString(s, out var c); return c; }
        static Material Mat(string name, Color color)
        {
            if (Materials.TryGetValue(name, out var found)) return found;
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            var m = new Material(shader) { name = name, color = color };
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
            var path = RootPath + "/Materials/" + name + ".mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) { Materials[name] = existing; return existing; }
            AssetDatabase.CreateAsset(m, path);
            Materials[name] = m;
            return m;
        }
        static Transform Group(string name, Vector3 pos, Transform parent = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent ? parent : world, false);
            go.transform.localPosition = pos;
            return go.transform;
        }
        static Transform Shape(PrimitiveType primitive, string name, Vector3 position, Vector3 scale, Material material, Transform parent = null)
        {
            var g = GameObject.CreatePrimitive(primitive); g.name = name;
            g.transform.SetParent(parent ? parent : world, false);
            g.transform.localPosition = position;
            g.transform.localScale = scale;
            g.GetComponent<Renderer>().sharedMaterial = material;
            return g.transform;
        }
        static Transform Cube(string name, float x, float y, float z, float width, float height, float depth, Material mat, Transform parent = null)
            => Shape(PrimitiveType.Cube, name, new Vector3(x, y, z), new Vector3(width, height, depth), mat, parent);
        static void FloorAt(string name, float x, float z, float w, float d)
        {
            Cube(name + " Floor", x, -.17f, z, w, .3f, d, Mat("WoodFloor", Floor));
            Cube(name + " Dark Foundation", x, -.33f, z, w, .07f, d, Mat("TrimDark", Trim));
        }
        static void Roof(string name, float x, float z, float w, float d)
        {
            Cube(name + " Ceiling", x, 4.05f, z, w, .08f, d, Mat("WhiteWall", Wall));
            for (float off = -w/2 + 1; off < w/2; off += 2.5f)
                Cube("Warm Ceiling Strip", x + off, 3.99f, z, .065f, .045f, d - .5f, Mat("WarmLED", Gold));
        }
        static void WallX(float z, float x1, float x2)
        {
            Cube("Wall X", (x1+x2)/2, 2, z, x2-x1, 4, .19f, Mat("WhiteWall", Wall));
            Cube("Baseboard X",(x1+x2)/2, .13f, z, x2-x1,.12f,.2f,Mat("TrimDark",Trim));
        }
        static void WallZ(float x, float z1, float z2)
        {
            Cube("Wall Z", x, 2,(z1+z2)/2,.19f,4,z2-z1,Mat("WhiteWall",Wall));
            Cube("Baseboard Z",x,.13f,(z1+z2)/2,.2f,.12f,z2-z1,Mat("TrimDark",Trim));
        }
        static void Label(string text, Vector3 position, float charSize, Color color, Vector3 rotation, Transform parent = null)
        {
            var go = new GameObject("Sign — " + text.Replace("\n"," "));
            go.transform.SetParent(parent ? parent : world, false);
            go.transform.localPosition = position;go.transform.localEulerAngles = rotation;
            var mesh = go.AddComponent<TextMesh>(); mesh.text = text;
            mesh.fontSize = 80;mesh.characterSize = charSize;mesh.color = color;
            mesh.alignment = TextAlignment.Center;mesh.anchor = TextAnchor.MiddleCenter;
        }
        static void Plant(float x, float z)
        {
            var g=Group("Anime Plant",new Vector3(x,0,z));
            Shape(PrimitiveType.Cylinder,"Planter",new Vector3(0,.33f,0),new Vector3(.55f,.65f,.55f),Mat("TrimDark",Trim),g);
            for(int i=0;i<5;i++)
            {
                var a=i*2*Mathf.PI/5;
                Shape(PrimitiveType.Sphere,"Plant foliage",new Vector3(Mathf.Cos(a)*.3f,1.08f,Mathf.Sin(a)*.3f),Vector3.one*.60f,Mat("Foliage",Green),g);
            }
        }
        static void Bench(float x,float z)
        {
            var g=Group("Exhibit Workbench",new Vector3(x,0,z));
            Cube("White tabletop",0,.67f,0,2.8f,.14f,1.1f,Mat("Worktop",ColorFromHex("#F0F5FA")),g);
            foreach(var leg in new float[]{-1.2f,1.2f}) Cube("Bench leg",leg,.3f,0,.13f,.6f,.8f,Mat("TrimDark",Trim),g);
            Cube("Blue Accent",0,.59f,0,2.5f,.04f,.93f,Mat("AUPPBlue",Blue),g);
        }
        static void Robot(float x,float z,float scale=1f)
        {
            var g=Group("Animated Robotic Arm (stylized)",new Vector3(x,0,z));g.localScale=Vector3.one*scale;
            Cube("Robot base",0,.9f,0,.7f,.18f,.7f,Mat("TrimDark",Trim),g);
            Shape(PrimitiveType.Cylinder,"Robot pedestal",new Vector3(0,1.25f,0),new Vector3(.32f,.6f,.32f),Mat("Worktop",Color.white),g);
            var shoulder=Shape(PrimitiveType.Cylinder,"Robot upper arm",new Vector3(-.25f,1.68f,0),new Vector3(.18f,.92f,.18f),Mat("Worktop",Color.white),g);
            shoulder.localEulerAngles=new Vector3(0,0,40);
            Shape(PrimitiveType.Sphere,"Robot joint",new Vector3(-.5f,2.05f,0),Vector3.one*.25f,Mat("AUPPBlue",Blue),g);
            var forearm=Shape(PrimitiveType.Cylinder,"Robot forearm",new Vector3(-.75f,2.25f,0),new Vector3(.14f,.48f,.14f),Mat("Worktop",Color.white),g);
            forearm.localEulerAngles=new Vector3(0,0,-40);
            Shape(PrimitiveType.Sphere,"Robot gripper",new Vector3(-.87f,2.52f,0),Vector3.one*.18f,Mat("Cyan",LightBlue),g);
            g.gameObject.AddComponent<SpinExhibit>().degreesPerSecond=6f;
        }
        static void Screen(string label, float x,float z)
        {
            var g=Group("Holographic exhibit " + label,new Vector3(x,0,z));
            Cube("Display frame",0,1.72f,0,2,1.15f,.13f,Mat("TrimDark",Trim),g);
            Cube("Blue display",0,1.72f,-.082f,1.8f,.99f,.02f,Mat("AUPPBlue",Blue),g);
            Label(label,new Vector3(0,1.72f,-.105f),.035f,Color.white,Vector3.zero,g);
            Cube("Monitor stand",0,.9f,0,.12f,.55f,.12f,Mat("TrimDark",Trim),g);
        }
        static Transform Stop(string name,float x,float z, float angle)
        {
            var stop=Group("DESTINATION — " + name,new Vector3(x,0,z));
            stop.eulerAngles=new Vector3(0,angle,0);
            Shape(PrimitiveType.Cylinder,"Teleport pad",new Vector3(0,.035f,0),new Vector3(1.15f,.07f,1.15f),Mat("AUPPBlue",Blue),stop);
            return stop;
        }
        static void Decor()
        {
            // Welcome lobby with ETL wall, benches and plants.
            FloorAt("Welcome Hall",0,0,12,10);Roof("Welcome Hall",0,0,12,10);
            WallX(5,-6,6); WallZ(-6,-5,5);WallZ(6,-5,5);
            WallX(-5,-6,-2.3f);WallX(-5,2.3f,6);
            Cube("Doorway Header",0,3.6f,-5,4.6f,.8f,.25f,Mat("TrimDark",Trim));
            Label("EMERGING TECHNOLOGY LAB",new Vector3(0,3.1f,4.8f),.06f,Trim,new Vector3(0,180,0));
            for(int i=0;i<3;i++) Cube("Award Display Shelves",0,1.2f+i*.58f,4.55f,5.3f,.09f,.35f,Mat("Worktop",Color.white));
            Bench(-3,0);Bench(3,0);Plant(-5,3);Plant(5,3);
            // Main corridor: doors at -12 to each lab.
            FloorAt("Corridor",0,-15,8,20);Roof("Corridor",0,-15,8,20);
            WallX(-25,-4,-2.4f);WallX(-25,2.4f,4);
            WallZ(-4,-25,-14.1f);WallZ(-4,-9.9f,-5);
            WallZ(4,-25,-14.1f);WallZ(4,-9.9f,-5);
            Cube("Left Door Lintel",-4,3.6f,-12,.3f,.8f,4.2f,Mat("TrimDark",Trim));
            Cube("Right Door Lintel",4,3.6f,-12,.3f,.8f,4.2f,Mat("TrimDark",Trim));
            Label("AI / XR",new Vector3(-3.87f,2.65f,-12),.064f,Blue,new Vector3(0,90,0));
            Label("ROBOTICS",new Vector3(3.87f,2.65f,-12),.064f,Blue,new Vector3(0,-90,0));
            // Robotics room.
            FloorAt("Robotics",10,-12,12,12);Roof("Robotics",10,-12,12,12);
            WallX(-18,4,16);WallX(-6,4,16);WallZ(16,-18,-6);
            WallZ(4,-18,-14.1f);WallZ(4,-9.9f,-6);
            Label("ROBOTICS + AUTOMATION",new Vector3(10,2.95f,-17.78f),.06f,Trim,Vector3.zero);
            Bench(12,-12);Robot(12,-12,.78f);Robot(14,-15,.55f);Screen("ROBOT CONTROL",10,-8.5f);Plant(15,-7);
            // AI/XR room.
            FloorAt("AI XR",-10,-12,12,12);Roof("AI XR",-10,-12,12,12);
            WallX(-18,-16,-4);WallX(-6,-16,-4);WallZ(-16,-18,-6);
            WallZ(-4,-18,-14.1f);WallZ(-4,-9.9f,-6);
            Label("AI + EXTENDED REALITY",new Vector3(-10,2.95f,-17.78f),.055f,Trim,Vector3.zero);
            Bench(-11,-13);Screen("AI SYSTEMS",-13,-8.5f);Screen("VR WORLD",-8,-8.5f);Plant(-15,-7);
            // Project room.
            FloorAt("Projects",0,-31,18,12);Roof("Projects",0,-31,18,12);
            WallX(-37,-9,9);WallZ(-9,-37,-25);WallZ(9,-37,-25);
            WallX(-25,-9,-2.4f);WallX(-25,2.4f,9);
            Cube("Project Door Header",0,3.6f,-25,4.8f,.8f,.25f,Mat("TrimDark",Trim));
            Label("3D PRINTING + PROJECT SPACE",new Vector3(0,2.95f,-36.78f),.05f,Trim,Vector3.zero);
            foreach(float x in new float[]{-4f,0f,4f})
            {
                Bench(x,-33);Cube("Mock printer enclosure",x,1.12f,-33,.9f,.6f,.7f,Mat("TrimDark",Trim));
                Cube("Printer screen",x,1.31f,-33.37f,.55f,.23f,.02f,Mat("AUPPBlue",Blue));
            }
            Plant(-8,-26);Plant(8,-26);
        }

        [MenuItem("AUPP/1 - Generate Anime VR World V1")]
        public static void Generate()
        {
            if (!EditorUtility.DisplayDialog("AUPP ETL", "Create a NEW generated scene with five connected zones? The current scene will be replaced. Save your work first.","Generate","Cancel"))return;
            if (!AssetDatabase.IsValidFolder(RootPath + "/Materials"))AssetDatabase.CreateFolder(RootPath,"Materials");
            if (!AssetDatabase.IsValidFolder(RootPath + "/Scenes"))AssetDatabase.CreateFolder(RootPath,"Scenes");
            Materials.Clear();
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            world=Group("AUPP World Geometry",Vector3.zero,new GameObject("AUPP V1 ROOT").transform);
            Decor();
            var managerGO=new GameObject("TOUR MANAGER");var manager=managerGO.AddComponent<TourManager>();
            manager.stops=new []{
                Stop("Welcome Hall",0,2,180),Stop("Showcase Corridor",0,-13,0),
                Stop("Robotics Lab",10,-12,90),Stop("AI XR Lab",-10,-12,270),Stop("Project Space",0,-31,0)
            };
            var camGO=new GameObject("DESKTOP CAMERA — disable for Quest XR");
            camGO.tag="MainCamera";camGO.transform.position=new Vector3(0,1.65f,2);
            camGO.transform.eulerAngles=new Vector3(0,180,0);
            var camera=camGO.AddComponent<Camera>();camera.nearClipPlane=.05f;camera.farClipPlane=100;
            camGO.AddComponent<AudioListener>();
            var cc=camGO.AddComponent<CharacterController>();cc.height=1.65f;cc.center=new Vector3(0,-.65f,0);cc.radius=.18f;
            var controller=camGO.AddComponent<DesktopTourController>();controller.tour=manager;
            var lightGO=new GameObject("Directional Sunlight");var light=lightGO.AddComponent<Light>();light.type=LightType.Directional;light.intensity=1.2f;light.shadows=LightShadows.None;
            lightGO.transform.rotation=Quaternion.Euler(46,-35,0);
            RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight=ColorFromHex("#C7D9ED");
            RenderSettings.fog=false;
            EditorSceneManager.SaveScene(scene,RootPath+"/Scenes/AUPP_ETL_Metaverse_V1.unity");
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("AUPP ETL", "Generated Assets/AUPP/Scenes/AUPP_ETL_Metaverse_V1.unity. Press Play for desktop tour. Read XR_SETUP.md before building Quest APK.","OK");
        }
    }
}
#endif
