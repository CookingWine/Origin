using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Internal;

namespace OriginRuntime
{
    /// <summary>
    /// 有关mono的驱动接口
    /// </summary>
    public interface IMonoBehaviourDriver
    {
        /// <summary>
        /// 启动一个名为methodName的协程。
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public Coroutine StartCoroutine(string methodName);

        /// <summary>
        /// 启动一个协程。
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        public Coroutine StartCoroutine(IEnumerator routine);

        /// <summary>
        /// 启动一个名为methodName的协程，传入一个参数value。
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Coroutine StartCoroutine(string methodName , [DefaultValue("null")] object value);

        /// <summary>
        /// 停止一个名为methodName的协程。
        /// </summary>
        /// <param name="methodName"></param>
        public void StopCoroutine(string methodName);

        /// <summary>
        /// 停止一个协程。
        /// </summary>
        /// <param name="routine"></param>
        public void StopCoroutine(IEnumerator routine);
        /// <summary>
        /// 停止一个协程。
        /// </summary>
        /// <param name="routine"></param>
        public void StopCoroutine(Coroutine routine);
        /// <summary>
        /// 停止所有协程。
        /// </summary>
        public void StopAllCoroutines( );

        /// <summary>
        /// 为外部提供Update监听。
        /// </summary>
        /// <param name="listener"></param>
        public void AddUpdateListener(Action listener);

        /// <summary>
        /// 为外部提供移除Update监听。
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveUpdateListener(Action listener);

        /// <summary>
        /// 为外部提供FixedUpdate监听。
        /// </summary>
        /// <param name="listener"></param>
        public void AddFixedUpdateListener(Action listener);

        /// <summary>
        /// 为外部提供移除FixedUpdate监听。
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveFixedUpdateListener(Action listener);

        /// <summary>
        /// 为外部提供LateUpdate监听。
        /// </summary>
        /// <param name="listener"></param>
        public void AddLateUpdateListener(Action listener);

        /// <summary>
        /// 为外部提供移除LateUpdate监听。
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveLateUpdateListener(Action listener);

        /// <summary>
        /// 为给外部提供的Destroy注册事件。
        /// </summary>
        /// <param name="action"></param>
        public void AddDestroyListener(Action action);

        /// <summary>
        /// 为给外部提供的Destroy反注册事件。
        /// </summary>
        /// <param name="action"></param>
        public void RemoveDestroyListener(Action action);

        /// <summary>
        /// 为给外部提供的OnDrawGizmos注册事件。
        /// </summary>
        /// <param name="action"></param>
        public void AddOnDrawGizmosListener(Action action);

        /// <summary>
        /// 为给外部提供的OnDrawGizmos反注册事件。
        /// </summary>
        /// <param name="action"></param>
        public void RemoveOnDrawGizmosListener(Action action);

        /// <summary>
        /// 为给外部提供的OnDrawGizmosSelected注册事件。
        /// </summary>
        /// <param name="action"></param>
        public void AddOnDrawGizmosSelectedListener(Action action);

        /// <summary>
        /// 为给外部提供的OnDrawGizmosSelected反注册事件。
        /// </summary>
        /// <param name="action"></param>
        public void RemoveOnDrawGizmosSelectedListener(Action action);

        /// <summary>
        /// 为给外部提供的OnApplicationPause注册事件。
        /// </summary>
        /// <param name="action"></param>
        public void AddOnApplicationPauseListener(Action<bool> action);

        /// <summary>
        /// 为给外部提供的OnApplicationPause反注册事件。
        /// </summary>
        /// <param name="action"></param>
        public void RemoveOnApplicationPauseListener(Action<bool> action);

    }
}
