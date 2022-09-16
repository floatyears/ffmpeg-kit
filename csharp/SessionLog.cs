#if GAMEMODE_ALLINONE

using System;
using System.Text;

namespace Assets.Scripts.FFmpegKit
{
    public enum SessionLogLevel
    {

        /**
         * This log level is defined by FFmpegKit. It is used to specify logs printed to stderr by
         * FFmpeg. Logs that has this level are not filtered and always redirected.
         */
        AV_LOG_STDERR = -16,

        /**
         * Print no output.
         */
        AV_LOG_QUIET = -8,

        /**
         * Something went really wrong and we will crash now.
         */
        AV_LOG_PANIC = 0,

        /**
         * Something went wrong and recovery is not possible.
         * For example, no header was found for a format which depends
         * on headers or an illegal combination of parameters is used.
         */
        AV_LOG_FATAL = 8,

        /**
         * Something went wrong and cannot losslessly be recovered.
         * However, not all future data is affected.
         */
        AV_LOG_ERROR = 16,

        /**
         * Something somehow does not look correct. This may or may not
         * lead to problems. An example would be the use of '-vstrict -2'.
         */
        AV_LOG_WARNING = 24,

        /**
         * Standard information.
         */
        AV_LOG_INFO = 32,

        /**
         * Detailed information.
         */
        AV_LOG_VERBOSE = 40,

        /**
         * Stuff which is only useful for libav* developers.
         */
        AV_LOG_DEBUG = 48,

        /**
         * Extremely verbose debugging, useful for libav* development.
         */
        AV_LOG_TRACE = 56,
    }

    /**
     * <p>Log entry for an <code>FFmpegKit</code> session.
     */
    public class SessionLog
    {
        private long sessionId;
        private SessionLogLevel level;
        private String message;

        public SessionLog(long sessionId, SessionLogLevel level, String message) {
            this.sessionId = sessionId;
            this.level = level;
            this.message = message;
        }

        public long getSessionId() {
            return sessionId;
        }

        public SessionLogLevel getLevel() {
            return level;
        }

        public String getMessage() {
            return message;
        }

        public String toString() {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("Log{");
            stringBuilder.Append("sessionId=");
            stringBuilder.Append(sessionId);
            stringBuilder.Append(", level=");
            stringBuilder.Append(level);
            stringBuilder.Append(", message=");
            stringBuilder.Append("\'");
            stringBuilder.Append(message);
            stringBuilder.Append('\'');
            stringBuilder.Append('}');

            return stringBuilder.ToString();
        }
    }

}
#endif