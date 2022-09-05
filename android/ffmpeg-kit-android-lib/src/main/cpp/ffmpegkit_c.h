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

#ifdef _MSC_VER
#define DllExport __declspec(dllexport)
#else
#if __CYGWIN__
#define DllExport	__declspec(dllexport)
#elif defined(__GNUC__)
#if __GNUC__ >= 4
#define  DllExport __attribute__((visibility("default")))
#else
#define DllExport __attribute__((dllexport))
#endif
#else
#define DllExport
#endif
#endif // MSVER

extern "C" void DllExport InitFunctions(void* _logFunc, void* _statisticsFunc, void* _safOpenFunc, void* _safCloseFunc);

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    enableNativeRedirection
 * Signature: ()V
 */
extern "C" void DllExport FFmpegKitConfig_enableNativeRedirection();

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    disableNativeRedirection
 * Signature: ()V
 */
extern "C" void DllExport FFmpegKitConfig_disableNativeRedirection();

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    setNativeLogLevel
 * Signature: (I)V
 */
extern "C" void DllExport FFmpegKitConfig_setNativeLogLevel(int);
L
/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    getNativeLogLevel
 * Signature: ()I
 */
extern "C" int DllExport FFmpegKitConfig_getNativeLogLevel();

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    getNativeFFmpegVersion
 * Signature: ()Ljava/lang/String;
 */
extern "C" char* DllExport FFmpegKitConfig_getNativeFFmpegVersion();

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    getNativeVersion
 * Signature: ()Ljava/lang/String;
 */
extern "C" char* DllExport FFmpegKitConfig_getNativeVersion();

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    nativeFFmpegExecute
 * Signature: (J[Ljava/lang/String;)I
 */
extern "C" int DllExport FFmpegKitConfig_nativeFFmpegExecute(long, char* []);

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    nativeFFmpegCancel
 * Signature: (J)V
 */
extern "C" void DllExport FFmpegKitConfig_nativeFFmpegCancel(long);

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    registerNewNativeFFmpegPipe
 * Signature: (Ljava/lang/String;)I
 */
extern "C" int DllExport FFmpegKitConfig_registerNewNativeFFmpegPipe(char* ffmpegPipePath);

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    getNativeBuildDate
 * Signature: ()Ljava/lang/String;
 */
extern "C" char* DllExport FFmpegKitConfig_getNativeBuildDate();

/**
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    setNativeEnvironmentVariable
 * Signature: (Ljava/lang/String;Ljava/lang/String;)I
 */
extern "C" int DllExport FFmpegKitConfig_setNativeEnvironmentVariable(char* variableName, char* variableValue);

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    ignoreNativeSignal
 * Signature: (I)V
 */
extern "C" void DllExport FFmpegKitConfig_ignoreNativeSignal(int signum);

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    messagesInTransmit
 * Signature: (J)I
 */
extern "C" int DllExport FFmpegKitConfig_messagesInTransmit(long id);

#endif /* FFMPEG_KIT_H */