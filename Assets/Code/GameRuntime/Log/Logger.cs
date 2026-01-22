using OriginRuntime;
using UnityEngine;
namespace RuntimeLogic
{
    /// <summary>
    /// 日志
    /// </summary>
    public sealed class Logger:GameFrameworkLog.ILogHelper
    {
        public void Log(GameFrameworkLogLevel level , object message)
        {
            switch(level)
            {
                case GameFrameworkLogLevel.Debug:
                    Debug.Log(Utility.Text.Format("<color=#00F5FF><App>--{0}</color>" , message.ToString( )));
                    break;
                case GameFrameworkLogLevel.Info:
                    Debug.Log(Utility.Text.Format("<color=#FFDAB9><App>--{0}</color>" , message.ToString( )));
                    break;
                case GameFrameworkLogLevel.Warning:
                    Debug.LogWarning(Utility.Text.Format("<App>--{0}" , message.ToString( )));
                    break;
                case GameFrameworkLogLevel.Error:
                    Debug.LogError(Utility.Text.Format("<App>--{0}" , message.ToString( )));
                    break;
                default:
                    throw new GameFrameworkException(message.ToString( ));
            }
        }
    }
}
