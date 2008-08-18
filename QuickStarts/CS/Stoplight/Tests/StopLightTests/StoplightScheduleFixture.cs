//===============================================================================
// Microsoft patterns & practices
// Unity Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StopLight.Logic;
using StopLightTests.TestDoubles;

namespace StopLightTests
{
	/// <summary>
	/// Summary description for StoplightScheduleFixture
	/// </summary>
	[TestClass]
	public class StoplightScheduleFixture
	{
		[TestMethod]
		public void ShouldFireChangeLightOnTimeExpiration()
		{
			bool lightChangedFired = false;

			MockStoplightTimer timer = new MockStoplightTimer();
			StoplightSchedule schedule = new StoplightSchedule(timer);
			schedule.ChangeLight += delegate { lightChangedFired = true; };

			timer.Expire();
			Assert.IsTrue(lightChangedFired);
		}

		[TestMethod]
		public void ShouldSetTimerDurationForNextLight()
		{
			MockStoplightTimer timer = new MockStoplightTimer();
			StoplightSchedule schedule = new StoplightSchedule(timer);
			TimeSpan greenSchedule = new TimeSpan(800);
			TimeSpan yellowSchedule = new TimeSpan(1500);
			TimeSpan redSchedule = new TimeSpan(100);
			schedule.Update(greenSchedule, yellowSchedule, redSchedule);

			timer.Expire();
			Assert.AreEqual(yellowSchedule, timer.Duration);
		}

		[TestMethod]
		public void ShouldFireEventAndUpdateScheduleOnForceChange()
		{
			MockStoplightTimer timer = new MockStoplightTimer();
			StoplightSchedule schedule = new StoplightSchedule(timer);
			TimeSpan greenSchedule = new TimeSpan(800);
			TimeSpan yellowSchedule = new TimeSpan(1500);
			TimeSpan redSchedule = new TimeSpan(100);
			schedule.Update(greenSchedule, yellowSchedule, redSchedule);

			bool eventFired = false;
			schedule.ChangeLight += delegate { eventFired = true; };

			schedule.ForceChange();
			Assert.IsTrue(eventFired);
			Assert.AreEqual(yellowSchedule, timer.Duration);
		}

		[TestMethod]
		public void ShouldLogOnUpdate()
		{
			MockStoplightTimer timer = new MockStoplightTimer();
			MockLogger logger = new MockLogger();
			StoplightSchedule schedule = new StoplightSchedule(timer);
			schedule.Logger = logger;

			TimeSpan greenSchedule = new TimeSpan(800);
			TimeSpan yellowSchedule = new TimeSpan(1500);
			TimeSpan redSchedule = new TimeSpan(100);

			schedule.Update(greenSchedule, yellowSchedule, redSchedule);

			Assert.IsNotNull(logger.LastMessage);
			Assert.IsTrue(logger.LastMessage.StartsWith("UPDATE"));
		}

		[TestMethod]
		public void ShouldLogOnForceChange()
		{
			MockStoplightTimer timer = new MockStoplightTimer();
			MockLogger logger = new MockLogger();
			StoplightSchedule schedule = new StoplightSchedule(timer);
			schedule.Logger = logger;
			TimeSpan greenSchedule = new TimeSpan(800);
			TimeSpan yellowSchedule = new TimeSpan(1500);
			TimeSpan redSchedule = new TimeSpan(100);
			schedule.Update(greenSchedule, yellowSchedule, redSchedule);

			schedule.ForceChange();
			Assert.IsNotNull(logger.LastMessage);
			Assert.IsTrue(logger.LastMessage.StartsWith("FORCED CHANGE"));
		}
	}
}
