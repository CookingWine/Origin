using System;
using UnityEngine;
using System.Collections.Generic;

namespace Origin.Expand
{
    /// <summary>
    /// Math扩展
    /// </summary>
    public static class MathExpand
    {
        /// <summary>
        /// 保留小数指定位数
        /// </summary>
        /// <param name="self">float值</param>
        /// <param name="point">保留位置</param>
        /// <returns>保留指定小数位数后的float值</returns>
        public static float Round(this float self , int point)
        {
            int scale = 1;
            for(int i = 0; i < point; i++)
            {
                scale *= 10;
            }
            self *= scale;
            return Mathf.Round(self) / scale;
        }

        /// <summary>
        /// 判断是否约等于目标值
        /// </summary>
        /// <param name="self">float值</param>
        /// <param name="targetValue">目标值</param>
        /// <returns>若约等于则返回true,否则返回false</returns>
        public static bool IsApproximately(this float self , float targetValue)
        {
            return Mathf.Approximately(self , targetValue);
        }

        /// <summary>
        /// 平方和根
        /// </summary>
        /// <param name="x">int值</param>
        /// <param name="y">int值</param>
        /// <returns>x与y的平方和根</returns>
        public static float Sqrt(this int x , int y)
        {
            int n2 = x ^ 2 + y ^ 2;
            return Mathf.Sqrt(n2);
        }

        /// <summary>
        /// 获取随机元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="self">数组</param>
        /// <returns>随机值</returns>
        public static T GetRandomValue<T>(this T[] self)
        {
            return self[UnityEngine.Random.Range(0 , self.Length)];
        }

        /// <summary>
        /// 获取随机元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="self">列表</param>
        /// <returns>随机值</returns>
        public static T GetRandomValue<T>(this List<T> self)
        {
            return self[UnityEngine.Random.Range(0 , self.Count)];
        }

        /// <summary>
        /// 获取指定个数随机元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="self">数组</param>
        /// <param name="count">个数</param>
        /// <returns>元素数组</returns>
        public static T[] GetRandomValue<T>(this T[] self , int count)
        {
            if(count > self.Length)
            {
                throw new ArgumentOutOfRangeException( );
            }
            List<T> tempList = new List<T>(self.Length);
            for(int i = 0; i < self.Length; i++)
            {
                tempList.Add(self[i]);
            }
            T[] retArray = new T[count];
            for(int i = 0; i < retArray.Length; i++)
            {
                int index = UnityEngine.Random.Range(0 , tempList.Count);
                retArray[i] = tempList[index];
                tempList.RemoveAt(index);
            }
            return retArray;
        }

        /// <summary>
        /// 获取指定个数随机元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="self">列表</param>
        /// <param name="count">个数</param>
        /// <returns>元素数组</returns>
        public static T[] GetRandomValue<T>(this List<T> self , int count)
        {
            if(count > self.Count)
            {
                throw new ArgumentOutOfRangeException( );
            }
            List<T> tempList = new List<T>(self.Count);
            for(int i = 0; i < self.Count; i++)
            {
                tempList.Add(self[i]);
            }
            T[] retArray = new T[count];
            for(int i = 0; i < retArray.Length; i++)
            {
                int index = UnityEngine.Random.Range(0 , tempList.Count);
                retArray[i] = tempList[index];
                tempList.RemoveAt(index);
            }
            return retArray;
        }

        /// <summary>
        /// 计算多边形周长
        /// </summary>
        /// <param name="self">多边形顶点数组</param>
        /// <returns>周长</returns>
        public static float GetPolygonPerimeter(this Vector3[] self)
        {
            if(self.Length < 3) return 0.0f;
            float retV = 0f;
            for(int i = 0; i < self.Length; i++)
            {
                retV += Vector3.Distance(self[i] , self[( i + 1 < self.Length ? i + 1 : 0 )]);
            }
            return retV;
        }

        /// <summary>
        /// 计算多边形周长
        /// </summary>
        /// <param name="self">多边形顶点列表</param>
        /// <returns>周长</returns>
        public static float GetPolygonPerimeter(this List<Vector3> self)
        {
            if(self.Count < 3) return 0.0f;
            float retV = 0f;
            for(int i = 0; i < self.Count; i++)
            {
                retV += Vector3.Distance(self[i] , self[( i + 1 < self.Count ? i + 1 : 0 )]);
            }
            return retV;
        }

        /// <summary>
        /// 计算多边形面积
        /// </summary>
        /// <param name="self">多边形顶点数组</param>
        /// <returns>面积</returns>
        public static float GetPolygonArea(this Vector3[] self)
        {
            if(self.Length < 3) return 0.0f;
            float retV = self[0].z * ( self[self.Length - 1].x - self[1].x );
            for(int i = 1; i < self.Length; i++)
            {
                retV += self[i].z * ( self[i - 1].x - self[( i + 1 ) % self.Length].x );
            }
            return Mathf.Abs(retV / 2.0f);
        }

        /// <summary>
        /// 计算多边形面积
        /// </summary>
        /// <param name="self">多边形顶点列表</param>
        /// <returns>面积</returns>
        public static float GetPolygonArea(this List<Vector3> self)
        {
            if(self.Count < 3) return 0.0f;
            float retV = self[0].z * ( self[self.Count - 1].x - self[1].x );
            for(int i = 1; i < self.Count; i++)
            {
                retV += self[i].z * ( self[i - 1].x - self[( i + 1 ) % self.Count].x );
            }
            return Mathf.Abs(retV / 2.0f);
        }

        /// <summary>
        /// 计算圆的周长
        /// </summary>
        /// <param name="self">半径</param>
        /// <returns>周长</returns>
        public static float GetCirclePerimeter(this float self)
        {
            return Mathf.PI * 2f * self;
        }

        /// <summary>
        /// 计算圆的面积
        /// </summary>
        /// <param name="self">半径</param>
        /// <returns>面积</returns>
        public static float GetCircleArea(this float self)
        {
            return Mathf.PI * Mathf.Pow(self , 2);
        }

        /// <summary>
        /// 三角函数计算对边的长度
        /// </summary>
        /// <param name="self">角度</param>
        /// <param name="neighbouringSideLength">邻边的长度</param>
        /// <returns>对边的长度</returns>
        public static float GetFaceSideLength(this float self , float neighbouringSideLength)
        {
            return neighbouringSideLength * Mathf.Tan(self * Mathf.Deg2Rad);
        }

        /// <summary>
        /// 三角函数计算邻边的长度
        /// </summary>
        /// <param name="self">角度</param>
        /// <param name="faceSideLength">对边的长度</param>
        /// <returns>邻边的长度</returns>
        public static float GetNeighbouringSideLength(this float self , float faceSideLength)
        {
            return faceSideLength / Mathf.Tan(self * Mathf.Deg2Rad);
        }

        /// <summary>
        /// 勾股定理计算斜边的长度
        /// </summary>
        /// <param name="self">直角边的长度</param>
        /// <param name="anotherRightangleSideLength">另一条直角边的长度</param>
        /// <returns>斜边的长度</returns>
        public static float GetHypotenuseLength(this float self , float anotherRightangleSideLength)
        {
            return Mathf.Sqrt(Mathf.Pow(self , 2f) + Mathf.Pow(anotherRightangleSideLength , 2f));
        }

        /// <summary>
        /// 正弦
        /// </summary>
        /// <param name="self">角度</param>
        /// <returns>正弦值</returns>
        public static float Sin(this float self)
        {
            return Mathf.Sin(self * Mathf.Deg2Rad);
        }

        /// <summary>
        /// 余弦
        /// </summary>
        /// <param name="self">角度</param>
        /// <returns></returns>
        public static float Cos(this float self)
        {
            return Mathf.Cos(self * Mathf.Deg2Rad);
        }

        /// <summary>
        /// 正切
        /// </summary>
        /// <param name="self">角度</param>
        /// <returns>正切值</returns>
        public static float Tan(this float self)
        {
            return Mathf.Tan(self * Mathf.Deg2Rad);
        }

        /// <summary>
        /// 反正弦
        /// </summary>
        /// <param name="self">正弦值</param>
        /// <returns>角度</returns>
        public static float ArcSin(this float self)
        {
            return Mathf.Asin(self) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 反余弦
        /// </summary>
        /// <param name="self">余弦值</param>
        /// <returns>角度</returns>
        public static float ArcCos(this float self)
        {
            return Mathf.Acos(self) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 反正切
        /// </summary>
        /// <param name="self">正切值</param>
        /// <returns>角度</returns>
        public static float ArcTan(this float self)
        {
            return Mathf.Atan(self) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 度转弧度
        /// </summary>
        /// <param name="self">度</param>
        /// <returns>弧度</returns>
        public static float Deg2Rad(this float self)
        {
            return self * Mathf.Deg2Rad;
        }

        /// <summary>
        /// 弧度转度
        /// </summary>
        /// <param name="self">弧度</param>
        /// <returns>度</returns>
        public static float Rad2Deg(this float self)
        {
            return self * Mathf.Rad2Deg;
        }
    }
}
