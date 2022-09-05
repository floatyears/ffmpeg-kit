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

#ifndef FFMPEG_KIT_C_H
#define FFMPEG_KIT_C_H

#include <jni.h>
#include <android/log.h>

#include "libavutil/log.h"
#include "libavutil/ffversion.h"


/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    enableNativeRedirection
 * Signature: ()V
 */
extern "C" void FFmpegKitConfig_enableNativeRedirection(JNIEnv *, jclass);

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    disableNativeRedirection
 * Signature: ()V
 */
extern "C" void FFmpegKitConfig_disableNativeRedirection(JNIEnv *, jclass);

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    setNativeLogLevel
 * Signature: (I)V
 */
extern "C" void FFmpegKitConfig_setNativeLogLevel(JNIEnv *, jclass, jint);
L
/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    getNativeLogLevel
 * Signature: ()I
 */
extern "C" int FFmpegKitConfig_getNativeLogLevel(JNIEnv *, jclass);

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    getNativeFFmpegVersion
 * Signature: ()Ljava/lang/String;
 */
extern "C" char* FFmpegKitConfig_getNativeFFmpegVersion(JNIEnv *, jclass);

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    getNativeVersion
 * Signature: ()Ljava/lang/String;
 */
extern "C" char* FFmpegKitConfig_getNativeVersion(JNIEnv *, jclass);

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    nativeFFmpegExecute
 * Signature: (J[Ljava/lang/String;)I
 */
extern "C" int FFmpegKitConfig_nativeFFmpegExecute(JNIEnv *, jclass, jlong, jobjectArray);

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    nativeFFmpegCancel
 * Signature: (J)V
 */
extern "C" void FFmpegKitConfig_nativeFFmpegCancel(JNIEnv *, jclass, jlong);

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    registerNewNativeFFmpegPipe
 * Signature: (Ljava/lang/String;)I
 */
extern "C" int FFmpegKitConfig_registerNewNativeFFmpegPipe(JNIEnv *env, jclass object, jstring ffmpegPipePath);

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    getNativeBuildDate
 * Signature: ()Ljava/lang/String;
 */
extern "C" char* FFmpegKitConfig_getNativeBuildDate(JNIEnv *env, jclass object);

/**
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    setNativeEnvironmentVariable
 * Signature: (Ljava/lang/String;Ljava/lang/String;)I
 */
extern "C" int FFmpegKitConfig_setNativeEnvironmentVariable(JNIEnv *env, jclass object, jstring variableName, jstring variableValue);

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    ignoreNativeSignal
 * Signature: (I)V
 */
extern "C" void FFmpegKitConfig_ignoreNativeSignal(JNIEnv *env, jclass object, jint signum);

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    messagesInTransmit
 * Signature: (J)I
 */
extern "C" int FFmpegKitConfig_messagesInTransmit(JNIEnv *env, jclass object, jlong id);

#endif /* FFMPEG_KIT_H */