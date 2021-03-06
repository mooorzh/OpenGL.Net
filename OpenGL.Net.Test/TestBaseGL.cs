﻿
// Copyright (C) 2015-2017 Luca Piccioni
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;

using Khronos;

using NUnit.Framework;

namespace OpenGL.Test
{
	/// <summary>
	/// Abstract base test creating an OpenGL context used for testing.
	/// </summary>
	[TestFixture]
#if !NETCORE
	[Apartment(ApartmentState.STA)]
#endif
	[Category("Graphics")]
	abstract class TestBaseGL : TestBase
	{
		#region Setup & Tear Down

		/// <summary>
		/// Create a an OpenGL context, and get the OpenGL extensions supported.
		/// </summary>
		[OneTimeSetUp]
		public new void FixtureSetUp()
		{
			try {
				// Create compatibility profile context
				if ((_Context = _DeviceContext.CreateContext(IntPtr.Zero)) == IntPtr.Zero)
					throw new InvalidOperationException("unable to create compatibility profile OpenGL context");

				// Make OpenGL context current
				if (_DeviceContext.MakeCurrent(_Context) == false)
					throw new InvalidOperationException("unable to make current the OpenGL context");

				// Get OpenGL version
				if ((_VersionString = Gl.GetString(StringName.Version)) == null)
					throw new InvalidOperationException("unable to get the OpenGL version");
				// Extract OpenGL version numbers
				_Version = KhronosVersion.Parse(_VersionString);
				// Get OpenGL extensions
				_GlExtensions.Query();
				// Get OpenGL window system extensions
				// Windows OpenGL extensions
				DeviceContextWGL windowsDeviceContext = _DeviceContext as DeviceContextWGL;
				if (windowsDeviceContext != null)
					_WglExtensions.Query(windowsDeviceContext);
				// GLX OpenGL extensions
				DeviceContextGLX xserverDeviceContext = _DeviceContext as DeviceContextGLX;
				if (xserverDeviceContext != null)
					_GlxExtensions.Query(xserverDeviceContext);
				// EGL OpenGL extensions
				DeviceContextEGL nativeDeviceContext = _DeviceContext as DeviceContextEGL;
				if (nativeDeviceContext != null)
					_EglExtensions.Query(nativeDeviceContext);
				
			} catch (Exception exception) {
				Console.WriteLine("Unable to initialize Fixture for OpenGL: " + exception.ToString());

				// Release resources manually
				FixtureTearDown();
				throw;
			}
		}

		/// <summary>
		/// Synchronize thread-local delegates.
		/// </summary>
		[SetUp]
		public void SetUp()
		{
			// Make OpenGL context current
			if (_DeviceContext.MakeCurrent(_Context) == false)
				throw new InvalidOperationException("unable to make current the OpenGL context");
		}

		/// <summary>
		/// Release resources allocated by <see cref="FixtureSetUp"/>.
		/// </summary>
		[OneTimeTearDown]
		public new void FixtureTearDown()
		{
			// Detroy context
			if (_Context != IntPtr.Zero)
				_DeviceContext.DeleteContext(_Context);
			_Context = IntPtr.Zero;
		}

		/// <summary>
		/// The OpenGL context for this tst fixture.
		/// </summary>
		protected IntPtr _Context;

		#endregion

		#region Context Creation

		private IntPtr CreateContext()
		{
			IntPtr context;

#if NETCOREAPP1_1
			ContextAttribute contextAttribute = null;
#else
			ContextAttribute contextAttribute = (ContextAttribute)Attribute.GetCustomAttribute(GetType(), typeof(ContextAttribute));
#endif

			if (contextAttribute == null) {
				// Create compatibility profile context
				if ((context = _DeviceContext.CreateContext(IntPtr.Zero)) == IntPtr.Zero)
					throw new InvalidOperationException("unable to create compatibility profile OpenGL context");
			} else {
				List<int> contextAttribs = new List<int>();

				if ((context = _DeviceContext.CreateContextAttrib(IntPtr.Zero, contextAttribs.ToArray())) == IntPtr.Zero)
					throw new InvalidOperationException("unable to create core profile OpenGL context");
			}

			return (context);
		}

		#endregion

		#region Version/Extension Support

		/// <summary>
		/// Get the OpenGL version of the OpenGL context.
		/// </summary>
		protected string Version { get { return (_VersionString); } }

		/// <summary>
		/// Check whether a specific OpenGL version is supported.
		/// </summary>
		/// <param name="major">
		/// A <see cref="Int32"/> that specifies the major OpenGL version to test for support.
		/// </param>
		/// <param name="minor">
		/// A <see cref="Int32"/> that specifies the minor OpenGL version to test for support.
		/// </param>
		/// <returns>
		/// It returns a boolean value indicating whether the specified OpenGL version is supported.
		/// </returns>
		protected bool HasVersion(int major, int minor)
		{
			return (HasVersion(major, minor, 0));
		}

		/// <summary>
		/// Check whether a specific OpenGL version is supported.
		/// </summary>
		/// <param name="major">
		/// A <see cref="Int32"/> that specifies the major OpenGL version to test for support.
		/// </param>
		/// <param name="minor">
		/// A <see cref="Int32"/> that specifies the minor OpenGL version to test for support.
		/// </param>
		/// <param name="revision">
		/// A <see cref="Int32"/> that specifies the revision OpenGL version to test for support.
		/// </param>
		/// <returns>
		/// It returns a boolean value indicating whether the specified OpenGL version is supported.
		/// </returns>
		protected bool HasVersion(int major, int minor, int revision)
		{
			return (_Version.Api == KhronosVersion.ApiGl && _Version >= new KhronosVersion(major, minor, revision, KhronosVersion.ApiGl));
		}

		/// <summary>
		/// Check whether a specific OpenGL ES version is supported.
		/// </summary>
		/// <param name="major">
		/// A <see cref="Int32"/> that specifies the major OpenGL version to test for support.
		/// </param>
		/// <param name="minor">
		/// A <see cref="Int32"/> that specifies the minor OpenGL version to test for support.
		/// </param>
		/// <returns>
		/// It returns a boolean value indicating whether the specified OpenGL version is supported.
		/// </returns>
		protected bool HasEsVersion(int major, int minor)
		{
			switch (_Version.Api) {
				case KhronosVersion.ApiGles1:
					return (_Version >= new KhronosVersion(major, minor, 0, KhronosVersion.ApiGles1));
				case KhronosVersion.ApiGles2:
					return (_Version >= new KhronosVersion(major, minor, 0, KhronosVersion.ApiGles2));
				default:
					return (false);
			}
		}

		/// <summary>
		/// Check whether an OpenGL extension is supported by the OpenGL context.
		/// </summary>
		/// <param name="extension">
		/// A <see cref="String"/> that specifies the extension to test for support.
		/// </param>
		/// <returns>
		/// It returns a boolean value indicating whether the extension identified with <paramref name="extension"/> is
		/// actually supported.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Exception thrown if <paramref name="extension"/> is null.
		/// </exception>
		protected bool IsGlExtensionSupported(string extension)
		{
			return (_GlExtensions.HasExtensions(extension));
		}

		/// <summary>
		/// Check whether an OpenGL for Windows extension is supported by the OpenGL context.
		/// </summary>
		/// <param name="extension">
		/// A <see cref="String"/> that specifies the extension to test for support.
		/// </param>
		/// <returns>
		/// It returns a boolean value indicating whether the extension identified with <paramref name="extension"/> is
		/// actually supported.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Exception thrown if <paramref name="extension"/> is null.
		/// </exception>
		protected bool IsWglExtensionSupported(string extension)
		{
			return (_WglExtensions.HasExtensions(extension));
		}

		/// <summary>
		/// Check whether an OpenGL for X extension is supported by the OpenGL context.
		/// </summary>
		/// <param name="extension">
		/// A <see cref="String"/> that specifies the extension to test for support.
		/// </param>
		/// <returns>
		/// It returns a boolean value indicating whether the extension identified with <paramref name="extension"/> is
		/// actually supported.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Exception thrown if <paramref name="extension"/> is null.
		/// </exception>
		protected bool IsGlxExtensionSupported(string extension)
		{
			return (_GlxExtensions.HasExtensions(extension));
		}

		/// <summary>
		/// The OpenGL version of the OpenGL context.
		/// </summary>
		private string _VersionString;

		/// <summary>
		/// The OpenGL version of the OpenGL context.
		/// </summary>
		private KhronosVersion _Version;

		/// <summary>
		/// OpenGL extensions support.
		/// </summary>
		private readonly Gl.Extensions _GlExtensions = new Gl.Extensions();

		/// <summary>
		/// Windows OpenGL extensions support.
		/// </summary>
		private readonly Wgl.Extensions _WglExtensions = new Wgl.Extensions();

		/// <summary>
		/// Windows OpenGL extensions support.
		/// </summary>
		private readonly Glx.Extensions _GlxExtensions = new Glx.Extensions();

		/// <summary>
		/// Windows OpenGL extensions support.
		/// </summary>
		private readonly Egl.Extensions _EglExtensions = new Egl.Extensions();

		#endregion
	}
}
