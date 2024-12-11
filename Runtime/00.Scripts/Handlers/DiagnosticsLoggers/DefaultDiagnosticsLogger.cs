using System.Diagnostics;

namespace Hian.Logger.Handlers.DiagnosticsLoggers
{
    /// <summary>
    /// System.Diagnostics 기반 로거의 기본 구현 클래스입니다.
    /// 날짜별 로그 파일을 생성하고 시간과 스레드 정보를 포함합니다.
    /// </summary>
    public class DefaultDiagnosticsLogger : BaseDiagnosticsLogger
    {
        /// <summary>
        /// 기본 리스너를 구성합니다.
        /// 날짜별 로그 파일을 생성하고 시간과 스레드 ID를 포함하도록 설정합니다.
        /// </summary>
        protected override void ConfigureDefaultListeners()
        {
            base.ConfigureDefaultListeners();

            var textListener = new TextWriterTraceListener(
                $"SystemDiagnostics_{System.DateTime.Now:yyyy-MM-dd}.log");
            textListener.TraceOutputOptions = TraceOptions.DateTime | TraceOptions.ThreadId;
            TraceSource.Listeners.Add(textListener);
        }
    }
} 