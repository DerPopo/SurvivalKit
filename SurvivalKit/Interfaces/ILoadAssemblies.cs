using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalKit.Interfaces
{
	/// <summary>
	/// Interface for the class that will load the assemblies.
	/// </summary>
	public interface ILoadAssemblies
	{
		/// <summary>
		/// Method that is called when the assemblies should be loaded.
		/// </summary>
		void LoadAssemblies();
	}
}
