//
// IDevice.cs
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
using ReactiveUI;
using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Linq;
using Splat;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Rx.Droid"), InternalsVisibleTo("Rx.iOS")]
namespace Rx.Core
{
    public enum TargetIdiom
    {
        Unknown,
        Phone,
        Tablet
    }

    public enum CreationType
    {
        New,
        Constant,
        Lazy
    }

    internal interface IDeviceType
    {
        TargetIdiom Idiom { get; }
    }

    public interface IWantsToRegisterStuff
    {
        void Register(Action<Func<object>, Type, CreationType> registerFunction);
    }

    public static class Device
    {
        private static bool _wasInit;

        public static TargetIdiom Idiom => Locator.Current.GetService<IDeviceType>()?.Idiom ?? TargetIdiom.Unknown;

        /// <summary>
        /// Initialiaze this instance.
        /// </summary>
        public static void Initialize(IWantsToRegisterStuff registerTypes = null)
        {
            if (_wasInit)
                return;
            _wasInit = true;
            registerTypes?.Register(RegisterType);
        }

        public static Task InitializeAsync(IEnumerable<Action<IEnumerable<TypeInfo>>> allTypesBlock)
        {
            return Task.Run(() =>
            {
                var allTypes = GetAllTypes();
                RegisterAllIWantToRegister(allTypes);
                foreach (var block in allTypesBlock)
                    block?.Invoke(allTypes);
            });
        }

        public static void InvokeOnMainThread(Action action)
        {
            RxApp.MainThreadScheduler.Schedule(action);
        }

        private static IEnumerable<TypeInfo> GetAllTypes()
        {
            var currentDomain = typeof(string).GetTypeInfo().Assembly.GetType("System.AppDomain").GetRuntimeProperty("CurrentDomain").GetMethod.Invoke(null, new object[] { });
            var getAssemblies = currentDomain.GetType().GetRuntimeMethod("GetAssemblies", new Type[] { });
            var assemblies = getAssemblies.Invoke(currentDomain, new object[] { }) as Assembly[];

            var allTypes = new List<TypeInfo>();

            foreach (var asm in assemblies)
            {
                try
                {
                    allTypes.AddRange(asm.DefinedTypes);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error:{ex}");
                }
            }
            return allTypes;
        }

        private static void RegisterAllIWantToRegister(IEnumerable<TypeInfo> allTypes)
        {
            var registerStuff = typeof(IWantsToRegisterStuff).GetTypeInfo();

            var iwantToRegisterStuff = allTypes.Where(t => !t.IsInterface && registerStuff.IsAssignableFrom(t))
                                               .Select(t => (IWantsToRegisterStuff)Activator.CreateInstance(t.AsType()));

            foreach (var wantToRegister in iwantToRegisterStuff)
            {
                wantToRegister.Register(RegisterType);
            }
        }

        private static void RegisterType(Func<object> f, Type t, CreationType creationType)
        {
            switch (creationType)
            {
                case CreationType.New:
                    Locator.CurrentMutable.Register(() => f(), t);
                    break;
                case CreationType.Constant:
                    Locator.CurrentMutable.RegisterConstant(f(), t);
                    break;
                case CreationType.Lazy:
                    Locator.CurrentMutable.RegisterLazySingleton(() => f(), t);
                    break;
            }
        }
    }
}