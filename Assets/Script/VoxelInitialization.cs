using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VoxelPlay;
using UnityEngine.UI;
using TMPro;
using LevelDB_DLLImport_Test;
using WorldViewer;
using System.Linq;

namespace WorldViewer
{
    public class VoxelInitialization  : MonoBehaviour
    {

        static VoxelInitialization _instance;
        public static VoxelInitialization Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<VoxelInitialization>();
                    if (_instance == null)
                    {
                        VoxelInitialization[] vv = Resources.FindObjectsOfTypeAll<VoxelInitialization>();
                        for (int k = 0; k < vv.Length; k++)
                        {
                            if (vv[k].hideFlags != HideFlags.HideInHierarchy)
                            {
                                _instance = vv[k];
                                break;
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        //Enviroment instance
        public VoxelPlayEnvironment envInstance;

        //World data
        //WorldData worldData;

        //Controller instance
        VoxelPlayFirstPersonController controllerInstance;

        //Inventory Gameobject
        public GameObject inventoryObject;

        //player camera
        public GameObject playerCam;

        //list of voxel definitions
        //public static BlockType[] voxelList = new BlockType[500];

        //reverse mapping for voxel definitions using VD name
        public static Dictionary<string, byte[]> reverseVoxelMap = new Dictionary<string, byte[]>();

        //Hashset of chunks that were modified while placing models
        public static HashSet<Vector3> chunksModifiedSaveSet = new HashSet<Vector3>();

        //system info text
        public TextMeshProUGUI warningText;

        public Transform spawn;

        //Active scene index
        public string activeScene;

        // Start is called before the first frame update
        void Awake()
        {
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
            Debug.unityLogger.logEnabled = false;
            Zion.updateSceneNo(3);
#endif

            //Application.targetFrameRate = 60;
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            activeScene = SceneManager.GetActiveScene().name;
            envInstance = VoxelPlayEnvironment.instance;
            //  worldData = SaveState.instance.worldData;
            //if (EditorUI.Instance != null &)
            //EditorUI.Instance.InitSettings(envInstance);
            if (VoxelPlayFirstPersonController.instance != null)
                controllerInstance = VoxelPlayFirstPersonController.instance;

            if (World.quality == 2)
            {
                QualitySettings.antiAliasing = 2;
            }
            envInstance.OnInitialized += () =>
            {

#if UNITY_EDITOR
                DBTest.dbPath = World.GetPath();
                DBTest.Initialize();
#endif
#if !UNITY_EDITOR
                Zion.openDB(World.GetPath());
                Zion.openModelDB();
#endif
                InitModelKeysRoomCreation();

               // StartCoroutine("WaitForLoad");


#if !UNITY_EDITOR

                            Zion.WorldLoaded();
#endif






                //                switch (activeScene)
                //                {

                //                    //Include Skin Viewer World ID initialization here 
                //                    case "Skin Viewer":

                //                        SetWorldId("9");
                //#if !UNITY_EDITOR
                //                        //Zion.updateSceneNo(3);
                //#endif

                //                        //Zion.GetZionPreferences().CallStatic("updateSceneNo", Zion.GetCurrentActivity(), 3);

                //                        DBTest.dbPath = World.GetPath();
                //                        DBTest.Initialize();
                //#if !UNITY_EDITOR
                //                            Zion.WorldLoaded();
                //#endif
                //                        //Zion.GetZionPreferences().CallStatic("WorldLoaded", Zion.GetCurrentActivity());

                //                        break;

                //                    case "ModelPreview":

                //                        SetWorldId("1");
                //                        DBTest.dbPath = World.GetPath();
                //                        DBTest.Initialize();

                //                        break;

                //                    case "RoomCreation":

                //                        DBTest.dbPath = World.GetPath();
                //                        DBTest.Initialize();

                //                        break;
                //                    //Include Main Viewership World ID initialization here
                //                    default:

                //                        StartCoroutine("WaitForLoad");

                //#if !UNITY_EDITOR 
                //                        //Zion.updateSceneNo(5);
                //#endif

                //                        DBTest.dbPath = World.GetPath();
                //                        DBTest.Initialize();

                //#if !UNITY_EDITOR

                //                            Zion.WorldLoaded();
                //#endif


                //                        //Set player position 
                //                        if (controllerInstance != null)
                //                            controllerInstance.transform.position = new Vector3(-SaveState.instance.spawnX, SaveState.instance.spawnY + 1.2f, SaveState.instance.spawnZ);

                //                        //Set world name
                //                        if (SaveState.instance.worldName.Length > 80)
                //                        {
                //                            string nameTruncated = SaveState.instance.worldName.Substring(0, 80);
                //                            nameTruncated += "...";
                //                            SaveState.instance.worldName = nameTruncated;
                //                        }
                //                        envInstance.welcomeMessage = "<color=green>Welcome to <color=white>" + SaveState.instance.worldName + "</color>!</color>";

                //                        break;

                // }

            };

        }

        //private void Start()
        //{

        //}

        // public void OnClickBack()
        // {
        //     Screen.orientation = ScreenOrientation.Portrait;

        //     Zion.onWorldBackPress();

        // }

//         public IEnumerator WaitForLoad()
//         {
//             yield return new WaitForSeconds(3f);
//             SaveState.instance.SpawnPlayer();
// #if !UNITY_EDITOR

//             //Zion.setQuality(World.quality);
// #endif
//             //Zion.GetZionPreferences().CallStatic("setQuality", Zion.GetCurrentActivity(), World.quality);
//             //Zion.GetZionPreferences().CallStatic("WorldLoaded", Zion.GetCurrentActivity());
//         }

        // public void SetWorldId(string id)
        // {
        //     SaveState.instance.worldData.worldID = id;

        // }

//         public void OnPressBackWorld()
//         {
//             //envInstance.DisposeAll();
//             //envInstance = null;

//             Screen.orientation = ScreenOrientation.Portrait;
//             Util.isEditorEnabled = false;
//             //SceneManager.LoadScene("Test");
//             unsafe
//             {
// #if UNITY_EDITOR
//                 DBTest.leveldb_close(DBTest.db);
//                 DBTest.leveldb_close(DBTest.dbModel);

// #endif
// #if !UNITY_EDITOR
//                 Zion.closeDB();
//                 Zion.closeModelDB();
// #endif

//             }

// #if !UNITY_EDITOR

//                //Zion.onWorldBackPress();            
// #endif

//         }

        public void OnPressSave()
        {
            if (chunksModifiedSaveSet.Count == 0)
                return;

            foreach (Vector3 chunkPos in chunksModifiedSaveSet)
            {

                VoxelChunk chunk = envInstance.GetChunk(chunkPos);

                Vector3 chunkCoordsMC;
                chunkCoordsMC.x = -(FastMath.FloorToInt(chunk.position.x / 16) + 1);
                chunkCoordsMC.y = FastMath.FloorToInt(chunk.position.y / 16);
                chunkCoordsMC.z = FastMath.FloorToInt(chunk.position.z / 16);

                string chunkKey = "chunk_" + chunkCoordsMC.x + "_" + chunkCoordsMC.y + "_" + chunkCoordsMC.z;
                int voxelWaterLevel;
                List<byte> chunkDataList = new List<byte>();
                //short idWater;

                for (short i = 0; i < 4096; i++)
                {
                    Voxel tempVoxel = chunk.voxels[i];
                    string voxelName = tempVoxel.type.name;
                    voxelWaterLevel = tempVoxel.GetWaterLevel();

                    byte[] voxelIndexBytes = new byte[2];
                    //Util.FromShort(i, out voxelIndexBytes[0], out voxelIndexBytes[1]);
                    //voxelIndexBytes = BitConverter.GetBytes(i);
                    if (!tempVoxel.isEmpty)
                    {
                        //if (voxelWaterLevel == 0)
                        //    idWater = reverseVoxelMap[voxelName][0];
                        //else
                        //    idWater = -reverseVoxelMap[voxelName][0];

                        chunkDataList.Add(voxelIndexBytes[0]);
                        chunkDataList.Add(voxelIndexBytes[1]);
                        chunkDataList.Add(reverseVoxelMap[voxelName][0]);
                        chunkDataList.Add(reverseVoxelMap[voxelName][1]);
                        chunkDataList.Add(reverseVoxelMap[voxelName][2]);
                        chunkDataList.Add(reverseVoxelMap[voxelName][3]);
                        chunkDataList.Add((byte)tempVoxel.GetTextureRotation());
                    }

                }

                //chunkDataJSON.Remove(chunkDataJSON.Length - 1);
                //chunkDataJSON += "]";

                byte[] chunkDataArray = chunkDataList.ToArray();

#if UNITY_EDITOR
                DBTest.SetValue(chunkKey, chunkDataArray);
#endif
#if !UNITY_EDITOR
                Zion.putChunkValue(chunkKey,chunkDataArray);
#endif
                //chunksModifiedSaveSet.Remove(chunkPos);

            }

            chunksModifiedSaveSet.Clear();

            warningText.text = "SAVE SUCCESSFUL!";
            if (warningText.transform.parent.gameObject.activeSelf == false)
                warningText.transform.parent.gameObject.SetActive(true);
            else
            {
                warningText.transform.parent.gameObject.GetComponent<Animator>().Play("warning", -1, 0f);
            }

        }
        public void InitModelKeysRoomCreation()
        {

#if !UNITY_EDITOR
        string[] modelKeyList = Zion.GetCollectablesKeysList();
#endif
#if UNITY_EDITOR
            string[] modelKeyList = { };
#endif
            // for (int i = 0; i < modelKeyList.Length; i++)
            // {
            //     CollectableModelKey tempModelKey = Util.GetModelKeyFromString(modelKeyList[i]);
            //     Util.modelKeys.Add(tempModelKey);
            // }
        }

    }


}