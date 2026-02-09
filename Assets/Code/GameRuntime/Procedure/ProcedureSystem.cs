using System;
using OriginRuntime;

namespace RuntimeLogic
{
    /// <summary>
    /// 流程系统
    /// </summary>
    internal sealed class ProcedureSystem:ISystemCore, IProcedureSystem
    {
        private IFsmSystem m_FsmModule;
        private IFsm<IProcedureSystem> m_ProcedureFsm;

        public ProcedureSystem( )
        {
            m_FsmModule = null;
            m_ProcedureFsm = null;
        }

        public int Priority => -2;

        public ProcedureBase CurrentProcedure
        {
            get
            {
                if(m_ProcedureFsm == null)
                    throw new GameFrameworkException("You must initialize procedure system first.");
                return (ProcedureBase)m_ProcedureFsm.CurrentState;
            }
        }

        public float CurrentProcedureTime
        {
            get
            {
                if(m_ProcedureFsm == null)
                    throw new GameFrameworkException("You must initialize procedure system first.");
                return m_ProcedureFsm.CurrentStateTime;
            }
        }

        public void InitSystem( )
        {
            m_FsmModule = null;
            m_ProcedureFsm = null;
        }
        public void ShutdownSystem( )
        {
            if(m_FsmModule != null)
            {
                if(m_ProcedureFsm != null)
                {
                    m_FsmModule.DestroyFsm(m_ProcedureFsm);
                    m_ProcedureFsm = null;
                }
                m_FsmModule = null;
            }
        }
        public void Initialize(IFsmSystem fsmModule , params ProcedureBase[] procedures)
        {
            m_FsmModule = fsmModule ?? throw new GameFrameworkException("FSM module is invalid.");
            m_ProcedureFsm = m_FsmModule.CreateFsm(this , procedures);
        }

        public void StartProcedure(Type procedureType)
        {
            ChangeProcedure( );
            m_ProcedureFsm.Start(procedureType);
        }

        public void StartProcedure<T>( ) where T : ProcedureBase
        {
            ChangeProcedure( );
            m_ProcedureFsm.Start<T>( );
        }

        public bool RestartProcedure(params ProcedureBase[] procedures)
        {
            if(procedures == null || procedures.Length <= 0)
                throw new GameFrameworkException("Procedures is invalid.");

            if(!m_FsmModule.DestroyFsm<IProcedureSystem>( ))
                return false;
            Initialize(m_FsmModule , procedures);
            StartProcedure(procedures[0].GetType( ));
            return true;
        }

        public ProcedureBase GetProcedure<T>( ) where T : ProcedureBase
        {
            ChangeProcedure( );
            return m_ProcedureFsm.GetState<T>( );
        }

        public ProcedureBase GetProcedure(Type procedureType)
        {
            ChangeProcedure( );
            return (ProcedureBase)m_ProcedureFsm.GetState(procedureType);
        }

        public bool HasProcedure<T>( ) where T : ProcedureBase
        {
            ChangeProcedure( );
            return m_ProcedureFsm.HasState<T>( );
        }

        public bool HasProcedure(Type procedureType)
        {
            ChangeProcedure( );
            return m_ProcedureFsm.HasState(procedureType);
        }

        private void ChangeProcedure( )
        {
            if(m_ProcedureFsm == null)
                throw new GameFrameworkException("You must initialize procedure system first.");
        }
    }
}
