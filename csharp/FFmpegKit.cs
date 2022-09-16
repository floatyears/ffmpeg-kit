#if GAMEMODE_ALLINONE

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Assets.Scripts.FFmpegKit
{

    public class FFmpegKit
    {

        /**
         * Default constructor hidden.
         */
        private FFmpegKit() {
        }

        /**
         * <p>Synchronously executes FFmpeg with arguments provided.
         *
         * @param arguments FFmpeg command options/arguments as string array
         * @return FFmpeg session created for this execution
         */
        public static FFmpegSession executeWithArguments(string[] arguments) {
            FFmpegSession session = new FFmpegSession(arguments);

            FFmpegKitConfig.ffmpegExecute(session);

            return session;
        }

        /**
         * <p>Starts an asynchronous FFmpeg execution with arguments provided.
         *
         * <p>Note that this method returns immediately and does not wait the execution to complete.
         * You must use an {@link FFmpegSessionCompleteCallback} if you want to be notified about the
         * result.
         *
         * @param arguments        FFmpeg command options/arguments as string array
         * @param completeCallback callback that will be called when the execution has completed
         * @return FFmpeg session created for this execution
         */
        public static FFmpegSession executeWithArgumentsAsync(string[] arguments,
                                                              AbstractSession.FFmpegSessionCompleteCallback completeCallback) {
            FFmpegSession session = new FFmpegSession(arguments, completeCallback);

            FFmpegKitConfig.asyncFFmpegExecute(session);

            return session;
        }

        /**
         * <p>Starts an asynchronous FFmpeg execution with arguments provided.
         *
         * <p>Note that this method returns immediately and does not wait the execution to complete.
         * You must use an {@link FFmpegSessionCompleteCallback} if you want to be notified about the
         * result.
         *
         * @param arguments          FFmpeg command options/arguments as string array
         * @param completeCallback   callback that will be called when the execution has completed
         * @param logCallback        callback that will receive logs
         * @param statisticsCallback callback that will receive statistics
         * @return FFmpeg session created for this execution
         */
        public static FFmpegSession executeWithArgumentsAsync(String[] arguments,
                                                              AbstractSession.FFmpegSessionCompleteCallback completeCallback,
                                                              AbstractSession.LogCallback logCallback,
                                                              AbstractSession.StatisticsCallback statisticsCallback) {
            FFmpegSession session = new FFmpegSession(arguments, completeCallback, logCallback, statisticsCallback);

            FFmpegKitConfig.asyncFFmpegExecute(session);

            return session;
        }

        /**
         * <p>Starts an asynchronous FFmpeg execution with arguments provided.
         *
         * <p>Note that this method returns immediately and does not wait the execution to complete.
         * You must use an {@link FFmpegSessionCompleteCallback} if you want to be notified about the
         * result.
         *
         * @param arguments        FFmpeg command options/arguments as string array
         * @param completeCallback callback that will be called when the execution has completed
         * @param executorService  executor service that will be used to run this asynchronous operation
         * @return FFmpeg session created for this execution
         */
        public static FFmpegSession executeWithArgumentsAsync(String[] arguments,
                                                              AbstractSession.FFmpegSessionCompleteCallback completeCallback,
                                                              ConcurrentQueue<AbstractSession> executorService) {
            FFmpegSession session = new FFmpegSession(arguments, completeCallback);

            FFmpegKitConfig.asyncFFmpegExecute(session, executorService);

            return session;
        }

        /**
         * <p>Starts an asynchronous FFmpeg execution with arguments provided.
         *
         * <p>Note that this method returns immediately and does not wait the execution to complete.
         * You must use an {@link FFmpegSessionCompleteCallback} if you want to be notified about the
         * result.
         *
         * @param arguments          FFmpeg command options/arguments as string array
         * @param completeCallback   callback that will be called when the execution has completed
         * @param logCallback        callback that will receive logs
         * @param statisticsCallback callback that will receive statistics
         * @param executorService    executor service that will be used to run this asynchronous
         *                           operation
         * @return FFmpeg session created for this execution
         */
        public static FFmpegSession executeWithArgumentsAsync(String[] arguments,
                                                              AbstractSession.FFmpegSessionCompleteCallback completeCallback,
                                                              AbstractSession.LogCallback logCallback,
                                                              AbstractSession.StatisticsCallback statisticsCallback,
                                                              ConcurrentQueue<AbstractSession> executorService) {
            FFmpegSession session = new FFmpegSession(arguments, completeCallback, logCallback, statisticsCallback);

            FFmpegKitConfig.asyncFFmpegExecute(session, executorService);

            return session;
        }

        /**
         * <p>Synchronously executes FFmpeg command provided. Space character is used to split command
         * into arguments. You can use single or double quote characters to specify arguments inside
         * your command.
         *
         * @param command FFmpeg command
         * @return FFmpeg session created for this execution
         */
        public static FFmpegSession execute(String command) {
            return executeWithArguments(FFmpegKitConfig.ParseArguments(command));
        }

        /**
         * <p>Starts an asynchronous FFmpeg execution for the given command. Space character is used to
         * split the command into arguments. You can use single or double quote characters to specify
         * arguments inside your command.
         *
         * <p>Note that this method returns immediately and does not wait the execution to complete.
         * You must use an {@link FFmpegSessionCompleteCallback} if you want to be notified about the
         * result.
         *
         * @param command          FFmpeg command
         * @param completeCallback callback that will be called when the execution has completed
         * @return FFmpeg session created for this execution
         */
        public static FFmpegSession executeAsync(String command,
                                                 AbstractSession.FFmpegSessionCompleteCallback completeCallback) {
            return executeWithArgumentsAsync(FFmpegKitConfig.ParseArguments(command), completeCallback);
        }

        /**
         * <p>Starts an asynchronous FFmpeg execution for the given command. Space character is used to
         * split the command into arguments. You can use single or double quote characters to specify
         * arguments inside your command.
         *
         * <p>Note that this method returns immediately and does not wait the execution to complete.
         * You must use an {@link FFmpegSessionCompleteCallback} if you want to be notified about the
         * result.
         *
         * @param command            FFmpeg command
         * @param completeCallback   callback that will be called when the execution has completed
         * @param logCallback        callback that will receive logs
         * @param statisticsCallback callback that will receive statistics
         * @return FFmpeg session created for this execution
         */
        public static FFmpegSession executeAsync(String command,
                                                 AbstractSession.FFmpegSessionCompleteCallback completeCallback,
                                                 AbstractSession.LogCallback logCallback,
                                                 AbstractSession.StatisticsCallback statisticsCallback) {
            return executeWithArgumentsAsync(FFmpegKitConfig.ParseArguments(command), completeCallback, logCallback, statisticsCallback);
        }

        /**
         * <p>Starts an asynchronous FFmpeg execution for the given command. Space character is used to
         * split the command into arguments. You can use single or double quote characters to specify
         * arguments inside your command.
         *
         * <p>Note that this method returns immediately and does not wait the execution to complete.
         * You must use an {@link FFmpegSessionCompleteCallback} if you want to be notified about the
         * result.
         *
         * @param command          FFmpeg command
         * @param completeCallback callback that will be called when the execution has completed
         * @param executorService  executor service that will be used to run this asynchronous operation
         * @return FFmpeg session created for this execution
         */
        public static FFmpegSession executeAsync(String command,
                                                 AbstractSession.FFmpegSessionCompleteCallback completeCallback,
                                                 ConcurrentQueue<AbstractSession> executorService) {
            FFmpegSession session = new FFmpegSession(FFmpegKitConfig.ParseArguments(command), completeCallback);

            FFmpegKitConfig.asyncFFmpegExecute(session, executorService);

            return session;
        }

        /**
         * <p>Starts an asynchronous FFmpeg execution for the given command. Space character is used to
         * split the command into arguments. You can use single or double quote characters to specify
         * arguments inside your command.
         *
         * <p>Note that this method returns immediately and does not wait the execution to complete.
         * You must use an {@link FFmpegSessionCompleteCallback} if you want to be notified about the
         * result.
         *
         * @param command            FFmpeg command
         * @param completeCallback   callback that will be called when the execution has completed
         * @param logCallback        callback that will receive logs
         * @param statisticsCallback callback that will receive statistics
         * @param executorService    executor service that will be used to run this asynchronous operation
         * @return FFmpeg session created for this execution
         */
        public static FFmpegSession executeAsync(String command,
                                                 AbstractSession.FFmpegSessionCompleteCallback completeCallback,
                                                 AbstractSession.LogCallback logCallback,
                                                 AbstractSession.StatisticsCallback statisticsCallback,
                                                 ConcurrentQueue<AbstractSession> executorService) {
            FFmpegSession session = new FFmpegSession(FFmpegKitConfig.ParseArguments(command), completeCallback, logCallback, statisticsCallback);

            FFmpegKitConfig.asyncFFmpegExecute(session, executorService);

            return session;
        }

        /**
         * <p>Cancels all running sessions.
         *
         * <p>This method does not wait for termination to complete and returns immediately.
         */
        public static void cancel() {

            /*
             * ZERO (0) IS A SPECIAL SESSION ID
             * WHEN IT IS PASSED TO THIS METHOD, A SIGINT IS GENERATED WHICH CANCELS ALL ONGOING
             * SESSIONS
             */
            FFmpegKitConfig.nativeFFmpegCancel(0);
        }

        /**
         * <p>Cancels the session specified with <code>sessionId</code>.
         *
         * <p>This method does not wait for termination to complete and returns immediately.
         *
         * @param sessionId id of the session that will be cancelled
         */
        public static void cancel(long sessionId) {
            FFmpegKitConfig.nativeFFmpegCancel(sessionId);
        }

        /**
         * <p>Lists all FFmpeg sessions in the session history.
         *
         * @return all FFmpeg sessions in the session history
         */
        public static List<FFmpegSession> listSessions() {
            return FFmpegKitConfig.getFFmpegSessions();
        }

    }
}

#endif
