using UnityEngine;
using OriginRuntime;
using System.Collections.Generic;

namespace RuntimeLogic
{
    /// <summary>
    /// 组件自动绑定工具
    /// </summary>
    [DisallowMultipleComponent]
    internal sealed class BuiltinComponentAutoBindTool:MonoBehaviour
    {
        [SerializeField]
        private List<Component> m_BindMapping = new List<Component>( );

        /// <summary>
        /// 获取绑定的组件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="index">索引</param>
        /// <returns>组件</returns>
        internal T GetBindComponent<T>(int index) where T : Component
        {
            if(index < 0 || index > m_BindMapping.Count)
            {
                throw new GameFrameworkException("Index out of range.");
            }
            T component = m_BindMapping[index] as T ?? throw new GameFrameworkException(Utility.Text.Format("No corresponding type found:{0}" , typeof(T).FullName));
            return component;
        }
    }
}
