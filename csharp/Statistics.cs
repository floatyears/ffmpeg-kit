#if GAMEMODE_ALLINONE

using System;
using System.Text;

namespace Assets.Scripts.FFmpegKit 
{
    /**
     * <p>Statistics entry for an FFmpeg execute session.
     */
    public class Statistics
    {
        private long sessionId;
        private int videoFrameNumber;
        private float videoFps;
        private float videoQuality;
        private long size;
        private int time;
        private double bitrate;
        private double speed;

        public Statistics(long sessionId, int videoFrameNumber, float videoFps, float videoQuality, long size, int time, double bitrate, double speed) {
            this.sessionId = sessionId;
            this.videoFrameNumber = videoFrameNumber;
            this.videoFps = videoFps;
            this.videoQuality = videoQuality;
            this.size = size;
            this.time = time;
            this.bitrate = bitrate;
            this.speed = speed;
        }

        public long getSessionId() 
        {
            return sessionId;
        }

        public void setSessionId(long sessionId) 
        {
            this.sessionId = sessionId;
        }

        public int getVideoFrameNumber() 
        {
            return videoFrameNumber;
        }

        public void setVideoFrameNumber(int videoFrameNumber) 
        {
            this.videoFrameNumber = videoFrameNumber;
        }

        public float getVideoFps() 
        {
            return videoFps;
        }

        public void setVideoFps(float videoFps) 
        {
            this.videoFps = videoFps;
        }

        public float getVideoQuality() 
        {
            return videoQuality;
        }

        public void setVideoQuality(float videoQuality) 
        {
            this.videoQuality = videoQuality;
        }

        public long getSize() 
        {
            return size;
        }

        public void setSize(long size) 
        {
            this.size = size;
        }

        public int getTime() 
        {
            return time;
        }

        public void setTime(int time) 
        {
            this.time = time;
        }

        public double getBitrate() 
        {
            return bitrate;
        }

        public void setBitrate(double bitrate) 
        {
            this.bitrate = bitrate;
        }

        public double getSpeed() 
        {
            return speed;
        }

        public void setSpeed(double speed) 
        {
            this.speed = speed;
        }

        public override string ToString() 
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("Statistics{");
            stringBuilder.Append("sessionId=");
            stringBuilder.Append(sessionId);
            stringBuilder.Append(", videoFrameNumber=");
            stringBuilder.Append(videoFrameNumber);
            stringBuilder.Append(", videoFps=");
            stringBuilder.Append(videoFps);
            stringBuilder.Append(", videoQuality=");
            stringBuilder.Append(videoQuality);
            stringBuilder.Append(", size=");
            stringBuilder.Append(size);
            stringBuilder.Append(", time=");
            stringBuilder.Append(time);
            stringBuilder.Append(", bitrate=");
            stringBuilder.Append(bitrate);
            stringBuilder.Append(", speed=");
            stringBuilder.Append(speed);
            stringBuilder.Append('}');

            return stringBuilder.ToString();
        }

    }

}
#endif