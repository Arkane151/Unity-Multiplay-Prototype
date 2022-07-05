using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEditor;
using System.Net;
using System.ComponentModel;
using System.Threading.Tasks;

namespace LevelDB_DLLImport_Test
{
    public static class DBTest
    {
        //public static TextMeshProUGUI dataTMPro;

        //import dll func

        [DllImport("leveldb", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern unsafe void* leveldb_open(void* db,
                                                               [MarshalAs(UnmanagedType.LPUTF8Str)] string dbName,
                                                               void* errorDouble);

        [DllImport("leveldb")] public static extern unsafe void leveldb_close(void* db);

        [DllImport("leveldb")] public static extern unsafe void leveldb_delete(void* db, void* writeOptions, [MarshalAs(UnmanagedType.LPUTF8Str)] string key, UIntPtr keySize, void* errorDouble); // leveldb_delete(self.db, wo, key, len(key), ctypes.byref(error))

        [DllImport("leveldb")] public static extern unsafe void* leveldb_options_create();

        [DllImport("leveldb")] public static extern unsafe void* leveldb_readoptions_create();

        [DllImport("leveldb")] public static extern unsafe void* leveldb_writeoptions_create();

        [DllImport("leveldb", CharSet = CharSet.Ansi)] public static extern unsafe IntPtr leveldb_get(void* db, void* readOptions, [MarshalAs(UnmanagedType.LPUTF8Str)] string key, UIntPtr keySize, void* valLen, void* errorDouble);

        [DllImport("leveldb", CharSet = CharSet.Ansi)] public static extern unsafe void leveldb_put(void* db, void* writeOptions, [MarshalAs(UnmanagedType.LPUTF8Str)] string key, UIntPtr keySize, IntPtr val, UIntPtr valSize, void* errorDouble);

        [DllImport("leveldb")]
        public static extern unsafe void
        leveldb_options_set_compression(void* p1, int compressionType);

        //other initialize
        public static string dbPath = "bundletajmahal2tempdb_jsonoptitajmahal_ldb3";
        public static unsafe void* db;
        public static unsafe void* dbModel;
        static unsafe void* readOptionsPtr;
        static unsafe void* writeOptionsPtr;
        static unsafe void* writeOptionsPtrModel;

        public static void Initialize()
        {

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += ModeChanged;
            //dataTMPro = GameObject.Find("Canvas").transform.GetChild(1).GetComponent<TextMeshProUGUI>();
#endif
            unsafe
            {
                Debug.Log(dbPath);

                void* optionsPtr = null;
                optionsPtr = leveldb_options_create();
                readOptionsPtr = leveldb_readoptions_create();
                writeOptionsPtr = leveldb_writeoptions_create();
                writeOptionsPtrModel = leveldb_writeoptions_create();

                if (optionsPtr != null)
                {
                    Debug.Log("OPTIONS CREATED");
                }

                leveldb_options_set_compression(optionsPtr, 4);

                char* error = null;
                char** errorDouble = &error;

                db = null;
                dbModel = null;
                db = leveldb_open(optionsPtr, dbPath, errorDouble);
                dbModel = leveldb_open(optionsPtr, Application.persistentDataPath + "/models", errorDouble);

                if (db != null)
                {
                    Debug.Log("DB LOADED");
                }
                if (dbModel != null)
                {
                    Debug.Log("DBMODEL LOADED");
                }
                else
                {
                    Debug.Log("DB NOT LOADED");
                }

            }

        }

        public static void DeleteModel(string key)
        {
            unsafe
            {
                UIntPtr keySize = new UIntPtr((uint)key.Length);
                char* errorDeleteModel = null;
                char** errorDoubleDeleteModel = &errorDeleteModel;
                leveldb_delete(dbModel, writeOptionsPtr, key, keySize, errorDoubleDeleteModel);
            }
        }

        public static byte[] GetValue(string key)
        {
            unsafe
            {
                UIntPtr keySize = new UIntPtr((uint)key.Length);

                UIntPtr valSize = new UIntPtr(0);
                void* valLenPtr = &valSize;
                char* errorGet = null;
                char** errorDoubleGet = &errorGet;

                IntPtr valPtr = leveldb_get(db, readOptionsPtr, key, keySize, valLenPtr, errorDoubleGet);

                byte[] managedArray = new byte[(int)valSize];
                Marshal.Copy(valPtr, managedArray, 0, (int)valSize);

                return managedArray;
            }

        }

        public static byte[] GetValueModel(string key)
        {
            unsafe
            {
                UIntPtr keySize = new UIntPtr((uint)key.Length);

                UIntPtr valSize = new UIntPtr(0);
                void* valLenPtr = &valSize;
                char* errorGet = null;
                char** errorDoubleGet = &errorGet;

                IntPtr valPtr = leveldb_get(dbModel, readOptionsPtr, key, keySize, valLenPtr, errorDoubleGet);

                byte[] managedArray = new byte[(int)valSize];
                Marshal.Copy(valPtr, managedArray, 0, (int)valSize);

                return managedArray;
            }

        }

        public static void GetValueBtn(string key)
        {
            unsafe
            {

                UIntPtr keySize = new UIntPtr((uint)key.Length);

                UIntPtr valSize = new UIntPtr(0);
                void* valLenPtr = &valSize;
                char* errorGet = null;
                char** errorDoubleGet = &errorGet;

                //Debug.Log("Keysize- " + keySize);
                IntPtr valPtr = leveldb_get(db, readOptionsPtr, key, keySize, valLenPtr, errorDoubleGet);
                //string val = Marshal.PtrToStringAnsi(valPtr, (int)valSize);

                byte[] managedArray = new byte[(int)valSize];
                Marshal.Copy(valPtr, managedArray, 0, (int)valSize);
                //Marshal.ptr

                //Debug.Log("ValSize- " + valSize + "\n");
                //Debug.Log("Value:- \n" + val);
                for (int i = 0; i < (int)valSize; i++)
                {
                    //dataTMPro.text += managedArray[i] + " ";
                }
                //return val;
            }

        }


        public static void SetValue(string key, byte[] value)
        {
            //string key = "-7_0_-52";
            //byte[] value = new byte[] { 1, 2, 3, 4, 5, 255 };
            unsafe
            {
                UIntPtr keySize = new UIntPtr((uint)key.Length);
                UIntPtr valSize = new UIntPtr((uint)value.Length);
                char* errorSet = null;
                char** errorDoubleSet = &errorSet;


                IntPtr pnt = Marshal.AllocHGlobal((int)valSize);
                Marshal.Copy(value, 0, pnt, value.Length);

                leveldb_put(db, writeOptionsPtr, key, keySize, pnt, valSize, errorDoubleSet);
                //Debug.Log("Value put");
            }
        }

        public static void SetValueModel(string key, byte[] value)
        {
            //string key = "-7_0_-52";
            //byte[] value = new byte[] { 1, 2, 3, 4, 5, 255 };
            unsafe
            {
                UIntPtr keySize = new UIntPtr((uint)key.Length);
                UIntPtr valSize = new UIntPtr((uint)value.Length);
                char* errorSet = null;
                char** errorDoubleSet = &errorSet;


                IntPtr pnt = Marshal.AllocHGlobal((int)valSize);
                Marshal.Copy(value, 0, pnt, value.Length);

                leveldb_put(dbModel, writeOptionsPtrModel, key, keySize, pnt, valSize, errorDoubleSet);
                //Debug.Log("Value put");
            }
        }

        public static void SetValueBtn()
        {
            string key = "-7_0_-52";
            byte[] value = new byte[] { 1, 2, 3, 4, 5, 255 };
            unsafe
            {
                UIntPtr keySize = new UIntPtr((uint)key.Length);
                UIntPtr valSize = new UIntPtr((uint)value.Length);
                char* errorSet = null;
                char** errorDoubleSet = &errorSet;


                IntPtr pnt = Marshal.AllocHGlobal((int)valSize);
                Marshal.Copy(value, 0, pnt, value.Length);

                leveldb_put(db, writeOptionsPtr, key, keySize, pnt, valSize, errorDoubleSet);
                //Debug.Log("Value put");
            }
        }

#if UNITY_EDITOR
        static void ModeChanged(PlayModeStateChange playModeState)
        {
            if (playModeState == PlayModeStateChange.EnteredEditMode)
            {
                Debug.Log("Entered Edit mode.");
                unsafe
                {
#if UNITY_EDITOR
                    leveldb_close(db);
                    leveldb_close(dbModel);
#endif
                    Debug.Log("DB Closed");
                }
            }
        }
#endif


    }

}