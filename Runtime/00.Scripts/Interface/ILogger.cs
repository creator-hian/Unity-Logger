namespace Hian.Logger
{
    /// <summary>
    /// 기본 로깅 기능을 정의하는 인터페이스입니다.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 일반 로그 메시지를 기록합니다.
        /// </summary>
        /// <param name="message">로그 메시지</param>
        void Log(string message);

        /// <summary>
        /// 경고 메시지를 기록합니다.
        /// </summary>
        /// <param name="message">경고 메시지</param>
        void LogWarning(string message);

        /// <summary>
        /// 에러 메시지를 기록합니다.
        /// </summary>
        /// <param name="message">에러 메시지</param>
        void LogError(string message);

        /// <summary>
        /// 예외를 기록합니다.
        /// </summary>
        /// <param name="exception">기록할 예외</param>
        void LogException(System.Exception exception);
    }
} 