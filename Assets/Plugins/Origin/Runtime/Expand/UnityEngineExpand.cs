using System;
using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Origin.Expand
{
    public static class UnityEngineExpand
    {
        ///<summary>获取组件，不存在则添加</summary>
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if(go.GetComponent<T>( ) == null)
            {
                go.AddComponent<T>( );
            }
            return go.GetComponent<T>( );
        }

        ///<summary>物体是否存在该组件</summary>
        public static bool HasComponet<T>(this GameObject go) where T : Component => go.GetComponent<T>( ) != null;


        /// <summary>移除物体上挂载的组件</summary>
        public static void Remove<T>(this GameObject go) where T : MonoBehaviour
        {
            if(go.HasComponet<T>( ))
            {
                GameObject.Destroy(go.GetComponent<T>( ));
            }
        }

        ///<summary>改变image透明度</summary>
        public static void ChangeAlpha(this UnityEngine.UI.Image image , float alpha)
        {
            Color oldcolor = image.color;
            image.color = new Color(oldcolor.r , oldcolor.g , oldcolor.b , alpha);
        }

        /// <summary>改变对象的层级</summary>
        public static void ChangeLayer(this GameObject go , string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if(go)
            {
                foreach(Transform item in go.GetComponentInChildren<Transform>(true))
                {
                    item.gameObject.layer = layer;
                }
            }
        }
        /// <summary>起协程,先等待延迟，然后运行事件</summary>
		public static Coroutine WaitSomeTime(this MonoBehaviour mono , float time , UnityEngine.Events.UnityAction action)
        {
            return mono.StartCoroutine(WaitSomeTime(action , time));
        }

        private static IEnumerator WaitSomeTime(UnityEngine.Events.UnityAction action , float time)
        {
            yield return new WaitForSeconds(time);
            action?.Invoke( );
        }

        /// <summary>
        /// 查找子物体
        /// </summary>
        public static Transform FindTransform(this Transform parent , string name)
        {
            try
            {
                Transform child = parent.Find(name);
                if(child != null)
                {
                    return child;
                }
                for(int i = 0; i < parent.childCount; ++i)
                {
                    Transform transform = parent.GetChild(i);
                    if(transform.childCount > 0)
                    {
                        Transform childTransform = transform.FindTransform(name);
                        if(childTransform != null)
                        {
                            return childTransform;
                        }
                    }
                }
                return null;
            }
            catch(Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
            return null;
        }

        /// <summary>
        /// 用名字和组件类型查找组件
        /// </summary>
        public static T FindComponent<T>(this Transform parent , string name) where T : Component
        {
            Transform trans = FindTransform(parent , name);
            if(trans == null)
            {
                return null;
            }
            return trans.GetComponent<T>( );
        }

        /// <summary>
        /// 用组件类型查找组件
        /// </summary>
        public static T FindComponent<T>(this Transform parent) where T : Component
        {
            if(parent.TryGetComponent<T>(out var c))
                return c;
            for(int i = 0; i < parent.childCount; ++i)
            {
                Transform transform = parent.GetChild(i);
                if(transform.TryGetComponent<T>(out var comp))
                {
                    return comp;
                }
                if(transform.childCount > 0)
                {
                    T childComp = FindComponent<T>(transform);
                    if(childComp != null)
                    {
                        return childComp;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 设置物体transform信息到默认值
        /// </summary>
        /// <param name="target">目前物体</param>
        /// <param name="localPostion">true为本地位置，false为世界位置</param>
        public static void SetTransformInfoToDefalut(this Transform target , bool localPostion = true)
        {
            if(localPostion)
            {
                target.SetLocalPositionAndRotation(Vector3.zero , Quaternion.identity);
            }
            else
            {
                target.SetPositionAndRotation(Vector3.zero , Quaternion.identity);
            }
            target.localScale = Vector3.one;
        }



        /// <summary>
        /// 设置显示隐藏
        /// </summary>
        /// <param name="component"></param>
        /// <param name="active"></param>
        public static void SetActive(this Component component , bool active)
        {
            if(component != null)
            {
                if(component.gameObject.activeSelf != active)
                {
                    component.gameObject.SetActive(active);
                }
            }
        }

        /// <summary>
        /// 设置物体的本地坐标X
        /// </summary>
        /// <param name="x"></param>
        public static void SetLocalPostionX(this Component component , float x)
        {
            if(component != null)
            {
                Vector3 temp = component.transform.localPosition;
                temp.x = x;
                component.transform.localPosition = temp;
            }
        }

        /// <summary>
        /// 设置物体的本地坐标Y
        /// </summary>
        /// <param name="y"></param>
        public static void SetLocalPostionY(this Component component , float y)
        {
            if(component != null)
            {
                Vector3 temp = component.transform.localPosition;
                temp.y = y;
                component.transform.localPosition = temp;
            }
        }

        /// <summary>
        /// 设置物体的本地坐标Z
        /// </summary>
        /// <param name="z"></param>
        public static void SetLocalPostionZ(this Component component , float z)
        {
            if(component != null)
            {
                Vector3 temp = component.transform.localPosition;
                temp.z = z;
                component.transform.localPosition = temp;
            }
        }


        /// <summary>
        /// 设置layer
        /// </summary>
        /// <param name="target"></param>
        /// <param name="layer"></param>
        public static void SetLayer(this GameObject target , int layer)
        {
            if(target.layer != layer)
            {
                target.layer = layer;
                foreach(Transform item in target.transform)
                {
                    if(item.childCount > 0)
                    {
                        SetLayer(item.gameObject , layer);
                    }
                    item.gameObject.layer = layer;
                }
            }
        }

        /// <summary>
        /// 生成多边形mesh网格
        /// </summary>
        /// <param name="vector3">多边形顶点数组</param>
        /// <returns>网格</returns>
        public static Mesh GenerateMesh(this Vector3[] vector3)
        {
            Mesh mesh = new Mesh( );
            List<int> triangls = new List<int>( );
            for(int i = 0; i < vector3.Length - 1; i++)
            {
                triangls.Add(i);
                triangls.Add(i + 1);
                triangls.Add(vector3.Length - i - 1);
            }
            mesh.vertices = vector3;
            mesh.triangles = triangls.ToArray( );
            mesh.RecalculateBounds( );
            mesh.RecalculateNormals( );
            return mesh;
        }


        ///<summary>
        ///音频转字节组
        ///</summary>
        public static byte[] GetAudioByteArray(this AudioClip clip)
        {
            float[] data = new float[clip.samples];
            clip.GetData(data , 0);
            int rescaleFactor = 32767;
            byte[] outData = new byte[data.Length * 2];
            for(int i = 0; i < data.Length; i++)
            {
                //TODO:因为float数据在-1~1之间，需要把数组转换到有符号2个字节的范围-32768~32767。因此这里乘以32767
                short temshort = (short)( data[i] * rescaleFactor );
                byte[] temdata = BitConverter.GetBytes(temshort);
                outData[i * 2] = temdata[0];
                outData[i * 2 + 1] = temdata[1];
            }
            return outData;
        }

        ///<summary>
        ///音频字节转音频
        ///</summary>
        public static AudioClip BytesToAudioClip(this byte[] data , string clipName)
        {
            float[] clipData = new float[data.Length / 2];
            for(int i = 0; i < clipData.Length; i++)
            {
                clipData[i / 2] = BytesToFloat(data[i * 2] , data[i * 2 + 1]);
            }
            AudioClip clip = AudioClip.Create(clipName , 16000 * 10 , 1 , 16000 , false);
            clip.SetData(clipData , 0);
            return clip;
        }

        private static float BytesToFloat(byte firstBytes , byte secondBytes)
        {
            return ( BitConverter.IsLittleEndian ? (short)( ( secondBytes << 8 ) | firstBytes ) : (short)( ( firstBytes << 8 ) | secondBytes ) ) / 32768.0F;
        }
    }
}
