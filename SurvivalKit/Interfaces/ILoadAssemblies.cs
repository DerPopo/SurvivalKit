using System.Collections.Generic;
using System.Reflection;
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

		/// <summary>
		///	Internal getter for the loaded assemblies.
		///	This call should be used in the <see cref="IResolveInstances"/> method.
		/// </summary>
		/// <returns>Returns a list of assemblies from the plugin folder.</returns>
		List<Assembly> GetLoadedAssemblies();
	}
}
