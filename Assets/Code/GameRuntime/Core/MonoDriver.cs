using System;
using UnityEngine;
using OriginRuntime;
using System.Collections;
using System.Diagnostics;
using UnityEngine.Internal;

namespace RuntimeLogic
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MonoDriver:IMonoBehaviourDriver, ISystemCore
    {
        private GameObject m_Mono;

        private MonoBehavieDriver m_Driver;
        public int Priority => 0;

        public void InitSystem( )
        {
            InternalInspection( );
        }
        public void ShutdownSystem( )
        {
            if(m_Driver != null)
                m_Driver.Release( );
            if(m_Mono != null)
                UnityEngine.Object.Destroy(m_Mono);
            m_Driver = null;
            m_Mono = null;
        }

        public void AddDestroyListener(Action action)
        {
            InternalInspection( );
            m_Driver.AddDestroyListener(action);
        }

        public void AddFixedUpdateListener(Action listener)
        {
            InternalInspection( );
            m_Driver.AddFixedUpdateListener(listener);
        }

        public void AddLateUpdateListener(Action listener)
        {
            InternalInspection( );
            m_Driver.AddLateUpdateListener(listener);
        }

        public void AddOnApplicationPauseListener(Action<bool> action)
        {
            InternalInspection( );
            m_Driver.AddOnApplicationPauseListener(action);
        }

        public void AddOnDrawGizmosListener(Action action)
        {
            InternalInspection( );
            m_Driver.AddOnDrawGizmosListener(action);
        }

        public void AddOnDrawGizmosSelectedListener(Action action)
        {
            InternalInspection( );
            m_Driver.AddOnDrawGizmosSelectedListener(action);
        }

        public void AddUpdateListener(Action listener)
        {
            InternalInspection( );
            m_Driver.AddUpdateListener(listener);
        }



        public void RemoveDestroyListener(Action action)
        {
            InternalInspection( );
            m_Driver.RemoveDestroyListener(action);
        }

        public void RemoveFixedUpdateListener(Action listener)
        {
            InternalInspection( );
            m_Driver.RemoveFixedUpdateListener(listener);
        }

        public void RemoveLateUpdateListener(Action listener)
        {
            InternalInspection( );
            m_Driver.RemoveLateUpdateListener(listener);
        }

        public void RemoveOnApplicationPauseListener(Action<bool> action)
        {
            InternalInspection( );
            m_Driver.RemoveOnApplicationPauseListener(action);
        }

        public void RemoveOnDrawGizmosListener(Action action)
        {
            InternalInspection( );
            m_Driver.RemoveOnDrawGizmosListener(action);
        }

        public void RemoveOnDrawGizmosSelectedListener(Action action)
        {
            InternalInspection( );
            m_Driver.RemoveOnDrawGizmosSelectedListener(action);
        }

        public void RemoveUpdateListener(Action listener)
        {
            InternalInspection( );
            m_Driver.RemoveUpdateListener(listener);
        }
        public Coroutine StartCoroutine(string methodName)
        {
            if(string.IsNullOrEmpty(methodName))
                return null;
            InternalInspection( );

            return m_Driver.StartCoroutine(methodName);
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            if(routine == null)
                return null;
            InternalInspection( );
            return m_Driver.StartCoroutine(routine);
        }

        public Coroutine StartCoroutine(string methodName , [DefaultValue("null")] object value)
        {
            if(string.IsNullOrEmpty(methodName))
                return null;
            InternalInspection( );
            return m_Driver.StartCoroutine(methodName , value);
        }

        public void StopAllCoroutines( )
        {
            if(m_Driver != null)
                m_Driver.StopAllCoroutines( );
        }

        public void StopCoroutine(string methodName)
        {
            if(string.IsNullOrEmpty(methodName))
                return;
            InternalInspection( );
            if(m_Driver != null)
                m_Driver.StopCoroutine(methodName);
        }

        public void StopCoroutine(IEnumerator routine)
        {
            if(routine == null)
                return;
            InternalInspection( );
            if(m_Driver != null)
                m_Driver.StopCoroutine(routine);
        }

        public void StopCoroutine(Coroutine routine)
        {
            if(routine == null)
                return;
            InternalInspection( );
            if(m_Driver != null)
                m_Driver.StopCoroutine(routine);
        }

        private void InternalInspection( )
        {
            if(m_Mono != null)
                return;

            m_Mono = new GameObject("[MonoDriver]");
            m_Mono.SetActive(true);
            m_Driver = m_Mono.AddComponent<MonoBehavieDriver>( );
            UnityEngine.Object.DontDestroyOnLoad(m_Mono);
        }

        private class MonoBehavieDriver:MonoBehaviour
        {
            private event Action UpdateEvent;
            private event Action FixedUpdateEvent;
            private event Action LateUpdateEvent;
            private event Action DestroyEvent;
            private event Action OnDrawGizmosEvent;
            private event Action OnDrawGizmosSelectedEvent;
            private event Action<bool> OnApplicationPauseEvent;

            private void Update( )
            {
                UpdateEvent?.Invoke( );
            }

            private void FixedUpdate( )
            {
                FixedUpdateEvent?.Invoke( );
            }
            private void LateUpdate( )
            {
                LateUpdateEvent?.Invoke( );
            }

            private void OnDestroy( )
            {
                DestroyEvent?.Invoke( );
            }

            [Conditional("UNITY_EDITOR")]
            private void OnDrawGizmos( )
            {
                OnDrawGizmosEvent?.Invoke( );
            }

            [Conditional("UNITY_EDITOR")]
            private void OnDrawGizmosSelected( )
            {
                OnDrawGizmosSelectedEvent?.Invoke( );
            }
            private void OnApplicationPause(bool pauseStatus)
            {
                OnApplicationPauseEvent?.Invoke(pauseStatus);
            }
            public void AddLateUpdateListener(Action action)
            {
                LateUpdateEvent += action;
            }

            public void RemoveLateUpdateListener(Action action)
            {
                LateUpdateEvent -= action;
            }

            public void AddFixedUpdateListener(Action action)
            {
                FixedUpdateEvent += action;
            }

            public void RemoveFixedUpdateListener(Action action)
            {
                FixedUpdateEvent -= action;
            }

            public void AddUpdateListener(Action action)
            {
                UpdateEvent += action;
            }

            public void RemoveUpdateListener(Action action)
            {
                UpdateEvent -= action;
            }

            public void AddDestroyListener(Action action)
            {
                DestroyEvent += action;
            }

            public void RemoveDestroyListener(Action action)
            {
                DestroyEvent -= action;
            }

            [Conditional("UNITY_EDITOR")]
            public void AddOnDrawGizmosListener(Action action)
            {
                OnDrawGizmosEvent += action;
            }

            [Conditional("UNITY_EDITOR")]
            public void RemoveOnDrawGizmosListener(Action action)
            {
                OnDrawGizmosEvent -= action;
            }

            [Conditional("UNITY_EDITOR")]
            public void AddOnDrawGizmosSelectedListener(Action action)
            {
                OnDrawGizmosSelectedEvent += action;
            }

            [Conditional("UNITY_EDITOR")]
            public void RemoveOnDrawGizmosSelectedListener(Action action)
            {
                OnDrawGizmosSelectedEvent -= action;
            }

            public void AddOnApplicationPauseListener(Action<bool> action)
            {
                OnApplicationPauseEvent += action;
            }

            public void RemoveOnApplicationPauseListener(Action<bool> action)
            {
                OnApplicationPauseEvent -= action;
            }

            public void Release( )
            {
                UpdateEvent = null;
                FixedUpdateEvent = null;
                LateUpdateEvent = null;
                OnDrawGizmosEvent = null;
                OnDrawGizmosSelectedEvent = null;
                DestroyEvent = null;
                OnApplicationPauseEvent = null;
            }
        }
    }
}
