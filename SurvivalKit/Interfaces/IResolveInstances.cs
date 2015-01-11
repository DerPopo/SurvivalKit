using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalKit.Interfaces
{
	/// <summary>
	///	Interface for a class that will resolve  instances of a certain type.
	///	The implementation should scan all loaded assemblies of the implementations 	
	/// </summary>
	public interface IResolveInstances
	{
		/// <summary>
		///	Method that will actually resolve the instances.
		/// </summary>
		/// <typeparam name="TInstance">The type of the instances we want.</typeparam>
		/// <returns>
		/// Returns a list with <see cref="TInstance"/> instances. 
		/// The list could be empty when no implementations were found.
		/// </returns>
		List<TInstance> ResolveInstances<TInstance>();
	}
}
