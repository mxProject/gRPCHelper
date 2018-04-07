using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;
using Grpc.Core.Logging;

namespace mxProject.Helpers.Grpc.Commons
{

    /// <summary>
    /// ログライター。
    /// </summary>
    public class GrpcLogWriter
    {

        #region コンストラクタ

        /// <summary>
        /// ログレベルを指定してインスタンスを生成します。
        /// </summary>
        /// <param name="level">出力対象のログレベル</param>
        public GrpcLogWriter(LogLevel level) : this(null, level, null)
        {
        }

        /// <summary>
        /// ログレベルとフォーマッターを指定してインスタンスを生成します。
        /// </summary>
        /// <param name="level">出力対象のログレベル</param>
        /// <param name="formatter">フォーマッター</param>
        public GrpcLogWriter(LogLevel level, FormatLogMessage formatter) : this(null, level, formatter)
        {
        }

        /// <summary>
        /// ロガーとログレベル・フォーマッターを指定してインスタンスを生成します。
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="level">出力対象のログレベル</param>
        /// <param name="formatter">フォーマッター</param>
        public GrpcLogWriter(ILogger logger, LogLevel level, FormatLogMessage formatter)
        {
            m_Logger = logger;
            m_LogLevel = level;
            m_Formatter = formatter ?? FormatDefaultMessage;
        }

        #endregion

        #region インスタンス

        /// <summary>
        /// ログの出力を行わないインスタンス。
        /// </summary>
        private static readonly GrpcLogWriter NullWriter = new GrpcLogWriter(new NullLogger(), LogLevel.Off, null);

        /// <summary>
        /// 既定のログ出力を行うインスタンス。
        /// </summary>
        private static readonly GrpcLogWriter DefaultDebugWriter = new GrpcLogWriter(LogLevel.Debug);

        /// <summary>
        /// 既定のインスタンスを取得します。
        /// </summary>
        public static GrpcLogWriter DefaultWriter
        {
            get { return s_DefaultWriter; }
        }

        /// <summary>
        /// 既定のインスタンスを設定します。
        /// </summary>
        /// <param name="writer"></param>
        public static void SetDefaultWriter(GrpcLogWriter writer)
        {
            s_DefaultWriter = writer ?? GrpcLogWriter.NullWriter;
        }

        private static GrpcLogWriter s_DefaultWriter = GrpcLogWriter.DefaultDebugWriter;

        /// <summary>
        /// 指定された型に対するログライターを取得します。
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <returns>ログライター</returns>
        public static GrpcLogWriter ForType<T>()
        {
            return ForType<T>(LogLevel.Debug, null);
        }

        /// <summary>
        /// 指定された型に対するログライターを取得します。
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <returns>ログライター</returns>
        /// <param name="level">出力対象のログレベル</param>
        public static GrpcLogWriter ForType<T>(LogLevel level)
        {
            return ForType<T>(level, null);
        }

        /// <summary>
        /// 指定された型に対するログライターを取得します。
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <returns>ログライター</returns>
        /// <param name="level">出力対象のログレベル</param>
        /// <param name="formatter">フォーマッター</param>
        public static GrpcLogWriter ForType<T>(LogLevel level, FormatLogMessage formatter)
        {
            return new GrpcLogWriter(GrpcEnvironment.Logger.ForType<T>(), level, formatter);
        }

        #endregion

        #region ロガー

        /// <summary>
        /// ロガーを取得します。
        /// </summary>
        public ILogger Logger
        {
            get { return m_Logger; }
        }
        private ILogger m_Logger;

        /// <summary>
        /// ロガーを取得します。
        /// </summary>
        /// <returns></returns>
        private ILogger GetLogger()
        {
            return m_Logger ?? GrpcEnvironment.Logger;
        }

        #endregion

        #region ログレベル

        /// <summary>
        /// 出力対象のログレベルを取得または設定します。
        /// </summary>
        public LogLevel LogLevel
        {
            get { return m_LogLevel; }
            set { m_LogLevel = value; }
        }
        private LogLevel m_LogLevel = LogLevel.Debug;

        /// <summary>
        /// ログを出力する必要があるかどうかを取得します。
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private bool NeedWrite(LogLevel level)
        {
            return (m_LogLevel<= level);
        }

        #endregion

        #region Debug

        /// <summary>
        /// デバッグログを出力します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        public void Debug(string message)
        {
            Write(LogLevel.Debug, message);
        }

        /// <summary>
        /// デバッグログを出力します。
        /// </summary>
        /// <param name="format">メッセージフォーマット</param>
        /// <param name="formatArgs">値</param>
        public void Debug(string format, params string[] formatArgs)
        {
            Write(LogLevel.Debug, format, formatArgs);
        }

        /// <summary>
        /// デバッグログを出力します。
        /// </summary>
        /// <param name="messageBuilder">メッセージ生成処理</param>
        public void Debug(Func<string> messageBuilder)
        {
            Write(LogLevel.Debug, messageBuilder);
        }

        /// <summary>
        /// デバッグログを出力します。
        /// </summary>
        /// <param name="log">ログ情報</param>
        public void Debug(ILogInfo log)
        {
            Write(LogLevel.Debug, log);
        }

        #endregion

        #region DebugAsync

        /// <summary>
        /// デバッグログを出力します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        public Task DebugAsync(string message)
        {
            return WriteAsync(LogLevel.Debug, message);
        }

        /// <summary>
        /// デバッグログを出力します。
        /// </summary>
        /// <param name="format">メッセージフォーマット</param>
        /// <param name="formatArgs">値</param>
        public Task DebugAsync(string format, params string[] formatArgs)
        {
            return WriteAsync(LogLevel.Debug, format, formatArgs);
        }

        /// <summary>
        /// デバッグログを出力します。
        /// </summary>
        /// <param name="messageBuilder">メッセージ生成処理</param>
        public Task DebugAsync(Func<string> messageBuilder)
        {
            return WriteAsync(LogLevel.Debug, messageBuilder);
        }

        /// <summary>
        /// デバッグログを出力します。
        /// </summary>
        /// <param name="log">ログ情報</param>
        public Task DebugAsync(ILogInfo log)
        {
            return WriteAsync(LogLevel.Debug, log);
        }

        #endregion

        #region Info

        /// <summary>
        /// 情報ログを出力します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        public void Info(string message)
        {
            Write(LogLevel.Info, message);
        }

        /// <summary>
        /// 情報ログを出力します。
        /// </summary>
        /// <param name="format">メッセージフォーマット</param>
        /// <param name="formatArgs">値</param>
        public void Info(string format, params string[] formatArgs)
        {
            Write(LogLevel.Info, format, formatArgs);
        }

        /// <summary>
        /// 情報ログを出力します。
        /// </summary>
        /// <param name="messageBuilder">メッセージ生成処理</param>
        public void Info(Func<string> messageBuilder)
        {
            Write(LogLevel.Info, messageBuilder);
        }

        /// <summary>
        /// 情報ログを出力します。
        /// </summary>
        /// <param name="log">ログ情報</param>
        public void Info(ILogInfo log)
        {
            Write(LogLevel.Info, log);
        }

        #endregion

        #region InfoAsync

        /// <summary>
        /// 情報ログを出力します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        public Task InfoAsync(string message)
        {
            return WriteAsync(LogLevel.Info, message);
        }

        /// <summary>
        /// 情報ログを出力します。
        /// </summary>
        /// <param name="format">メッセージフォーマット</param>
        /// <param name="formatArgs">値</param>
        public Task InfoAsync(string format, params string[] formatArgs)
        {
            return WriteAsync(LogLevel.Info, format, formatArgs);
        }

        /// <summary>
        /// 情報ログを出力します。
        /// </summary>
        /// <param name="messageBuilder">メッセージ生成処理</param>
        public Task InfoAsync(Func<string> messageBuilder)
        {
            return WriteAsync(LogLevel.Info, messageBuilder);
        }

        /// <summary>
        /// 情報ログを出力します。
        /// </summary>
        /// <param name="log">ログ情報</param>
        public Task InfoAsync(ILogInfo log)
        {
            return WriteAsync(LogLevel.Info, log);
        }

        #endregion

        #region Warning

        /// <summary>
        /// 警告ログを出力します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        public void Warning(string message)
        {
            Write(LogLevel.Warning, message);
        }

        /// <summary>
        /// 警告ログを出力します。
        /// </summary>
        /// <param name="format">メッセージフォーマット</param>
        /// <param name="formatArgs">値</param>
        public void Warning(string format, params string[] formatArgs)
        {
            Write(LogLevel.Warning, format, formatArgs);
        }

        /// <summary>
        /// 警告ログを出力します。
        /// </summary>
        /// <param name="messageBuilder">メッセージ生成処理</param>
        public void Warning(Func<string> messageBuilder)
        {
            Write(LogLevel.Warning, messageBuilder);
        }

        /// <summary>
        /// 警告ログを出力します。
        /// </summary>
        /// <param name="log">ログ情報</param>
        public void Warning(ILogInfo log)
        {
            Write(LogLevel.Warning, log);
        }

        #endregion

        #region WarningAsync

        /// <summary>
        /// 警告ログを出力します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        public Task WarningAsync(string message)
        {
            return WriteAsync(LogLevel.Warning, message);
        }

        /// <summary>
        /// 警告ログを出力します。
        /// </summary>
        /// <param name="format">メッセージフォーマット</param>
        /// <param name="formatArgs">値</param>
        public Task WarningAsync(string format, params string[] formatArgs)
        {
            return WriteAsync(LogLevel.Warning, format, formatArgs);
        }

        /// <summary>
        /// 警告ログを出力します。
        /// </summary>
        /// <param name="messageBuilder">メッセージ生成処理</param>
        public Task WarningAsync(Func<string> messageBuilder)
        {
            return WriteAsync(LogLevel.Warning, messageBuilder);
        }

        /// <summary>
        /// 警告ログを出力します。
        /// </summary>
        /// <param name="log">ログ情報</param>
        public Task WarningAsync(ILogInfo log)
        {
            return WriteAsync(LogLevel.Warning, log);
        }

        #endregion

        #region Error

        /// <summary>
        /// エラーログを出力します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        public void Error(string message)
        {
            Write(LogLevel.Error, message);
        }

        /// <summary>
        /// エラーログを出力します。
        /// </summary>
        /// <param name="format">メッセージフォーマット</param>
        /// <param name="formatArgs">値</param>
        public void Error(string format, params string[] formatArgs)
        {
            Write(LogLevel.Error, format, formatArgs);
        }

        /// <summary>
        /// エラーログを出力します。
        /// </summary>
        /// <param name="messageBuilder">メッセージ生成処理</param>
        public void Error(Func<string> messageBuilder)
        {
            Write(LogLevel.Error, messageBuilder);
        }

        /// <summary>
        /// エラーログを出力します。
        /// </summary>
        /// <param name="log">ログ情報</param>
        public void Error(ILogInfo log)
        {
            Write(LogLevel.Error, log);
        }

        #endregion

        #region ErrorAsync

        /// <summary>
        /// エラーログを出力します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        public Task ErrorAsync(string message)
        {
            return WriteAsync(LogLevel.Error, message);
        }

        /// <summary>
        /// エラーログを出力します。
        /// </summary>
        /// <param name="format">メッセージフォーマット</param>
        /// <param name="formatArgs">値</param>
        public Task ErrorAsync(string format, params string[] formatArgs)
        {
            return WriteAsync(LogLevel.Error, format, formatArgs);
        }

        /// <summary>
        /// エラーログを出力します。
        /// </summary>
        /// <param name="messageBuilder">メッセージ生成処理</param>
        public Task ErrorAsync(Func<string> messageBuilder)
        {
            return WriteAsync(LogLevel.Error, messageBuilder);
        }

        /// <summary>
        /// エラーログを出力します。
        /// </summary>
        /// <param name="log">ログ情報</param>
        public Task ErrorAsync(ILogInfo log)
        {
            return WriteAsync(LogLevel.Error, log);
        }

        #endregion

        #region Core

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="message">メッセージ</param>
        private void Write(LogLevel level, string message)
        {
            if (!NeedWrite(level)) { return; }
            WriteCore(level, GetNextSequence(), DateTime.Now, message);
        }

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="format">メッセージフォーマット</param>
        /// <param name="formatArgs">値</param>
        private void Write(LogLevel level, string format, params string[] formatArgs)
        {
            if (!NeedWrite(level)) { return; }
            WriteCore(level, GetNextSequence(), DateTime.Now, string.Format(format, formatArgs));
        }

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="messageBuilder">メッセージ生成処理</param>
        private void Write(LogLevel level, Func<string> messageBuilder)
        {
            if (!NeedWrite(level)) { return; }
            WriteCore(level, GetNextSequence(), DateTime.Now, messageBuilder());
        }

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="log">ログ情報</param>
        private void Write(LogLevel level, ILogInfo log)
        {
            if (!NeedWrite(level)) { return; }
            WriteCore(level, GetNextSequence(), DateTime.Now, log.BuildMessage());
        }

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="logSequence">シーケンス</param>
        /// <param name="logTime">ログ発生時刻</param>
        /// <param name="message">メッセージ</param>
        private void WriteCore(LogLevel level, int logSequence, DateTime logTime, string message)
        {

            if (!NeedWrite(level)) { return; }

            switch (level)
            {
                case LogLevel.Debug:
                    GetLogger().Debug(FormatMessage(logSequence, logTime, message));
                    break;

                case LogLevel.Info:
                    GetLogger().Info(FormatMessage(logSequence, logTime, message));
                    break;

                case LogLevel.Warning:
                    GetLogger().Warning(FormatMessage(logSequence, logTime, message));
                    break;

                case LogLevel.Error:
                    GetLogger().Error(FormatMessage(logSequence, logTime, message));
                    break;
            }

        }

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="message">メッセージ</param>
        private async Task WriteAsync(LogLevel level, string message)
        {
            if (!NeedWrite(level)) { return; }
            await WriteAsyncCore(level, GetNextSequence(), DateTime.Now, message);
        }

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="format">メッセージフォーマット</param>
        /// <param name="formatArgs">値</param>
        private async Task WriteAsync(LogLevel level, string format, params string[] formatArgs)
        {
            if (!NeedWrite(level)) { return; }
            await WriteAsyncCore(level, GetNextSequence(), DateTime.Now, string.Format(format, formatArgs));
        }

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="messageBuilder">メッセージ生成処理</param>
        private async Task WriteAsync(LogLevel level, Func<string> messageBuilder)
        {
            if (!NeedWrite(level)) { return; }
            await WriteAsyncCore(level, GetNextSequence(), DateTime.Now, messageBuilder());
        }

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="log">ログ情報</param>
        private async Task WriteAsync(LogLevel level, ILogInfo log)
        {
            if (!NeedWrite(level)) { return; }
            await WriteAsyncCore(level, GetNextSequence(), DateTime.Now, log.BuildMessage());
        }

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="logSequence">シーケンス</param>
        /// <param name="logTime">ログ発生時刻</param>
        /// <param name="message">メッセージ</param>
        private async Task WriteAsyncCore(LogLevel level, int logSequence, DateTime logTime, string message)
        {

            await Task.Yield();

            WriteCore(level, logSequence, logTime, message);

        }

        private static int s_Sequence;
        private static readonly object s_Lock = new object();

        /// <summary>
        /// 次のシーケンスを取得します。
        /// </summary>
        /// <returns></returns>
        private static int GetNextSequence()
        {
            lock (s_Lock)
            {
                if (s_Sequence == Int32.MaxValue)
                {
                    s_Sequence = 0;
                }
                return ++s_Sequence;
            }
        }

        #endregion

        #region フォーマット

        private FormatLogMessage m_Formatter;

        /// <summary>
        /// メッセージをフォーマットします。
        /// </summary>
        /// <param name="logSequence"></param>
        /// <param name="logTime"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private string FormatMessage(int logSequence, DateTime logTime, string message)
        {
            return m_Formatter(logSequence, logTime, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logSequence"></param>
        /// <param name="logTime"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string FormatDefaultMessage(int logSequence, DateTime logTime, string message)
        {
            return string.Format("{0}, {1}, {2}", logSequence, logTime, message);
        }

        #endregion

    }

}
