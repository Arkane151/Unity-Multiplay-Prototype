using LevelDB_DLLImport_Test;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using WorldViewer;
using static WorldViewer.World;

namespace VoxelPlay
{

    [CreateAssetMenu(menuName = "Voxel Play/Terrain Generators/My Generator", fileName = "My Generator", order = 103)]
    public class MyGenerator : VoxelPlayTerrainGenerator
    {

        public int altitude = 500;

        //list of voxel definitions
        public BlockType[] voxelList = new BlockType[500];

        //list of quality voxel definitions
        public BlockType[] voxelListQuality = new BlockType[5];

        //Enviroment instance
        private VoxelPlayEnvironment instance;

        //Position of chunk
        private Vector3 chunkPos;

        //Standard water voxel definition used by VoxelPlay
        public VoxelDefinition waterVoxel;

        //Blocks that have variants stored as subtypes
        private short[] variantBlocks = { 1, 5, 6, 8, 18, 24, 35, 38, 98, 155, 159, 161, 168, 236, 237, 241, 559 };

        //Blocks that use more than one voxel
        private short[] bigBlocks = { 64, 71, 193, 194, 195, 196, 197, 499, 500, 26 };

        //Blocks with rotation (for inventory map)
        private short[] rotBlocks = { 17, 23, 27, 28, 29, 33, 34, 50, 53, 54, 55, 61, 62, 63, 65, 66, 67, 68, 69, 75, 76, 77, 90, 93, 94, 96, 106, 107, 108, 109, 114, 125, 126, 128, 130, 134, 135, 136, 143, 145, 146, 149, 150, 156, 162, 163, 164, 167, 176, 177, 180, 183, 184, 185, 186, 187, 199, 202, 203, 204, 208, 239, 257, 258, 259, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 424, 425, 426, 427, 428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 461, 464, 469, 472, 480, 481, 501, 502, 505, 506, 507, 508, 509, 510, 513, 514, 515, 516, 523, 531, 551 };

        //Rail blocks
        private short[] railBlocks = { 27, 28, 66, 126 };

        //Connected blocks
        private short[] connectedBlocks = { 85, 101, 102, 113, 139, 160, 190, 191, 511, 512, 532, 533, 552 };

        //Slabs
        private short[] slabBlocks = { 44, 158, 182, 421, 519, 520, 537, 539, 548 };

        //Omit blocks
        private short[] omitBlocks = { 36, 52, 62, 74, 97, 124, 137, 188, 189, 192, 209, 252, 247, 521, 522, 538, 540, 549, 43, 157, 181, 422, 423, 454 };

        //
        //
        //
        private short[] stairBlocks = { 53, 67, 108, 109, 114, 128, 134, 135, 136, 156, 163, 164, 180, 203, 257, 258, 259, 424, 425, 426, 427, 428, 429, 430, 431, 432, 433, 434, 435, 439, 440, 509, 510, 530, 531, 547 };

        //Blocks that are always underwater
        private int[] underwaterBlocks = { 393, 385 };

        ////Blocks with orientation
        //private int[] orientationBlocks = { };

        //Active Scene Index
        private string activeScene;

        //Chunk coordinates
        private Vector3 chunkCoords;

        //prefabs to instantiate
        public GameObject firePrefab;
        public GameObject soulFirePrefab;

        public AudioClip footfall1;
        public AudioClip footfall2;
        public AudioClip footfall3;
        /// <summary>
        /// Used to initialize any data structure or reload
        /// </summary>
        protected override void Init()
        {
            instance = VoxelPlayEnvironment.instance;
            env.currentWaterVoxelDefinition = waterVoxel;
            activeScene = SceneManager.GetActiveScene().name;
            InitializationManager.voxelList = voxelList;

            //REVERSE MAPPING

            InitializationManager.reverseVoxelMap["Null"] = new byte[] { 0, 0, 0, 0 };

            for (short i = 0; i < voxelList.Length; i++)
            {
                byte[] temp = new byte[2];
                //Util.FromShort(i, out temp[0], out temp[1]);

                //blocks without orientation
                if (voxelList[i].f2.Length == 0)
                {

                    for (byte j = 0; j < (byte)voxelList[i].subTypes.Length; j++)
                    {

                        if (voxelList[i].subTypes[j] != null)
                        {
                            if (!InitializationManager.reverseVoxelMap.ContainsKey(voxelList[i].subTypes[j].name))
                                InitializationManager.reverseVoxelMap.Add(voxelList[i].subTypes[j].name, new byte[] { temp[0], temp[1], j, 17 });
                        }

                    }
                }
                //blocks with orientation
                else
                {

                    for (byte m = 0; m < (byte)voxelList[i].f2.Length; m++)
                    {
                        for (byte n = 0; n < (byte)voxelList[i].f2[m].f1.Length; n++)
                        {
                            if (voxelList[i].f2[m].f1[n] != null)
                            {
                                if (!InitializationManager.reverseVoxelMap.ContainsKey(voxelList[i].f2[m].f1[n].name))
                                    InitializationManager.reverseVoxelMap.Add(voxelList[i].f2[m].f1[n].name, new byte[] { temp[0], temp[1], m, n });
                            }
                        }
                    }
                }
            }

            //
            //
            //
            //
            //
            //

          
            //
            //
            //
            //
            //
            //

            //REVERSE MAPPING

            InitializationManager.reverseVoxelMap["Null"] = new byte[] { 0, 0, 0, 0 };

            for (short i = 0; i < voxelList.Length; i++)
            {
                byte[] temp = new byte[2];
                Util.FromShort(i, out temp[0], out temp[1]);

                //blocks without orientation
                if (voxelList[i].f2.Length == 0)
                {

                    for (byte j = 0; j < (byte)voxelList[i].subTypes.Length; j++)
                    {

                        if (voxelList[i].subTypes[j] != null)
                        {
                            if (!InitializationManager.reverseVoxelMap.ContainsKey(voxelList[i].subTypes[j].name))
                                InitializationManager.reverseVoxelMap.Add(voxelList[i].subTypes[j].name, new byte[] { temp[0], temp[1], j, 17 });
                        }

                    }
                }
                //blocks with orientation
                else
                {

                    for (byte m = 0; m < (byte)voxelList[i].f2.Length; m++)
                    {
                        for (byte n = 0; n < (byte)voxelList[i].f2[m].f1.Length; n++)
                        {
                            if (voxelList[i].f2[m].f1[n] != null)
                            {
                                if (!InitializationManager.reverseVoxelMap.ContainsKey(voxelList[i].f2[m].f1[n].name))
                                    InitializationManager.reverseVoxelMap.Add(voxelList[i].f2[m].f1[n].name, new byte[] { temp[0], temp[1], m, n });
                            }
                        }
                    }
                }
            }

        }


        /// <summary>
        /// Gets the altitude and moisture
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        /// <param name="altitude">Altitude.</param>
        /// <param name="moisture">Moisture.</param>


        public override void GetHeightAndMoisture(double x, double z, out float altitude, out float moisture)
        {
            altitude = this.altitude / maxHeight;
            moisture = 0;
        }

        /// <summary>
        /// Paints the terrain inside the chunk defined by its central "position"
        /// </summary>
        /// <returns><c>true</c>, if terrain was painted, <c>false</c> otherwise.</returns>
        public override bool PaintChunk(VoxelChunk chunk)
        {
            if (activeScene == "ModelPreview")
            {
                chunk.isAboveSurface = true;

                return true; // true = > this chunk has contents
            }

            chunkPos = chunk.position;

            // Fill the chunk.voxels linear array with voxels

            chunkCoords.x = FastMath.FloorToInt(chunk.position.x / 16);
            chunkCoords.y = FastMath.FloorToInt(chunk.position.y / 16);
            chunkCoords.z = FastMath.FloorToInt(chunk.position.z / 16);
            float xOffset = 1;

            string chunkKey = "chunk_" + -(chunkCoords.x + xOffset) + "_" + chunkCoords.y + "_" + chunkCoords.z;

            byte[] chunkDataBuf;
            try
            {
                //float benchInit = Time.realtimeSinceStartup;
#if UNITY_EDITOR
                chunkDataBuf = DBTest.GetValue(chunkKey);
#endif
#if !UNITY_EDITOR
                chunkDataBuf = Zion.getchunkValue(chunkKey);// new byte[(int)valSize];
#endif
                if (chunkDataBuf == null)
                {
                    // Debug.Log("false");

                    return false;
                }
                //float benchEnd = Time.realtimeSinceStartup - benchInit;
                //Debug.Log(benchEnd);
            }
            catch (Exception ex)
            {
                //Debug.Log(ex.Message);
                return false;
            }

            /* Divided by 7 since each voxel will have 7 bytes of above described data
             * On dividing we'll get number of voxels count in each chunk
             * i.e. for ex : chunkDataBuf.Length / 7 = 2 that means this chunk has 2 voxels in that chunk
             */

            int blocksCount = chunkDataBuf.Length / 7;

            /* 1st and 2nd byte - index
             * 3rd and 4th byte - id + waterlogged
             * 5th byte - subId
             * 6th byte - orientation
             * 7th byte - rotation
             */

            short index;
            short id;
            byte subId;
            byte orientation;
            byte waterlogged;
            byte rotation;

            int idWater;

            for (int i = 0; i < blocksCount; i++)
            {
                
                index = Util.ToShort(chunkDataBuf[i * 7], chunkDataBuf[i * 7 + 1]);             /* 1st and 2nd byte, position of voxel in that chunk is given by "index" */
                idWater = Util.ToShort(chunkDataBuf[i * 7 + 2], chunkDataBuf[i * 7 + 3]);       // 3rd and 4th byte
                id = (short)Mathf.Abs(idWater);                                                 // Which type of voxel is it is given by "id"
                subId = chunkDataBuf[i * 7 + 4];                                                /* 5th byte, sub type of the voxel is given by "subId" */
                orientation = chunkDataBuf[i * 7 + 5];                                          // 6th byte
                rotation = chunkDataBuf[i * 7 + 6];                                             // 7th byte

                if (idWater < 0)
                    waterlogged = 1;                                                            // Voxel which are filled with water will have waterLooged as 1
                else
                    waterlogged = 0;                                                            // Voxel which aren't filled with water will have waterLooged as 0

                switch (orientation)
                {

                    //Blocks without orientation
                    case 17:
                        switch (id)
                        {

                            //specific block debug

                            //case 2:
                            //    Debug.Log(id + " " + subId + " VP chunk " + chunkPos);
                            //    break;

                            //invalid block id
                            case -1:
                                chunk.SetVoxel(index, voxelList[0].subTypes[1]);
                                break;

                            default:

                                //Big Blocks (without orientation)
                                if (bigBlocks.Any(b => b == id))
                                //if (bigBlocks.Contains(id))
                                {
                                    if (subId <= 7 && voxelList[id].subTypes[subId] != null)
                                        chunk.SetVoxel(index, voxelList[id].subTypes[subId]);
                                }

                                //Generic Blocks
                                else if (voxelList[id].subTypes[subId] != null)
                                {
                                    //modifications for Skin Viewership 
                                    if (activeScene == "Quality" || activeScene == "QualityEditor" || activeScene == "Skin Viewer")
                                    {
                                        switch (id)
                                        {
                                            case 12:
                                                switch (subId)
                                                {
                                                    case 0:
                                                        chunk.SetVoxel(index, voxelList[155].subTypes[0]);
                                                        break;
                                                    default:
                                                        chunk.SetVoxel(index, voxelList[id].subTypes[subId]);
                                                        break;

                                                }
                                                break;

                                            case 24:
                                                switch (subId)
                                                {
                                                    case 0:
                                                        chunk.SetVoxel(index, voxelList[559].subTypes[0]);
                                                        break;
                                                    case 1:
                                                        chunk.SetVoxel(index, voxelList[155].subTypes[1]);
                                                        break;
                                                    case 2:
                                                        chunk.SetVoxel(index, voxelList[155].subTypes[2]);
                                                        break;
                                                    case 3:
                                                        chunk.SetVoxel(index, voxelList[155].subTypes[3]);
                                                        break;
                                                    default:
                                                        chunk.SetVoxel(index, voxelList[id].subTypes[subId]);
                                                        break;
                                                }
                                                break;

                                            case 128:
                                                chunk.SetVoxel(index, voxelList[156].subTypes[subId]);
                                                break;

                                            case 44:
                                            case 43:
                                                switch (subId)
                                                {
                                                    case 1:
                                                        chunk.SetVoxel(index, voxelList[id].subTypes[6]);
                                                        break;
                                                    case 9:
                                                        chunk.SetVoxel(index, voxelList[id].subTypes[14]);
                                                        break;
                                                    default:
                                                        chunk.SetVoxel(index, voxelList[id].subTypes[subId]);
                                                        break;
                                                }
                                                break;

                                            default:
                                                chunk.SetVoxel(index, voxelList[id].subTypes[subId]);
                                                break;
                                        }
                                    }
                                    //Main Viewership
                                    else
                                    {
                                        chunk.SetVoxel(index, voxelList[id].subTypes[subId]);
                                    }
                                }

                                //If corresponding Voxel Definition is NULL - spawn invisible voxel
                                else
                                {
                                    chunk.voxels[index].Set(voxelList[0].subTypes[0]);
                                }
                                break;
                        }
                        break;

                    //Special Voxels with orientation
                    default:

                        if (voxelList[id].f2[subId].f1[orientation] != null)
                            chunk.SetVoxel(index, voxelList[id].f2[subId].f1[orientation]);
                        else
                            chunk.voxels[index].Set(voxelList[0].subTypes[0]);
                        break;
                }

                //add water if water coexists with voxel
                if (waterlogged == 1 || underwaterBlocks.Any(b => b == id))
                {
                    chunk.voxels[index].SetWaterLevel(14);
                }

                chunk.voxels[index].SetTextureRotation(rotation);
            }

            chunk.isAboveSurface = true;

            return true; // true = > this chunk has contents
        }
    }

    //Data Structure for all blocks
    [System.Serializable]
    public class BlockType
    {
        public string blockname;
        public VoxelDefinition[] subTypes = new VoxelDefinition[20];
        public DataStruct2[] f2 = new DataStruct2[0];

    }

    //Data structure for blocks with orientation (included in main Data Structure)
    [System.Serializable]
    public class DataStruct2
    {
        public VoxelDefinition[] f1 = new VoxelDefinition[0];
    }


}
