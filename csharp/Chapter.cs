#if GAMEMODE_ALLINONE

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Assets.Scripts.FFmpegKit
{

    public class Chapter {

    /* KEYS */
    public static String KEY_ID = "id";
    public static String KEY_TIME_BASE = "time_base";
    public static String KEY_START = "start";
    public static String KEY_START_TIME = "start_time";
    public static String KEY_END = "end";
    public static String KEY_END_TIME = "end_time";
    public static String KEY_TAGS = "tags";

    private JObject JObject;

    public Chapter(JObject JObject) {
        this.JObject = JObject;
    }

    public long? getId() {
        return getNumberProperty(KEY_ID);
    }

    public String getTimeBase() {
        return getStringProperty(KEY_TIME_BASE);
    }

    public long? getStart() {
        return getNumberProperty(KEY_START);
    }

    public String getStartTime() {
        return getStringProperty(KEY_START_TIME);
    }

    public long? getEnd() {
        return getNumberProperty(KEY_END);
    }

    public String getEndTime() {
        return getStringProperty(KEY_END_TIME);
    }

    public JObject getTags() {
        return getProperties(KEY_TAGS);
    }

    /**
     * Returns the chapter property associated with the key.
     *
     * @param key property key
     * @return chapter property as string or null if the key is not found
     */
    public String getStringProperty(String key) {
        JObject chapterProperties = getAllProperties();
        if (chapterProperties == null) {
            return null;
        }

        JToken value = null;
        if (chapterProperties.TryGetValue(key, out value)) {
            return (string)value;
        } else {
            return null;
        }
    }

    /**
     * Returns the chapter property associated with the key.
     *
     * @param key property key
     * @return chapter property as Long or null if the key is not found
     */
    public long? getNumberProperty(String key) {
        JObject chapterProperties = getAllProperties();
        if (chapterProperties == null) {
            return null;
        }

        JToken value = null;
        if (chapterProperties.TryGetValue(key, out value)) {
            return (long?)value;
        } else {
            return null;
        }
    }

    /**
     * Returns the chapter properties associated with the key.
     *
     * @param key properties key
     * @return chapter properties as a JObject or null if the key is not found
     */
    public JObject getProperties(String key) {
        JObject chapterProperties = getAllProperties();
        if (chapterProperties == null) {
            return null;
        }

        JToken value = null;
        if(chapterProperties.TryGetValue(key, out value))
        {
            return (JObject)value;
        }
        else
        {
            return null;
        }
    }

    /**
     * Returns all chapter properties defined.
     *
     * @return all chapter properties as a JObject or null if no properties are defined
     */
    public JObject getAllProperties() {
        return JObject;
    }

}

}
#endif