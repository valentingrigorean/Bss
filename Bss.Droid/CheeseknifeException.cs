/*
* Copyright 2014 Marcel Braghetto
* 
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
* 
* http://www.apache.org/licenses/LICENSE-2.0
* 
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
* 
*/

using System;
using System.Linq;
using System.Text;

namespace Bss.Droid.Cheeseknife
{
    /// <summary>
    /// Cheeseknife exception which gets thrown when injection mappings
    /// are wrong or fail ...
    /// </summary>
    public class CheeseknifeException : Exception
    {
        const string PREFIX = "Cheeseknife Exception: ";

        /// <summary>
        /// Call this constructor with an Android view class type and a UI
        /// event name to indicate that the view class is not compatible
        /// with the particular event type specified.
        /// </summary>
        /// <param name="viewType">View type.</param>
        /// <param name="eventName">Event name.</param>
        public CheeseknifeException(Type viewType, string eventName) : base(GetViewTypeExceptionMessage(viewType, eventName)) { }

        /// <summary>
        /// Call this constructor with a list of required event type 
        /// parameters to indicate that the parameters couldn't be found
        /// or matched to the signature of the user attributed method.
        /// </summary>
        /// <param name="requiredEventParameters">Required event types.</param>
        public CheeseknifeException(Type[] requiredEventParameters) : base(GetArgumentTypeExceptionMessage(requiredEventParameters)) { }

        /// <summary>
        /// Gets the view type exception message for an Android view class
        /// that can't receive the specified event type.
        /// </summary>
        /// <returns>The view type exception message.</returns>
        /// <param name="viewType">View type.</param>
        /// <param name="eventName">Event name.</param>
        static string GetViewTypeExceptionMessage(Type viewType, string eventName)
        {
            var sb = new StringBuilder();
            sb.Append(PREFIX);
            sb.Append(" Incompatible Android view type specified for event '");
            sb.Append(eventName);
            sb.Append("', the Android view type '");
            sb.Append(viewType.ToString());
            sb.Append("' doesn't appear to support this event.");
            return sb.ToString();
        }

        /// <summary>
        /// Gets the argument type exception message when the user attributed
        /// method doesn't have the same number of parameters as the specified
        /// event signature, or the parameter types don't match between the
        /// event and user method.
        /// </summary>
        /// <returns>The argument type exception message.</returns>
        /// <param name="requiredEventParameters">Required event parameters.</param>
        static string GetArgumentTypeExceptionMessage(Type[] requiredEventParameters)
        {
            var sb = new StringBuilder();
            sb.Append(PREFIX);
            sb.Append(" Incorrect arguments in receiving method, should be => (");
            for (var i = 0; i < requiredEventParameters.Length; i++)
            {
                sb.Append(requiredEventParameters[i].ToString());
                if (i < requiredEventParameters.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}