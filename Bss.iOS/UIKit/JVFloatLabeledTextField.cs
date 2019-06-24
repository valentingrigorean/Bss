﻿//
// JVFloatLabeledTextField.cs
//
// Author:
//       Valentin Grigorean <v.grigorean@software-dep.net>
//
// Copyright (c) 2016 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using UIKit;
using Foundation;
using CoreGraphics;

namespace Bss.iOS.UIKit
{

    public enum JVTextDirection
    {
        /// <summary>
        /// JVTextDirectionNeutral` indicates text with no directionality
        /// </summary>
        JVTextDirectionNeutral = 0,
        /// <summary>
        /// JVTextDirectionLeftToRight` indicates text left-to-right directionality
        /// </summary>
        JVTextDirectionLeftToRight,
        /// <summary>
        /// JVTextDirectionRightToLeft` indicates text right-to-left directionality
        /// </summary>
        JVTextDirectionRightToLeft,
    }

    public static class TextDirectionality
    {
        private static bool IsCodePointStrongRTL(int c)
        {
            return ((c == 0x5BE) || (c == 0x5C0) || (c == 0x5C3) || (c == 0x5C6) || (c >= 0x5D0 && c <= 0x5EA) || (c >= 0x5F0 && c <= 0x5F4) || (c == 0x608) || (c == 0x60B) || (c == 0x60D) || (c == 0x61B) || (c >= 0x61E && c <= 0x64A) || (c >= 0x66D && c <= 0x66F) || (c >= 0x671 && c <= 0x6D5) || (c >= 0x6E5 && c <= 0x6E6) || (c >= 0x6EE && c <= 0x6EF) || (c >= 0x6FA && c <= 0x70D) || (c >= 0x70F && c <= 0x710) || (c >= 0x712 && c <= 0x72F) || (c >= 0x74D && c <= 0x7A5) || (c == 0x7B1) || (c >= 0x7C0 && c <= 0x7EA) || (c >= 0x7F4 && c <= 0x7F5) || (c == 0x7FA) || (c >= 0x800 && c <= 0x815) || (c == 0x81A) || (c == 0x824) || (c == 0x828) || (c >= 0x830 && c <= 0x83E) || (c >= 0x840 && c <= 0x858) || (c == 0x85E) || (c == 0x8A0) || (c >= 0x8A2 && c <= 0x8AC) || (c == 0x200F) || (c == 0xFB1D) || (c >= 0xFB1F && c <= 0xFB28) || (c >= 0xFB2A && c <= 0xFB36) || (c >= 0xFB38 && c <= 0xFB3C) || (c == 0xFB3E) || (c >= 0xFB40 && c <= 0xFB41) || (c >= 0xFB43 && c <= 0xFB44) || (c >= 0xFB46 && c <= 0xFBC1) || (c >= 0xFBD3 && c <= 0xFD3D) || (c >= 0xFD50 && c <= 0xFD8F) || (c >= 0xFD92 && c <= 0xFDC7) || (c >= 0xFDF0 && c <= 0xFDFC) || (c >= 0xFE70 && c <= 0xFE74) || (c >= 0xFE76 && c <= 0xFEFC) || (c >= 0x10800 && c <= 0x10805) || (c == 0x10808) || (c >= 0x1080A && c <= 0x10835) || (c >= 0x10837 && c <= 0x10838) || (c == 0x1083C) || (c >= 0x1083F && c <= 0x10855) || (c >= 0x10857 && c <= 0x1085F) || (c >= 0x10900 && c <= 0x1091B) || (c >= 0x10920 && c <= 0x10939) || (c == 0x1093F) || (c >= 0x10980 && c <= 0x109B7) || (c >= 0x109BE && c <= 0x109BF) || (c == 0x10A00) || (c >= 0x10A10 && c <= 0x10A13) || (c >= 0x10A15 && c <= 0x10A17) || (c >= 0x10A19 && c <= 0x10A33) || (c >= 0x10A40 && c <= 0x10A47) || (c >= 0x10A50 && c <= 0x10A58) || (c >= 0x10A60 && c <= 0x10A7F) || (c >= 0x10B00 && c <= 0x10B35) || (c >= 0x10B40 && c <= 0x10B55) || (c >= 0x10B58 && c <= 0x10B72) || (c >= 0x10B78 && c <= 0x10B7F) || (c >= 0x10C00 && c <= 0x10C48) || (c >= 0x1EE00 && c <= 0x1EE03) || (c >= 0x1EE05 && c <= 0x1EE1F) || (c >= 0x1EE21 && c <= 0x1EE22) || (c == 0x1EE24) || (c == 0x1EE27) || (c >= 0x1EE29 && c <= 0x1EE32) || (c >= 0x1EE34 && c <= 0x1EE37) || (c == 0x1EE39) || (c == 0x1EE3B) || (c == 0x1EE42) || (c == 0x1EE47) || (c == 0x1EE49) || (c == 0x1EE4B) || (c >= 0x1EE4D && c <= 0x1EE4F) || (c >= 0x1EE51 && c <= 0x1EE52) || (c == 0x1EE54) || (c == 0x1EE57) || (c == 0x1EE59) || (c == 0x1EE5B) || (c == 0x1EE5D) || (c == 0x1EE5F) || (c >= 0x1EE61 && c <= 0x1EE62) || (c == 0x1EE64) || (c >= 0x1EE67 && c <= 0x1EE6A) || (c >= 0x1EE6C && c <= 0x1EE72) || (c >= 0x1EE74 && c <= 0x1EE77) || (c >= 0x1EE79 && c <= 0x1EE7C) || (c == 0x1EE7E) || (c >= 0x1EE80 && c <= 0x1EE89) || (c >= 0x1EE8B && c <= 0x1EE9B) || (c >= 0x1EEA1 && c <= 0x1EEA3) || (c >= 0x1EEA5 && c <= 0x1EEA9) || (c >= 0x1EEAB && c <= 0x1EEBB));
        }

        private static bool IsCodePointStrongLTR(int c)
        {
            return (c >= 0x41 && c <= 0x5A) || (c >= 0x61 && c <= 0x7A) || (c == 0xAA) || (c == 0xB5) || (c == 0xBA) || (c >= 0xC0 && c <= 0xD6) || (c >= 0xD8 && c <= 0xF6) || (c >= 0xF8 && c <= 0x2B8) || (c >= 0x2BB && c <= 0x2C1) || (c >= 0x2D0 && c <= 0x2D1) || (c >= 0x2E0 && c <= 0x2E4) || (c == 0x2EE) || (c >= 0x370 && c <= 0x373) || (c >= 0x376 && c <= 0x377) || (c >= 0x37A && c <= 0x37D) || (c == 0x386) || (c >= 0x388 && c <= 0x38A) || (c == 0x38C) || (c >= 0x38E && c <= 0x3A1) || (c >= 0x3A3 && c <= 0x3F5) || (c >= 0x3F7 && c <= 0x482) || (c >= 0x48A && c <= 0x527) || (c >= 0x531 && c <= 0x556) || (c >= 0x559 && c <= 0x55F) || (c >= 0x561 && c <= 0x587) || (c == 0x589) || (c >= 0x903 && c <= 0x939) || (c == 0x93B) || (c >= 0x93D && c <= 0x940) || (c >= 0x949 && c <= 0x94C) || (c >= 0x94E && c <= 0x950) || (c >= 0x958 && c <= 0x961) || (c >= 0x964 && c <= 0x977) || (c >= 0x979 && c <= 0x97F) || (c >= 0x982 && c <= 0x983) || (c >= 0x985 && c <= 0x98C) || (c >= 0x98F && c <= 0x990) || (c >= 0x993 && c <= 0x9A8) || (c >= 0x9AA && c <= 0x9B0) || (c == 0x9B2) || (c >= 0x9B6 && c <= 0x9B9) || (c >= 0x9BD && c <= 0x9C0) || (c >= 0x9C7 && c <= 0x9C8) || (c >= 0x9CB && c <= 0x9CC) || (c == 0x9CE) || (c == 0x9D7) || (c >= 0x9DC && c <= 0x9DD) || (c >= 0x9DF && c <= 0x9E1) || (c >= 0x9E6 && c <= 0x9F1) || (c >= 0x9F4 && c <= 0x9FA) || (c == 0xA03) || (c >= 0xA05 && c <= 0xA0A) || (c >= 0xA0F && c <= 0xA10) || (c >= 0xA13 && c <= 0xA28) || (c >= 0xA2A && c <= 0xA30) || (c >= 0xA32 && c <= 0xA33) || (c >= 0xA35 && c <= 0xA36) || (c >= 0xA38 && c <= 0xA39) || (c >= 0xA3E && c <= 0xA40) || (c >= 0xA59 && c <= 0xA5C) || (c == 0xA5E) || (c >= 0xA66 && c <= 0xA6F) || (c >= 0xA72 && c <= 0xA74) || (c == 0xA83) || (c >= 0xA85 && c <= 0xA8D) || (c >= 0xA8F && c <= 0xA91) || (c >= 0xA93 && c <= 0xAA8) || (c >= 0xAAA && c <= 0xAB0) || (c >= 0xAB2 && c <= 0xAB3) || (c >= 0xAB5 && c <= 0xAB9) || (c >= 0xABD && c <= 0xAC0) || (c == 0xAC9) || (c >= 0xACB && c <= 0xACC) || (c == 0xAD0) || (c >= 0xAE0 && c <= 0xAE1) || (c >= 0xAE6 && c <= 0xAF0) || (c >= 0xB02 && c <= 0xB03) || (c >= 0xB05 && c <= 0xB0C) || (c >= 0xB0F && c <= 0xB10) || (c >= 0xB13 && c <= 0xB28) || (c >= 0xB2A && c <= 0xB30) || (c >= 0xB32 && c <= 0xB33) || (c >= 0xB35 && c <= 0xB39) || (c >= 0xB3D && c <= 0xB3E) || (c == 0xB40) || (c >= 0xB47 && c <= 0xB48) || (c >= 0xB4B && c <= 0xB4C) || (c == 0xB57) || (c >= 0xB5C && c <= 0xB5D) || (c >= 0xB5F && c <= 0xB61) || (c >= 0xB66 && c <= 0xB77) || (c == 0xB83) || (c >= 0xB85 && c <= 0xB8A) || (c >= 0xB8E && c <= 0xB90) || (c >= 0xB92 && c <= 0xB95) || (c >= 0xB99 && c <= 0xB9A) || (c == 0xB9C) || (c >= 0xB9E && c <= 0xB9F) || (c >= 0xBA3 && c <= 0xBA4) || (c >= 0xBA8 && c <= 0xBAA) || (c >= 0xBAE && c <= 0xBB9) || (c >= 0xBBE && c <= 0xBBF) || (c >= 0xBC1 && c <= 0xBC2) || (c >= 0xBC6 && c <= 0xBC8) || (c >= 0xBCA && c <= 0xBCC) || (c == 0xBD0) || (c == 0xBD7) || (c >= 0xBE6 && c <= 0xBF2) || (c >= 0xC01 && c <= 0xC03) || (c >= 0xC05 && c <= 0xC0C) || (c >= 0xC0E && c <= 0xC10) || (c >= 0xC12 && c <= 0xC28) || (c >= 0xC2A && c <= 0xC33) || (c >= 0xC35 && c <= 0xC39) || (c == 0xC3D) || (c >= 0xC41 && c <= 0xC44) || (c >= 0xC58 && c <= 0xC59) || (c >= 0xC60 && c <= 0xC61) || (c >= 0xC66 && c <= 0xC6F) || (c == 0xC7F) || (c >= 0xC82 && c <= 0xC83) || (c >= 0xC85 && c <= 0xC8C) || (c >= 0xC8E && c <= 0xC90) || (c >= 0xC92 && c <= 0xCA8) || (c >= 0xCAA && c <= 0xCB3) || (c >= 0xCB5 && c <= 0xCB9) || (c >= 0xCBD && c <= 0xCC4) || (c >= 0xCC6 && c <= 0xCC8) || (c >= 0xCCA && c <= 0xCCB) || (c >= 0xCD5 && c <= 0xCD6) || (c == 0xCDE) || (c >= 0xCE0 && c <= 0xCE1) || (c >= 0xCE6 && c <= 0xCEF) || (c >= 0xCF1 && c <= 0xCF2) || (c >= 0xD02 && c <= 0xD03) || (c >= 0xD05 && c <= 0xD0C) || (c >= 0xD0E && c <= 0xD10) || (c >= 0xD12 && c <= 0xD3A) || (c >= 0xD3D && c <= 0xD40) || (c >= 0xD46 && c <= 0xD48) || (c >= 0xD4A && c <= 0xD4C) || (c == 0xD4E) || (c == 0xD57) || (c >= 0xD60 && c <= 0xD61) || (c >= 0xD66 && c <= 0xD75) || (c >= 0xD79 && c <= 0xD7F) || (c >= 0xD82 && c <= 0xD83) || (c >= 0xD85 && c <= 0xD96) || (c >= 0xD9A && c <= 0xDB1) || (c >= 0xDB3 && c <= 0xDBB) || (c == 0xDBD) || (c >= 0xDC0 && c <= 0xDC6) || (c >= 0xDCF && c <= 0xDD1) || (c >= 0xDD8 && c <= 0xDDF) || (c >= 0xDF2 && c <= 0xDF4) || (c >= 0xE01 && c <= 0xE30) || (c >= 0xE32 && c <= 0xE33) || (c >= 0xE40 && c <= 0xE46) || (c >= 0xE4F && c <= 0xE5B) || (c >= 0xE81 && c <= 0xE82) || (c == 0xE84) || (c >= 0xE87 && c <= 0xE88) || (c == 0xE8A) || (c == 0xE8D) || (c >= 0xE94 && c <= 0xE97) || (c >= 0xE99 && c <= 0xE9F) || (c >= 0xEA1 && c <= 0xEA3) || (c == 0xEA5) || (c == 0xEA7) || (c >= 0xEAA && c <= 0xEAB) || (c >= 0xEAD && c <= 0xEB0) || (c >= 0xEB2 && c <= 0xEB3) || (c == 0xEBD) || (c >= 0xEC0 && c <= 0xEC4) || (c == 0xEC6) || (c >= 0xED0 && c <= 0xED9) || (c >= 0xEDC && c <= 0xEDF) || (c >= 0xF00 && c <= 0xF17) || (c >= 0xF1A && c <= 0xF34) || (c == 0xF36) || (c == 0xF38) || (c >= 0xF3E && c <= 0xF47) || (c >= 0xF49 && c <= 0xF6C) || (c == 0xF7F) || (c == 0xF85) || (c >= 0xF88 && c <= 0xF8C) || (c >= 0xFBE && c <= 0xFC5) || (c >= 0xFC7 && c <= 0xFCC) || (c >= 0xFCE && c <= 0xFDA) || (c >= 0x1000 && c <= 0x102C) || (c == 0x1031) || (c == 0x1038) || (c >= 0x103B && c <= 0x103C) || (c >= 0x103F && c <= 0x1057) || (c >= 0x105A && c <= 0x105D) || (c >= 0x1061 && c <= 0x1070) || (c >= 0x1075 && c <= 0x1081) || (c >= 0x1083 && c <= 0x1084) || (c >= 0x1087 && c <= 0x108C) || (c >= 0x108E && c <= 0x109C) || (c >= 0x109E && c <= 0x10C5) || (c == 0x10C7) || (c == 0x10CD) || (c >= 0x10D0 && c <= 0x1248) || (c >= 0x124A && c <= 0x124D) || (c >= 0x1250 && c <= 0x1256) || (c == 0x1258) || (c >= 0x125A && c <= 0x125D) || (c >= 0x1260 && c <= 0x1288) || (c >= 0x128A && c <= 0x128D) || (c >= 0x1290 && c <= 0x12B0) || (c >= 0x12B2 && c <= 0x12B5) || (c >= 0x12B8 && c <= 0x12BE) || (c == 0x12C0) || (c >= 0x12C2 && c <= 0x12C5) || (c >= 0x12C8 && c <= 0x12D6) || (c >= 0x12D8 && c <= 0x1310) || (c >= 0x1312 && c <= 0x1315) || (c >= 0x1318 && c <= 0x135A) || (c >= 0x1360 && c <= 0x137C) || (c >= 0x1380 && c <= 0x138F) || (c >= 0x13A0 && c <= 0x13F4) || (c >= 0x1401 && c <= 0x167F) || (c >= 0x1681 && c <= 0x169A) || (c >= 0x16A0 && c <= 0x16F0) || (c >= 0x1700 && c <= 0x170C) || (c >= 0x170E && c <= 0x1711) || (c >= 0x1720 && c <= 0x1731) || (c >= 0x1735 && c <= 0x1736) || (c >= 0x1740 && c <= 0x1751) || (c >= 0x1760 && c <= 0x176C) || (c >= 0x176E && c <= 0x1770) || (c >= 0x1780 && c <= 0x17B3) || (c == 0x17B6) || (c >= 0x17BE && c <= 0x17C5) || (c >= 0x17C7 && c <= 0x17C8) || (c >= 0x17D4 && c <= 0x17DA) || (c == 0x17DC) || (c >= 0x17E0 && c <= 0x17E9) || (c >= 0x1810 && c <= 0x1819) || (c >= 0x1820 && c <= 0x1877) || (c >= 0x1880 && c <= 0x18A8) || (c == 0x18AA) || (c >= 0x18B0 && c <= 0x18F5) || (c >= 0x1900 && c <= 0x191C) || (c >= 0x1923 && c <= 0x1926) || (c >= 0x1929 && c <= 0x192B) || (c >= 0x1930 && c <= 0x1931) || (c >= 0x1933 && c <= 0x1938) || (c >= 0x1946 && c <= 0x196D) || (c >= 0x1970 && c <= 0x1974) || (c >= 0x1980 && c <= 0x19AB) || (c >= 0x19B0 && c <= 0x19C9) || (c >= 0x19D0 && c <= 0x19DA) || (c >= 0x1A00 && c <= 0x1A16) || (c >= 0x1A19 && c <= 0x1A1B) || (c >= 0x1A1E && c <= 0x1A55) || (c == 0x1A57) || (c == 0x1A61) || (c >= 0x1A63 && c <= 0x1A64) || (c >= 0x1A6D && c <= 0x1A72) || (c >= 0x1A80 && c <= 0x1A89) || (c >= 0x1A90 && c <= 0x1A99) || (c >= 0x1AA0 && c <= 0x1AAD) || (c >= 0x1B04 && c <= 0x1B33) || (c == 0x1B35) || (c == 0x1B3B) || (c >= 0x1B3D && c <= 0x1B41) || (c >= 0x1B43 && c <= 0x1B4B) || (c >= 0x1B50 && c <= 0x1B6A) || (c >= 0x1B74 && c <= 0x1B7C) || (c >= 0x1B82 && c <= 0x1BA1) || (c >= 0x1BA6 && c <= 0x1BA7) || (c == 0x1BAA) || (c >= 0x1BAC && c <= 0x1BE5) || (c == 0x1BE7) || (c >= 0x1BEA && c <= 0x1BEC) || (c == 0x1BEE) || (c >= 0x1BF2 && c <= 0x1BF3) || (c >= 0x1BFC && c <= 0x1C2B) || (c >= 0x1C34 && c <= 0x1C35) || (c >= 0x1C3B && c <= 0x1C49) || (c >= 0x1C4D && c <= 0x1C7F) || (c >= 0x1CC0 && c <= 0x1CC7) || (c == 0x1CD3) || (c == 0x1CE1) || (c >= 0x1CE9 && c <= 0x1CEC) || (c >= 0x1CEE && c <= 0x1CF3) || (c >= 0x1CF5 && c <= 0x1CF6) || (c >= 0x1D00 && c <= 0x1DBF) || (c >= 0x1E00 && c <= 0x1F15) || (c >= 0x1F18 && c <= 0x1F1D) || (c >= 0x1F20 && c <= 0x1F45) || (c >= 0x1F48 && c <= 0x1F4D) || (c >= 0x1F50 && c <= 0x1F57) || (c == 0x1F59) || (c == 0x1F5B) || (c == 0x1F5D) || (c >= 0x1F5F && c <= 0x1F7D) || (c >= 0x1F80 && c <= 0x1FB4) || (c >= 0x1FB6 && c <= 0x1FBC) || (c == 0x1FBE) || (c >= 0x1FC2 && c <= 0x1FC4) || (c >= 0x1FC6 && c <= 0x1FCC) || (c >= 0x1FD0 && c <= 0x1FD3) || (c >= 0x1FD6 && c <= 0x1FDB) || (c >= 0x1FE0 && c <= 0x1FEC) || (c >= 0x1FF2 && c <= 0x1FF4) || (c >= 0x1FF6 && c <= 0x1FFC) || (c == 0x200E) || (c == 0x2071) || (c == 0x207F) || (c >= 0x2090 && c <= 0x209C) || (c == 0x2102) || (c == 0x2107) || (c >= 0x210A && c <= 0x2113) || (c == 0x2115) || (c >= 0x2119 && c <= 0x211D) || (c == 0x2124) || (c == 0x2126) || (c == 0x2128) || (c >= 0x212A && c <= 0x212D) || (c >= 0x212F && c <= 0x2139) || (c >= 0x213C && c <= 0x213F) || (c >= 0x2145 && c <= 0x2149) || (c >= 0x214E && c <= 0x214F) || (c >= 0x2160 && c <= 0x2188) || (c >= 0x2336 && c <= 0x237A) || (c == 0x2395) || (c >= 0x249C && c <= 0x24E9) || (c == 0x26AC) || (c >= 0x2800 && c <= 0x28FF) || (c >= 0x2C00 && c <= 0x2C2E) || (c >= 0x2C30 && c <= 0x2C5E) || (c >= 0x2C60 && c <= 0x2CE4) || (c >= 0x2CEB && c <= 0x2CEE) || (c >= 0x2CF2 && c <= 0x2CF3) || (c >= 0x2D00 && c <= 0x2D25) || (c == 0x2D27) || (c == 0x2D2D) || (c >= 0x2D30 && c <= 0x2D67) || (c >= 0x2D6F && c <= 0x2D70) || (c >= 0x2D80 && c <= 0x2D96) || (c >= 0x2DA0 && c <= 0x2DA6) || (c >= 0x2DA8 && c <= 0x2DAE) || (c >= 0x2DB0 && c <= 0x2DB6) || (c >= 0x2DB8 && c <= 0x2DBE) || (c >= 0x2DC0 && c <= 0x2DC6) || (c >= 0x2DC8 && c <= 0x2DCE) || (c >= 0x2DD0 && c <= 0x2DD6) || (c >= 0x2DD8 && c <= 0x2DDE) || (c >= 0x3005 && c <= 0x3007) || (c >= 0x3021 && c <= 0x3029) || (c >= 0x302E && c <= 0x302F) || (c >= 0x3031 && c <= 0x3035) || (c >= 0x3038 && c <= 0x303C) || (c >= 0x3041 && c <= 0x3096) || (c >= 0x309D && c <= 0x309F) || (c >= 0x30A1 && c <= 0x30FA) || (c >= 0x30FC && c <= 0x30FF) || (c >= 0x3105 && c <= 0x312D) || (c >= 0x3131 && c <= 0x318E) || (c >= 0x3190 && c <= 0x31BA) || (c >= 0x31F0 && c <= 0x321C) || (c >= 0x3220 && c <= 0x324F) || (c >= 0x3260 && c <= 0x327B) || (c >= 0x327F && c <= 0x32B0) || (c >= 0x32C0 && c <= 0x32CB) || (c >= 0x32D0 && c <= 0x32FE) || (c >= 0x3300 && c <= 0x3376) || (c >= 0x337B && c <= 0x33DD) || (c >= 0x33E0 && c <= 0x33FE) || (c == 0x3400) || (c == 0x4DB5) || (c == 0x4E00) || (c == 0x9FCC) || (c >= 0xA000 && c <= 0xA48C) || (c >= 0xA4D0 && c <= 0xA60C) || (c >= 0xA610 && c <= 0xA62B) || (c >= 0xA640 && c <= 0xA66E) || (c >= 0xA680 && c <= 0xA697) || (c >= 0xA6A0 && c <= 0xA6EF) || (c >= 0xA6F2 && c <= 0xA6F7) || (c >= 0xA722 && c <= 0xA787) || (c >= 0xA789 && c <= 0xA78E) || (c >= 0xA790 && c <= 0xA793) || (c >= 0xA7A0 && c <= 0xA7AA) || (c >= 0xA7F8 && c <= 0xA801) || (c >= 0xA803 && c <= 0xA805) || (c >= 0xA807 && c <= 0xA80A) || (c >= 0xA80C && c <= 0xA824) || (c == 0xA827) || (c >= 0xA830 && c <= 0xA837) || (c >= 0xA840 && c <= 0xA873) || (c >= 0xA880 && c <= 0xA8C3) || (c >= 0xA8CE && c <= 0xA8D9) || (c >= 0xA8F2 && c <= 0xA8FB) || (c >= 0xA900 && c <= 0xA925) || (c >= 0xA92E && c <= 0xA946) || (c >= 0xA952 && c <= 0xA953) || (c >= 0xA95F && c <= 0xA97C) || (c >= 0xA983 && c <= 0xA9B2) || (c >= 0xA9B4 && c <= 0xA9B5) || (c >= 0xA9BA && c <= 0xA9BB) || (c >= 0xA9BD && c <= 0xA9CD) || (c >= 0xA9CF && c <= 0xA9D9) || (c >= 0xA9DE && c <= 0xA9DF) || (c >= 0xAA00 && c <= 0xAA28) || (c >= 0xAA2F && c <= 0xAA30) || (c >= 0xAA33 && c <= 0xAA34) || (c >= 0xAA40 && c <= 0xAA42) || (c >= 0xAA44 && c <= 0xAA4B) || (c == 0xAA4D) || (c >= 0xAA50 && c <= 0xAA59) || (c >= 0xAA5C && c <= 0xAA7B) || (c >= 0xAA80 && c <= 0xAAAF) || (c == 0xAAB1) || (c >= 0xAAB5 && c <= 0xAAB6) || (c >= 0xAAB9 && c <= 0xAABD) || (c == 0xAAC0) || (c == 0xAAC2) || (c >= 0xAADB && c <= 0xAAEB) || (c >= 0xAAEE && c <= 0xAAF5) || (c >= 0xAB01 && c <= 0xAB06) || (c >= 0xAB09 && c <= 0xAB0E) || (c >= 0xAB11 && c <= 0xAB16) || (c >= 0xAB20 && c <= 0xAB26) || (c >= 0xAB28 && c <= 0xAB2E) || (c >= 0xABC0 && c <= 0xABE4) || (c >= 0xABE6 && c <= 0xABE7) || (c >= 0xABE9 && c <= 0xABEC) || (c >= 0xABF0 && c <= 0xABF9) || (c == 0xAC00) || (c == 0xD7A3) || (c >= 0xD7B0 && c <= 0xD7C6) || (c >= 0xD7CB && c <= 0xD7FB) || (c == 0xD800) || (c >= 0xDB7F && c <= 0xDB80) || (c >= 0xDBFF && c <= 0xDC00) || (c >= 0xDFFF && c <= 0xE000) || (c >= 0xF8FF && c <= 0xFA6D) || (c >= 0xFA70 && c <= 0xFAD9) || (c >= 0xFB00 && c <= 0xFB06) || (c >= 0xFB13 && c <= 0xFB17) || (c >= 0xFF21 && c <= 0xFF3A) || (c >= 0xFF41 && c <= 0xFF5A) || (c >= 0xFF66 && c <= 0xFFBE) || (c >= 0xFFC2 && c <= 0xFFC7) || (c >= 0xFFCA && c <= 0xFFCF) || (c >= 0xFFD2 && c <= 0xFFD7) || (c >= 0xFFDA && c <= 0xFFDC) || (c >= 0x10000 && c <= 0x1000B) || (c >= 0x1000D && c <= 0x10026) || (c >= 0x10028 && c <= 0x1003A) || (c >= 0x1003C && c <= 0x1003D) || (c >= 0x1003F && c <= 0x1004D) || (c >= 0x10050 && c <= 0x1005D) || (c >= 0x10080 && c <= 0x100FA) || (c == 0x10100) || (c == 0x10102) || (c >= 0x10107 && c <= 0x10133) || (c >= 0x10137 && c <= 0x1013F) || (c >= 0x101D0 && c <= 0x101FC) || (c >= 0x10280 && c <= 0x1029C) || (c >= 0x102A0 && c <= 0x102D0) || (c >= 0x10300 && c <= 0x1031E) || (c >= 0x10320 && c <= 0x10323) || (c >= 0x10330 && c <= 0x1034A) || (c >= 0x10380 && c <= 0x1039D) || (c >= 0x1039F && c <= 0x103C3) || (c >= 0x103C8 && c <= 0x103D5) || (c >= 0x10400 && c <= 0x1049D) || (c >= 0x104A0 && c <= 0x104A9) || (c == 0x11000) || (c >= 0x11002 && c <= 0x11037) || (c >= 0x11047 && c <= 0x1104D) || (c >= 0x11066 && c <= 0x1106F) || (c >= 0x11082 && c <= 0x110B2) || (c >= 0x110B7 && c <= 0x110B8) || (c >= 0x110BB && c <= 0x110C1) || (c >= 0x110D0 && c <= 0x110E8) || (c >= 0x110F0 && c <= 0x110F9) || (c >= 0x11103 && c <= 0x11126) || (c == 0x1112C) || (c >= 0x11136 && c <= 0x11143) || (c >= 0x11182 && c <= 0x111B5) || (c >= 0x111BF && c <= 0x111C8) || (c >= 0x111D0 && c <= 0x111D9) || (c >= 0x11680 && c <= 0x116AA) || (c == 0x116AC) || (c >= 0x116AE && c <= 0x116AF) || (c == 0x116B6) || (c >= 0x116C0 && c <= 0x116C9) || (c >= 0x12000 && c <= 0x1236E) || (c >= 0x12400 && c <= 0x12462) || (c >= 0x12470 && c <= 0x12473) || (c >= 0x13000 && c <= 0x1342E) || (c >= 0x16800 && c <= 0x16A38) || (c >= 0x16F00 && c <= 0x16F44) || (c >= 0x16F50 && c <= 0x16F7E) || (c >= 0x16F93 && c <= 0x16F9F) || (c >= 0x1B000 && c <= 0x1B001) || (c >= 0x1D000 && c <= 0x1D0F5) || (c >= 0x1D100 && c <= 0x1D126) || (c >= 0x1D129 && c <= 0x1D166) || (c >= 0x1D16A && c <= 0x1D172) || (c >= 0x1D183 && c <= 0x1D184) || (c >= 0x1D18C && c <= 0x1D1A9) || (c >= 0x1D1AE && c <= 0x1D1DD) || (c >= 0x1D360 && c <= 0x1D371) || (c >= 0x1D400 && c <= 0x1D454) || (c >= 0x1D456 && c <= 0x1D49C) || (c >= 0x1D49E && c <= 0x1D49F) || (c == 0x1D4A2) || (c >= 0x1D4A5 && c <= 0x1D4A6) || (c >= 0x1D4A9 && c <= 0x1D4AC) || (c >= 0x1D4AE && c <= 0x1D4B9) || (c == 0x1D4BB) || (c >= 0x1D4BD && c <= 0x1D4C3) || (c >= 0x1D4C5 && c <= 0x1D505) || (c >= 0x1D507 && c <= 0x1D50A) || (c >= 0x1D50D && c <= 0x1D514) || (c >= 0x1D516 && c <= 0x1D51C) || (c >= 0x1D51E && c <= 0x1D539) || (c >= 0x1D53B && c <= 0x1D53E) || (c >= 0x1D540 && c <= 0x1D544) || (c == 0x1D546) || (c >= 0x1D54A && c <= 0x1D550) || (c >= 0x1D552 && c <= 0x1D6A5) || (c >= 0x1D6A8 && c <= 0x1D6DA) || (c >= 0x1D6DC && c <= 0x1D714) || (c >= 0x1D716 && c <= 0x1D74E) || (c >= 0x1D750 && c <= 0x1D788) || (c >= 0x1D78A && c <= 0x1D7C2) || (c >= 0x1D7C4 && c <= 0x1D7CB) || (c >= 0x1F110 && c <= 0x1F12E) || (c >= 0x1F130 && c <= 0x1F169) || (c >= 0x1F170 && c <= 0x1F19A) || (c >= 0x1F1E6 && c <= 0x1F202) || (c >= 0x1F210 && c <= 0x1F23A) || (c >= 0x1F240 && c <= 0x1F248) || (c >= 0x1F250 && c <= 0x1F251) || (c == 0x20000) || (c == 0x2A6D6) || (c == 0x2A700) || (c == 0x2B734) || (c == 0x2B740) || (c == 0x2B81D) || (c >= 0x2F800 && c <= 0x2FA1D) || (c == 0xF0000) || (c == 0xFFFFD) || (c == 0x100000) || (c == 0x10FFFD);
        }

        public static JVTextDirection GetBaseDirection(string text)
        {
            var arr = text.GetUnicodeCodePoints();

            foreach (var item in arr)
            {
                if (IsCodePointStrongRTL(item))
                    return JVTextDirection.JVTextDirectionRightToLeft;
                if (IsCodePointStrongLTR(item))
                    return JVTextDirection.JVTextDirectionLeftToRight;
            }
            return JVTextDirection.JVTextDirectionNeutral;
        }

    }

    /// <summary>
    /// Port from:
    ///     -https://github.com/jverdi/JVFloatLabeledTextField
    /// </summary>
    [Register("JVFloatLabeledTextField")]
    public class JVFloatLabeledTextField : NextResponderTextField
    {
        private UIFont _floatingLabelFont;
        private bool _alwaysShowFloatingLabel;      

        private UIColor _placeholderColor;

		private readonly bool _isFloatingLabelFontDefault = false;



        public JVFloatLabeledTextField()
        {
            Initialize();
        }

        public JVFloatLabeledTextField(CGRect frame) : base(frame)
        {
            Initialize();
        }

        public JVFloatLabeledTextField(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        public bool AllowAnimation { get; set; } = true;

        /// <summary>
        /// Read-only access to the floating label.
        /// </summary>
        /// <value>The floating label.</value>
        public UILabel FloatingLabel { get; private set; }

        /// <summary>
        /// Padding to be applied to the x coordinate of the floating label upon presentation.
        /// Defaults to zero
        /// </summary>      
        [Export("floatingLabelYPadding")]
        public nfloat FloatingLabelYPadding { get; set; }

        /// <summary>
        /// Padding to be applied to the x coordinate of the floating label upon presentation.
        /// Defaults to zero
        /// </summary>     
        [Export("floatingLabelXPadding")]
        public nfloat FloatingLabelXPadding { get; set; }

        /// <summary>
        /// Padding to be applied to the x coordinate of the floating label upon presentation.
        /// Defaults to zero
        /// </summary>
        [Export("placeholderYPadding")]
        public nfloat PlaceholderYPadding { get; set; }

        /// <summary>
        /// Font to be applied to the floating label. 
        /// Defaults to the first applicable of the following:
        /// - the custom specified attributed placeholder font at 70% of its size
        /// - the custom specified textField font at 70% of its size
        /// </summary>
        public UIFont FloatingLabelFont
        {
            get { return _floatingLabelFont; }
            set
            {
                if (value != null)
                    _floatingLabelFont = value;
                FloatingLabel.Font = _floatingLabelFont ?? DefaultFloatingLabelFont();
                SetFloatingLabelText(Placeholder);
                InvalidateIntrinsicContentSize();
            }
        }

        /// <summary>
        /// Text color to be applied to the floating label. 
        /// Defaults to `[UIColor grayColor]`.
        /// </summary>
        [Export("floatingLabelTextColor")]
        public UIColor FloatingLabelTextColor { get; set; }

        /// <summary>
        /// Text color to be applied to the floating label while the field is a first responder.
        /// Tint color is used by default if an `floatingLabelActiveTextColor` is not provided.
        /// </summary>
        [Export("floatingLabelActiveTextColor")]
        public UIColor FloatingLabelActiveTextColor { get; set; }

        /// <summary>
        /// Indicates whether the floating label's appearance should be animated regardless of first responder status.
        /// By default, animation only occurs if the text field is a first responder.
        /// </summary>
        [Export("animateEvenIfNotFirstResponder")]
        public bool AnimateEvenIfNotFirstResponder { get; set; }

        /// <summary>
        /// Duration of the animation when showing the floating label. 
        /// Defaults to 0.3 seconds.
        /// </summary>
        public nfloat FloatingLabelShowAnimationDuration { get; set; } = 0.3f;

        /// <summary>
        /// Duration of the animation when hiding the floating label. 
        /// Defaults to 0.3 seconds.
        /// </summary>
        public nfloat FloatingLabelHideAnimationDuration { get; set; } = 0.3f;

        /// <summary>
        /// Indicates whether the clearButton position is adjusted to align with the text
        /// Defaults to true.
        /// </summary>
        [Export("adjustsClearButtonRect")]
        public bool AdjustsClearButtonRect { get; set; } = true;

        /// <summary>
        /// Indicates whether or not to drop the baseline when entering text. Setting to YES (not the default) means the standard greyed-out placeholder will be aligned with the entered text
        /// Defaults to NO (standard placeholder will be above whatever text is entered)
        /// </summary>
        [Export("keepBaseline")]
        public bool KeepBaseline { get; set; }

        /// <summary>
        /// Force floating label to be always visible
        /// Defaults to NO
        /// </summary>
        public bool AlwaysShowFloatingLabel
        {
            get { return _alwaysShowFloatingLabel; }
            set
            {
                _alwaysShowFloatingLabel = value;
                SetNeedsLayout();
            }
        }

        /// <summary>
        /// Color of the placeholder
        /// </summary>
        [Export("placeholderColor")]
        public UIColor PlaceholderColor
        {
            get { return _placeholderColor; }
            set
            {
                _placeholderColor = value;
                if (Placeholder.Length > 0)
                    SetCorrectPlaceholder(Placeholder);
            }
        }


        public override UIFont Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                UpdateDefaultFloatingLabelFont();
            }
        }


        public override NSAttributedString AttributedText
        {
            get
            {
                return base.AttributedText;
            }
            set
            {
                base.AttributedText = value;
                UpdateDefaultFloatingLabelFont();
            }
        }

        public override UITextAlignment TextAlignment
        {
            get
            {
                return base.TextAlignment;
            }
            set
            {
                base.TextAlignment = value;
                SetNeedsLayout();
            }
        }

        public override CGSize IntrinsicContentSize
        {
            get
            {
                var size = base.IntrinsicContentSize;
                FloatingLabel.SizeToFit();
                return new CGSize(size.Width, size.Height + FloatingLabelYPadding + FloatingLabel.Bounds.Size.Height);
            }
        }

        public override string Placeholder
        {
            get
            {
                return base.Placeholder;
            }
            set
            {
                SetCorrectPlaceholder(value);
                SetFloatingLabelText(value);
            }
        }

        public override NSAttributedString AttributedPlaceholder
        {
            get
            {
                return base.AttributedPlaceholder;
            }
            set
            {
                base.AttributedPlaceholder = value;
                SetFloatingLabelText(AttributedText.Value);
                UpdateDefaultFloatingLabelFont();
            }
        }

        /// <summary>
        /// Sets the placeholder and the floating title
        /// </summary>
        /// <param name="placeholder">The string that to be shown in the text field when no other text is present.</param>
        /// <param name="floatingTitle">The string to be shown above the text field once it has been populated with text by the user.</param>
        public void SetPlaceholder(string placeholder, string floatingTitle)
        {
            SetCorrectPlaceholder(placeholder);
            SetFloatingLabelText(floatingTitle);
        }


        /// <summary>
        /// Sets the attributed placeholder and the floating title
        /// </summary>
        /// <param name="attributedPlaceholder">The string that to be shown in the text field when no other text is present.</param>
        /// <param name="floatingTitle">The string to be shown above the text field once it has been populated with text by the user.</param>
        public void SetAttributedPlaceholder(NSAttributedString attributedPlaceholder, string floatingTitle)
        {
            AttributedPlaceholder = attributedPlaceholder;
            SetFloatingLabelText(floatingTitle);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            SetLabelOriginForTextAlignment();

            var floatingLabelSize = FloatingLabel.SizeThatFits(FloatingLabel.Superview.Bounds.Size);

            FloatingLabel.Frame = new CGRect(FloatingLabel.Frame.X, FloatingLabel.Frame.Y,
                                             floatingLabelSize.Width, floatingLabelSize.Height);

            var firstResponder = IsFirstResponder;
            FloatingLabel.TextColor = firstResponder && Text.Length > 0 ?
                LabelActiveColor() : FloatingLabelTextColor;

            if (Text.Length == 0 && !AlwaysShowFloatingLabel)
                HideFloatingLabel(firstResponder);
            else
                ShowFloatingLabel(firstResponder);
        }

        public override CGRect TextRect(CGRect forBounds)
        {
            var rect = base.TextRect(forBounds);
            if (Text.Length > 0 || KeepBaseline)
                rect = InsetRectForBounds(rect);
            return rect.Integral();
        }

        public override CGRect EditingRect(CGRect forBounds)
        {
            var rect = base.EditingRect(forBounds);
            if (Text.Length > 0 || KeepBaseline)
                rect = InsetRectForBounds(rect);
            return rect.Integral();
        }

        public override CGRect ClearButtonRect(CGRect forBounds)
        {
            var rect = base.ClearButtonRect(forBounds);
            if (AdjustsClearButtonRect && FloatingLabel.Text.Length > 0)
            {
                if (Text.Length > 0 || KeepBaseline)
                {
                    var topInset = Math.Ceiling(FloatingLabel.Font.LineHeight + PlaceholderYPadding);
                    topInset = Math.Min(topInset, MaxTopInset);
                    rect = new CGRect(rect.X, rect.Y + topInset / 2f, rect.Size.Width, rect.Size.Height);
                }
            }
            return rect.Integral();
        }

        public override CGRect LeftViewRect(CGRect forBounds)
        {
            var rect = base.LeftViewRect(forBounds);
            var topInset = Math.Ceiling(FloatingLabel.Font.LineHeight + PlaceholderYPadding);
            rect.Offset(0, topInset / 2f);
            return rect;
        }

        public override CGRect RightViewRect(CGRect forBounds)
        {
            var rect = base.RightViewRect(forBounds);
            var topInset = Math.Ceiling(FloatingLabel.Font.LineHeight + PlaceholderYPadding);
            topInset = Math.Min(topInset, MaxTopInset);
            rect.Offset(0, topInset / 2f);
            return rect;
        }

        private void SetCorrectPlaceholder(string text)
        {
            if (PlaceholderColor != null && !string.IsNullOrEmpty(Placeholder))
            {
                var attributedPlaceholder = new NSAttributedString(text, foregroundColor: PlaceholderColor);
                AttributedPlaceholder = attributedPlaceholder;
            }
            else
                base.Placeholder = text;
        }

        private nfloat MaxTopInset => new nfloat(Math.Max(0, Math.Floor(
            Bounds.Size.Height - Font.LineHeight - 4.0f)));


        private CGRect InsetRectForBounds(CGRect rect)
        {
            var topInset = Math.Ceiling(FloatingLabel.Bounds.Size.Height + PlaceholderYPadding);
            topInset = Math.Min(topInset, MaxTopInset);
            return new CGRect(
                rect.X,
                rect.Y + topInset / 2.0f,
				rect.Size.Width,
                rect.Size.Height);
        }

        private void ShowFloatingLabel(bool animated)
        {
            Action showBlock = () =>
               {
                   FloatingLabel.Alpha = 1f;
                   FloatingLabel.Frame = new CGRect(FloatingLabel.Frame.X,
                                                     FloatingLabelYPadding,
                                                     FloatingLabel.Frame.Size.Width,
                                                     FloatingLabel.Frame.Size.Height);
               };
            if (!animated || !AllowAnimation)
            {
                showBlock();
                return;
            }
            AnimateNotify(FloatingLabelShowAnimationDuration, 0f,
                          UIViewAnimationOptions.BeginFromCurrentState | UIViewAnimationOptions.CurveEaseOut,
                              showBlock, null);
        }



        private void HideFloatingLabel(bool animated)
        {
            Action hideBlock = () =>
               {
                   FloatingLabel.Alpha = 0f;
                   FloatingLabel.Frame = new CGRect(FloatingLabel.Frame.X,
                                                    FloatingLabel.Font.LineHeight + FloatingLabelYPadding,
                                                     FloatingLabel.Frame.Size.Width,
                                                     FloatingLabel.Frame.Size.Height);
               };
            if (!animated || !AllowAnimation)
            {
                hideBlock();
                return;
            }
            AnimateNotify(FloatingLabelShowAnimationDuration, 0f,
                          UIViewAnimationOptions.BeginFromCurrentState | UIViewAnimationOptions.CurveEaseIn,
                              hideBlock, null);
        }

        private void UpdateDefaultFloatingLabelFont()
        {
            var derivedFont = DefaultFloatingLabelFont();
            if (_isFloatingLabelFontDefault)
                FloatingLabelFont = derivedFont;
            else
                _floatingLabelFont = derivedFont;
        }


        private void SetLabelOriginForTextAlignment()
        {
            var textRect = TextRect(Bounds);
            var originX = textRect.X;

            switch (TextAlignment)
            {
                case UITextAlignment.Center:
                    originX = textRect.X + (textRect.Size.Width / 2f) -
                        (FloatingLabel.Frame.Size.Width / 2f);
                    break;
                case UITextAlignment.Right:
                    originX = textRect.X + textRect.Size.Width - FloatingLabel.Frame.Size.Width;
                    break;
                case UITextAlignment.Natural:
                    var baseDirection = TextDirectionality.GetBaseDirection(FloatingLabel.Text);
                    if (baseDirection == JVTextDirection.JVTextDirectionRightToLeft)
                        originX = textRect.X + textRect.Size.Width - FloatingLabel.Frame.Size.Width;
                    break;
            }

            FloatingLabel.Frame = new CGRect(originX + FloatingLabelXPadding, FloatingLabel.Frame.Y,
                                             FloatingLabel.Frame.Size.Width, FloatingLabel.Frame.Size.Height);

        }

        private UIFont DefaultFloatingLabelFont()
        {
            UIFont textFieldFont = null;
            if (textFieldFont == null && AttributedPlaceholder != null && AttributedPlaceholder.Length > 0)
            {
                var range = new NSRange();
                textFieldFont = AttributedPlaceholder.GetAttribute(UIStringAttributeKey.Font, 0, out range)
                                                     as UIFont;
            }

            if (textFieldFont == null && AttributedText != null && AttributedText.Length > 0)
            {
                var range = new NSRange();
                textFieldFont = AttributedText.GetAttribute(UIStringAttributeKey.Font, 0, out range)
                                                     as UIFont;
            }
            if (textFieldFont == null)
                textFieldFont = Font;
            return textFieldFont.WithSize(new nfloat(textFieldFont.PointSize * 0.7));
        }

        private void SetFloatingLabelText(string text)
        {
            FloatingLabel.Text = text;
            SetNeedsLayout();
        }

        private void UpdateFloatingLabelFont()
        {
            var derivedFont = DefaultFloatingLabelFont();
            FloatingLabelFont = derivedFont;
        }

        private UIColor LabelActiveColor()
        {
            if (FloatingLabelActiveTextColor != null)
                return FloatingLabelActiveTextColor;
            if (RespondsToSelector(new ObjCRuntime.Selector("tintColor:")))
                return PerformSelector(new ObjCRuntime.Selector("tintColor:")) as UIColor;
            return UIColor.Blue;
        }



        private void Initialize()
        {
            FloatingLabel = new UILabel();
            FloatingLabel.Alpha = 0.0f;

            AddSubview(FloatingLabel);

            FloatingLabelFont = DefaultFloatingLabelFont();
            FloatingLabel.Font = FloatingLabelFont;

            FloatingLabelTextColor = UIColor.Gray;
            FloatingLabel.TextColor = FloatingLabelTextColor;

            SetFloatingLabelText(Placeholder);
        }
    }
}
