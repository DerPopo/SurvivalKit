using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using SurvivalKit.Tests.Events;
using SurvivalKit.Tests.Mocks;
using SurvivalKit.Utility;

namespace SurvivalKit.Tests.Utility
{
	[TestClass]
	public class InstanceResolverTests
	{
		[TestMethod]
		public void InstanceResolverTests_ResolveInstances_EmptyList()
		{
			var list = new List<Assembly>();
			var mockLoader = new Mocks.MockAssemblyLoader(list);
			var resolver = new SurvivalKit.Utility.InstanceResolver(mockLoader);
			var result = resolver.ResolveInstances<EventAggregatorTests>();
			
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public void InstanceResolverTests_ResolveInstances_ValidList()
		{
			var list = new List<Assembly>{Assembly.GetExecutingAssembly()};
			var mockLoader = new Mocks.MockAssemblyLoader(list);
			var resolver = new SurvivalKit.Utility.InstanceResolver(mockLoader);
			var result = resolver.ResolveInstances<EventAggregatorTests>();

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
		}

		[TestMethod]
		public void InstanceResolverTests_ResolveInstances_ConstructorException()
		{
			LogUtility.SetLogToConsole();
			var list = new List<Assembly>{Assembly.GetExecutingAssembly()};
			var mockLoader = new Mocks.MockAssemblyLoader(list);
			var resolver = new SurvivalKit.Utility.InstanceResolver(mockLoader);
			var result = resolver.ResolveInstances<MockClassWithConstructorException>();

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public void InstanceResolverTests_ResolveInstances_ConstructorArgument()
		{
			LogUtility.SetLogToConsole();
			var list = new List<Assembly>{Assembly.GetExecutingAssembly()};
			var mockLoader = new Mocks.MockAssemblyLoader(list);
			var resolver = new SurvivalKit.Utility.InstanceResolver(mockLoader);
			var result = resolver.ResolveInstances<MockClassWithConstructorArguments>();

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public void InstanceResolverTests_ResolveInstances_MultipleConstructors()
		{
			LogUtility.SetLogToConsole();
			var list = new List<Assembly> { Assembly.GetExecutingAssembly() };
			var mockLoader = new Mocks.MockAssemblyLoader(list);
			var resolver = new SurvivalKit.Utility.InstanceResolver(mockLoader);
			var result = resolver.ResolveInstances<MockClassWithConstructors>();

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
		}

		[TestMethod]
		public void InstanceResolverTests_ResolveInstances_LookInAppDomain()
		{
			LogUtility.SetLogToConsole();
			var list = new List<Assembly>();
			var mockLoader = new Mocks.MockAssemblyLoader(list);
			var resolver = new SurvivalKit.Utility.InstanceResolver(mockLoader);
			var result = resolver.ResolveInstances<MockClassWithConstructors>(false);

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
		}
		
		/// <summary>
		///	Test to cover the default constructor.
		/// </summary>
		[TestMethod]
		public void InstanceResolverTests_DefaultConstructor()
		{
			var resolver = new SurvivalKit.Utility.InstanceResolver();
			Assert.IsNotNull(resolver);
		}
	}
}
