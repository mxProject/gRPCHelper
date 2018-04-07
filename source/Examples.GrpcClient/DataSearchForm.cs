using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Grpc.Core;

using mxProject.Helpers.Grpc.Commons;
using mxProject.Helpers.Grpc.Clients;

using Examples.GrpcModels;
using Examples.GrpcClient.DataGridViews;

namespace Examples.GrpcClient
{

    /// <summary>
    /// 
    /// </summary>
    internal partial class DataSearchForm : Form
    {

        #region ctor

        /// <summary>
        /// 
        /// </summary>
        public DataSearchForm()
        {
            InitializeComponent();
            Initialize();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        private void Initialize()
        {

            this.Load += Form_Load;

            InitializeDataGrid();

        }

        #region eventHandlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Load(object sender, EventArgs e)
        {

            btnExecute.Enabled = false;

            if ( !TryStartGrpc())
            {
                return;
            }

            btnExecute.Enabled = true;

            txtDataCount.Text = "100";

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnExecute_Click(object sender, EventArgs e)
        {

            btnExecute.Enabled = false;
            btnCancel.Enabled = false;

            ClearLog();

            try
            {

                if (rdoMethodUnary.Checked)
                {
                    ExecuteUnary();
                }
                else
                {

                    btnCancel.Enabled = true;

                    if (rdoMethodUnaryAsync.Checked)
                    {
                        await ExecuteUnaryAsync();
                    }
                    else if (rdoMethodServerStream.Checked)
                    {
                        await ExecuteServerStreamAsync();
                    }
                    else if (rdoMethodClientStream.Checked)
                    {
                        await ExecuteClientStreamAsync();
                    }
                    else if (rdoMethodDuplexStream.Checked)
                    {
                        await ExecuteDuplexStreamAsync();
                    }

                }

            }
            catch ( Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btnExecute.Enabled = true;
                btnCancel.Enabled = false;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {

            RequestCancel();

        }

        #endregion

        #region grpc client

        private PlayerSearch.PlayerSearchClient m_Client;
        private Channel m_Channel;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool TryStartGrpc()
        {

            try
            {

                // create settings.

                GrpcClientSettings settings = new GrpcClientSettings();

                settings.MarshallerFactory = GrpcMarshallerFactory.DefaultInstance;

                settings.PerformanceListener = new GrpcClientPerformanceListener();

                RegistPerformanceEventHandlers(settings.PerformanceListener);

                // create client.

                ChannelCredentials credential = ChannelCredentials.Insecure;

                m_Channel = new Channel("localhost:50000", credential);

                GrpcCallInvoker invoker = GrpcCallInvoker.Create(m_Channel, settings);

                //TestMethodInterceptor interceptor = new TestMethodInterceptor();

                //m_Invoker.AddInvokingInterceptor(interceptor);
                //m_Invoker.AddInvokedInterceptor(interceptor);
                //m_Invoker.AddExceptionHandler(interceptor);

                m_Client = new PlayerSearch.PlayerSearchClient(invoker);

                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

        }

        #endregion

        #region logging

        /// <summary>
        /// 
        /// </summary>
        private void RegistPerformanceEventHandlers(GrpcClientPerformanceListener listener)
        {

            listener.Serialized += PerformanceListener_Serialized;
            listener.Deserialized += PerformanceListener_Deserialized;
            listener.MethodCalling += PerformanceListener_MethodCalling;
            listener.MethodCalled += PerformanceListener_MethodCalled;
            listener.MethodIntercepted += PerformanceListener_MethodIntercepted;
            listener.RequestWriting += PerformanceListener_RequestWriting;
            listener.RequestWrote += PerformanceListener_RequestWrote;
            listener.ResponseReading += PerformanceListener_ResponseReading;
            listener.ResponseReaded += PerformanceListener_ResponseReaded;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="methodName"></param>
        /// <param name="host"></param>
        /// <param name="elapsedMilliseconds"></param>
        private void PerformanceListener_ResponseReaded(string serviceName, string methodName, string host, double elapsedMilliseconds)
        {
            string message = string.Format("【Readed Response】{0} {1} {2} {3:f4}ms", host, serviceName, methodName, elapsedMilliseconds);
            AppendLog(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="methodName"></param>
        /// <param name="host"></param>
        private void PerformanceListener_ResponseReading(string serviceName, string methodName, string host)
        {
            string message = string.Format("【Reading Response】{0} {1} {2}", host, serviceName, methodName);
            AppendLog(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="methodName"></param>
        /// <param name="host"></param>
        /// <param name="elapsedMilliseconds"></param>
        private void PerformanceListener_RequestWrote(string serviceName, string methodName, string host, double elapsedMilliseconds)
        {
            string message = string.Format("【Wrote Request】{0} {1} {2} {3:f4}ms", host, serviceName, methodName, elapsedMilliseconds);
            AppendLog(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="methodName"></param>
        /// <param name="host"></param>
        private void PerformanceListener_RequestWriting(string serviceName, string methodName, string host)
        {
            string message = string.Format("【Writing Request】{0} {1} {2}", host, serviceName, methodName);
            AppendLog(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="methodName"></param>
        /// <param name="objectTypeName"></param>
        /// <param name="elapsedMilliseconds"></param>
        /// <param name="byteSize"></param>
        private void PerformanceListener_Deserialized(string serviceName, string methodName, string objectTypeName, double elapsedMilliseconds, long byteSize)
        {
            string message = string.Format("【Deserialized】{0} {1} {2} {3:f4}ms {4}bytes", serviceName, methodName, objectTypeName, elapsedMilliseconds, byteSize);
            AppendLog(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="methodName"></param>
        /// <param name="objectTypeName"></param>
        /// <param name="elapsedMilliseconds"></param>
        /// <param name="byteSize"></param>
        private void PerformanceListener_Serialized(string serviceName, string methodName, string objectTypeName, double elapsedMilliseconds, long byteSize)
        {
            string message = string.Format("【Serialized】{0} {1} {2} {3:f4}ms {4}bytes", serviceName, methodName, objectTypeName, elapsedMilliseconds, byteSize);
            AppendLog(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="methodName"></param>
        /// <param name="host"></param>
        /// <param name="interceptor"></param>
        /// <param name="elapsedMilliseconds"></param>
        private void PerformanceListener_MethodIntercepted(string serviceName, string methodName, string host, IGrpcInterceptor interceptor, double elapsedMilliseconds)
        {
            string message = string.Format("【Intercept】{0} {1} {2} {3} {4:f4}ms", host, serviceName, methodName, interceptor.Name, elapsedMilliseconds);
            AppendLog(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="methodName"></param>
        /// <param name="host"></param>
        private void PerformanceListener_MethodCalling(string serviceName, string methodName, string host)
        {
            string message = string.Format("【CallingMethod】{0} {1} {2}", host, serviceName, methodName);
            AppendLog(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="methodName"></param>
        /// <param name="host"></param>
        /// <param name="elapsedMilliseconds"></param>
        private void PerformanceListener_MethodCalled(string serviceName, string methodName, string host, double elapsedMilliseconds)
        {
            string message = string.Format("【CalledMethod】{0} {1} {2} {3:f4}ms", host, serviceName, methodName, elapsedMilliseconds);
            AppendLog(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        private void AppendLog(string log)
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendLog), new object[] { log });
                return;
            }

            this.txtLog.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + " " + log + Environment.NewLine);

            this.txtLog.SelectionStart = this.txtLog.Text.Length;
            this.txtLog.Focus();
            this.txtLog.ScrollToCaret();

        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearLog()
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ClearLog), new object[] {});
                return;
            }

            this.txtLog.Clear();

        }

        #endregion

        #region Unary method

        /// <summary>
        /// 
        /// </summary>
        private void ExecuteUnary()
        {

            TeamSearchRequest request = CreateTeamSearchRequest();

            TeamSearchResponse response = m_Client.SearchTeam(request, CreateCallOptions());

            ShowTeams(response.Teams);

        }

        #endregion

        #region Unary method (async)

        /// <summary>
        /// 
        /// </summary>
        private async Task ExecuteUnaryAsync()
        {

            TeamSearchRequest request = CreateTeamSearchRequest();

            TeamSearchResponse response = await m_Client.SearchTeamAsync(request, CreateCallOptions());

            ShowTeams(response.Teams);

            await Task.CompletedTask;

        }

        #endregion

        #region ClientStream method

        /// <summary>
        /// 
        /// </summary>
        private async Task ExecuteClientStreamAsync()
        {

            using (var call = m_Client.SearchPlayer_ClientStream(CreateCallOptions()))
            {

                GrpcResult<PlayerSearchResponse> ret = await call.WriteAndCompleteAsync(GetPlayerSearchRequests());

                if (ret.IsCanceled) { return; }

                ShowPlayers(ret.Response.Players, ret.Response.Teams);

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<AsyncFunc<PlayerSearchRequest>> GetPlayerSearchRequests()
        {
            return new AsyncFunc<PlayerSearchRequest>[] { GetPlayerSearchRequestAsync1, GetPlayerSearchRequestAsync2 };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<PlayerSearchRequest> GetPlayerSearchRequestAsync1()
        {
            await Task.Delay(500);
            return new PlayerSearchRequest() { ExpectedDataCount = GetExpectedDataCount() };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<PlayerSearchRequest> GetPlayerSearchRequestAsync2()
        {
            await Task.Delay(1000);
            return new PlayerSearchRequest() { ExpectedDataCount = GetExpectedDataCount() };
        }

        #endregion

        #region ServerStream method

        /// <summary>
        /// 
        /// </summary>
        private async Task ExecuteServerStreamAsync()
        {

            PlayerSearchRequest request = CreatePlayerSearchRequest();

            using (var call = m_Client.SearchPlayer_ServerStream(request, CreateCallOptions()))
            {

                bool firstTime = true;

                await call.ForEachAsync(delegate (PlayerSearchResponse response)
                {
                    if (firstTime)
                    {
                        firstTime = false;
                        ShowPlayers(response.Players, response.Teams);
                    }
                    else
                    {
                        AppendPlayers(response.Players, response.Teams);
                    }
                });

            }

        }

        #endregion

        #region ServerStream method

        /// <summary>
        /// 
        /// </summary>
        private async Task ExecuteDuplexStreamAsync()
        {

            PlayerSearchRequest request = CreatePlayerSearchRequest();

            using (var call = m_Client.SearchPlayer_DuplexStream(CreateCallOptions()))
            {

                bool firstTime = true;

                await call.WriteAndForEachAsync(GetPlayerSearchRequests(), delegate (PlayerSearchResponse response)
                {
                    if (firstTime)
                    {
                        firstTime = false;
                        ShowPlayers(response.Players, response.Teams);
                    }
                    else
                    {
                        AppendPlayers(response.Players, response.Teams);
                    }
                });

            }

        }

        #endregion

        #region request

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private TeamSearchRequest CreateTeamSearchRequest()
        {

            TeamSearchRequest request = new TeamSearchRequest();

            request.Name = "";
            request.Country = "";
            request.ExpectedDataCount = GetExpectedDataCount();

            return request;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private PlayerSearchRequest CreatePlayerSearchRequest()
        {

            PlayerSearchRequest request = new PlayerSearchRequest();

            request.PlayerName = "";
            request.TeamName = "";
            request.Position = "";
            request.ExpectedDataCount = GetExpectedDataCount();

            return request;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int GetExpectedDataCount()
        {

            int value;

            if (int.TryParse(txtDataCount.Text, out value))
            {
                return value;
            }
            else
            {
                return 0;
            }

        }

        #endregion

        #region callOptions

        private int m_CallCounter = 0;

        private System.Threading.CancellationTokenSource m_CancelToken;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private CallOptions CreateCallOptions()
        {

            if (m_CancelToken != null)
            {
                m_CancelToken.Dispose();
            }

            m_CancelToken = new System.Threading.CancellationTokenSource();

            ++m_CallCounter;

            Metadata headers = new Metadata();

            headers.Add("CallCounter", m_CallCounter.ToString());

            CallOptions options = new CallOptions(headers, null, m_CancelToken.Token);

            return options;

        }

        /// <summary>
        /// 
        /// </summary>
        private void RequestCancel()
        {

            if (m_CancelToken== null) { return; }
            if (m_CancelToken.IsCancellationRequested) { return; }

            m_CancelToken.Cancel();

        }

        #endregion

        #region team

        private TeamGridBehavior m_TeamGridBehavior = new TeamGridBehavior();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teams"></param>
        private void ShowTeams(IList<Team> teams)
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new Action<IList<Team>>(ShowTeams), new object[] { teams });
                return;
            }

            m_TeamGridBehavior.ClearItems();

            if (teams!= null)
            {
                m_TeamGridBehavior.AddItems(teams);
            }

            this.DataGridBehavior = m_TeamGridBehavior;

        }

        /// <summary>
        /// 
        /// </summary>
        private class TeamGridBehavior : DataGridBehaviorBase<Team>
        {

            /// <summary>
            /// 
            /// </summary>
            internal TeamGridBehavior() : base() { }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected override IList<DataField<Team>> CreateDataFields()
            {

                List<DataField<Team>> fields = new List<DataField<Team>>();

                fields.Add(new DataField<Team>("Code", "Code", delegate (Team obj) { return obj.Code; }));
                fields.Add(new DataField<Team>("Name", "Name", delegate (Team obj) { return obj.Name; }));
                fields.Add(new DataField<Team>("Country", "Country", delegate (Team obj) { return obj.Country; }));

                return fields;

            }

        }

        #endregion

        #region player

        private PlayerGridBehavior m_PlayerGridBehavior = new PlayerGridBehavior();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="players"></param>
        /// <param name="teams"></param>
        private void ShowPlayers(IList<Player> players, IDictionary<string, Team> teams )
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new Action<IList<Player>, IDictionary<string, Team>>(ShowPlayers), new object[] { players, teams });
                return;
            }

            m_PlayerGridBehavior.ClearItems();
            m_PlayerGridBehavior.ClearTeams();

            if (teams != null)
            {
                m_PlayerGridBehavior.AddTeams(teams.Values);
            }

            if (players != null)
            {
                m_PlayerGridBehavior.AddItems(players);
            }

            this.DataGridBehavior = m_PlayerGridBehavior;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="players"></param>
        /// <param name="teams"></param>
        private void AppendPlayers(IList<Player> players, IDictionary<string, Team> teams)
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new Action<IList<Player>, IDictionary<string, Team>>(AppendPlayers), new object[] { players, teams });
                return;
            }

            if (teams != null)
            {
                m_PlayerGridBehavior.AddTeams(teams.Values);
            }

            if (players != null)
            {
                m_PlayerGridBehavior.AddItems(players);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private class PlayerGridBehavior : DataGridBehaviorBase<Player>
        {

            /// <summary>
            /// 
            /// </summary>
            internal PlayerGridBehavior() : base() { }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected override IList<DataField<Player>> CreateDataFields()
            {

                List<DataField<Player>> fields = new List<DataField<Player>>();

                fields.Add(new DataField<Player>("Name", "Name", delegate (Player obj) { return obj.Name; }));

                fields.Add(new DataField<Player>("Positions", "Positions", delegate (Player obj)
                {
                    if (obj.Positions == null) { return null; }
                    return string.Join(" / ", obj.Positions);
                }
                ));

                fields.Add(new DataField<Player>("Team", "Team", delegate (Player obj)
                {

                    if (string.IsNullOrEmpty(obj.TeamCode)) { return null; }

                    Team team;

                    if (m_Teams.TryGetValue(obj.TeamCode, out team))
                    {
                        return team.Name;
                    }
                    else
                    {
                        return null;
                    }

                }
                ));

                return fields;

            }

            private readonly Dictionary<string, Team> m_Teams = new Dictionary<string, Team>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="team"></param>
            /// <returns></returns>
            internal bool TryAddTeam(Team team)
            {

                if (team == null) { return false; }

                if (m_Teams.ContainsKey(team.Code)) { return false; }

                lock (m_Teams)
                {
                    if (m_Teams.ContainsKey(team.Code)) { return false; }
                    m_Teams.Add(team.Code, team);
                    return true;
                }

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="teams"></param>
            internal void AddTeams(IEnumerable<Team> teams)
            {

                if ( teams == null) { return; }

                foreach ( Team team in teams)
                {
                    TryAddTeam(team);
                }

            }

            /// <summary>
            /// 
            /// </summary>
            internal void ClearTeams()
            {
                m_Teams.Clear();
            }

        }

        #endregion

        #region datagrid

        /// <summary>
        /// 
        /// </summary>
        private void InitializeDataGrid()
        {

            this.dataGridView1.AllowUserToAddRows = false;

            this.dataGridView1.VirtualMode = true;

            this.dataGridView1.CellValueNeeded += dataGridView1_CellValueNeeded;

            this.dataGridView1.RowCount = 0;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            e.Value = GetCellValue(m_DataGridBehavior, e.RowIndex, e.ColumnIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        private IDataGridBehavior DataGridBehavior
        {
            get { return m_DataGridBehavior; }
            set {
                if ( object.Equals(m_DataGridBehavior, value)) { return; }
                UnbindDataGridBehavior(m_DataGridBehavior);
                m_DataGridBehavior = value;
                BindDataGridBehavior(m_DataGridBehavior);
                OnDataGridBehaviorChanged();
            }
        }
        private IDataGridBehavior m_DataGridBehavior;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="behavior"></param>
        private void UnbindDataGridBehavior(IDataGridBehavior behavior)
        {
            if (behavior == null) { return; }
            behavior.ItemsChanged -= dataGridBehavior_ItemsChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="behavior"></param>
        private void BindDataGridBehavior(IDataGridBehavior behavior)
        {
            if (behavior == null) { return; }
            behavior.ItemsChanged += dataGridBehavior_ItemsChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridBehavior_ItemsChanged(object sender, EventArgs e)
        {

            IDataGridBehavior behavior = sender as IDataGridBehavior;

            this.dataGridView1.RowCount = behavior.GetDataCount();

        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDataGridBehaviorChanged()
        {

            CreateColumns(this.dataGridView1, m_DataGridBehavior);

            if (m_DataGridBehavior != null)
            {
                this.dataGridView1.RowCount = m_DataGridBehavior.GetDataCount();
            }
            else
            {
                this.dataGridView1.RowCount = 0;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="behavior"></param>
        private void CreateColumns(DataGridView grid, IDataGridBehavior behavior)
        {

            grid.Columns.Clear();

            if (behavior != null)
            {
                foreach (IDataField field in behavior.GetDataFields())
                {
                    grid.Columns.Add(field.Name, field.HeaderText);
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="behavior"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private object GetCellValue(IDataGridBehavior behavior, int rowIndex, int columnIndex)
        {

            if (behavior == null) { return null; }

            return behavior.GetCellValue(rowIndex, columnIndex);

        }

        #endregion

    }

}
