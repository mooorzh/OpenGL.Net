﻿
// Copyright (c) 2017 Luca Piccioni
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

namespace OpenGL.CoreUI
{
	/// <summary>
	/// Arguments for <see cref="NativeWindow"/> mouse events.
	/// </summary>
	public sealed class NativeWindowMouseEventArgs : NativeWindowEventArgs
	{
		#region Constructors

		/// <summary>
		/// Construct a NativeWindowMouseEventArgs.
		/// </summary>
		/// <param name="deviceContext">
		/// The <see cref="DeviceContext"/> used for the underlying <see cref="NativeWindow"/>.
		/// </param>
		/// <param name="renderContext">
		/// The OpenGL context used for rendering.
		/// </param>
		/// <param name="location">
		/// The <see cref="Point"/> representing the position relative to the event.
		/// </param>
		internal NativeWindowMouseEventArgs(DeviceContext deviceContext, IntPtr renderContext, Point location) :
			base(deviceContext, renderContext)
		{
			
		}

		#endregion

		#region Event Arguments

		/// <summary>
		/// Mouse location relative to the event.
		/// </summary>
		public readonly Point Location;

		#endregion
	}
}
