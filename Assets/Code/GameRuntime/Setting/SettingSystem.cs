using System;
using System.IO;
using UnityEngine;
using System.Text;
using OriginRuntime;
using System.Collections.Generic;
namespace RuntimeLogic
{
    internal sealed class SettingSystem:ISystemCore, ISettingSystem
    {
        private const string SettingFileName = "OriginSetting.dat";
        private readonly SortedDictionary<string , string> m_Setting = new SortedDictionary<string , string>(StringComparer.Ordinal);
        private SettingSerializer m_Serializer = null;
        private string m_FilePath = null;

        public int Priority => 0;

        public int Count
        {
            get
            {
                return m_Setting.Count;
            }
        }

        public void InitSystem( )
        {
            m_FilePath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath , SettingFileName));
            m_Serializer = new SettingSerializer( );
            m_Serializer.RegisterSerializeCallback(0 , SerializeDefaultSettingCallback);
            m_Serializer.RegisterDeserializeCallback(0 , DeserializeDefaultSettingCallback);
        }

        public void ShutdownSystem( )
        {
            Save( );
        }
        public string[] GetAllSettingNames( )
        {
            int index = 0;
            string[] allSettingNames = new string[m_Setting.Count];
            foreach(KeyValuePair<string , string> setting in m_Setting)
            {
                allSettingNames[index++] = setting.Key;
            }
            return allSettingNames;
        }

        public void GetAllSettingNames(List<string> results)
        {
            if(results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }
            results.Clear( );
            foreach(KeyValuePair<string , string> setting in m_Setting)
            {
                results.Add(setting.Key);
            }
        }

        public bool GetBool(string settingName)
        {
            if(!m_Setting.TryGetValue(settingName , out string value))
            {
                Log.Warning("Setting '{0}' is not exist." , settingName);
                return false;
            }
            return int.Parse(value) != 0;
        }

        public bool GetBool(string settingName , bool defaultValue)
        {
            if(!m_Setting.TryGetValue(settingName , out string value))
            {
                return defaultValue;
            }

            return int.Parse(value) != 0;
        }

        public float GetFloat(string settingName)
        {
            if(!m_Setting.TryGetValue(settingName , out string value))
            {
                Log.Warning("Setting '{0}' is not exist." , settingName);
                return 0f;
            }
            return float.Parse(value);
        }

        public float GetFloat(string settingName , float defaultValue)
        {
            if(!m_Setting.TryGetValue(settingName , out string value))
            {
                return defaultValue;
            }
            return float.Parse(value);
        }

        public int GetInt(string settingName)
        {
            if(!m_Setting.TryGetValue(settingName , out string value))
            {
                Log.Warning("Setting '{0}' is not exist." , settingName);
                return 0;
            }
            return int.Parse(value);
        }

        public int GetInt(string settingName , int defaultValue)
        {
            if(!m_Setting.TryGetValue(settingName , out string value))
            {
                return defaultValue;
            }

            return int.Parse(value);
        }

        public T GetObject<T>(string settingName)
        {
            return Utility.Json.ToObject<T>(GetString(settingName));
        }

        public object GetObject(Type objectType , string settingName)
        {
            return Utility.Json.ToObject(objectType , GetString(settingName));
        }

        public T GetObject<T>(string settingName , T defaultObj)
        {
            string json = GetString(settingName);
            if(json == null)
            {
                return defaultObj;
            }
            return Utility.Json.ToObject<T>(json);
        }

        public object GetObject(Type objectType , string settingName , object defaultObj)
        {
            string json = GetString(settingName);
            if(json == null)
            {
                return defaultObj;
            }
            return Utility.Json.ToObject(objectType , json);
        }

        public string GetString(string settingName)
        {
            if(!m_Setting.TryGetValue(settingName , out string value))
            {
                Log.Warning("Setting '{0}' is not exist." , settingName);
                return null;
            }
            return value;
        }

        public string GetString(string settingName , string defaultValue)
        {
            if(!m_Setting.TryGetValue(settingName , out string value))
            {
                return defaultValue;
            }
            return value;
        }

        public bool HasSetting(string settingName)
        {
            return m_Setting.ContainsKey(settingName);
        }

        public bool Load( )
        {
            try
            {
                if(!File.Exists(m_FilePath))
                {
                    return false;
                }
                using(FileStream fileStream = new FileStream(m_FilePath , FileMode.Open , FileAccess.Read))
                {
                    m_Serializer.Deserialize(fileStream);
                    return true;
                }
            }
            catch(Exception exception)
            {
                Log.Warning("Load settings failure with exception '{0}'." , exception.ToString( ));
                return false;
            }
        }

        public void RemoveAllSettings( )
        {
            m_Setting.Clear( );
        }

        public bool RemoveSetting(string settingName)
        {
            return m_Setting.Remove(settingName);
        }

        public bool Save( )
        {
            try
            {
                using(FileStream fileStream = new FileStream(m_FilePath , FileMode.Create , FileAccess.Write))
                {
                    return m_Serializer.Serialize(fileStream , this);
                }
            }
            catch(Exception exception)
            {
                Log.Warning("Save settings failure with exception '{0}'." , exception.ToString( ));
                return false;
            }
        }

        public void SetBool(string settingName , bool value)
        {
            m_Setting[settingName] = value ? "1" : "0";
        }

        public void SetFloat(string settingName , float value)
        {
            m_Setting[settingName] = value.ToString( );
        }

        public void SetInt(string settingName , int value)
        {
            m_Setting[settingName] = value.ToString( );
        }

        public void SetObject<T>(string settingName , T obj)
        {
            SetString(settingName , Utility.Json.ToJson(obj));
        }

        public void SetObject(string settingName , object obj)
        {
            SetString(settingName , Utility.Json.ToJson(obj));
        }

        public void SetString(string settingName , string value)
        {
            m_Setting[settingName] = value;
        }


        private bool SerializeDefaultSettingCallback(Stream stream , SettingSystem defaultSetting)
        {
            Serialize(stream);
            return true;
        }

        private SettingSystem DeserializeDefaultSettingCallback(Stream stream)
        {
            Deserialize(stream);
            return this;
        }

        /// <summary>
        /// 序列化数据。
        /// </summary>
        /// <param name="stream">目标流。</param>
        public void Serialize(Stream stream)
        {
            using(BinaryWriter binaryWriter = new BinaryWriter(stream , Encoding.UTF8))
            {
                binaryWriter.Write7BitEncodedInt32(m_Setting.Count);
                foreach(KeyValuePair<string , string> setting in m_Setting)
                {
                    binaryWriter.Write(setting.Key);
                    binaryWriter.Write(setting.Value);
                }
            }
        }

        /// <summary>
        /// 反序列化数据。
        /// </summary>
        /// <param name="stream">指定流。</param>
        public void Deserialize(Stream stream)
        {
            m_Setting.Clear( );
            using(BinaryReader binaryReader = new BinaryReader(stream , Encoding.UTF8))
            {
                int settingCount = binaryReader.Read7BitEncodedInt32( );
                for(int i = 0; i < settingCount; i++)
                {
                    m_Setting.Add(binaryReader.ReadString( ) , binaryReader.ReadString( ));
                }
            }
        }
    }
}
