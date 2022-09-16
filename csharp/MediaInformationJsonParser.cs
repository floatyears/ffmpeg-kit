#if GAMEMODE_ALLINONE

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Assets.Scripts.FFmpegKit
{

    /**
     * A parser that constructs {@link MediaInformation} from FFprobe's json output.
     */
    public class MediaInformationJsonParser {

    public static String KEY_STREAMS = "streams";
    public static String KEY_CHAPTERS = "chapters";

    /**
     * Extracts <code>MediaInformation</code> from the given FFprobe json output. Note that this
     * method does not throw {@link JSONException} as {@link #fromWithError(String)} does and
     * handles errors internally.
     *
     * @param ffprobeJsonOutput FFprobe json output
     * @return created {@link MediaInformation} instance of null if a parsing error occurs
     */
    public static MediaInformation from(String ffprobeJsonOutput) {
        try {
            return fromWithError(ffprobeJsonOutput);
        } catch (Exception e) {
            ocsys.NSFormatLog(true, "MediaInformation parsing failed, {0}", e.StackTrace);
            return null;
        }
    }

    /**
     * Extracts MediaInformation from the given FFprobe json output.
     *
     * @param ffprobeJsonOutput ffprobe json output
     * @return created {@link MediaInformation} instance
     * @throws JSONException if a parsing error occurs
     */
    public static MediaInformation fromWithError(String ffprobeJsonOutput) 
    {
        UnityEngine.Debug.Log("MediaInformationJsonParser output: " + ffprobeJsonOutput);
        JObject JObject = JObject.Parse(ffprobeJsonOutput);
        JToken tmp = null;
        JObject.TryGetValue(KEY_STREAMS, out tmp);
        JArray streamArray = (JArray)tmp;
        JObject.TryGetValue(KEY_CHAPTERS, out tmp);
        JArray chapterArray = (JArray)tmp;

        List<StreamInformation> streamList = new List<StreamInformation>();
        for (int i = 0; streamArray != null && i < streamArray.Count; i++) {
            JObject streamObject = (JObject)streamArray[i];
            if (streamObject != null) {
                streamList.Add(new StreamInformation(streamObject));
            }
        }

            List<Chapter> chapterList = new List<Chapter>();
        for (int i = 0; chapterArray != null && i < chapterArray.Count; i++) {
            JObject chapterObject = (JObject)chapterArray[i];
            if (chapterObject != null) {
                chapterList.Add(new Chapter(chapterObject));
            }
        }

        return new MediaInformation(JObject, streamList, chapterList);
    }

}

}
#endif