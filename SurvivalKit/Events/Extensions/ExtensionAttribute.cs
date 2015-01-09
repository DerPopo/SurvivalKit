namespace System.Runtime.CompilerServices
{
	/// <summary>
	///  Add this attribute in order to create extension methods in the .NET 3.0 framework.
	///  Found this on: http://stackoverflow.com/a/11346555
	///  Need to test if MONO will play nice with this!
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class ExtensionAttribute : Attribute
	{
	}
}
