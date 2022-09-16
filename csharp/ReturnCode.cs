#if GAMEMODE_ALLINONE

using System;

namespace Assets.Scripts.FFmpegKit
{

    public class ReturnCode
    {

        public static int SUCCESS = 0;

        public static int CANCEL = 255;

        private int value;

        public ReturnCode(int value) {
            this.value = value;
        }

        public static bool isSuccess(ReturnCode returnCode) {
            return (returnCode != null && returnCode.getValue() == SUCCESS);
        }

        public static bool isCancel(ReturnCode returnCode) {
            return (returnCode != null && returnCode.getValue() == CANCEL);
        }

        public int getValue() {
            return value;
        }

        public bool isValueSuccess() {
            return (value == SUCCESS);
        }

        public bool isValueError() {
            return ((value != SUCCESS) && (value != CANCEL));
        }

        public bool isValueCancel() {
            return (value == CANCEL);
        }


    }
}

#endif
