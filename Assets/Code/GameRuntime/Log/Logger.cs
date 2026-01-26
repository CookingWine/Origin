using OriginRuntime;
using System;
using UnityEngine;
namespace RuntimeLogic
{
    /// <summary>
    /// 日志
    /// </summary>
    public sealed class Logger:GameFrameworkLog.ILogHelper, YooAsset.ILogger
    {
        public void Log(string message)
        {
            Log(GameFrameworkLogLevel.Info , message);
        }

        public void Warning(string message)
        {
            Log(GameFrameworkLogLevel.Warning , message);
        }
        public void Error(string message)
        {
            Log(GameFrameworkLogLevel.Error , message);
        }

        public void Exception(Exception exception)
        {
            Log(GameFrameworkLogLevel.Fatal , exception.Message);
        }

        public void Log(GameFrameworkLogLevel level , object message)
        {
            switch(level)
            {
                case GameFrameworkLogLevel.Debug:
                    Debug.Log(Utility.Text.Format("<color=#00F5FF><{0}>--{1}</color>" , Application.productName , message.ToString( )));
                    break;
                case GameFrameworkLogLevel.Info:
                    Debug.Log(Utility.Text.Format("<color=#FFDAB9><{0}>--{1}</color>" , Application.productName , message.ToString( )));
                    break;
                case GameFrameworkLogLevel.Warning:
                    Debug.LogWarning(Utility.Text.Format("<{0}>--{1}" , Application.productName , message.ToString( )));
                    break;
                case GameFrameworkLogLevel.Error:
                    Debug.LogError(Utility.Text.Format("<{0}>--{1}" , Application.productName , message.ToString( )));
                    break;
                default:
                    throw new GameFrameworkException(message.ToString( ));
            }
        }


    }
}
