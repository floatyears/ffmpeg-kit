#if GAMEMODE_ALLINONE

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Assets.Scripts.FFmpegKit
{

    public enum SessionState
    {
        CREATED,
        RUNNING,
        FAILED,
        COMPLETED
    }

    public enum LogRedirectionStrategy
    {
        ALWAYS_PRINT_LOGS,
        PRINT_LOGS_WHEN_NO_CALLBACKS_DEFINED,
        PRINT_LOGS_WHEN_GLOBAL_CALLBACK_NOT_DEFINED,
        PRINT_LOGS_WHEN_SESSION_CALLBACK_NOT_DEFINED,
        NEVER_PRINT_LOGS
    }


    /**
     * Abstract session implementation which includes common features shared by <code>FFmpeg</code>,
     * <code>FFprobe</code> and <code>MediaInformation</code> sessions.
     */
    public abstract class AbstractSession // : Session 
    {
        public delegate void LogCallback(SessionLog log);

        public delegate void FFmpegSessionCompleteCallback();

        public delegate void FFprobeSessionCompleteCallback();

        public delegate void StatisticsCallback(Statistics statistics);

        public delegate void MediaInformationSessionCompleteCallback(MediaInformationSession session);

        /**
         * Generates unique ids for sessions.
         */
        //protected static final AtomicLong sessionIdGenerator = new AtomicLong(1);

        /**
         * Defines how long default "getAll" methods wait, in milliseconds.
         */
        public const int DEFAULT_TIMEOUT_FOR_ASYNCHRONOUS_MESSAGES_IN_TRANSMIT = 5000;

        /**
         * Session identifier.
         */
        protected long sessionId;

        /**
         * Session specific log callback.
         */
        protected LogCallback logCallback;

        /**
         * DateTime and time the session was created.
         */
        protected DateTime createTime;

        /**
         * DateTime and time the session was started.
         */
        protected DateTime startTime;

        /**
         * DateTime and time the session has ended.
         */
        protected DateTime endTime;

        /**
         * Command arguments as an array.
         */
        protected string[] arguments;

        /**
         * Log entries received for this session.
         */
        protected List<SessionLog> logs;

        /**
         * Log entry lock.
         */
        protected Object logsLock;

        /**
         * Future created for sessions executed asynchronously.
         */
        //protected Future<?> future;

        /**
         * State of the session.
         */
        protected SessionState state;

        /**
         * Return code for the completed sessions.
         */
        protected ReturnCode returnCode;

        /**
         * Stack trace of the error received while trying to execute this session.
         */
        protected string failStackTrace;

        /**
         * Session specific log redirection strategy.
         */
        protected LogRedirectionStrategy logRedirectionStrategy;

        /**
         * Creates a new abstract session.
         *
         * @param arguments              command arguments
         * @param logCallback            session specific log callback
         * @param logRedirectionStrategy session specific log redirection strategy
         */
        public AbstractSession(string[] arguments,
                               LogCallback logCallback,
                               LogRedirectionStrategy logRedirectionStrategy) {
            this.sessionId = Interlocked.Increment(ref this.sessionId);
            this.logCallback = logCallback;
            this.createTime = new DateTime();
            //this.startTime = null;
            //this.endTime = null;
            this.arguments = arguments;
            this.logs = new List<SessionLog>();
            this.logsLock = new Object();
            //this.future = null;
            this.state = SessionState.CREATED;
            this.returnCode = null;
            this.failStackTrace = null;
            this.logRedirectionStrategy = logRedirectionStrategy;

            FFmpegKitConfig.AddSession(this);
        }

        public LogCallback getLogCallback() {
            return logCallback;
        }

        public long getSessionId() {
            return sessionId;
        }

        public DateTime getCreateTime() {
            return createTime;
        }

        public DateTime getStartTime() {
            return startTime;
        }

        public DateTime getEndTime() {
            return endTime;
        }

        public long getDuration() 
        {
            DateTime startTime = this.startTime;
            DateTime endTime = this.endTime;
            if (startTime != null && endTime != null) {
                return (endTime.ToFileTime() - startTime.ToFileTime());
            }

            return 0;
        }

        public String[] getArguments() {
            return arguments;
        }

        public String getCommand() {
            return FFmpegKitConfig.ArgumentsToString(arguments);
        }

        public List<SessionLog> getAllLogs(int waitTimeout) {
            waitForAsynchronousMessagesInTransmit(waitTimeout);

            if (thereAreAsynchronousMessagesInTransmit()) {
                ocsys.NSFormatLog(true, "getAllLogs was called to return all logs but there are still logs being transmitted for session id {0}.", sessionId);
                //android.util.Log.i(FFmpegKitConfig.TAG, String.format("getAllLogs was called to return all logs but there are still logs being transmitted for session id %d.", sessionId));
            }

            return getLogs();
        }

        /**
         * Returns all log entries generated for this session. If there are asynchronous
         * messages that are not delivered yet, this method waits for them until
         * {@link #DEFAULT_TIMEOUT_FOR_ASYNCHRONOUS_MESSAGES_IN_TRANSMIT} expires.
         *
         * @return list of log entries generated for this session
         */
        public List<SessionLog> getAllLogs() {
            return getAllLogs(DEFAULT_TIMEOUT_FOR_ASYNCHRONOUS_MESSAGES_IN_TRANSMIT);
        }

        public List<SessionLog> getLogs() {
            lock(logsLock) 
            {
                return new List<SessionLog>(logs);
            }
        }

        public String getAllLogsAsString(int waitTimeout) {
            waitForAsynchronousMessagesInTransmit(waitTimeout);

            if (thereAreAsynchronousMessagesInTransmit()) {
                ocsys.NSFormatLog(true, "getAllLogsAsString was called to return all logs but there are still logs being transmitted for session id {0}.", sessionId);
                //android.util.Log.i(FFmpegKitConfig.TAG, String.format("getAllLogsAsString was called to return all logs but there are still logs being transmitted for session id %d.", sessionId));
            }

            return getLogsAsString();
        }

        /**
         * Returns all log entries generated for this session as a concatenated string. If there are
         * asynchronous messages that are not delivered yet, this method waits for them until
         * {@link #DEFAULT_TIMEOUT_FOR_ASYNCHRONOUS_MESSAGES_IN_TRANSMIT} expires.
         *
         * @return all log entries generated for this session as a concatenated string
         */
        public String getAllLogsAsString() {
            return getAllLogsAsString(DEFAULT_TIMEOUT_FOR_ASYNCHRONOUS_MESSAGES_IN_TRANSMIT);
        }

        public String getLogsAsString() {
            StringBuilder concatenatedString = new StringBuilder();

            lock(logsLock) {
                foreach (SessionLog log in logs) {
                    concatenatedString.Append(log.getMessage());
                }
            }

            return concatenatedString.ToString();
        }

        public String getOutput() {
            return getAllLogsAsString();
        }

        public SessionState getState() {
            return state;
        }

        public ReturnCode getReturnCode() {
            return returnCode;
        }

        public String getFailStackTrace() {
            return failStackTrace;
        }

        public LogRedirectionStrategy getLogRedirectionStrategy() {
            return logRedirectionStrategy;
        }

        public bool thereAreAsynchronousMessagesInTransmit() {
            return (FFmpegKitConfig.messagesInTransmit(sessionId) != 0);
        }

        public void addLog(SessionLog log) {
            lock(logsLock) 
            {
                this.logs.Add(log);
            }
        }

        public void cancel() {
            if (state == SessionState.RUNNING) {
                FFmpegKit.cancel(sessionId);
            }
        }

        /**
         * Waits for all asynchronous messages to be transmitted until the given timeout.
         *
         * @param timeout wait timeout in milliseconds
         */
        protected void waitForAsynchronousMessagesInTransmit(int timeout) 
        {
            var start = DateTime.Now;

            while (thereAreAsynchronousMessagesInTransmit() && (DateTime.Now - start).TotalMilliseconds < timeout) 
            {
                lock(this) 
                {
                    UnityEngine.Debug.Log("waitForAsynchronousMessagesInTransmit: " + start);
                    while(FFmpegKitConfig.consumeLogAndStatistics() == 0){
                        
                    }
                    // Thread.Sleep(10);
                    //try 
                    //{
                    //    wait(100);
                    //} catch (InterruptedException ignored) {
                    //}
                }
            }
        }

        /**
         * Sets the future created for this session.
         *
         * @param future future that runs this session asynchronously
         */
        //void setFuture(final Future<?> future) {
        //    this.future = future;
        //}

        /**
         * Starts running the session.
         */
        public void startRunning() {
            this.state = SessionState.RUNNING;
            this.startTime = DateTime.Now;
        }

        /**
         * Completes running the session with the provided return code.
         *
         * @param returnCode return code of the execution
         */
        public void complete(ReturnCode returnCode) {
            this.returnCode = returnCode;
            this.state = SessionState.COMPLETED;
            this.endTime = DateTime.Now;
        }

        /**
         * Ends running the session with a failure.
         *
         * @param exception execution received
         */
        public void fail(Exception exception) {
            //this.failStackTrace = Exception.getStackTraceString(exception);
            this.state = SessionState.FAILED;
            this.endTime = DateTime.Now;
        }

        /**
     * Returns whether it is an <code>FFmpeg</code> session or not.
     *
     * @return true if it is an <code>FFmpeg</code> session, false otherwise
     */
        public abstract bool isFFmpeg();

        /**
         * Returns whether it is an <code>FFprobe</code> session or not.
         *
         * @return true if it is an <code>FFprobe</code> session, false otherwise
         */
        public abstract bool isFFprobe();

    }

}

#endif