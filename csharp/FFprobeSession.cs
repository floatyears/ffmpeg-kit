#if GAMEMODE_ALLINONE

using System.IO;
using System.Text;
using System;


namespace Assets.Scripts.FFmpegKit
{

/**
 * <p>An FFprobe session.
 */
public class FFprobeSession : AbstractSession
{

    /**
     * Session specific complete callback.
     */
    private FFprobeSessionCompleteCallback completeCallback;

    /**
     * Builds a new FFprobe session.
     *
     * @param arguments command arguments
     */
    public FFprobeSession(string[] arguments) : this(arguments, null)
    {
    }

    /**
     * Builds a new FFprobe session.
     *
     * @param arguments        command arguments
     * @param completeCallback session specific complete callback
     */
    public FFprobeSession(string[] arguments, FFprobeSessionCompleteCallback completeCallback) : this(arguments, completeCallback, null)
    {
    }

    /**
     * Builds a new FFprobe session.
     *
     * @param arguments        command arguments
     * @param completeCallback session specific complete callback
     * @param logCallback      session specific log callback
     */
    public FFprobeSession(string[] arguments,
                          FFprobeSessionCompleteCallback completeCallback,
                          LogCallback logCallback) : this(arguments, completeCallback, logCallback, FFmpegKitConfig.GetLogRedirectionStrategy())
    {
    }

    /**
     * Builds a new FFprobe session.
     *
     * @param arguments              command arguments
     * @param completeCallback       session specific complete callback
     * @param logCallback            session specific log callback
     * @param logRedirectionStrategy session specific log redirection strategy
     */
    public FFprobeSession(string[] arguments,
                          FFprobeSessionCompleteCallback completeCallback,
                          LogCallback logCallback,
                          LogRedirectionStrategy logRedirectionStrategy) : base(arguments, logCallback, logRedirectionStrategy)
    {
        this.completeCallback = completeCallback;
    }

    /**
     * Returns the session specific complete callback.
     *
     * @return session specific complete callback
     */
    public FFprobeSessionCompleteCallback getCompleteCallback() {
        return completeCallback;
    }

    public override bool isFFmpeg() {
        return false;
    }

    public override bool isFFprobe() {
        return true;
    }

    public bool isMediaInformation() {
        return false;
    }

    public string toString() {
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append("FFprobeSession{");
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