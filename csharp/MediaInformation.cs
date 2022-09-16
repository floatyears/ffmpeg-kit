#if GAMEMODE_ALLINONE

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Assets.Scripts.FFmpegKit
{
    /**
     * Media information class.
     */
    public class MediaInformation {

        /* COMMON KEYS */
        public static String KEY_MEDIA_PROPERTIES = "format";
        public static String KEY_FILENAME = "filename";
        public static String KEY_FORMAT = "format_name";
        public static String KEY_FORMAT_LONG = "format_long_name";
        public static String KEY_START_TIME = "start_time";
        public static String KEY_DURATION = "duration";
        public static String KEY_SIZE = "size";
        public static String KEY_BIT_RATE = "bit_rate";
        public static String KEY_TAGS = "tags";

        /**
         * Stores all properties.
         */
        private JObject jobject;

        /**
         * Stores streams.
         */
        private List<StreamInformation> streams;

        /**
         * Stores chapters.
         */
        private List<Chapter> chapters;

        public MediaInformation(JObject JObject, List<StreamInformation> streams, List<Chapter> chapters) {
            this.jobject = JObject;
            this.streams = streams;
            this.chapters = chapters;
        }

        /**
         * Returns file name.
         *
         * @return media file name
         */
        public String getFilename() {
            return getStringProperty(KEY_FILENAME);
        }

        /**
         * Returns format.
         *
         * @return media format
         */
        public String getFormat() {
            return getStringProperty(KEY_FORMAT);
        }

        /**
         * Returns long format.
         *
         * @return media long format
         */
        public String getLongFormat() {
            return getStringProperty(KEY_FORMAT_LONG);
        }

        /**
         * Returns duration.
         *
         * @return media duration in milliseconds
         */
        public String getDuration() {
            return getStringProperty(KEY_DURATION);
        }

        /**
         * Returns start time.
         *
         * @return media start time in milliseconds
         */
        public String getStartTime() {
            return getStringProperty(KEY_START_TIME);
        }

        /**
         * Returns size.
         *
         * @return media size in bytes
         */
        public String getSize() {
            return getStringProperty(KEY_SIZE);
        }

        /**
         * Returns bitrate.
         *
         * @return media bitrate in kb/s
         */
        public String getBitrate() {
            return getStringProperty(KEY_BIT_RATE);
        }

        /**
         * Returns all tags.
         *
         * @return tags dictionary
         */
        public JObject getTags() {
            return getProperties(KEY_TAGS);
        }

        /**
         * Returns all streams.
         *
         * @return list of streams
         */
        public List<StreamInformation> getStreams() {
            return streams;
        }

        /**
         * Returns all chapters.
         *
         * @return list of chapters
         */
        public List<Chapter> getChapters() {
            return chapters;
        }

        /**
         * Returns the media property associated with the key.
         *
         * @param key property key
         * @return media property as string or null if the key is not found
         */
        public String getStringProperty(String key) {
            JObject mediaProperties = getMediaProperties();
            if (mediaProperties == null) {
                return null;
            }

            JToken value = null;
            if (mediaProperties.TryGetValue(key, out value)) {
                return (string)value;
            } else {
                return null;
            }
        }

        /**
         * Returns the media property associated with the key.
         *
         * @param key property key
         * @return media property as Long or null if the key is not found
         */
        public long? getNumberProperty(String key) {
            JObject mediaProperties = getMediaProperties();
            if (mediaProperties == null) {
                return null;
            }

            JToken value = null;
            if (mediaProperties.TryGetValue(key, out value)) {
                return (long?)value;
            } else {
                return null;
            }
        }

        /**
         * Returns the media properties associated with the key.
         *
         * @param key properties key
         * @return media properties as a JObject or null if the key is not found
         */
        public JObject getProperties(String key) {
            JObject mediaProperties = getMediaProperties();
            if (mediaProperties == null) {
                return null;
            }

            JToken value = null;
            if(mediaProperties.TryGetValue(key, out value))
            {
                return (JObject)value;
            }
            else
            {
                return null;
            }
        }

        /**
         * Returns all media properties.
         *
         * @return all media properties as a JObject or null if no media properties are defined
         */
        public JObject getMediaProperties() {
            JToken value = null;
            if(jobject.TryGetValue(KEY_MEDIA_PROPERTIES, out value))
            {
                return (JObject)value;
            }
            else
            {
                return null;
            }
        }

        /**
         * Returns all properties defined.
         *
         * @return all properties as a JObject or null if no properties are defined
         */
        public JObject getAllProperties() {
            return jobject;
        }

    }
}

#endif
