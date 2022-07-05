
//using FlexBuffers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using VoxelPlay;

namespace WorldViewer
{

 public static class World
  {
    // public static string worldID=SaveState.instance.worldId;
    // public static int spawnX = SaveState.instance.spawnX;
    //public static int spawnY = SaveState.instance.spawnY;
    //public static int spawnZ = SaveState.instance.spawnZ;
    //public static string worldName = SaveState.instance.worldName;
    public static int quality = 1;

    //Dictionary to parse json data of all chunks
    public static Dictionary<string, List<int>> worldData;

    //Flex buffer
    //public static FlxValue worldDataFlex;
    public static class Util
  {
    public static bool isEditorEnabled = false;
    public static bool isInventoryEnabled = true;
    private static int[] bodyPartIndices;
    public static int[] GetIntData(byte[] buf)
    {
      int[] dataInt = new int[buf.Length / 2];
      int j = 0;

      for (int i = 0; i < buf.Length; i += 2)
      {
        dataInt[j] = BitConverter.ToInt16(buf, i);
        Console.WriteLine(dataInt[j]);
        j++;
      }

      return dataInt;
    }

    public static short GetVoxelId(Voxel voxel)
    {
      return ToShort(InitializationManager.reverseVoxelMap[voxel.type.name][0], InitializationManager.reverseVoxelMap[voxel.type.name][1]);
    }

    public static short GetVoxelId(string voxelName)
    {
      return ToShort(InitializationManager.reverseVoxelMap[voxelName][0], InitializationManager.reverseVoxelMap[voxelName][1]);
    }
    

    //public static short ToShort(short byte1, short byte2, out short shortNum)
    //{
    //    shortNum = (short)((byte2 << 8) + byte1);
    //    return shortNum;
    //}

    public static short ToShort(short byte1, short byte2)
    {

      return (short)((byte2 << 8) + byte1);
    }

    public static void FromShort(short number, out byte byte1, out byte byte2)
    {
      byte2 = (byte)(number >> 8);
      byte1 = (byte)(number & 255);
    }

    public static void UpdateRenderDistance(int distance, VoxelPlayEnvironment envInstance)
    {
      //int dxz = (int)distance * 2 + 1;
      //int dy = Mathf.Min((int)distance, 8) * 2 + 1;
      //envInstance.maxChunks = Mathf.Max(3000, dxz * dxz * dy * 2);

      envInstance.visibleChunksDistance = (int)distance;
      Camera.main.farClipPlane = Mathf.Min(400, distance * 16);
    }
    // public static string GetStringFromModelKey(CollectableModelKey modelKey)
    // {
    //   return modelKey.modelName + "," + modelKey.creationTime + "," + modelKey.lastModified + "," + modelKey.usageCount;
    // }

    // public static CollectableModelKey GetModelKeyFromString(string modelKeyString)
    // {
    //   string[] modelKeyStringSplit = modelKeyString.Split(',');
    //   return new CollectableModelKey(modelKeyStringSplit[0], long.Parse(modelKeyStringSplit[1]), long.Parse(modelKeyStringSplit[2]), int.Parse(modelKeyStringSplit[3]));
    // }

    // public static string[] GetStringArrayFromModelKeys()
    // {
    //   string[] modelKeysString = new string[modelKeys.Count];



    //   for (int i = 0; i < modelKeys.Count; i++)
    //   {
    //     modelKeysString[i] = GetStringFromModelKey(modelKeys[i]);
    //   }
    //   return modelKeysString;
    // }

    //List of saved modelKeys
    // public static List<CollectableModelKey> modelKeys = new List<CollectableModelKey>();


    public static float ChangeRange(float oldMax, float oldMin, float newMax, float newMin, float value)
    {
      float OldRange = (oldMax - oldMin);
      float NewValue;

      if (OldRange == 0)
        NewValue = newMin;
      else
      {
        float NewRange = (newMax - newMin);
        NewValue = (((value - oldMin) * NewRange) / OldRange) + newMin;
      }
      return NewValue;
    }
  }


    public static string GetPath()
    {
      Debug.Log(Application.persistentDataPath);
      return Application.persistentDataPath + "/" + "mob";
      // return "D:/Totality/TotalityGit/UnityRendering/VoxelPlayFresh/Assets/World Data" + "/" + worldID;

    }
    public static bool isDirectoryExists()
    {
      bool val = false;
      if (Directory.Exists(GetPath()))
        val = true;
      return val;
    }
  }

}
