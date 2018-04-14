using System;
using System.Collections.Generic;
using System.Text;

namespace mxProject.Helpers.Grpc.Commons
{

    /// <summary>
    /// ログ情報に必要な機能を提供します。
    /// </summary>
    public interface ILogInfo
    {

        /// <summary>
        /// ログメッセージを生成します。
        /// </summary>
        /// <returns></returns>
        string BuildMessage();

    }

}
