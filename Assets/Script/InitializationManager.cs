using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VoxelPlay;
using UnityEngine.UI;
using TMPro;
using LevelDB_DLLImport_Test;
using WorldViewer;
using System.Text;

namespace WorldViewer
{
    public class InitializationManager : MonoBehaviour
    {

        static InitializationManager _instance;
        public static InitializationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<InitializationManager>();
                    if (_instance == null)
                    {
                        InitializationManager[] vv = Resources.FindObjectsOfTypeAll<InitializationManager>();
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

        //World Data
        //public WorldData worldData;

        //Spawn Pos
        public int[] spawnPosArray = new int[3];

        //Controller instance
        VoxelPlayFirstPersonController controllerInstance;

        //Inventory Gameobject
        public GameObject inventoryObject;

        //player camera
        public GameObject playerCam;

        //list of voxel definitions
        public static BlockType[] voxelList = new BlockType[500];

        //reverse mapping for voxel definitions using VD name
        public static Dictionary<string, byte[]> reverseVoxelMap = new Dictionary<string, byte[]>();

        //Hashset of chunks that were modified while placing models
        public static HashSet<Vector3> chunksModifiedSaveSet = new HashSet<Vector3>();

        //system info text
        public TextMeshProUGUI warningText;

        public Transform spawn;

        //Active scene index
        public string activeScene;
        public Text log;

        // Start is called before the first frame update
        void Awake()
        {
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
            Debug.unityLogger.logEnabled = false;
#endif

            //Application.targetFrameRate = 60;

            if (activeScene == "ModelPreview")
            {
                Screen.orientation = ScreenOrientation.Portrait;
            }
            else
            {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }

            activeScene = SceneManager.GetActiveScene().name;

            envInstance = VoxelPlayEnvironment.instance;
            // if (EditorUI.Instance != null)
            //     EditorUI.Instance.InitSettings(envInstance);
            // if (VoxelPlayFirstPersonController.instance != null)
            //     controllerInstance = VoxelPlayFirstPersonController.instance;
            // if (Util.isInventoryEnabled && inventoryObject != null)
            //     inventoryObject.SetActive(true);

            if (World.quality == 2)
            {
                QualitySettings.antiAliasing = 2;
            }

            envInstance.OnInitialized += () =>
            {

                switch (activeScene)
                {

                    //Include Skin Viewer World ID initialization here 
                    case "Skin Viewer":

                        //SetWorldId("9");



                        //Zion.GetZionPreferences().CallStatic("updateSceneNo", Zion.GetCurrentActivity(), 3);

#if UNITY_EDITOR

                        DBTest.dbPath = Application.persistentDataPath +"/mob";;
                        DBTest.Initialize();
#endif


                        //Zion.GetZionPreferences().CallStatic("WorldLoaded", Zion.GetCurrentActivity());

                        break;

                    case "ModelPreview":

                        //SetWorldId("1");
#if UNITY_EDITOR

                        DBTest.dbPath = Application.persistentDataPath +"/mob";
                        DBTest.Initialize();

#endif


#if !UNITY_EDITOR
                        Zion.updateSceneNo(6);
                        Zion.openDB(SaveState.instance.worldData.worldPath);
                        Zion.openModelDB();

#endif
                        //PreviewManager.isInitialized = true;


                        break;

                    //Include Main Viewership World ID initialization here
                    default:

                        StartCoroutine("WaitForLoad");


#if UNITY_EDITOR

                        DBTest.dbPath = Application.persistentDataPath +"/mob";
                        DBTest.Initialize();
#endif

#if !UNITY_EDITOR
                        Zion.openDB(SaveState.instance.worldData.worldPath);
                        Zion.openModelDB();
#endif
                        // SaveState.instance.gameModeData = new GameModeData();
                        // EditorUI.Instance.RefreshPlayerSpawnPosUI(12, GameMode.FreeForAll);
                        // EditorUI.Instance.InitModels();

#if !UNITY_EDITOR

                            Zion.WorldLoaded();
                            Zion.updateSceneNo(7);
#endif

                        //Set player position 

                        //                        if (SaveState.instance.worldData.spawnPos == "0,0,0" || !SaveState.instance.isSpawnPosSet)
                        //                        {
                        //                            byte[] spawnPosBytes = Encoding.ASCII.GetBytes("0,0,0");
                        //#if UNITY_EDITOR
                        //                            DBTest.SetValue("spawnPos", spawnPosBytes);
                        //#endif
                        //#if !UNITY_EDITOR
                        //                Zion.putChunkValue("spawnPos",spawnPosBytes);
                        //#endif
                        //                        }
                        //                        else
                        //                        {
                        //                            byte[] spawnPosBytes = Encoding.ASCII.GetBytes(SaveState.instance.worldData.spawnPos);

                        //#if UNITY_EDITOR
                        //                            DBTest.SetValue("spawnPos", spawnPosBytes);
                        //#endif
                        //#if !UNITY_EDITOR
                        //                Zion.putChunkValue("spawnPos",spawnPosBytes);
                        //#endif
                        //                        }

                        string[] spawnPosSplit;
                        // if (SaveState.instance.isSpawnPosSet)
                        //     spawnPosSplit = SaveState.instance.worldData.spawnPos.Split(',');
                        
                        {
                            string spawnPosString = string.Empty;
#if UNITY_EDITOR
                            try
                            {
                                spawnPosString = Encoding.ASCII.GetString(DBTest.GetValue("spawnPos"));
                            }
                            catch
                            {
                                byte[] spawnPosBytes = Encoding.ASCII.GetBytes("0,0,0");
                                DBTest.SetValue("spawnPos", spawnPosBytes);
                                spawnPosString = "0,0,0";
                            }
#endif
#if !UNITY_EDITOR
                            try
                            {
                                spawnPosString = Encoding.ASCII.GetString(Zion.getchunkValue("spawnPos"));
                            }
                            catch
                            {
                                byte[] spawnPosBytes = Encoding.ASCII.GetBytes("0,0,0");
                                Zion.putChunkValue("spawnPos", spawnPosBytes);
                                spawnPosString = "0,0,0";
                            }
#endif
                            spawnPosSplit = spawnPosString.Split(',');
                        }
                        spawnPosArray[0] = int.Parse(spawnPosSplit[0]);
                        spawnPosArray[1] = int.Parse(spawnPosSplit[1]);
                        spawnPosArray[2] = int.Parse(spawnPosSplit[2]);


                        if (controllerInstance != null)
                            controllerInstance.transform.position = new Vector3(spawnPosArray[0], spawnPosArray[1] + 1.2f, spawnPosArray[2]);

                        //Set world name
                        // if (SaveState.instance.worldData.title.Length > 80)
                        // {
                        //     string nameTruncated = SaveState.instance.worldData.title.Substring(0, 80);
                        //     nameTruncated += "...";
                        //     SaveState.instance.worldData.title = nameTruncated;
                        // }
                        //envInstance.welcomeMessage = "<color=green>Welcome to <color=white>" + SaveState.instance.worldData.title + "</color>!</color>";

                        break;

                }

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

        public IEnumerator WaitForLoad()
        {
            yield return new WaitForSeconds(3f);
#if !UNITY_EDITOR

            //Zion.setQuality(World.quality);
#endif
            //Zion.GetZionPreferences().CallStatic("setQuality", Zion.GetCurrentActivity(), World.quality);
            //Zion.GetZionPreferences().CallStatic("WorldLoaded", Zion.GetCurrentActivity());
        }

        // public void SetWorldId(string id)
        // {
        //     SaveState.instance.worldData.worldID = id;

        // }

        public void OnPressBackWorld(bool isSaved = false)
        {
            //envInstance.DisposeAll();
            //envInstance = null;           
            Debug.Log("Exit");
            //Util.isEditorEnabled = false;

            //SceneManager.LoadScene("Test");

#if UNITY_EDITOR
            unsafe
            {
                DBTest.leveldb_close(DBTest.db);
                DBTest.leveldb_close(DBTest.dbModel);
            }

#endif
#if !UNITY_EDITOR
                    Zion.closeDB();
                    Zion.closeModelDB();
                    Zion.SetCollectablesKeysList(Util.GetStringArrayFromModelKeys());
                    Screen.orientation = ScreenOrientation.Portrait;
                    Zion.onWorldBackPress();
                    Zion.IsWorldSaved(isSaved);
#endif

        }

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
                //Debug.Log(chunkKey);
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
                Zion.putChunkValue(chunkKey, chunkDataArray);
#endif
            }

            // if (SaveState.instance.worldData.isPublished)
            // {
            //     SaveState.instance.worldData.version += 0.1f;
            //     Zion.PublishWorld(JsonUtility.ToJson(SaveState.instance.worldData));
            // }

            chunksModifiedSaveSet.Clear();

            warningText.text = "SAVE SUCCESSFUL!";
            if (warningText.transform.parent.gameObject.activeSelf == false)
                warningText.transform.parent.gameObject.SetActive(true);
            else
            {
                warningText.transform.parent.gameObject.GetComponent<Animator>().Play("warning", -1, 0f);
            }

        }

    }

}
