namespace Hian.Logger
{
    /// <summary>
    /// 로그 핸들러를 생성하는 팩토리 인터페이스입니다.
    /// </summary>
    public interface ILogHandlerFactory
    {
        /// <summary>
        /// 로그 핸들러를 생성합니다.
        /// </summary>
        /// <param name="path">로그 파일 경로 (선택적)</param>
        /// <param name="enableConsoleOutput">Unity 콘솔 출력 활성화 여부</param>
        /// <returns>생성된 ILogHandler 인스턴스</returns>
        ILogHandler CreateHandler(string path = null, bool enableConsoleOutput = true);
    }
} 