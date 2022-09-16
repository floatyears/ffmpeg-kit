#if GAMEMODE_ALLINONE
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.FFmpegKit
{
    /**
     * <p>An FFmpeg session.
     */
    public class FFmpegSession : AbstractSession
    {

        /**
         * Session specific statistics callback.
         */
        private StatisticsCallback statisticsCallback;

        /**
         * Session specific complete callback.
         */
        private FFmpegSessionCompleteCallback completeCallback;

        /**
         * Statistics entries received for this session.
         */
        private List<Statistics> statistics;

        /**
         * Statistics entry lock.
         */
        private object statisticsLock;

        /**
         * Builds a new FFmpeg session.
         *
         * @param arguments command arguments
         */
        public FFmpegSession(string[] arguments) : this(arguments, null)
        {
        }

        /**
         * Builds a new FFmpeg session.
         *
         * @param arguments        command arguments
         * @param completeCallback session specific complete callback
         */
        public FFmpegSession(string[] arguments, FFmpegSessionCompleteCallback completeCallback) : this(arguments, completeCallback, null, null)
        {
            
        }

        /**
         * Builds a new FFmpeg session.
         *
         * @param arguments          command arguments
         * @param completeCallback   session specific complete callback
         * @param logCallback        session specific log callback
         * @param statisticsCallback session specific statistics callback
         */
        public FFmpegSession(string[] arguments,
                             FFmpegSessionCompleteCallback completeCallback,
                             LogCallback logCallback,
                             StatisticsCallback statisticsCallback) : this(arguments, completeCallback, logCallback, statisticsCallback, FFmpegKitConfig.GetLogRedirectionStrategy())
        {
        }

        /**
         * Builds a new FFmpeg session.
         *
         * @param arguments              command arguments
         * @param completeCallback       session specific complete callback
         * @param logCallback            session specific log callback
         * @param statisticsCallback     session specific statistics callback
         * @param logRedirectionStrategy session specific log redirection strategy
         */
        public FFmpegSession(string[] arguments,
                             FFmpegSessionCompleteCallback completeCallback,
                             LogCallback logCallback,
                             StatisticsCallback statisticsCallback,
                             LogRedirectionStrategy logRedirectionStrategy) : base(arguments, logCallback, logRedirectionStrategy) {
            this.completeCallback = completeCallback;
            this.statisticsCallback = statisticsCallback;

            this.statistics = new List<Statistics>();
            this.statisticsLock = new Object();
        }

        /**
         * Returns the session specific statistics callback.
         *
         * @return session specific statistics callback
         */
        public StatisticsCallback getStatisticsCallback() {
            return statisticsCallback;
        }

        /**
         * Returns the session specific complete callback.
         *
         * @return session specific complete callback
         */
        public FFmpegSessionCompleteCallback getCompleteCallback() {
            return completeCallback;
        }

        /**
         * Returns all statistics entries generated for this session. If there are asynchronous
         * messages that are not delivered yet, this method waits for them until the given timeout.
         *
         * @param waitTimeout wait timeout for asynchronous messages in milliseconds
         * @return list of statistics entries generated for this session
         */
        public List<Statistics> getAllStatistics(int waitTimeout) {
            waitForAsynchronousMessagesInTransmit(waitTimeout);

            if (thereAreAsynchronousMessagesInTransmit()) {
                //android.util.Log.i(FFmpegKitConfig.TAG, String.format("getAllStatistics was called to return all statistics but there are still statistics being transmitted for session id %d.", sessionId));
                ocsys.NSFormatLog(true, "getAllStatistics was called to return all statistics but there are still statistics being transmitted for session id {0}.", sessionId);
            }

            return getStatistics();
        }

        /**
         * Returns all statistics entries generated for this session. If there are asynchronous
         * messages that are not delivered yet, this method waits for them until
         * {@link #DEFAULT_TIMEOUT_FOR_ASYNCHRONOUS_MESSAGES_IN_TRANSMIT} expires.
         *
         * @return list of statistics entries generated for this session
         */
        public List<Statistics> getAllStatistics() {
            return getAllStatistics(DEFAULT_TIMEOUT_FOR_ASYNCHRONOUS_MESSAGES_IN_TRANSMIT);
        }

        /**
         * Returns all statistics entries delivered for this session. Note that if there are
         * asynchronous messages that are not delivered yet, this method will not wait for
         * them and will return immediately.
         *
         * @return list of statistics entries received for this session
         */
        public List<Statistics> getStatistics() {
            lock(statisticsLock) 
            {
                return statistics;
            }
        }

        /**
         * Returns the last received statistics entry.
         *
         * @return the last received statistics entry or null if there are not any statistics entries
         * received
         */
        public Statistics getLastReceivedStatistics() {
            lock(statisticsLock) 
            {
                if (statistics.Count > 0) {
                    return statistics[statistics.Count - 1];
                } else {
                    return null;
                }
            }
        }

        /**
         * Adds a new statistics entry for this session. It is invoked internally by
         * <code>FFmpegKit</code> library methods. Must not be used by user applications.
         *
         * @param statistics statistics entry
         */
        public void addStatistics(Statistics statistics) {
            lock(statisticsLock) 
            {
                this.statistics.Add(statistics);
            }
        }

        public override bool isFFmpeg() {
            return true;
        }

        public override bool isFFprobe() {
            return false;
        }

        public bool isMediaInformation() {
            return false;
        }

        public override string ToString() {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("FFmpegSession{");
            stringBuilder.Append("sessionId=");
            stringBuilder.Append(sessionId);
            stringBuilder.Append(", createTime=");
            stringBuilder.Append(createTime);
            stringBuilder.Append(", startTime=");
            stringBuilder.Append(startTime);
            stringBuilder.Append(", endTime=");
            stringBuilder.Append(endTime);
            stringBuilder.Append(", arguments=");
            stringBuilder.Append(FFmpegKitConfig.ArgumentsToString(arguments));
            stringBuilder.Append(", logs=");
            stringBuilder.Append(getLogsAsString());
            stringBuilder.Append(", state=");
            stringBuilder.Append(state);
            stringBuilder.Append(", returnCode=");
            stringBuilder.Append(returnCode);
            stringBuilder.Append(", failStackTrace=");
            stringBuilder.Append('\'');
            stringBuilder.Append(failStackTrace);
            stringBuilder.Append('\'');
            stringBuilder.Append('}');

            return stringBuilder.ToString();
        }

    }
}

#endif