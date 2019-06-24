//
// NSLayoutManagerExtensions.cs
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

using Foundation;
using CoreGraphics;
namespace UIKit
{
    public static class NSLayoutManagerExtensions
    {
        public static NSRange CharacterRangeThatFits(this NSLayoutManager @this, NSTextContainer textContainer)
        {
            var rangeThatFits = @this.GetGlyphRange(textContainer);
            rangeThatFits = @this.CharacterRangeForGlyphRange(rangeThatFits);
            return rangeThatFits;
        }

        public static CGRect BoundingRectForCharacterRange(this NSLayoutManager @this, NSRange range,
                                                           NSTextContainer textContainer, CGPoint textContainerOffset)
        {

            var glyphRange = @this.CharacterRangeForGlyphRange(range);
            var boundingRect = @this.BoundingRectForGlyphRange(glyphRange, textContainer);
            return boundingRect;
        }

    }
}
