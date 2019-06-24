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
using Android.Views;
using System.Reflection;
using Android.App;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Bss.Droid.Cheeseknife
{
    /// <summary>
    /// Inject view attribute. Android widgets based on the
    /// View super class can be resolved at runtime when
    /// annotated with this attribute. This attribute is only
    /// permitted on instance fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class InjectView : Attribute
	{
		public InjectView(int resourceId)
		{
			ResourceId = resourceId;
		}

		public int ResourceId { get; private set; }
	}

    /// <summary>
    /// Cheeseknife! It's like a Butterknife with a weird shape!
    /// <para></para><para></para>
    /// Inspired by the extremely helpful Java based Butterknife
    /// Android library, this helper class allows for easy Android
    /// view and common event handler injections for Xamarin.Android.
    /// This injection happens at runtime rather than compile time.
    /// </summary>
    public static class Cheeseknife
	{
		private readonly static ConcurrentDictionary<Type, CacheObject> _internalCache = new ConcurrentDictionary<Type, CacheObject>();

		private static readonly MethodInfo _resolveMethod = typeof(Cheeseknife).GetMethod(
			METHOD_NAME_RESOLVE_ANDROID_VIEW, BindingFlags.Static | BindingFlags.NonPublic);

		private static readonly Type _injectType = typeof(InjectView);

		private static bool _initiated;

        public static bool IsEnabled { get; set; } = true;

		public static void Initialize()
        {
			if (_initiated) return;
			_initiated = true;
			Task.Run(() =>
			{
				var assm = AppDomain.CurrentDomain.GetAssemblies()[1];
				var types = assm.GetTypes();
				Parallel.ForEach(types, type =>
				{
					var fields = GetAttributedFields(_injectType, type);
					var properties = GetAttributedProperties(_injectType, type);

					if (fields.Any() || properties.Any())
					{
						var cached = CreateCacheObject(fields, properties);
						while (true)
							if (_internalCache.TryAdd(type, cached)) break;
					}
				});
			});
		}

		#region EVENT / METHOD CONSTANTS
		const string METHOD_NAME_INVOKE = "Invoke";
		const string METHOD_NAME_RESOLVE_ANDROID_VIEW = "ResolveAndroidView";
		#endregion

		#region PRIVATE CONSTANTS
		const BindingFlags INJECTION_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
		#endregion

		#region PUBLIC API
		/// <summary>
		/// Inject the specified parent activity, scanning all class
		/// member fields and methods for injection attributions. The
		/// assumption is that the activitie's 'Window.DecorView.RootView'
		/// represents the root view in the layout hierarchy for the
		/// given activity.<para></para>
		/// <para></para>
		/// Sample activity usage:<para></para>
		/// <para></para>
		/// [InjectView(Resource.Id.my_text_view)]<para></para>
		/// TextView myTextView;<para></para>
		/// <para></para>
		/// [InjectOnClick(Resource.Id.my_button)]<para></para>
		/// void OnMyButtonClick(object sender, EventArgs e) {<para></para>
		/// . . . myTextView.Text = "I clicked my button!";<para></para>
		/// }<para></para>
		/// <para></para>
		/// protected override void OnCreate(Bundle bundle) {<para></para>
		/// . . . base.OnCreate(bundle);<para></para>
		///<para></para>
		/// . . . SetContentView(Resource.Layout.Main);<para></para>
		/// . . . Cheeseknife.Inject(this);<para></para>
		/// <para></para>
		/// . . . myTextView.Text = "I was injected!";<para></para>
		/// }<para></para>
		/// </summary>
		/// <param name="parent">Parent.</param>
		public static void Inject(Activity parent)
		{
			InjectView(parent, parent.Window.DecorView.RootView);
		}

		/// <summary>
		/// Inject the specified parent and view, scanning all class
		/// member fields and methods for injection attributions.
		/// This method would normally be called to inject a fragment
		/// or other arbitrary view container. eg:<para></para>
		/// <para></para>
		/// Fragment Example Usage:<para></para>
		/// <para></para>
		/// In your OnCreateView method ...<para></para>
		/// var view = inflater.Inflate(Resource.Layout.fragment, null);<para></para>
		/// Cheeseknife.Inject(this, view);<para></para>
		/// return view;<para></para>
		/// <para></para>
		/// In your OnDestroyView method ...<para></para>
		/// Cheeseknife.Reset(this);<para></para>
		/// </summary>
		/// <param name="parent">Parent.</param>
		/// <param name="view">View.</param>
		public static void Inject(object parent, View view)
		{
            if (view.IsInEditMode)
                return;
			InjectView(parent, view);
		}

		/// <summary>
		/// Reset the specified parent fields to null, which is useful
		/// within the OnDestroyView fragment lifecycle method, particularly
		/// if you are using RetainInstance = true.
		/// </summary>
		/// <param name="parent">Parent.</param>
		public static void Reset(object parent)
		{
			if (!_internalCache.ContainsKey(parent.GetType()))
                return;

			var cached = GetOrCreate(parent.GetType());

			foreach (var item in cached.Items)
				item.ResetWidget(parent);
		}
		#endregion

		#region PRIVATE API
		static CacheObject GetOrCreate(Type type)
		{
			CacheObject cached = null;
			if (_internalCache.ContainsKey(type))
			{
				while (true)
				{
					if (_internalCache.TryGetValue(type, out cached)) break;
				}
				return cached;
			}

			cached = CreateCacheObject(type);

			while (true)
			{
				if (_internalCache.TryAdd(type, cached)) break;
			}

			return cached;
		}

		private static CacheObject CreateCacheObject(Type type)
		{
			var cached = new CacheObject();
			cached.AddFields(GetAttributedFields(_injectType, type));
			cached.AddProperties(GetAttributedProperties(_injectType, type));
			return cached;
		}

		private static CacheObject CreateCacheObject(IEnumerable<FieldInfo> fields, IEnumerable<PropertyInfo> properties)
		{
			var cached = new CacheObject();
			cached.AddFields(fields);
			cached.AddProperties(properties);
			return cached;
		}

		/// <summary>
		/// Gets the attributed fields inside the parent object with
		/// the matching type of attribute.
		/// </summary>
		/// <returns>The attributed fields.</returns>
		/// <param name="attributeType">Attribute type.</param>
		/// <param name="parent">Parent.</param>
		static IEnumerable<FieldInfo> GetAttributedFields(Type attributeType, object parent)
		{
			return parent.GetType().GetFields(INJECTION_BINDING_FLAGS).Where(x => x.IsDefined(attributeType));
		}

		/// <summary>
		/// Gets the attributed fields inside the parent object with
		/// the matching type of attribute.
		/// </summary>
		/// <returns>The attributed fields.</returns>
		/// <param name="attributeType">Attribute type.</param>
		/// <param name="parent">Parent.</param>
		static IEnumerable<FieldInfo> GetAttributedFields(Type attributeType, Type parent)
		{
			return parent.GetFields(INJECTION_BINDING_FLAGS).Where(x => x.IsDefined(attributeType));
		}

		/// <summary>
		/// Gets the attributed properties inside the parent object with
		/// the matching type of attribute.
		/// </summary>
		/// <returns>The attributed properties.</returns>
		/// <param name="attributeType">Attribute type.</param>
		/// <param name="parent">Parent.</param>
		static IEnumerable<PropertyInfo> GetAttributedProperties(Type attributeType, object parent)
		{
			return parent.GetType().GetProperties(INJECTION_BINDING_FLAGS).Where(x => x.IsDefined(attributeType));
		}

		/// <summary>
		/// Gets the attributed properties inside the parent object with
		/// the matching type of attribute.
		/// </summary>
		/// <returns>The attributed properties.</returns>
		/// <param name="attributeType">Attribute type.</param>
		/// <param name="parent">Parent.</param>
		static IEnumerable<PropertyInfo> GetAttributedProperties(Type attributeType, Type parent)
		{
			return parent.GetProperties(INJECTION_BINDING_FLAGS).Where(x => x.IsDefined(attributeType));
		}

		/// <summary>
		/// Resolves an android view to a specific view type. This is
		/// needed to allow custom Android view classes to resolve
		/// correctly (eg, Com.Android.Volley.NetworkImageView etc).
		/// </summary>
		/// <returns>The android view.</returns>
		/// <param name="view">Parent view to resolve view from.</param>
		/// <param name="resourceId">Resource identifier.</param>
		/// <typeparam name="T">The required specific Android view type.</typeparam>
		static T ResolveAndroidView<T>(View view, int resourceId) where T : View
		{
			return view.FindViewById<T>(resourceId);
		}

		/// <summary>
		/// Injects the parent class by iterating over all of its
		/// fields, properties and methods, checking if they have
		/// injection attributes. For any fields/props/methods that
		/// have injection attributes do the following:<para></para>
		/// <para></para>
		/// 1. If it is a field/prop -> attempt to resolve the actual
		/// Android widget in the given view and assign it as the
		/// field value, effectively 'injecting' it.<para></para>
		/// <para></para>
		/// 2. If it is a method -> attempt to apply an event
		/// handler of the related type to the widget identified
		/// by the resource id specified in the attribute. Some
		/// widget types are verified before applying the events.
		/// </summary>
		/// <param name="parent">Parent.</param>
		/// <param name="view">View.</param>
		static void InjectView(object parent, View view)
		{
            if (!IsEnabled)
                return;
            
			var cached = GetOrCreate(parent.GetType());

			var items = cached.Items;
			var length = items.Count;
			for (var i = 0; i < length; i++)
				items[i].SetWidget(parent, view);
		}
		#endregion

		private struct Pair
		{
			private readonly int _resourceId;
			private readonly MethodInfo _methodInfo;
			private readonly Action<object, object> _setValue;

			public Pair(FieldInfo fieldInfo, int resourceId)
			{
				_resourceId = resourceId;
				_methodInfo = _resolveMethod.MakeGenericMethod(fieldInfo.FieldType);
				_setValue = fieldInfo.SetValue;
			}

			public Pair(PropertyInfo propertyInfo, int resourceId)
			{
				_resourceId = resourceId;
				_methodInfo = _resolveMethod.MakeGenericMethod(propertyInfo.PropertyType);
				_setValue = propertyInfo.SetValue;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void SetWidget(object parent, View view)
			{
				_setValue(parent, _methodInfo.Invoke(parent, new object[] { view, _resourceId }));
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void ResetWidget(object parent)
			{
				_setValue(parent, null);
			}
		}

		private class CacheObject
		{
			public void AddProperties(IEnumerable<PropertyInfo> properties)
			{
				var list = Items as List<Pair>;
				list.AddRange(properties.Select(prop =>
				{
					var attribute = prop.GetCustomAttribute<InjectView>();
					return new Pair(prop, attribute.ResourceId);
				}));
			}

			public void AddFields(IEnumerable<FieldInfo> fields)
			{
				var list = Items as List<Pair>;
				list.AddRange(fields.Select(field =>
				{
					var attribute = field.GetCustomAttribute<InjectView>();
					return new Pair(field, attribute.ResourceId);
				}));
			}

			public readonly IReadOnlyList<Pair> Items = new List<Pair>();
		}
	}
}