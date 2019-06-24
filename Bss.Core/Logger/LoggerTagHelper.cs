//
// LoggerHelper.cs
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

namespace Bss.Core.Logger
{
    public class LoggerTagHelper
    {
        private readonly ILogger _logger;

        private LoggerTagHelper(ILogger logger, string tag)
        {
            _logger = logger;
            Tag = tag;
        }

        public static LoggerTagHelper Create(ILogger logger, string tag)
        {
            return new LoggerTagHelper(logger, tag);
        }

        public string Tag { get; }


        public void Log(Severity severity, string message)
        {
            switch (severity)
            {
                case Severity.Debug:
                    Debug(message);
                    break;
                case Severity.Error:
                    Error(message);
                    break;
                case Severity.Info:
                    Info(message);
                    break;
                case Severity.Warn:
                    Warn(message);
                    break;
            }
        }

        public void Log(Severity severity, string format, params object[] args)
        {
            Log(severity, string.Format(format, args));
        }

        public void Error(string message)
        {
            _logger.Error(Tag, message);
        }

        public void Error(string format, params object[] args)
        {
            _logger.Error(Tag, format, args);
        }

        public void Info(string message)
        {
            _logger.Info(Tag, message);
        }

        public void Info(string format, params object[] args)
        {
            _logger.Info(Tag, format, args);
        }

        public void Debug(string message)
        {
            _logger.Debug(Tag, message);
        }

        public void Debug(string format, params object[] args)
        {
            _logger.Debug(Tag, format, args);
        }

        public void Warn(string message)
        {
            _logger.Warn(Tag, message);
        }

        public void Warn(string format, params object[] args)
        {
            _logger.Warn(Tag, format, args);
        }
    }
}