#if GAMEMODE_ALLINONE
using System;
using System.Runtime.InteropServices;
using AOT;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Assets.Scripts.FFmpegKit
{
    public class FFmpegKitConfig
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void LogFuncDelegate(long sessionID, int logLevel, int size, IntPtr bytes);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void StatisticsFuncDelegate(long sessionId, int statisticsFrameNumber, float statisticsFps,
                            float statisticsQuality, long statisticsSize, int statisticsTime,
                            double statisticsBitrate, double statisticsSpeed);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SafOpenFuncDelegate(int safID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SafCloseFuncDelegate(int fd);

        [DllImport("ffmpegkit", EntryPoint = "InitFunctions")]
        private extern static void InitFunctions(IntPtr _logFunc, IntPtr _statisticsFunc, IntPtr _safOpenFunc, IntPtr _safCloseFunc);

        [DllImport("ffmpegkit", EntryPoint = "FFmpegKitConfig_nativeFFprobeExecute")]
        private extern static int nativeFFprobeExecute(long id, string[] stringArray, int programArgumentCount);

        [DllImport("ffmpegkit", EntryPoint = "FFmpegKitConfig_enableNativeRedirection")]
        public extern static void enableNativeRedirection();

        [DllImport("ffmpegkit", EntryPoint = "FFmpegKitConfig_disableNativeRedirection")]
        private extern static void disableNativeRedirection();

        [DllImport("ffmpegkit", EntryPoint = "FFmpegKitConfig_getNativeLogLevel")]
        private extern static int getNativeLogLevel();

        [DllImport("ffmpegkit", EntryPoint = "FFmpegKitConfig_setNativeLogLevel")]
        private extern static void setNativeLogLevel(int level);

        [DllImport("ffmpegkit", EntryPoint = "FFmpegKitConfig_getNativeFFmpegVersion")]
        private extern static string getNativeFFmpegVersion();

        [DllImport("ffmpegkit", EntryPoint = "FFmpegKitConfig_getNativeVersion")]
        private extern static string getNativeVersion();

        [DllImport("ffmpegkit", EntryPoint = "FFmpegKitConfig_nativeFFmpegExecute")]
        private extern static int nativeFFmpegExecute(long sessionID, string[] args, int argCount);

        [DllImport("ffmpegkit", EntryPoint = "FFmpegKitConfig_nativeFFmpegCancel")]
        public extern static void nativeFFmpegCancel(long id);

        [DllImport("ffmpegkit", EntryPoint = "FFmpegKitConfig_registerNewNativeFFmpegPipe")]
        private extern static int registerNewNativeFFmpegPipe(string ffmpegPipePath);

        [DllImport("ffmpegkit", EntryPoint = "FFmpegKitConfig_getNativeBuildDate")]
        private extern static string getNativeBuildDate();

        [DllImport("ffmpegkit", EntryPoint = "FFmpegKitConfig_setNativeEnvironmentVariable")]
        private extern static int setNativeEnvironmentVariable(string variableName, string variableValue);

        [DllImport("ffmpegkit", EntryPoint = "FFmpegKitConfig_getNativeBuildDate")]
        private extern static void ignoreNativeSignal(int signum);

        [DllImport("ffmpegkit", EntryPoint = "FFmpegKitConfig_messagesInTransmit")]
        public extern static int messagesInTransmit(long id);

        [DllImport("ffmpegkit", EntryPoint = "consumeLogAndStatistics")]
        public extern static int consumeLogAndStatistics();

        private static LogFuncDelegate logFunc = new LogFuncDelegate(LogFunc);
        private static StatisticsFuncDelegate statisticsFunc = new StatisticsFuncDelegate(StatisticsFunc);
        private static SafOpenFuncDelegate safOpenFunc = new SafOpenFuncDelegate(SafOpenFunc);
        private static SafCloseFuncDelegate safCloseFunc = new SafCloseFuncDelegate(SafCloseFunc);

        /* Global callbacks */
        private static AbstractSession.LogCallback globalLogCallback;
        private static AbstractSession.StatisticsCallback globalStatisticsCallback;
        private static AbstractSession.FFmpegSessionCompleteCallback globalFFmpegSessionCompleteCallback;
        private static AbstractSession.FFprobeSessionCompleteCallback globalFFprobeSessionCompleteCallback;
        private static AbstractSession.MediaInformationSessionCompleteCallback globalMediaInformationSessionCompleteCallback;
        //private static SparseArray<SAFProtocolUrl> safIdMap;
        //private static SparseArray<SAFProtocolUrl> safFileDescriptorMap;

        private static long sessionID;
        private static LogRedirectionStrategy globalLogRedirectionStrategy;
        private static DictionaryView<long, AbstractSession> sessionHistoryMap;
        private static List<AbstractSession> sessionHistoryList;
        private static Object sessionHistoryLock = new object();
        private static int sessionHistorySize;
        private static SessionLogLevel activeLogLevel;


        public static void Init()
        {
            sessionID = 0;
            globalLogRedirectionStrategy = LogRedirectionStrategy.PRINT_LOGS_WHEN_NO_CALLBACKS_DEFINED;
            sessionHistoryMap = new DictionaryView<long, AbstractSession>();
            sessionHistoryList = new List<AbstractSession>();

            InitFunctions(Marshal.GetFunctionPointerForDelegate(logFunc), Marshal.GetFunctionPointerForDelegate(statisticsFunc),
                Marshal.GetFunctionPointerForDelegate(safOpenFunc), Marshal.GetFunctionPointerForDelegate(safCloseFunc));
        }

        public static void StartExecute(params string[] args)
        {
            nativeFFmpegExecute(sessionID, args, args.Length);
        }

        [MonoPInvokeCallback(typeof(LogFuncDelegate))]
        private static void LogFunc(long sessionId, int logLevel, int size, IntPtr bytes)
        {
            SessionLogLevel level = (SessionLogLevel)logLevel;
            string text = Marshal.PtrToStringAnsi(bytes, size);
            SessionLog log = new SessionLog(sessionId, level, text);
            bool globalCallbackDefined = false;
            bool sessionCallbackDefined = false;
            LogRedirectionStrategy activeLogRedirectionStrategy = globalLogRedirectionStrategy;

            // AV_LOG_STDERR logs are always redirected
            if ((activeLogLevel == SessionLogLevel.AV_LOG_QUIET && level != SessionLogLevel.AV_LOG_STDERR) || level > activeLogLevel)
            {
                // LOG NEITHER PRINTED NOR FORWARDED
                return;
            }

            AbstractSession session = getSession(sessionId);
            if (session != null)
            {
                activeLogRedirectionStrategy = session.getLogRedirectionStrategy();
                session.addLog(log);

                if (session.getLogCallback() != null)
                {
                    sessionCallbackDefined = true;

                    try
                    {
                        // NOTIFY SESSION CALLBACK DEFINED
                        session.getLogCallback()(log);
                    }
                    catch (Exception e)
                    {
                        ocsys.NSFormatLog(true, "Exception thrown inside session log callback.{0}", e.StackTrace);
                    }
                }
            }

            AbstractSession.LogCallback globalLogCallbackFunction = FFmpegKitConfig.globalLogCallback;
            if (globalLogCallbackFunction != null)
            {
                globalCallbackDefined = true;

                try
                {
                    // NOTIFY GLOBAL CALLBACK DEFINED
                    globalLogCallbackFunction(log);
                }
                catch (Exception e)
                {
                    ocsys.NSFormatLog(true, "Exception thrown inside global log callback.{0}", e.StackTrace);
                }
            }

            // EXECUTE THE LOG STRATEGY
            switch (activeLogRedirectionStrategy)
            {
                case LogRedirectionStrategy.NEVER_PRINT_LOGS:
                    {
                        return;
                    }
                case LogRedirectionStrategy.PRINT_LOGS_WHEN_GLOBAL_CALLBACK_NOT_DEFINED:
                    {
                        if (globalCallbackDefined)
                        {
                            return;
                        }
                    }
                    break;
                case LogRedirectionStrategy.PRINT_LOGS_WHEN_SESSION_CALLBACK_NOT_DEFINED:
                    {
                        if (sessionCallbackDefined)
                        {
                            return;
                        }
                    }
                    break;
                case LogRedirectionStrategy.PRINT_LOGS_WHEN_NO_CALLBACKS_DEFINED:
                    {
                        if (globalCallbackDefined || sessionCallbackDefined)
                        {
                            return;
                        }
                    }
                    break;
                case LogRedirectionStrategy.ALWAYS_PRINT_LOGS:
                    {
                    }
                    break;
            }

            // PRINT LOGS
            switch (level)
            {
                case SessionLogLevel.AV_LOG_QUIET:
                    {
                        // PRINT NO OUTPUT
                    }
                    break;
                case SessionLogLevel.AV_LOG_TRACE:
                case SessionLogLevel.AV_LOG_DEBUG:
                    {
                        ocsys.NSFormatLog(false, text);
                    }
                    break;
                case SessionLogLevel.AV_LOG_INFO:
                    {
                        ocsys.NSFormatLog(false, text);
                    }
                    break;
                case SessionLogLevel.AV_LOG_WARNING:
                    {
                        ocsys.NSFormatLog(false, text);
                    }
                    break;
                case SessionLogLevel.AV_LOG_ERROR:
                case SessionLogLevel.AV_LOG_FATAL:
                case SessionLogLevel.AV_LOG_PANIC:
                    {
                        ocsys.NSFormatLog(true, text);
                    }
                    break;
                case SessionLogLevel.AV_LOG_STDERR:
                case SessionLogLevel.AV_LOG_VERBOSE:
                default:
                    {
                        ocsys.NSFormatLog(true, text);
                    }
                    break;
            }
        }

        [MonoPInvokeCallback(typeof(StatisticsFuncDelegate))]
        private static void StatisticsFunc(long sessionId, int videoFrameNumber, float videoFps,
                            float videoQuality, long size, int time,
                            double bitrate, double speed)
        {
            Statistics statistics = new Statistics(sessionId, videoFrameNumber, videoFps, videoQuality, size, time, bitrate, speed);

            AbstractSession session = getSession(sessionId);
            if (session != null && session.isFFmpeg())
            {
                FFmpegSession ffmpegSession = (FFmpegSession)session;
                ffmpegSession.addStatistics(statistics);

                if (ffmpegSession.getStatisticsCallback() != null)
                {
                    try
                    {
                        // NOTIFY SESSION CALLBACK IF DEFINED
                        ffmpegSession.getStatisticsCallback()(statistics);
                    }
                    catch (Exception e)
                    {
                        ocsys.NSFormatLog(true, "Exception thrown inside session statistics callback.{0}", e.StackTrace);
                    }
                }
            }

            AbstractSession.StatisticsCallback globalStatisticsCallbackFunction = FFmpegKitConfig.globalStatisticsCallback;
            if (globalStatisticsCallbackFunction != null)
            {
                try
                {
                    // NOTIFY GLOBAL CALLBACK IF DEFINED
                    globalStatisticsCallbackFunction(statistics);
                }
                catch (Exception e)
                {
                    ocsys.NSFormatLog(true, "Exception thrown inside global statistics callback.{0}", e.StackTrace);
                }
            }
        }

        [MonoPInvokeCallback(typeof(SafOpenFuncDelegate))]
        private static void SafOpenFunc(int safID)
        {

        }

        [MonoPInvokeCallback(typeof(SafCloseFuncDelegate))]
        private static void SafCloseFunc(int fd)
        {

        }

        /**
     * <p>Sets a global callback to redirect FFmpeg/FFprobe logs.
     *
     * @param logCallback log callback or null to disable a previously defined callback
     */
        public static void enableLogCallback(AbstractSession.LogCallback logCallback)
        {
            globalLogCallback = logCallback;
        }

        /**
 * <p>Sets a global callback to redirect FFmpeg statistics.
 *
 * @param statisticsCallback statistics callback or null to disable a previously
 *                           defined callback
 */
        public static void enableStatisticsCallback(AbstractSession.StatisticsCallback statisticsCallback)
        {
            globalStatisticsCallback = statisticsCallback;
        }

        /**
     * Returns the session specified with <code>sessionId</code> from the session history.
     *
     * @param sessionId session identifier
     * @return session specified with sessionId or null if it is not found in the history
     */
        public static AbstractSession getSession(long sessionId)
        {
            lock(sessionHistoryLock) 
            {
                AbstractSession session = null;
                if(sessionHistoryMap.TryGetValue(sessionId, out session))
                {
                    return session;
                }
                else
                {
                    return null;
                }
            }
        }

        public static string ArgumentsToString(string[] arguments)
        {
            if (arguments == null)
            {
                return "null";
            }

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < arguments.Length; i++)
            {
                if (i > 0)
                {
                    stringBuilder.Append(" ");
                }
                stringBuilder.Append(arguments[i]);
            }

            return stringBuilder.ToString();
        }

        public static LogRedirectionStrategy GetLogRedirectionStrategy()
        {
            return globalLogRedirectionStrategy;
        }

        public static void AddSession(AbstractSession session)
        {
            lock (sessionHistoryLock)
            {

                /*
                 * ASYNC SESSIONS CALL THIS METHOD TWICE
                 * THIS CHECK PREVENTS ADDING THE SAME SESSION TWICE
                 */
                bool sessionAlreadyAdded = sessionHistoryMap.ContainsKey(session.getSessionId());
                if (!sessionAlreadyAdded)
                {
                    sessionHistoryMap[session.getSessionId()] = session;
                    sessionHistoryList.Add(session);
                    if (sessionHistoryList.Count > sessionHistorySize)
                    {
                        sessionHistoryList.RemoveAt(0);
                    }
                }
            }
        }

        /**
     * <p>Synchronously executes the FFmpeg session provided.
     *
     * @param ffmpegSession FFmpeg session which includes command options/arguments
     */
        public static void ffmpegExecute(FFmpegSession ffmpegSession)
        {
            ffmpegSession.startRunning();

            try
            {
                var args = ffmpegSession.getArguments();
                int returnCode = nativeFFmpegExecute(ffmpegSession.getSessionId(), args, args.Length);
                ffmpegSession.complete(new ReturnCode(returnCode));
            }
            catch (Exception e)
            {
                ffmpegSession.fail(e);
                ocsys.NSFormatLog(true, "FFmpeg execute failed: %s.%s", FFmpegKitConfig.ArgumentsToString(ffmpegSession.getArguments()), e.StackTrace);
            }
        }

        public static void asyncFFmpegExecute(FFmpegSession ffmpegSession)
        {
            //AsyncFFmpegExecuteTask asyncFFmpegExecuteTask = new AsyncFFmpegExecuteTask(ffmpegSession);
            //Future <?> future = asyncExecutorService.submit(asyncFFmpegExecuteTask);
            //ffmpegSession.setFuture(future);
            throw new NotImplementedException("asyncFFmpegExecute is not supported in C#");
        }

        /**
     * <p>Starts an asynchronous FFmpeg execution for the given session.
     *
     * <p>Note that this method returns immediately and does not wait the execution to complete.
     * You must use an {@link FFmpegSessionCompleteCallback} if you want to be notified about the
     * result.
     *
     * @param ffmpegSession   FFmpeg session which includes command options/arguments
     * @param executorService executor service that will be used to run this asynchronous operation
     */
        public static void asyncFFmpegExecute(FFmpegSession ffmpegSession, ConcurrentQueue<AbstractSession> executorService)
        {
            //AsyncFFmpegExecuteTask asyncFFmpegExecuteTask = new AsyncFFmpegExecuteTask(ffmpegSession);
            //Future <?> future = executorService.submit(asyncFFmpegExecuteTask);
            //ffmpegSession.setFuture(future);
            throw new NotImplementedException("asyncFFmpegExecute is not supported in C#");

        }

        /**
     * <p>Synchronously executes the FFprobe session provided.
     *
     * @param ffprobeSession FFprobe session which includes command options/arguments
     */
        public static void ffprobeExecute(FFprobeSession ffprobeSession)
        {
            ffprobeSession.startRunning();

            try
            {
                var args = ffprobeSession.getArguments();
                int returnCode = nativeFFprobeExecute(ffprobeSession.getSessionId(), args, args.Length);
                ffprobeSession.complete(new ReturnCode(returnCode));
            }
            catch (Exception e) {
                ffprobeSession.fail(e);
                ocsys.NSFormatLog(true, "FFprobe execute failed: {0}.{1}", FFmpegKitConfig.ArgumentsToString(ffprobeSession.getArguments()),e.StackTrace);
            }
        }

        /**
     * <p>Starts an asynchronous FFprobe execution for the given session.
     *
     * <p>Note that this method returns immediately and does not wait the execution to complete.
     * You must use an {@link FFprobeSessionCompleteCallback} if you want to be notified about the
     * result.
     *
     * @param ffprobeSession FFprobe session which includes command options/arguments
     */
        public static void asyncFFprobeExecute(FFprobeSession ffprobeSession)
        {
            //AsyncFFprobeExecuteTask asyncFFmpegExecuteTask = new AsyncFFprobeExecuteTask(ffprobeSession);
            //Future <?> future = asyncExecutorService.submit(asyncFFmpegExecuteTask);
            //ffprobeSession.setFuture(future);
            throw new NotImplementedException("asyncFFprobeExecute is not supported in C#");

        }

        /**
         * <p>Starts an asynchronous FFprobe execution for the given session.
         *
         * <p>Note that this method returns immediately and does not wait the execution to complete.
         * You must use an {@link FFprobeSessionCompleteCallback} if you want to be notified about the
         * result.
         *
         * @param ffprobeSession  FFprobe session which includes command options/arguments
         * @param executorService executor service that will be used to run this asynchronous operation
         */
        public static void asyncFFprobeExecute(FFprobeSession ffprobeSession, ConcurrentQueue<AbstractSession> executorService)
        {
            //AsyncFFprobeExecuteTask asyncFFmpegExecuteTask = new AsyncFFprobeExecuteTask(ffprobeSession);
            //Future <?> future = executorService.submit(asyncFFmpegExecuteTask);
            //ffprobeSession.setFuture(future);
            throw new NotImplementedException("asyncFFprobeExecute is not supported in C#");
        }

        /**
 * <p>Synchronously executes the media information session provided.
 *
 * @param mediaInformationSession media information session which includes command options/arguments
 * @param waitTimeout             max time to wait until media information is transmitted
 */
        public static void getMediaInformationExecute(MediaInformationSession mediaInformationSession, int waitTimeout)
        {
            mediaInformationSession.startRunning();

            try
            {
                var args = mediaInformationSession.getArguments();
                int returnCodeValue = nativeFFprobeExecute(mediaInformationSession.getSessionId(), args, args.Length);
                ReturnCode returnCode = new ReturnCode(returnCodeValue);
                mediaInformationSession.complete(returnCode);
                if (returnCode.isValueSuccess())
                {
                    MediaInformation mediaInformation = MediaInformationJsonParser.fromWithError(mediaInformationSession.getAllLogsAsString(waitTimeout));
                    mediaInformationSession.setMediaInformation(mediaInformation);
                }
            }
            catch (Exception e) {
                mediaInformationSession.fail(e);
                ocsys.NSFormatLog(true, "Get media information execute failed: {0}.{1}.{2}", FFmpegKitConfig.ArgumentsToString(mediaInformationSession.getArguments()), e.Message, e.StackTrace);
            }
        }

        /**
 * <p>Starts an asynchronous FFprobe execution for the given media information session.
 *
 * <p>Note that this method returns immediately and does not wait the execution to complete.
 * You must use a {@link MediaInformationSessionCompleteCallback} if you want to be notified
 * about the result.
 *
 * @param mediaInformationSession media information session which includes command
 *                                options/arguments
 * @param waitTimeout             max time to wait until media information is transmitted
 */
        public static void asyncGetMediaInformationExecute(MediaInformationSession mediaInformationSession, int waitTimeout)
        {
            //AsyncGetMediaInformationTask asyncGetMediaInformationTask = new AsyncGetMediaInformationTask(mediaInformationSession, waitTimeout);
            //Future <?> future = asyncExecutorService.submit(asyncGetMediaInformationTask);
            //mediaInformationSession.setFuture(future);
            throw new NotImplementedException("asyncGetMediaInformationExecute is not supported in C#");

        }

        /**
         * <p>Starts an asynchronous FFprobe execution for the given media information session.
         *
         * <p>Note that this method returns immediately and does not wait the execution to complete.
         * You must use a {@link MediaInformationSessionCompleteCallback} if you want to be notified
         * about the result.
         *
         * @param mediaInformationSession media information session which includes command
         *                                options/arguments
         * @param executorService         executor service that will be used to run this asynchronous
         *                                operation
         * @param waitTimeout             max time to wait until media information is transmitted
         */
        public static void asyncGetMediaInformationExecute(MediaInformationSession mediaInformationSession, ConcurrentQueue<AbstractSession> executorService, int waitTimeout)
        {
            //AsyncGetMediaInformationTask asyncGetMediaInformationTask = new AsyncGetMediaInformationTask(mediaInformationSession, waitTimeout);
            //Future <?> future = executorService.submit(asyncGetMediaInformationTask);
            //mediaInformationSession.setFuture(future);
            throw new NotImplementedException("asyncGetMediaInformationExecute is not supported in C#");
        }


        /**
     * <p>Parses the given command into arguments. Uses space character to split the arguments.
     * Supports single and double quote characters.
     *
     * @param command string command
     * @return array of arguments
     */
        public static String[] ParseArguments(String command)
        {
            List<String> argumentList = new List<string>();
            StringBuilder currentArgument = new StringBuilder();

            bool singleQuoteStarted = false;
            bool doubleQuoteStarted = false;

            for (int i = 0; i < command.Length; i++)
            {
                char? previousChar;
                if (i > 0)
                {
                    previousChar = command[i - 1];
                }
                else
                {
                    previousChar = null;
                }
                char currentChar = command[i];

                if (currentChar == ' ')
                {
                    if (singleQuoteStarted || doubleQuoteStarted)
                    {
                        currentArgument.Append(currentChar);
                    }
                    else if (currentArgument.Length > 0)
                    {
                        argumentList.Add(currentArgument.ToString());
                        currentArgument = new StringBuilder();
                    }
                }
                else if (currentChar == '\'' && (previousChar == null || previousChar.Value != '\\'))
                {
                    if (singleQuoteStarted)
                    {
                        singleQuoteStarted = false;
                    }
                    else if (doubleQuoteStarted)
                    {
                        currentArgument.Append(currentChar);
                    }
                    else
                    {
                        singleQuoteStarted = true;
                    }
                }
                else if (currentChar == '\"' && (previousChar == null || previousChar.Value != '\\'))
                {
                    if (doubleQuoteStarted)
                    {
                        doubleQuoteStarted = false;
                    }
                    else if (singleQuoteStarted)
                    {
                        currentArgument.Append(currentChar);
                    }
                    else
                    {
                        doubleQuoteStarted = true;
                    }
                }
                else
                {
                    currentArgument.Append(currentChar);
                }
            }

            if (currentArgument.Length > 0)
            {
                argumentList.Add(currentArgument.ToString());
            }

            return argumentList.ToArray();
        }

        /**
     * <p>Returns all FFmpeg sessions in the session history.
     *
     * @return all FFmpeg sessions in the session history
     */
        public static List<FFmpegSession> getFFmpegSessions()
        {
            List<FFmpegSession> list = new List<FFmpegSession>();

            lock(sessionHistoryLock) 
            {
                foreach (AbstractSession session in sessionHistoryList)
                {
                    if (session.isFFmpeg())
                    {
                        list.Add((FFmpegSession)session);
                    }
                }
            }

            return list;
        }

        /**
     * <p>Returns all FFprobe sessions in the session history.
     *
     * @return all FFprobe sessions in the session history
     */
        public static List<FFprobeSession> getFFprobeSessions()
        {
            List<FFprobeSession> list = new List<FFprobeSession>();

            lock(sessionHistoryLock) 
            {
                foreach (AbstractSession session in sessionHistoryList)
                {
                    if (session.isFFprobe())
                    {
                        list.Add((FFprobeSession)session);
                    }
                }
            }

            return list;
        }

    }
}


#endif