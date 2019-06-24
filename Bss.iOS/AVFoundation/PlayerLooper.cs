//
// PlayerLooper.cs
//
// Author:
//       valentingrigorean <v.grigorean@software-dep.net>
//
// Copyright (c) 2017 (c) Grigorean Valentin
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
using AVFoundation;
using CoreMedia;
using Foundation;
using UIKit;

namespace Bss.iOS.AVFoundation
{
    public class PlayerLooper : IDisposable
    {
        private readonly IPlayerLooper _looper;

        private PlayerLooper(IPlayerLooper looper)
        {
            _looper = looper;
        }

        public static PlayerLooper FromPlayer(AVQueuePlayer player)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                return new PlayerLooper(new NativeLooper(player));
            return new PlayerLooper(new FallbackLooper(player));
        }

        public void Dispose()
        {
            _looper.Dispose();
        }

        public void Play()
        {
            _looper.Play();
        }


        private interface IPlayerLooper : IDisposable
        {
            void Play();
        }


        private class FallbackLooper : IPlayerLooper, IDisposable
        {
            private readonly AVPlayer _player;
            private readonly IDisposable _disp;

            public FallbackLooper(AVPlayer player)
            {
                _player = player;

                _disp = AVPlayerItem.Notifications
                                    .ObserveDidPlayToEndTime(VideoDidFinishPlaying);
            }

            public void Dispose()
            {
                _disp?.Dispose();
            }

            public void Play()
            {
                _player.Play();
            }

            private void VideoDidFinishPlaying(object sender, NSNotificationEventArgs e)
            {
                _player.Seek(CMTime.Zero);
                _player.Play();
            }
        }

        private class NativeLooper : IPlayerLooper
        {
            private readonly AVQueuePlayer _player;
            private AVPlayerLooper _playerLooper;

            public NativeLooper(AVQueuePlayer player)
            {
                _player = player;
            }

            public void Play()
            {
                _playerLooper = AVPlayerLooper.FromPlayer(_player, _player.CurrentItem);
            }

            public void Dispose()
            {
                _playerLooper.DisableLooping();
            }
        }

    }
}
