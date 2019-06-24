//
// IStickyHeaderInterface.cs
//
// Author:
//       valentingrigorean <v.grigorean@software-dep.net>
//
// Copyright (c) 2018 (c) Grigorean Valentin
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
using Android.Views;

namespace Bss.Droid.Widgets
{
    /// <summary>
    /// Sticky header interface.  
    /// Example : StickyHeaderAdapter.cs
    /// </summary>
    public interface IStickyHeaderInterface
    {
        /// <summary>
        /// This method gets called by {@link HeaderItemDecoration} to fetch the position of the header item in the adapter
        /// that is used for (represents) item at specified position.
        /// </summary>
        /// <returns>The header position for item.</returns>
        /// <param name="itemPosition">Item position.</param>
        int GetHeaderPositionForItem(int itemPosition);

        /// <summary>
        /// This method gets called by {@link HeaderItemDecoration} to get layout resource id for the header item at specified adapter's position.
        /// </summary>
        /// <returns>The header layout.</returns>
        /// <param name="headerPosition">Header position.</param>
        int GetHeaderLayout(int headerPosition);

        /// <summary>
        /// This method gets called by {@link HeaderItemDecoration} to setup the header View.
        /// </summary>
        /// <param name="header">Header.</param>
        /// <param name="headerPosition">Header position.</param>
        void BindHeaderData(View header, int headerPosition);

        /// <summary>
        /// This method gets called by {@link HeaderItemDecoration} to verify whether the item represents a header.
        /// </summary>
        /// <returns><c>true</c>, if header was ised, <c>false</c> otherwise.</returns>
        /// <param name="itemPosition">Item position.</param>
        bool IsHeader(int itemPosition);
    }
}
