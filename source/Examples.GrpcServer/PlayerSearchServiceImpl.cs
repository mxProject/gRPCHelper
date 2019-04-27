using System;
using System.Collections.Generic;
using System.Text;

using Grpc.Core;
using Grpc.Core.Logging;

using mxProject.Helpers.Grpc.Commons;
using mxProject.Helpers.Grpc.Servers;

using Examples.GrpcModels;
using System.Threading.Tasks;

namespace Examples.GrpcServer
{

    /// <summary>
    /// 
    /// </summary>
    [GrpcPerformanceNotify(true)]
    internal class PlayerSearchServiceImpl : PlayerSearch.PlayerSearchBase
    {

        static readonly ILogger s_Logger = GrpcEnvironment.Logger.ForType<PlayerSearchServiceImpl>();
        static readonly int s_FetchSize = 10;
        static readonly int s_DelayMilliseconds = 200;

        #region Unary method examle

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [GrpcPerformanceNotify(false)]
        public override Task<TeamSearchResponse> SearchTeam(TeamSearchRequest request, ServerCallContext context)
        {

            string callCounter = GetCallCounter(context);

            s_Logger.Info(string.Format("[{0}] Requested {1} teams.", callCounter, request.ExpectedDataCount));

            IDictionary<string, Team> teams = SampleDataRepository.GetTeams(request.ExpectedDataCount);

            TeamSearchResponse response = new TeamSearchResponse();

            response.Teams.AddRange(teams.Values);

            return Task.FromResult(response);

        }

        #endregion

        #region ServerStream method examle

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task SearchPlayer_ServerStream(PlayerSearchRequest request, IServerStreamWriter<PlayerSearchResponse> responseStream, ServerCallContext context)
        {

            string callCounter = GetCallCounter(context);

            s_Logger.Info(string.Format("[{0}] Requested {1} players.", callCounter, request.ExpectedDataCount));

            IDictionary<string, Team> teams = SampleDataRepository.GetTeams(10);

            PlayerSearchResponse response = new PlayerSearchResponse();

            int fetchCount = 0;

            foreach (Player player in SampleDataRepository.GeneratePlayers(1, request.ExpectedDataCount, teams))
            {

                if (ExitIfRequestedCancel(context)) { return; }

                response.Players.Add(player);

                if (!response.Teams.ContainsKey(player.TeamCode))
                {
                    response.Teams.Add(player.TeamCode, teams[player.TeamCode]);
                }

                ++fetchCount;

                if (fetchCount == s_FetchSize)
                {
                    await Task.Delay(s_DelayMilliseconds).ConfigureAwait(false);
                    await responseStream.WriteAsync(response).ConfigureAwait(false);

                    response.Players.Clear();
                    response.Teams.Clear();
                    fetchCount = 0;
                }

            }

            if (response.Players.Count > 0)
            {
                await Task.Delay(s_DelayMilliseconds).ConfigureAwait(false);
                await responseStream.WriteAsync(response).ConfigureAwait(false);
            }

        }

        #endregion

        #region ClientStream method examle

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<PlayerSearchResponse> SearchPlayer_ClientStream(IAsyncStreamReader<PlayerSearchRequest> requestStream, ServerCallContext context)
        {

            string callCounter = GetCallCounter(context);

            IDictionary<string, Team> teams = SampleDataRepository.GetTeams(10);

            PlayerSearchResponse response = new PlayerSearchResponse();

            int initial = 1;

            while (await requestStream.MoveNext().ConfigureAwait(false))
            {

                ThrowExceptionIfRequestedCancel(context);

                PlayerSearchRequest request = requestStream.Current;

                s_Logger.Info(string.Format("[{0}] Requested {1} players.", callCounter, request.ExpectedDataCount));

                foreach (Player player in SampleDataRepository.GeneratePlayers(initial, request.ExpectedDataCount, teams))
                {

                    ThrowExceptionIfRequestedCancel(context);

                    response.Players.Add(player);

                    if (!response.Teams.ContainsKey(player.TeamCode))
                    {
                        response.Teams.Add(player.TeamCode, teams[player.TeamCode]);
                    }

                }

                initial += request.ExpectedDataCount;

            }

            return response;

        }

        #endregion

        #region DuplexStream method examle

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task SearchPlayer_DuplexStream(IAsyncStreamReader<PlayerSearchRequest> requestStream, IServerStreamWriter<PlayerSearchResponse> responseStream, ServerCallContext context)
        {

            string callCounter = GetCallCounter(context);

            IDictionary<string, Team> teams = SampleDataRepository.GetTeams(10);

            int initial = 1;

            while (await requestStream.MoveNext().ConfigureAwait(false))
            {

                PlayerSearchResponse response = new PlayerSearchResponse();

                if (ExitIfRequestedCancel(context)) { return; }

                PlayerSearchRequest request = requestStream.Current;

                s_Logger.Info(string.Format("[{0}] Requested {1} players.", callCounter, request.ExpectedDataCount));

                int fetchCount = 0;

                foreach (Player player in SampleDataRepository.GeneratePlayers(initial, request.ExpectedDataCount, teams))
                {

                    if (ExitIfRequestedCancel(context)) { return; }

                    response.Players.Add(player);

                    if (!response.Teams.ContainsKey(player.TeamCode))
                    {
                        response.Teams.Add(player.TeamCode, teams[player.TeamCode]);
                    }

                    ++fetchCount;

                    if (fetchCount == s_FetchSize)
                    {
                        await Task.Delay(s_DelayMilliseconds).ConfigureAwait(false);
                        await responseStream.WriteAsync(response).ConfigureAwait(false);

                        response.Players.Clear();
                        response.Teams.Clear();
                        fetchCount = 0;
                    }

                }

                if (response.Players.Count > 0)
                {
                    await Task.Delay(s_DelayMilliseconds).ConfigureAwait(false);
                    await responseStream.WriteAsync(response).ConfigureAwait(false);
                }

                initial += request.ExpectedDataCount;

            }

        }

        #endregion

        #region ServerPush method examle

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task PushPlayer(PlayerSearchRequest request, IServerStreamWriter<PlayerSearchResponse> responseStream, ServerCallContext context)
        {

            string callCounter = GetCallCounter(context);

            s_Logger.Info(string.Format("[{0}] Requested {1} players.", callCounter, request.ExpectedDataCount));

            IDictionary<string, Team> teams = SampleDataRepository.GetTeams(10);

            PlayerSearchResponse response = new PlayerSearchResponse();

            Random r = new Random();

            while (true)
            {

                await Task.Delay(r.Next(5, 20) * 1000);

                if (ExitIfRequestedCancel(context)) { return; }

                foreach (Player player in SampleDataRepository.GeneratePlayers(1, request.ExpectedDataCount, teams))
                {

                    response.Players.Add(player);

                    if (!response.Teams.ContainsKey(player.TeamCode))
                    {
                        response.Teams.Add(player.TeamCode, teams[player.TeamCode]);
                    }

                }

                if (response.Players.Count > 0)
                {
                    await Task.Delay(s_DelayMilliseconds).ConfigureAwait(false);
                    await responseStream.WriteAsync(response).ConfigureAwait(false);
                }

            }

        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static string GetCallCounter(ServerCallContext context)
        {
            return context.RequestHeaders.GetValueOrDefault("CallCounter", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static bool ExitIfRequestedCancel(ServerCallContext context)
        {

            if (!IsRequestedCancel(context)) { return false; }

            context.Status = new Status(StatusCode.Cancelled, "canceled by user.");

            return true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private void ThrowExceptionIfRequestedCancel(ServerCallContext context)
        {

            if (!IsRequestedCancel(context)) { return; }

            throw new RpcException(new Status(StatusCode.Cancelled, "canceled by user."));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static bool IsRequestedCancel(ServerCallContext context)
        {

            if ( context.CancellationToken== null) { return false; }

            return context.CancellationToken.IsCancellationRequested;

        }

    }

}
