using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;

namespace SurvivalKit.Utility
{
	/// <summary>
	///		Internal instance resolver.
	/// </summary>
	internal class InstanceResolver : IResolveInstances
	{
		/// <summary>
		///	Method that will actually resolve the instances.
		/// </summary>
		/// <typeparam name="TInstance">The type of the instances we want.</typeparam>
		/// <returns>
		/// Returns a list with <see cref="TInstance"/> instances. 
		/// The list could be empty when no implementations were found.
		/// </returns>
		public List<TInstance> ResolveInstances<TInstance>()
		{
			// get the type once, instead of once every type.
			var typeOfTInstance = typeof(TInstance);
			var returnList = new List<TInstance>();

			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				// loop all assemblies, see if it contains types we are looking for.
				Type[] typesInAssembly = new Type[0];
				try
				{
					typesInAssembly = assembly.GetTypes();
				}
				catch (Exception exception)
				{
					Log.Error(exception.Message);
					Log.Error(exception.StackTrace);
				}

				foreach (var typeInAssembly in typesInAssembly)
				{
					// loop all types in the assembly. only act when it matches our needs.
					if (typeInAssembly.IsAssignableFrom(typeOfTInstance))
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
							Log.Error(exception.Message);
							Log.Error(exception.StackTrace);
						}
					}
				}
			}

			return returnList;
		}
	}
}
