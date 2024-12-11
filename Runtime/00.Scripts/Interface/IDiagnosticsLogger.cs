using System.Diagnostics;

namespace Hian.Logger
{
    /// <summary>
    /// System.Diagnostics 기반의 로깅 기능을 정의하는 인터페이스입니다.
    /// </summary>
    public interface IDiagnosticsLogger : ILogger
    {
        /// <summary>
        /// 진단 로거를 초기화합니다.
        /// </summary>
        /// <param name="sourceName">로그 소스 이름</param>
        void Initialize(string sourceName);

        /// <summary>
        /// 사용자 정의 리스너를 추가합니다.
        /// </summary>
        /// <param name="listener">추가할 TraceListener</param>
        void AddListener(TraceListener listener);

        /// <summary>
        /// 지정된 리스너를 제거합니다.
        /// </summary>
        /// <param name="listener">제거할 TraceListener</param>
        void RemoveListener(TraceListener listener);

        /// <summary>
        /// 로거의 리소스를 정리합니다.
        /// </summary>
        void Cleanup();
    }
} 