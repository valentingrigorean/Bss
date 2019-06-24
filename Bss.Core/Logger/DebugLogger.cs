//
// ILogger.cs
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
namespace Bss.Core.Logger
{
    public class DebugLogger : ILogger
    {
        private Severity _severity = Severity.Info;
        private const string Format = "{0} [{1}] {2} -  {3}";


        public event Action<Severity, string> OnLog;

        public void EnableLogToDebugOutput(Severity severity)
        {
            _severity = severity;
        }

        public void Debug(string tag, string message)
        {
            if (_severity < Severity.Debug)
                return;
            WriteInternal(tag, Severity.Debug, message);
        }

        public void Debug(string tag, string format, params object[] args)
        {
            if (_severity < Severity.Debug)
                return;
            WriteInternal(tag, Severity.Debug, string.Format(format, args));
        }

        public void Error(string tag, string message)
        {
            if (_severity < Severity.Error)
                return;
            WriteInternal(tag, Severity.Error, message);
        }

        public void Error(string tag, string format, params object[] args)
        {
            if (_severity < Severity.Error)
                return;
            WriteInternal(tag, Severity.Error, string.Format(format, args));
        }

        public void Info(string tag, string message)
        {
            if (_severity < Severity.Info)
                return;
            WriteInternal(tag, Severity.Debug, message);
        }

        public void Info(string tag, string format, params object[] args)
        {
            if (_severity < Severity.Info)
                return;
            WriteInternal(tag, Severity.Debug, string.Format(format, args));
        }

        public void Warn(string tag, string message)
        {
            if (_severity < Severity.Warn)
                return;
            WriteInternal(tag, Severity.Debug, message);
        }

        public void Warn(string tag, string format, params object[] args)
        {
            if (_severity < Severity.Warn)
                return;
            WriteInternal(tag, Severity.Debug, string.Format(format, args));
        }

        private void WriteInternal(string tag, Severity type, string message)
        {
            OnLog?.Invoke(type, message);
            System.Diagnostics.Debug.WriteLine(
                Format, DateTime.Now.ToString("HH:mm:ss.fff"), tag, type.ToString().ToUpperInvariant(), message);
        }






    }

}