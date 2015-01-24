﻿using SurvivalKit.Exceptions;
using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SurvivalKit.Utility
{
	/// <summary>
	///		Internal instance resolver.
	/// </summary>
	internal class InstanceResolver : IResolveInstances
	{
		private delegate bool IsOfType(Type typeToCheck, Type expectedType);

		private ILoadAssemblies pluginLoader = null;

		/// <summary>
		///	Constructor that gets an instance of the plugin loader.
		/// </summary>
		public InstanceResolver()
		{
			pluginLoader = PluginLoader.GetInstance();
		}

		/// <summary>
		///	Internal constructor for testing purposes.
		/// </summary>
		/// <param name="loader">The loader to use.</param>
		internal InstanceResolver(ILoadAssemblies loader)
		{
			pluginLoader = loader;
		}

		/// <summary>
		///	Method that will actually resolve the instances.
		/// </summary>
		/// <typeparam name="TInstance">The type of the instances we want.</typeparam>
		/// <returns>
		/// Returns a list with <see cref="TInstance"/> instances. 
		/// The list could be empty when no implementations were found.
		/// </returns>
		public List<TInstance> ResolveInstances<TInstance>(bool onlyLookInPlugins = true)
		{
			// get the type once, instead of once every type.
			IsOfType typeCheckMethod = IsSubclassOf;
			var typeOfTInstance = typeof(TInstance);
			if (typeOfTInstance.IsInterface)
			{
				typeCheckMethod = ImplementsInterface;
			}
			var returnList = new List<TInstance>();

			var assemblies = getAssemblyCollection(onlyLookInPlugins);

			if (assemblies.Count == 0)
			{
				LogUtility.Out("[SK] InstanceResolver: No assemblies found.");
				return returnList;
			}

			foreach (var assembly in assemblies)
			{
				LogUtility.Out("[SK] InstanceResolver: Scanning assembly " + assembly.Location);
				// loop all assemblies, see if it contains types we are looking for.
				Type[] typesInAssembly = new Type[0];
				try
				{
					typesInAssembly = assembly.GetTypes();
				}
				catch (Exception exception)
				{
					// Not yet able to cover this piece of code with a unit test.
					var survivalKitException = new SurvivalKit.Exceptions.SurvivalKitPluginException("SK.ResolveInstances", "SurvivalKit.Utility.InstanceResolver", "Unable to extract types from assembly: " + assembly.FullName, exception);
					LogUtility.Exception(survivalKitException);
				}

				foreach (var typeInAssembly in typesInAssembly)
				{
					if (typeInAssembly.IsInterface || typeInAssembly.IsAbstract)
					{
						continue;
					}
					
					// loop all types in the assembly. only act when it matches our needs.
					if (typeCheckMethod(typeInAssembly, typeOfTInstance))
					{
						var constructors = typeInAssembly.GetConstructors();
						var foundValidConstructor = false;
						foreach (var item in constructors)
						{
							if (item.GetParameters().Length == 0)
							{
								foundValidConstructor = true;
							}
						}

						if (!foundValidConstructor)
						{
							// there is no constructor without arguments, we can't instantiate those (yet?)
							continue;
						}

						try
						{
							var instance = (TInstance)Activator.CreateInstance(typeInAssembly);
							returnList.Add(instance);

						}
						catch (Exception exception)
						{
							var wrappedException = new SurvivalKitPluginException("ResolvePlugin","SurvivalKit.InstanceResolver.ResolveInstances", "Error while invoking constructor", exception);
							LogUtility.Exception(wrappedException);
						}
					}
				}
			}

			return returnList;
		}

		/// <summary>
		///	Method to get the collection of assemblies that should be scanned.
		///	When resolving an instance, you can select to limit yourself to the plugin assemblies.
		/// </summary>
		/// <param name="onlyLookInPlugins">Should we limit to scanning the plugins</param>
		/// <returns>Returns a list of assemblies.</returns>
		private List<Assembly> getAssemblyCollection(bool onlyLookInPlugins)
		{
			if (onlyLookInPlugins)
			{
				pluginLoader.LoadAssemblies();
				return pluginLoader.GetLoadedAssemblies();
			}
			else
			{
				return new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
			}
		}

		/// <summary>
		/// Check if a type implements an interface.
		/// </summary>
		/// <param name="typeToCheck">The type that might implement the interface</param>
		/// <param name="expectedType">The type of the interface.</param>
		/// <returns>Returns <c>true</c> if the type implements the interface</returns>
		private bool ImplementsInterface(Type typeToCheck, Type expectedType)
		{
			var interfaces = typeToCheck.GetInterfaces();

			foreach (var item in interfaces)
			{
				if (item == expectedType)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Check if the type or it's base type is of the desired type.
		/// </summary>
		/// <param name="typeToCheck">The type that might be the desired type, or a subclass of.</param>
		/// <param name="expectedType">The desired type</param>
		/// <returns>Returns <c>true</c> if the type is found.</returns>
		private bool IsSubclassOf(Type typeToCheck, Type expectedType)
		{
			var isMatch = false;
			if (typeToCheck.BaseType != null)
			{
				isMatch = IsSubclassOf(typeToCheck.BaseType, expectedType);
			}

			if(!isMatch)
			{
				isMatch = typeToCheck.BaseType == expectedType;
			}

			if (!isMatch)
			{
				isMatch = typeToCheck == expectedType;
			}

			return isMatch;
		}
	}
}
