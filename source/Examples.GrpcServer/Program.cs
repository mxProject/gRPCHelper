using System;

using Grpc.Core;

using mxProject.Helpers.Grpc.Commons;
using mxProject.Helpers.Grpc.Servers;
using mxProject.Helpers.Grpc.Utilities;

using Examples.GrpcModels;

namespace Examples.GrpcServer
{

    /// <summary>
    /// 
    /// </summary>
    class Program
    {

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            Server server = new Server();

            try
            {

                SampleServerSettings settings = SampleServerSettings.LoadFromFile("SampleServerSettings.config");

                //ServiceTimeFillterAttribute.DefaultOpenTime = null;
                //ServiceTimeFillterAttribute.DefaultCloseTime = null;

                // regist service.

                GrpcServiceBuilder builder = new GrpcServiceBuilder();

                GrpcServiceBuilderSettings builderSettings = new GrpcServiceBuilderSettings();

                builderSettings.MarshallerFactory = GrpcMarshallerFactory.DefaultInstance;

                builderSettings.PerformanceListener = new GrpcServerPerformanceListener();

                RegistPerformanceEventHandlers(builderSettings.PerformanceListener);

                server.Services.Add(builder.BuildService("PlayerSearch", typeof(PlayerSearch), new PlayerSearchServiceImpl(), builderSettings));


                // regist heartbeat.

                server.Services.Add(GrpcHeartbeat.BuildService());


                // start server.

                Grpc.Core.ServerCredentials credential = Grpc.Core.ServerCredentials.Insecure;

                server.Ports.Add(settings.ServerName, settings.ServerPort, credential);

                Console.WriteLine(string.Format("Starting... {0}:{1}", settings.ServerName, settings.ServerPort));
                server.Start();
                Console.WriteLine("Started.");

                Console.WriteLine("If you want to shutdown, please press any key.");
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.WriteLine("In shutdown...");
                try
                {
                    if (server != null)
                    {
                        server.ShutdownAsync().Wait();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                Console.WriteLine("Shutdown completed.");
            }

            Console.WriteLine("If you want to close, please press any key.");
            Console.ReadLine();

        }

        #region logging

        /// <summary>
        /// 
        /// </summary>
        private static void RegistPerformanceEventHandlers(GrpcServerPerformanceListener listener)
        {

            listener.Serialized += PerformanceListener_Serialized;
            listener.Deserialized += PerformanceListener_Deserialized;
            listener.MethodCalling += PerformanceListener_MethodCalling;
            listener.MethodCalled += PerformanceListener_MethodCalled;
            listener.MethodIntercepted += PerformanceListener_MethodIntercepted;
            listener.RequestReading += PerformanceListener_RequestReading;
            listener.RequestReaded += PerformanceListener_RequestReaded;
            listener.ResponseWriting += PerformanceListener_ResponseWriting;
            listener.ResponseWrote += PerformanceListener_ResponseWrote;

        }

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
        private static void PerformanceListener_ResponseWriting(ServerCallContext context)
        {
            string message = string.Format("[{0}]【Writing】{1}", GetCallCounter(context), context.Method);
            Console.WriteLine(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="elapsedMilliseconds"></param>
        private static void PerformanceListener_ResponseWrote(ServerCallContext context, double elapsedMilliseconds)
        {
            string message = string.Format("[{0}]【Wrote】{1} {2:f4}ms", GetCallCounter(context), context.Method, elapsedMilliseconds);
            Console.WriteLine(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private static void PerformanceListener_RequestReading(ServerCallContext context)
        {
            string message = string.Format("[{0}]【Reading】{1}", GetCallCounter(context), context.Method);
            Console.WriteLine(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="elapsedMilliseconds"></param>
        private static void PerformanceListener_RequestReaded(ServerCallContext context, double elapsedMilliseconds)
        {
            string message = string.Format("[{0}]【Readed】{1} {2:f4}ms", GetCallCounter(context), context.Method, elapsedMilliseconds);
            Console.WriteLine(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="interceptor"></param>
        /// <param name="elapsedMilliseconds"></param>
        private static void PerformanceListener_MethodIntercepted(ServerCallContext context, IGrpcInterceptor interceptor, double elapsedMilliseconds)
        {
            string message = string.Format("[{0}]【Intercept】{1} {2} {3:f4}ms", GetCallCounter(context), context.Method, interceptor.Name, elapsedMilliseconds);
            Console.WriteLine(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private static void PerformanceListener_MethodCalling(ServerCallContext context)
        {
            string message = string.Format("[{0}]【CallingMethod】{1}", GetCallCounter(context), context.Method);
            Console.WriteLine(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="elapsedMilliseconds"></param>
        private static void PerformanceListener_MethodCalled(ServerCallContext context, double elapsedMilliseconds)
        {
            string message = string.Format("[{0}]【CalledMethod】{1} {2:f4}ms", GetCallCounter(context), context.Method, elapsedMilliseconds);
            Console.WriteLine(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="methodName"></param>
        /// <param name="objectTypeName"></param>
        /// <param name="elapsedMilliseconds"></param>
        /// <param name="byteSize"></param>
        private static void PerformanceListener_Deserialized(string serviceName, string methodName, string objectTypeName, double elapsedMilliseconds, long byteSize)
        {
            string message = string.Format("【Deserialized】/{0}/{1} {2} {3:f4}ms {4:n0}bytes", serviceName, methodName, objectTypeName, elapsedMilliseconds, byteSize);
            Console.WriteLine(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="methodName"></param>
        /// <param name="objectTypeName"></param>
        /// <param name="elapsedMilliseconds"></param>
        /// <param name="byteSize"></param>
        private static void PerformanceListener_Serialized(string serviceName, string methodName, string objectTypeName, double elapsedMilliseconds, long byteSize)
        {
            string message = string.Format("【Serialized】/{0}/{1} {2} {3:f4}ms {4:n0}bytes", serviceName, methodName, objectTypeName, elapsedMilliseconds, byteSize);
            Console.WriteLine(message);
        }

        #endregion

    }

}
