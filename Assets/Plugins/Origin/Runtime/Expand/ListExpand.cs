using System;
using System.Collections.Generic;
using UnityEngine;

namespace Origin.Expand
{
    /// <summary>
    /// list 扩展
    /// </summary>
    public static class ListExpand
    {
        /// <summary>
        /// 随机获取一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T RandomListElement<T>(this List<T> list)
        {
            if(list.Count == 0)
            {
                return default;
            }
            if(list.Count == 1)
            {
                return list[0];
            }
            return list[new System.Random( ).Next(0 , list.Count)];
        }

        /// <summary>
        /// 在末尾追加另一list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="bList"></param>
        /// <returns></returns>
        public static List<T> AppendList<T>(this List<T> list , List<T> bList)
        {
            for(int i = 0; i < bList.Count; i++)
            {
                list.Add(bList[i]);
            }
            return list;
        }

        /// <summary>
        /// 把List的全部Item复制到listB中,让两个list所有元素相同,而不是引用关系
        /// </summary>
        public static List<T> CopyTo<T>(this List<T> list , ref List<T> listB)
        {
            listB.Clear( );
            for(int i = 0; i < list.Count; i++)
            {
                listB.Add(list[i]);
            }
            return list;
        }

        /// <summary>
        /// 删除后重复的数据,重复的值只保留一份
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> DelRepeat<T>(this List<T> list)
        {
            for(int i = 0; i < list.Count; i++)
            {
                for(int j = list.Count - 1; j > i; j--) //j>i 的意思是:>i前面的已经比较过了
                {
                    var A = list[i].ToString( );
                    var B = list[j].ToString( );
                    if(A == B)
                    {
                        list.RemoveAt(j);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 得到后重复的数据的下标,重复的值只保留一份,
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<int> GetRepeatIndex<T>(this List<T> list)
        {
            var indexList = new List<int>( );
            for(int i = 0; i < list.Count - 1; i++)
            {
                for(int j = list.Count - 1; j > i; j--)
                {
                    var A = list[i].ToString( );
                    var B = list[j].ToString( );
                    if(A == B && !indexList.Contains(j))
                    {
                        indexList.Add(j);
                    }
                }
            }
            return indexList;
        }

        /// <summary>
        /// 从大到小排序
        /// </summary>
        public static List<int> SortDescending<T>(this List<int> list)
        {
            list.Sort((x , y) => -x.CompareTo(y));
            return list;
        }
        /// <summary>
        /// 从大到小排序
        /// </summary>
        public static List<string> SortDescending<T>(this List<string> list)
        {
            list.Sort((x , y) => -x.CompareTo(y));
            return list;
        }

        public static List<T> RandomListFromBigList<T>(this List<T> pList , int randomNum)
        {
            int num = randomNum >= pList.Count ? pList.Count - 1 : randomNum;
            System.Random random = new System.Random(unchecked((int)DateTime.Now.Ticks));
            List<T> list = new List<T>(num);
            for(int i = 0; i < num; i++)
            {
                int r = random.Next(1 , pList.Count);
                T temp = pList[r];
                if(list.Contains(temp))
                {
                    --i;
                    continue;
                }
                else
                {
                    list.Add(temp);
                }
            }
            return list;
        }

        /// <summary>
        /// 列表同步方法封装 - 列表拷贝
        /// </summary>
        public static List<T> ListClone<T>(this List<T> ab)
        {
            List<T> ret = new List<T>( );
            ab.ForEach(item => ret.Add(item));
            return ret;
        }

        public static void ListSafeForEach<T>(this List<T> ab , Func<T , bool> ac)
        {
            List<T> recy = ab.ListClone<T>( );
            foreach(T item in recy)
            {
                if(!ac(item))
                {
                    break;
                }
            }
            recy.Clear( );
        }

        /// <summary>
        /// 列表同步方法封装 - 
        /// </summary>
        /// <param name="item">添加项</param>
        public static void ListAdd<T>(this List<T> ab , T item)
        {
            if(item != null)
            {
                lock(ab)
                {
                    ab.Add(item);
                }
            }
        }
        /// <summary>
        /// 列表同步方法封装 - 添加项, 但不会重复添加同一个对象
        /// </summary>
        /// <param name="item">添加项</param>
        public static void ListAddNotForSelf<T>(this List<T> ab , T item)
        {
            if(item != null)
            {
                lock(ab)
                {
                    if(!ab.Contains(item))
                    {
                        ab.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// 列表同步方法封装
        /// </summary>
        /// <param name="item">删除项</param>
        public static void ListRemove<T>(this List<T> ab , T item)
        {
            if(item != null)
            {
                lock(ab)
                {
                    ab.Remove(item);
                }
            }
        }

        /// <summary>
        /// 清空list
        /// </summary>
        public static void ListClear<T>(this List<T> ab)
        {
            lock(ab)
            {
                ab.Clear( );
            }
        }

        /// <summary>
        /// 获取list长度
        /// </summary>
        public static int ListCount<T>(this List<T> ab)
        {
            lock(ab)
            {
                return ab.Count;
            }
        }
    }
}
