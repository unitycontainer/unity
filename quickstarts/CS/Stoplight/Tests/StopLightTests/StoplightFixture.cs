// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using StopLight.Logic;
using StopLightTests.TestDoubles;

namespace StopLightTests
{
	/// <summary>
	/// Summary description for StopLightFixture
	/// </summary>
	[TestClass]
	public class StoplightFixture
	{
		[TestMethod]
		public void ShouldInitializeToGreen()
		{
			Stoplight light = new Stoplight();
			Assert.AreEqual(Stoplight.Green, light.CurrentColor);
		}

		[TestMethod]
		public void ShouldTransitionGreenToYellow()
		{
			Stoplight light = new Stoplight();
			light.Next();

			Assert.AreEqual(Stoplight.Yellow, light.CurrentColor);
		}

		[TestMethod]
		public void ShouldTransitionYellowToRed()
		{
			Stoplight light = new Stoplight();
			light.Next();
			light.Next();

			Assert.AreEqual(Stoplight.Red, light.CurrentColor);
		}

		[TestMethod]
		public void ShouldTransitionRedToGreen()
		{
			Stoplight light = new Stoplight();
			light.Next();
			light.Next();
			light.Next();

			Assert.AreEqual(Stoplight.Green, light.CurrentColor);
		}

		[TestMethod]
		public void ShouldRaiseChangedEventOnTransition()
		{
			bool eventFired = false;
			StoplightColors newColor = StoplightColors.Green;
			Stoplight light = new Stoplight();
			light.Changed += delegate(object sender, LightChangedEventArgs e)
			                 {
			                 	eventFired = true;
			                 	newColor = e.CurrentColor;
			                 };
			
			light.Next();
			Assert.IsTrue(eventFired);
			Assert.AreEqual(Stoplight.Yellow, newColor);
		}

		[TestMethod]
		public void ShouldLogMessageOnChange()
		{
			Stoplight light = new Stoplight();
			MockLogger logger = new MockLogger();
			light.Logger = logger;

			light.Next();

			Assert.IsNotNull(logger.LastMessage);
			Assert.IsTrue(logger.LastMessage.StartsWith("LIGHT CHANGED TO"));
		}
	}
}
