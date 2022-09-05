/*
 * Copyright (c) 2020-2021 Taner Sener
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

#ifndef FFPROBE_KIT_CSHARP_H
#define FFPROBE_KIT_CSHARP_H

/*
 * Class:     com_arthenica_ffmpegkit_FFmpegKitConfig
 * Method:    nativeFFprobeExecute
 * Signature: (J[Ljava/lang/String;)I
 */
extern "C" int FFmpegKitConfig_nativeFFprobeExecute(long id, char* stringArray[]);

#endif /* FFPROBE_KIT_H */