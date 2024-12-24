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

        /// <summary>
        /// 조건이 false인 경우 에러를 기록합니다.
        /// </summary>
        /// <param name="condition">검사할 조건</param>
        /// <param name="message">조건이 false일 때 기록할 메시지</param>
        void Assert(bool condition, string message);

        /// <summary>
        /// 조건이 false인 경우 에러를 기록하고 예외를 발생시킵니다.
        /// </summary>
        /// <param name="condition">검사할 조건</param>
        /// <param name="message">조건이 false일 때 기록할 메시지</param>
        /// <exception cref="System.Exception">조건이 false인 경우 발생</exception>
        void AssertOrThrow(bool condition, string message);
    }
}
