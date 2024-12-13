using UnityEngine;

namespace Hian.Logger
{
    /// <summary>
    /// 조건부 로깅 기능을 구현하는 클래스입니다.
    /// </summary>
    public class ConditionalLogger : IConditionalLogger
    {
        private static readonly UnityEngine.ILogger _originalUnityLogger = Debug.unityLogger;
        private bool _isEditorOrDevelopmentBuild = false;

        public ConditionalLogger(bool? isEditorOrDevelopmentBuild = null)
        {
            _isEditorOrDevelopmentBuild = isEditorOrDevelopmentBuild ?? 
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                true;
                #else
                false;
                #endif
        }

        /// <summary>
        /// 조건부 디버그 로그를 출력합니다.
        /// </summary>
        /// <param name="message">로그 메시지</param>
        public void LogConditionalDebug(string message)
        {
            if (_isEditorOrDevelopmentBuild)
            {
                _originalUnityLogger.Log(LogType.Log, message);
            }
        }

        /// <summary>
        /// 조건부 디버그 경고 로그를 출력합니다.
        /// </summary>
        /// <param name="message">로그 메시지</param>
        public void LogConditionalDebugWarning(string message)
        {
            if (_isEditorOrDevelopmentBuild)
            {
                _originalUnityLogger.Log(LogType.Warning, message);
            }
        }

        /// <summary>
        /// 조건부 디버그 에러 로그를 출력합니다.
        /// </summary>
        /// <param name="message">로그 메시지</param>
        public void LogConditionalDebugError(string message)
        {
            if (_isEditorOrDevelopmentBuild)
            {
                _originalUnityLogger.Log(LogType.Error, message);
            }
        }
    }
}