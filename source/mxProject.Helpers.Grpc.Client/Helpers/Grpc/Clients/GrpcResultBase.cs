using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;

using mxProject.Helpers.Grpc.Commons;

namespace mxProject.Helpers.Grpc.Clients
{

    /// <summary>
    /// 実行結果の基底実装。
    /// </summary>
    public abstract class GrpcResultBase : IGrpcResult
    {

        #region コンストラクタ

        /// <summary>
        /// 呼び出しオブジェクトを指定してインスタンスを生成します。
        /// </summary>
        /// <param name="call">呼び出しオブジェクト</param>
        protected GrpcResultBase(IGrpcAsyncCall call)
        {
            m_Call = call;
        }

        /// <summary>
        /// 例外を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="exception">例外</param>
        protected GrpcResultBase(Exception exception)
        {
            m_Exception = exception;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        internal GrpcResultBase(GrpcResultBase source) 
        {
            m_Call = source.m_Call;
            m_Status = source.m_Status;
            m_Trailers = source.m_Trailers;
            m_Exception = source.m_Exception;
        }

        #endregion

        private IGrpcAsyncCall m_Call;
        private Status? m_Status;
        private Metadata m_Trailers;
        private Exception m_Exception;

        #region ステータス

        /// <summary>
        /// ステータスを取得可能かどうかを取得します。
        /// </summary>
        public bool CanGetStatus
        {
            get
            {
                if (m_Status.HasValue) { return true; }
                if (m_Call == null) { return true; }
                return m_Call.IsRequestStreamCompleted && m_Call.IsEndResponse;
            }
        }

        /// <summary>
        /// ステータスを取得します。
        /// </summary>
        /// <returns></returns>
        private Status GetStatus()
        {

            if (m_Status.HasValue) { return m_Status.Value; }

            if (m_Call== null) { return Status.DefaultSuccess; }

            m_Status = m_Call.GetStatus();
            return m_Status.Value;

        }

        ///// <summary>
        ///// 実行中または OK かどうかを取得します。
        ///// </summary>
        //public bool IsRunningOrOK
        //{
        //    get
        //    {

        //        StatusCode? status = GetStatusCode();

        //        if (status.HasValue)
        //        {
        //            return (status.Value == StatusCode.OK);
        //        }
        //        else
        //        {
        //            return true;
        //        }

        //    }
        //}

        /// <summary>
        /// ステータスが OK かどうかを取得します。
        /// </summary>
        public bool IsOK
        {
            get
            {
                StatusCode? status = GetStatusCode();

                if (status.HasValue)
                {
                    return (status.Value == StatusCode.OK);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// キャンセルされたかどうかを取得します。
        /// </summary>
        public bool IsCanceled
        {
            get
            {
                StatusCode? status = GetStatusCode();

                if (status.HasValue)
                {
                    return (status.Value == StatusCode.Cancelled);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// エラーが発生したかどうかを取得します。
        /// </summary>
        public bool IsError
        {
            get
            {
                StatusCode? status = GetStatusCode();

                if (status.HasValue)
                {
                    return (status.Value != StatusCode.OK && status.Value != StatusCode.Cancelled);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// キャンセルまたはエラーが発生したかどうかを取得します。
        /// </summary>
        public bool IsCanceledOrError
        {
            get
            {
                return IsCanceled || IsError;
            }
        }

        /// <summary>
        /// ステータスコードを取得します。
        /// </summary>
        public StatusCode? GetStatusCode()
        {

            if (m_Exception == null)
            {
                if (CanGetStatus)
                {
                    return GetStatus().StatusCode;
                }
                else
                {
                    return null;
                }
            }
            else
            {

                RpcException rpc = m_Exception as RpcException;

                if (rpc == null)
                {
                    return StatusCode.Unknown;
                }
                else
                {
                    return rpc.Status.StatusCode;
                }

            }

        }

        /// <summary>
        /// ステータスの詳細を取得します。
        /// </summary>
        public string GetStatusDetail()
        {

            if (m_Exception == null)
            {
                if (CanGetStatus)
                {
                    return GetStatus().Detail;
                }
                else
                {
                    return null;
                }
            }
            else
            {

                RpcException rpc = m_Exception as RpcException;

                if (rpc == null)
                {
                    return m_Exception.Message;
                }
                else
                {
                    return rpc.Status.Detail;
                }

            }

        }

        #endregion

        #region トレーラー

        /// <summary>
        /// トレーラーを取得可能かどうかを取得します。
        /// </summary>
        public bool CanGetTrailers
        {
            get
            {
                if (m_Trailers != null) { return true; }
                if (m_Call == null) { return false; }
                return m_Call.IsRequestStreamCompleted && m_Call.IsEndResponse;
            }
        }

        /// <summary>
        /// トレーラーを取得します。
        /// </summary>
        public Metadata Trailers
        {
            get
            {

                if (m_Trailers != null) { return m_Trailers; }

                if (!CanGetTrailers) { return null; }

                if (m_Call == null) { return null; }

                var obj = m_Call.ResponseHeadersAsync.Result;

                m_Trailers = m_Call.GetTrailers();

                return m_Trailers;

            }
        }

        #endregion

        #region 例外

        /// <summary>
        /// 発生した例外を取得します。
        /// </summary>
        public Exception Exception
        {
            get { return m_Exception; }
        }

        #endregion

    }

}
