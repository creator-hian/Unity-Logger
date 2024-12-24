namespace Hian.Logger
{
    /// <summary>
    /// Unity의 ILogHandler를 확장한 로그 핸들러 인터페이스입니다.
    /// </summary>
    public interface ILogHandler : UnityEngine.ILogHandler
    {
        /// <summary>
        /// 로그 파일의 경로를 설정합니다.
        /// </summary>
        /// <param name="path">로그 파일 경로</param>
        void SetLogPath(string path);

        /// <summary>
        /// Unity 콘솔 출력을 활성화 또는 비활성화합니다.
        /// </summary>
        /// <param name="enable">활성화 여부</param>
        void EnableConsoleOutput(bool enable);

        /// <summary>
        /// 핸들러의 리소스를 정리합니다.
        /// </summary>
        void Cleanup();
    }
}
