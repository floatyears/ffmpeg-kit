/*
 * Copyright (c) 2018-2021 Taner Sener
 *
 * This file is part of FFmpegKit.
 *
 * FFmpegKit is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * FFmpegKit is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with FFmpegKit.  If not, see <http://www.gnu.org/licenses/>.
 */

#include <pthread.h>
#include <stdatomic.h>
#include <sys/types.h>
#include <sys/stat.h>

#include "config.h"
#include "libavcodec/jni.h"
#include "libavutil/bprint.h"
#include "libavutil/file.h"
#include "fftools_ffmpeg.h"
#include "ffmpegkit.h"
#include "ffmpegkit_c.h"
#include "ffprobekit.h"

typedef void (*LogFunc)(long sessionId, int logLevel, uint8_t* byteArray);

/** Global reference of log redirection method in Java */
static LogFunc logFunc;

typedef void (*StatisticsFunc)(long sessionId, int statisticsFrameNumber, float statisticsFps,
                        float statisticsQuality, int64_t statisticsSize, int statisticsTime, 
                        double statisticsBitrate, double statisticsSpeed);

static StatisticsFunc statisticsFunc;

typedef void (*SafOpenFunc)(int safId);
static SafOpenFunc safOpenFunc;

typedef void (*SafCloseFunc)(int fd);
static SafCloseFunc safCloseFunc;

pthread_t callbackThreadEx;

/** Forward declaration for function defined in fftools_ffmpeg.c */
int ffmpeg_execute(int argc, char **argv);

/**
 * Forwards callback messages to Java classes.
 */
void *callbackThreadFunctionEx() {
    // JNIEnv *env;
    // jint getEnvRc = (*globalVm)->GetEnv(globalVm, (void**) &env, JNI_VERSION_1_6);
    // if (getEnvRc != JNI_OK) {
    //     if (getEnvRc != JNI_EDETACHED) {
    //         LOGE("Callback thread failed to GetEnv for class %s with rc %d.\n", configClassName, getEnvRc);
    //         return NULL;
    //     }

    //     if ((*globalVm)->AttachCurrentThread(globalVm, &env, NULL) != 0) {
    //         LOGE("Callback thread failed to AttachCurrentThread for class %s.\n", configClassName);
    //         return NULL;
    //     }
    // }

    LOGD("Async callback block started.\n");

    while(redirectionEnabled) {

        struct CallbackData *callbackData = callbackDataRemove();
        if (callbackData != NULL) {
            if (callbackData->type == LogType) {

                // LOG CALLBACK

                int size = callbackData->logData.len;

                // jbyteArray byteArray = (jbyteArray) (*env)->NewByteArray(env, size);
                // (*env)->SetByteArrayRegion(env, byteArray, 0, size, callbackData->logData.str);
                // (*env)->CallStaticVoidMethod(env, configClass, logMethod, (jlong) callbackData->sessionId, callbackData->logLevel, byteArray);
                // (*env)->DeleteLocalRef(env, byteArray);
                if(logFunc != NULL)
                {
                    logFunc((long)callbackData->sessionId, callbackData->logLevel, callbackData->logData.str);
                }

                // CLEAN LOG DATA
                av_bprint_finalize(&callbackData->logData, NULL);

            } else {

                // STATISTICS CALLBACK

                // (*env)->CallStaticVoidMethod(env, configClass, statisticsMethod,
                //     (jlong) callbackData->sessionId, callbackData->statisticsFrameNumber,
                //     callbackData->statisticsFps, callbackData->statisticsQuality,
                //     callbackData->statisticsSize, callbackData->statisticsTime,
                //     callbackData->statisticsBitrate, callbackData->statisticsSpeed);
                if(statisticsFunc != NULL)
                {
                    statisticsFunc(callbackData->sessionId, callbackData->statisticsFrameNumber,
                    callbackData->statisticsFps, callbackData->statisticsQuality,
                    callbackData->statisticsSize, callbackData->statisticsTime,
                    callbackData->statisticsBitrate, callbackData->statisticsSpeed);
                }
            }

            atomic_fetch_sub(&sessionInTransitMessageCountMap[callbackData->sessionId % SESSION_MAP_SIZE], 1);

            // CLEAN STRUCT
            callbackData->next = NULL;
            av_free(callbackData);

        } else {
            monitorWait(100);
        }
    }

    // (*globalVm)->DetachCurrentThread(globalVm);

    LOGD("Async callback block stopped.\n");

    return NULL;
}

/**
 * Used by saf protocol; is expected to be called from a Java thread, therefore we don't need attach/detach
 */
int saf_open_c(int safId) {
    // JNIEnv *env = NULL;
    // (*globalVm)->GetEnv(globalVm, (void**) &env, JNI_VERSION_1_6);
    // return (*env)->CallStaticIntMethod(env, configClass, safOpenMethod, safId);

    if(safOpenFunc != NULL)
    {
        safOpenFunc(safId);
    }
}

/**
 * Used by saf protocol; is expected to be called from a Java thread, therefore we don't need attach/detach
 */
int saf_close_c(int fd) {
    // JNIEnv *env = NULL;
    // (*globalVm)->GetEnv(globalVm, (void**) &env, JNI_VERSION_1_6);
    // return (*env)->CallStaticIntMethod(env, configClass, safCloseMethod, fd);

    if(safCloseFunc != NULL)
    {
        safCloseFunc(fd);
    }
}

/**
 * Called when 'ffmpegkit' native library is loaded.
 *
 * @param vm pointer to the running virtual machine
 * @param reserved reserved
 * @return JNI version needed by 'ffmpegkit' library
 */
extern "C" void DllExport InitFunctions(void* _logFunc, void* _statisticsFunc, void* _safOpenFunc, void* _safCloseFunc) {
    // JNIEnv *env;
    // if ((*vm)->GetEnv(vm, (void**)(&env), JNI_VERSION_1_6) != JNI_OK) {
    //     LOGE("OnLoad failed to GetEnv for class %s.\n", configClassName);
    //     return JNI_FALSE;
    // }

    // jclass localConfigClass = (*env)->FindClass(env, configClassName);
    // if (localConfigClass == NULL) {
    //     LOGE("OnLoad failed to FindClass %s.\n", configClassName);
    //     return JNI_FALSE;
    // }

    // if ((*env)->RegisterNatives(env, localConfigClass, configMethods, 14) < 0) {
    //     LOGE("OnLoad failed to RegisterNatives for class %s.\n", configClassName);
    //     return JNI_FALSE;
    // }

    // jclass localStringClass = (*env)->FindClass(env, stringClassName);
    // if (localStringClass == NULL) {
    //     LOGE("OnLoad failed to FindClass %s.\n", stringClassName);
    //     return JNI_FALSE;
    // }

    // (*env)->GetJavaVM(env, &globalVm);

    logFunc = (LogFunc)_logFunc;// (*env)->GetStaticMethodID(env, localConfigClass, "log", "(JI[B)V");
    if (logFunc == NULL) {
        LOGE("OnLoad thread failed to GetStaticMethodID for %s.\n", "log");
        return;
    }

    statisticsFunc = (StatisticsFunc)_statisticsFunc;//(*env)->GetStaticMethodID(env, localConfigClass, "statistics", "(JIFFJIDD)V");
    if (statisticsFunc == NULL) {
        LOGE("OnLoad thread failed to GetStaticMethodID for %s.\n", "statistics");
        return;
    }

    safOpenFunc = (SafOpenFunc)_safOpenFunc;//(*env)->GetStaticMethodID(env, localConfigClass, "safOpen", "(I)I");
    if (safOpenFunc == NULL) {
        LOGE("OnLoad thread failed to GetStaticMethodID for %s.\n", "safOpen");
        return;
    }

    safCloseFunc = (SafCloseFunc)_safCloseFunc;//(*env)->GetStaticMethodID(env, localConfigClass, "safClose", "(I)I");
    if (safCloseFunc == NULL) {
        LOGE("OnLoad thread failed to GetStaticMethodID for %s.\n", "safClose");
        return;
    }

    // stringConstructor = (*env)->GetMethodID(env, localStringClass, "<init>", "([BLjava/lang/String;)V");
    // if (stringConstructor == NULL) {
    //     LOGE("OnLoad thread failed to GetMethodID for %s.\n", "<init>");
    //     return JNI_FALSE;
    // }

    // av_jni_set_java_vm(vm, NULL);

    // configClass = (jclass) ((*env)->NewGlobalRef(env, localConfigClass));
    // stringClass = (jclass) ((*env)->NewGlobalRef(env, localStringClass));
    
    //如果java内有初始化成功，那么C#的初始化可以跳过这部分内容
    if(globalVm == NULL)
    {
        callbackDataHead = NULL;
        callbackDataTail = NULL;
        
        for(int i = 0; i<SESSION_MAP_SIZE; i++) {
            atomic_init(&sessionMap[i], 0);
            atomic_init(&sessionInTransitMessageCountMap[i], 0);
        }

        mutexInit();
        monitorInit();

        redirectionEnabled = 0;
    
        av_set_saf_open(saf_open_c);
        av_set_saf_close(saf_close_c);
    }

    return;
}

/**
 * Sets log level.
 *
 * @param level log level
 */
extern "C" void DllExport FFmpegKitConfig_setNativeLogLevel(jint level) {
    configuredLogLevel = level;
}

/**
 * Returns current log level.
 *
 */
extern "C" int DllExport FFmpegKitConfig_getNativeLogLevel() {
    return configuredLogLevel;
}

/**
 * Enables log and statistics redirection.
 *
 */
extern "C" void DllExport FFmpegKitConfig_enableNativeRedirection() {
    mutexLock();

    if (redirectionEnabled != 0) {
        mutexUnlock();
        return;
    }
    redirectionEnabled = 1;

    mutexUnlock();

    int rc = pthread_create(&callbackThreadEx, 0, callbackThreadFunctionEx, 0);
    if (rc != 0) {
        LOGE("Failed to create callback thread (rc=%d).\n", rc);
        return;
    }

    av_log_set_callback(ffmpegkit_log_callback_function);
    set_report_callback(ffmpegkit_statistics_callback_function);
}

/**
 * Disables log and statistics redirection.
 *
 */
extern "C" void DllExport FFmpegKitConfig_disableNativeRedirection() {

    mutexLock();

    if (redirectionEnabled != 1) {
        mutexUnlock();
        return;
    }
    redirectionEnabled = 0;

    mutexUnlock();

    av_log_set_callback(av_log_default_callback);
    set_report_callback(NULL);

    monitorNotify();
}

/**
 * Returns FFmpeg version bundled within the library natively.
 *
 * @return FFmpeg version string
 */
extern "C" char* DllExport FFmpegKitConfig_getNativeFFmpegVersion() {
    return FFMPEG_VERSION;//(*env)->NewStringUTF(env, FFMPEG_VERSION);
}

/**
 * Returns FFmpegKit library version natively.
 *
 * @return FFmpegKit version string
 */
extern "C" char* DllExport FFmpegKitConfig_getNativeVersion() {
    return FFMPEG_KIT_VERSION;//(*env)->NewStringUTF(env, FFMPEG_KIT_VERSION);
}

/**
 * Synchronously executes FFmpeg natively with arguments provided.
 *
 * @param id session id
 * @param stringArray reference to the object holding FFmpeg command arguments
 * @return zero on successful execution, non-zero on error
 */
extern "C" int DllExport FFmpegKitConfig_nativeFFmpegExecute(long id, char* stringArray[], int programArgumentCount) {
    // jstring *tempArray = NULL;
    int argumentCount = 1;
    char **argv = NULL;

    // SETS DEFAULT LOG LEVEL BEFORE STARTING A NEW RUN
    av_log_set_level(configuredLogLevel);

    if (stringArray) {
        // int programArgumentCount = (*env)->GetArrayLength(env, stringArray);
        argumentCount = programArgumentCount + 1;

        // tempArray = (jstring *) av_malloc(sizeof(jstring) * programArgumentCount);
    }

    /* PRESERVE USAGE FORMAT
     *
     * ffmpeg <arguments>
     */
    argv = (char **)av_malloc(sizeof(char*) * (argumentCount));
    argv[0] = (char *)av_malloc(sizeof(char) * (strlen(LIB_NAME) + 1));
    strcpy(argv[0], LIB_NAME);

    // PREPARE ARRAY ELEMENTS
    if (stringArray) {
        for (int i = 0; i < (argumentCount - 1); i++) {
            // tempArray[i] = (jstring) (*env)->GetObjectArrayElement(env, stringArray, i);
            // if (tempArray[i] != NULL) 
            {
                argv[i + 1] = stringArray[i];//(char *) (*env)->GetStringUTFChars(env, tempArray[i], 0);
            }
        }
    }

    // REGISTER THE ID BEFORE STARTING THE SESSION
    globalSessionId = (long) id;
    addSession((long) id);

    resetMessagesInTransmit(globalSessionId);

    // RUN
    int returnCode = ffmpeg_execute(argumentCount, argv);

    // ALWAYS REMOVE THE ID FROM THE MAP
    removeSession((long) id);

    // CLEANUP
    // if (tempArray) {
    //     for (int i = 0; i < (argumentCount - 1); i++) {
    //         (*env)->ReleaseStringUTFChars(env, tempArray[i], argv[i + 1]);
    //     }

    //     av_free(tempArray);
    // }
    av_free(argv[0]);
    av_free(argv);

    return returnCode;
}

/**
 * Cancels an ongoing FFmpeg operation natively.
 *
 * @param id session id
 */
extern "C" void DllExport FFmpegKitConfig_nativeFFmpegCancel(long id) {
    cancel_operation(id);
}

/**
 * Creates natively a new named pipe to use in FFmpeg operations.
 *
 * @param ffmpegPipePath full path of ffmpeg pipe
 * @return zero on successful creation, non-zero on error
 */
extern "C" int DllExport FFmpegKitConfig_registerNewNativeFFmpegPipe(char* ffmpegPipePath) {
    const char *ffmpegPipePathString = ffmpegPipePath;//(*env)->GetStringUTFChars(env, ffmpegPipePath, 0);

    return mkfifo(ffmpegPipePathString, S_IRWXU | S_IRWXG | S_IROTH);
}

/**
 * Returns FFmpegKit library build date natively.
 *
 * @return FFmpegKit library build date
 */
extern "C" char* DllExport FFmpegKitConfig_getNativeBuildDate() {
    static char buildDate[10];
    sprintf(buildDate, "%d", FFMPEG_KIT_BUILD_DATE);
    return buildDate;//(*env)->NewStringUTF(env, buildDate);
}

/**
 * Sets an environment variable natively
 *
 * @param variableName environment variable name
 * @param variableValue environment variable value
 * @return zero on success, non-zero on error
 */
extern "C" int DllExport FFmpegKitConfig_setNativeEnvironmentVariable(char* variableName, char* variableValue) {
    const char *variableNameString = variableName;//(*env)->GetStringUTFChars(env, variableName, 0);
    const char *variableValueString = variableName;//(*env)->GetStringUTFChars(env, variableValue, 0);

    int rc = setenv(variableNameString, variableValueString, 1);

    // (*env)->ReleaseStringUTFChars(env, variableName, variableNameString);
    // (*env)->ReleaseStringUTFChars(env, variableValue, variableValueString);
    return rc;
}

/**
 * Registers a new ignored signal. Ignored signals are not handled by the library.
 *
 * @param signum signal number
 */
extern "C" void DllExport FFmpegKitConfig_ignoreNativeSignal(int signum) {
    if (signum == SIGQUIT) {
        handleSIGQUIT = 0;
    } else if (signum == SIGINT) {
        handleSIGINT = 0;
    } else if (signum == SIGTERM) {
        handleSIGTERM = 0;
    } else if (signum == SIGXCPU) {
        handleSIGXCPU = 0;
    } else if (signum == SIGPIPE) {
        handleSIGPIPE = 0;
    }
}

/**
 * Returns the number of native messages which are not transmitted to the Java callbacks for the
 * given session.
 *
 * @param id session id
 */
extern "C" int DllExport FFmpegKitConfig_messagesInTransmit(long id) {
    return atomic_load(&sessionInTransitMessageCountMap[id % SESSION_MAP_SIZE]);
}
