#if GAMEMODE_ALLINONE

using System;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Assets.Scripts.FFmpegKit
{
    /**
     * <p>A custom FFprobe session, which produces a <code>MediaInformation</code> object using the
     * FFprobe output.
     */
    public class MediaInformationSession : AbstractSession
    {


        /**
         * Media information extracted in the session.
         */
        private MediaInformation mediaInformation;

        /**
         * Session specific complete callback.
         */
        private MediaInformationSessionCompleteCallback completeCallback;

        /**
         * Creates a new media information session.
         *
         * @param arguments command arguments
         */
        public MediaInformationSession(String[] arguments) : this(arguments, null)
        {
        }

        /**
         * Creates a new media information session.
         *
         * @param arguments        command arguments
         * @param completeCallback session specific complete callback
         */
        public MediaInformationSession(String[] arguments, MediaInformationSessionCompleteCallback completeCallback) : this(arguments, completeCallback, null)
        {
        }

        /**
         * Creates a new media information session.
         *
         * @param arguments        command arguments
         * @param completeCallback session specific complete callback
         * @param logCallback      session specific log callback
         */
        public MediaInformationSession(String[] arguments, MediaInformationSessionCompleteCallback completeCallback, LogCallback logCallback) : base(arguments, logCallback, LogRedirectionStrategy.NEVER_PRINT_LOGS)
        {

            this.completeCallback = completeCallback;
        }

        /**
         * Returns the media information extracted in this session.
         *
         * @return media information extracted or null if the command failed or the output can not be
         * parsed
         */
        public MediaInformation getMediaInformation()
        {
            return mediaInformation;
        }

        /**
         * Sets the media information extracted in this session.
         *
         * @param mediaInformation media information extracted
         */
        public void setMediaInformation(MediaInformation mediaInformation)
        {
            this.mediaInformation = mediaInformation;
        }

        /**
         * Returns the session specific complete callback.
         *
         * @return session specific complete callback
         */
        public MediaInformationSessionCompleteCallback getCompleteCallback()
        {
            return completeCallback;
        }

        public override bool isFFmpeg()
        {
            return false;
        }

        public override bool isFFprobe()
        {
            return false;
        }

        public bool isMediaInformation()
        {
            return true;
        }
    

        public override String ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("MediaInformationSession{");
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
