using System;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
namespace RuntimeLogic
{
    /// <summary>
    /// 自定义loop
    /// </summary>
    internal static class CustomPlayerLoop
    {
        private struct UpdateMarker { }

        private struct LateUpdateMarker { }

        private struct FixedUpdateMarker { }

        /// <summary>
        /// 每帧逻辑
        /// </summary>
        public static Action OnCustomUpdate;
        /// <summary>
        /// 帧尾逻辑
        /// </summary>
        public static Action OnCustomLateUpdate;
        /// <summary>
        /// 物理帧逻辑
        /// </summary>
        public static Action OnCustomFixedUpdate;

        private static bool _inUpdate;
        private static bool _inLateUpdate;
        private static bool _inFixedUpdate;

        /// <summary>
        /// 创建自定义循环
        /// </summary>
        public static void CreateCustomPlayerLoop( )
        {
            PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop( );

            //注入到Update.ScriptRunBehaviourUpdate
            loop = InsertSystemIntoSubSystem(loop ,
                typeof(Update) ,
                typeof(Update.ScriptRunBehaviourUpdate) ,
                CreateCustomSystem(typeof(UpdateMarker) , CustomUpdate) ,
                insertAfterTarget: false);

            loop = InsertSystemIntoSubSystem(loop ,
                typeof(PreLateUpdate) ,
                typeof(PreLateUpdate.ScriptRunBehaviourLateUpdate) ,
                CreateCustomSystem(typeof(LateUpdateMarker) , CustomLateUpdate) ,
                insertAfterTarget: false);

            loop = InsertSystemIntoSubSystem(loop ,
                typeof(FixedUpdate) ,
                typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate) ,
                CreateCustomSystem(typeof(FixedUpdateMarker) , CustomFixedUpdate) ,
                insertAfterTarget: false);

            PlayerLoop.SetPlayerLoop(loop);
        }

        /// <summary>
        /// 卸载自定义循环
        /// </summary>
        public static void UnCustomPlayerLoop(bool releaseEvent = true)
        {
            PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop( );

            loop = RemoveSystemFromSubSystem(loop , typeof(Update) , typeof(UpdateMarker));
            loop = RemoveSystemFromSubSystem(loop , typeof(PreLateUpdate) , typeof(LateUpdateMarker));
            loop = RemoveSystemFromSubSystem(loop , typeof(FixedUpdate) , typeof(FixedUpdateMarker));

            PlayerLoop.SetPlayerLoop(loop);
            if(releaseEvent)
            {
                OnCustomUpdate = null;
                OnCustomLateUpdate = null;
                OnCustomFixedUpdate = null;
            }
        }

        private static PlayerLoopSystem CreateCustomSystem(Type type , PlayerLoopSystem.UpdateFunction function)
        {
            return new PlayerLoopSystem
            {
                type = type ,
                updateDelegate = function
            };
        }
        private static void CustomUpdate( )
        {
            if(_inUpdate) return; // 防止递归重入
            _inUpdate = true;
            try { OnCustomUpdate?.Invoke( ); }
            catch(Exception e) { UnityEngine.Debug.LogException(e); }
            finally { _inUpdate = false; }
        }

        private static void CustomLateUpdate( )
        {
            if(_inLateUpdate) return;
            _inLateUpdate = true;
            try { OnCustomLateUpdate?.Invoke( ); }
            catch(Exception e) { UnityEngine.Debug.LogException(e); }
            finally { _inLateUpdate = false; }
        }

        private static void CustomFixedUpdate( )
        {
            if(_inFixedUpdate) return;
            _inFixedUpdate = true;
            try { OnCustomFixedUpdate?.Invoke( ); }
            catch(Exception e) { UnityEngine.Debug.LogException(e); }
            finally { _inFixedUpdate = false; }
        }
        /// <summary>
        /// 将系统插入子系统
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parentType"></param>
        /// <param name="targetType"></param>
        /// <param name="systemToInsert"></param>
        /// <param name="insertAfterTarget"></param>
        /// <returns></returns>
        private static PlayerLoopSystem InsertSystemIntoSubSystem(PlayerLoopSystem root , Type parentType , Type targetType , PlayerLoopSystem systemToInsert , bool insertAfterTarget)
        {
            // 先找到 parent 节点
            if(!TryFindSystem(ref root , parentType , out var parentIndex , out var parentSystem))
                return root;

            var list = parentSystem.subSystemList ?? Array.Empty<PlayerLoopSystem>( );

            // 幂等：已经存在就不插
            if(ContainsSystem(list , systemToInsert.type))
                return root;

            // PlayerLoopSystem[]arget 节点位置（比如 ScriptRunBehaviourUpdate）
            var targetIndex = IndexOfSystem(list , targetType);

            // 如果找不到 target，就插到末尾（或你想要的位置）
            int insertIndex;
            if(targetIndex < 0)
            {
                insertIndex = list.Length;
            }
            else
            {
                insertIndex = insertAfterTarget ? targetIndex + 1 : targetIndex;
            }

            var newList = new PlayerLoopSystem[list.Length + 1];

            // [0..insertIndex-1]
            if(insertIndex > 0)
                Array.Copy(list , 0 , newList , 0 , insertIndex);

            // [insertIndex]
            newList[insertIndex] = systemToInsert;

            // [insertIndex+1 .. end]
            if(insertIndex < list.Length)
                Array.Copy(list , insertIndex , newList , insertIndex + 1 , list.Length - insertIndex);

            parentSystem.subSystemList = newList;
            root.subSystemList[parentIndex] = parentSystem;
            return root;
        }

        private static PlayerLoopSystem RemoveSystemFromSubSystem(PlayerLoopSystem root , Type parentType , Type markerType)
        {
            if(!TryFindSystem(ref root , parentType , out var parentIndex , out var parentSystem))
                return root;

            var list = parentSystem.subSystemList ?? Array.Empty<PlayerLoopSystem>( );
            var idx = IndexOfSystem(list , markerType);
            if(idx < 0) return root;

            var newList = new PlayerLoopSystem[list.Length - 1];
            if(idx > 0)
                Array.Copy(list , 0 , newList , 0 , idx);
            if(idx < list.Length - 1)
                Array.Copy(list , idx + 1 , newList , idx , list.Length - idx - 1);

            parentSystem.subSystemList = newList;
            root.subSystemList[parentIndex] = parentSystem;
            return root;
        }
        private static bool TryFindSystem(ref PlayerLoopSystem root , Type type , out int index , out PlayerLoopSystem system)
        {
            var list = root.subSystemList;
            if(list != null)
            {
                for(int i = 0; i < list.Length; i++)
                {
                    if(list[i].type == type)
                    {
                        index = i;
                        system = list[i];
                        return true;
                    }
                }
            }

            index = -1;
            system = default;
            return false;
        }
        private static bool ContainsSystem(PlayerLoopSystem[] list , Type type)
        {
            return IndexOfSystem(list , type) >= 0;
        }
        private static int IndexOfSystem(PlayerLoopSystem[] list , Type type)
        {
            for(int i = 0; i < list.Length; i++)
            {
                if(list[i].type == type)
                    return i;
            }
            return -1;
        }
    }
}
