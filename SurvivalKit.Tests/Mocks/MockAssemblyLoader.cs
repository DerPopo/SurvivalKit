using SurvivalKit.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SurvivalKit.Tests.Mocks
{
	public class MockAssemblyLoader : ILoadAssemblies
	{
		private List<Assembly> _assemblies;

		public MockAssemblyLoader(List<Assembly> assemblies)
		{
			_assemblies = assemblies;
		}

		public void LoadAssemblies()
		{
			return;
		}

		public List<System.Reflection.Assembly> GetLoadedAssemblies()
		{
			return _assemblies;
		}
	}
}
