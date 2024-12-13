using System;
using System.Diagnostics;

namespace Hian.Logger
{
    /// <summary>
    /// 조건부 로깅 기능을 제공하는 인터페이스입니다.
    /// </summary>
    public interface IConditionalLogger
    {
        /// <summary>
        /// 조건부 디버그 로그를 출력합니다.
        /// </summary>
        /// <param name="message">로그 메시지</param>
        void LogConditionalDebug(string message);

        /// <summary>
        /// 조건부 디버그 경고 로그를 출력합니다.
        /// </summary>
        /// <param name="message">로그 메시지</param>
        void LogConditionalDebugWarning(string message);

        /// <summary>
        /// 조건부 디버그 에러 로그를 출력합니다.
        /// </summary>
        /// <param name="message">로그 메시지</param>
        void LogConditionalDebugError(string message);
    }
}
