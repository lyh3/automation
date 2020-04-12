using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace McAfeeLabs.Engineering.Automation.Base
{
    [TestFixture]
    public class DateTimeExtensionTest
    {
        private const int LeapYear = 2012;
        private const int RegularYear = 2013;

        private DateTime RegularYearPivot = new DateTime(RegularYear, 1, 30, 23, 59, 59);
        private DateTime RegularYearResult_Add1Day = new DateTime(RegularYear, 1, 31, 23, 59, 59);
        private DateTime RegularYearResult_Add7Days = new DateTime(RegularYear, 2, 6, 23, 59, 59);
        private DateTime RegularYearResult_Add100Hours = new DateTime(RegularYear, 2, 4, 3, 59, 59);

        private DateTime LeapYearPivot = new DateTime(LeapYear, 2, 28, 23, 59, 59);
        private DateTime LeapYearResult_Add1Day = new DateTime(LeapYear, 2, 29, 23, 59, 59);
        private DateTime LeapYearResult_Add7Days = new DateTime(LeapYear, 3, 6, 23, 59, 59);

        private DateTime RegularYearRoundupPivot = new DateTime(RegularYear, 1, 30, 23, 0, 0);
        private DateTime RegularYearRoundupResult_Add1Day = new DateTime(RegularYear, 1, 31, 23, 0, 0);
        private DateTime RegularYearRoundupResult_Add7Days = new DateTime(RegularYear, 2, 6, 23, 0, 0);
        private DateTime LeapYearRoundupPivot = new DateTime(LeapYear, 2, 28, 23, 0, 0);
        private DateTime LeapYearRoundupResult_Add1Day = new DateTime(LeapYear, 2, 29, 23, 0, 0);
        private DateTime LeapYearRoundupResult_Add7Days = new DateTime(LeapYear, 3, 6, 23, 0, 0);
        private DateTime LeapYearRoundupResult_Add7Hours = new DateTime(LeapYear, 2, 29, 6, 0, 0);

        private DateTime RegularYearCrossYearPivot = new DateTime(RegularYear, 12, 31, 23, 59, 59);
        private DateTime RegularYearCrossYearResult_Add1Day = new DateTime(RegularYear + 1, 1, 1, 23, 59, 59);
        private DateTime RegularYearCrossYearResult_Add7Days = new DateTime(RegularYear + 1, 1, 7, 23, 59, 59);
        private DateTime RegularYearCrossYearResult_Add1Hour = new DateTime(RegularYear + 1, 1, 1, 0, 59, 59);
        
        private DateTime LeapYearCrossYearPivot = new DateTime(LeapYear, 12, 31, 23, 59, 59);
        private DateTime LeapYearCrossYearResult_Add1Day = new DateTime(LeapYear + 1, 1, 1, 23, 59, 59);
        private DateTime LeapYearCrossYearResult_Add7Days = new DateTime(LeapYear + 1, 1, 7, 23, 59, 59);
        private DateTime LeapYearCrossYearResult_Add1Hour = new DateTime(LeapYear + 1, 1, 1, 0, 59, 59);
        private DateTime LeapYearCrossYearResult_Add7Hours = new DateTime(LeapYear + 1, 1, 1, 6, 59, 59);
       
        [Test]
        public void RegularYearIncreaseBy1DayTest()
        {
            Assert.AreEqual(1.IncreaseByDays(RegularYearPivot), RegularYearResult_Add1Day);
            Assert.AreEqual(1.IncreaseByDays(RegularYearRoundupPivot), RegularYearRoundupResult_Add1Day);
        }

        [Test]
        public void RegularYearIncreaseBy7DaysTest()
        {
            Assert.AreEqual(7.IncreaseByDays(RegularYearPivot), RegularYearResult_Add7Days);
            Assert.AreEqual(7.IncreaseByDays(RegularYearRoundupPivot), RegularYearRoundupResult_Add7Days);
        }

        [Test]
        public void RegularYearIncreaseBy7DaysTest2()
        {
            DateTime __t1 = RegularYearPivot.AddDays(7); 
            DateTime __t2 = LeapYearPivot.AddDays(7); 
            DateTime __t3 = RegularYearRoundupPivot.AddDays(7);
            DateTime __t4 = LeapYearRoundupPivot.AddDays(7);
            DateTime __t5 = RegularYearCrossYearPivot.AddDays(7);
            DateTime __t6 = LeapYearCrossYearPivot.AddDays(7);

            Assert.AreEqual(__t1.CompareTo(RegularYearResult_Add7Days), 0);
            Assert.AreEqual(__t2.CompareTo(LeapYearResult_Add7Days), 0);
            Assert.AreEqual(__t3.CompareTo(RegularYearRoundupResult_Add7Days), 0);
            Assert.AreEqual(__t4.CompareTo(LeapYearRoundupResult_Add7Days), 0);
            Assert.AreEqual(__t5.CompareTo(RegularYearCrossYearResult_Add7Days), 0);
            Assert.AreEqual(__t6.CompareTo(LeapYearCrossYearResult_Add7Days), 0);
        }

        [Test]
        public void RegularYearIncreaseBy1DayCrossYearTest()
        {
            Assert.AreEqual(1.IncreaseByDays(RegularYearCrossYearPivot), RegularYearCrossYearResult_Add1Day);
        }

        [Test]
        public void RegularYearIncreaseBy7DaysCrossYearTest()
        {
            Assert.AreEqual(7.IncreaseByDays(RegularYearCrossYearPivot), RegularYearCrossYearResult_Add7Days);
        }

        [Test]
        public void LeapYearIncreaseBy1DayTest()
        {
            Assert.AreEqual(1.IncreaseByDays(LeapYearPivot), LeapYearResult_Add1Day);
            Assert.AreEqual(1.IncreaseByDays(LeapYearRoundupPivot), LeapYearRoundupResult_Add1Day);
        }

        [Test]
        public void LeapYearIncreaseBy7DaysTest()
        {
            Assert.AreEqual(7.IncreaseByDays(LeapYearPivot), LeapYearResult_Add7Days);
            Assert.AreEqual(7.IncreaseByDays(LeapYearRoundupPivot), LeapYearRoundupResult_Add7Days);
        }

        [Test]
        public void LeapYearIncreaseBy1DayCrossYearTest()
        {
            Assert.AreEqual(1.IncreaseByDays(LeapYearCrossYearPivot), LeapYearCrossYearResult_Add1Day);
        }

        [Test]
        public void LeapYearIncreaseBy7DaysCrossYearTest()
        {
            Assert.AreEqual(7.IncreaseByDays(LeapYearCrossYearPivot), LeapYearCrossYearResult_Add7Days);
        }

        [Test]
        public void RegularYearIncreaseBy1HourCrossYearTest()
        {
            Assert.AreEqual(1.IncreaseByHours(RegularYearCrossYearPivot), RegularYearCrossYearResult_Add1Hour);
        }

        [Test]
        public void RegularYearIncreaseBy100HourYearTest()
        {
            Assert.AreEqual(100.IncreaseByHours(RegularYearPivot), RegularYearResult_Add100Hours);
        }

        [Test]
        public void LeapYearIncreaseBy1HourCrossYearTest()
        {
            Assert.AreEqual(1.IncreaseByHours(LeapYearCrossYearPivot), LeapYearCrossYearResult_Add1Hour);
        }

        [Test]
        public void LeapYearIncreaseBy7HoursCrossYearTest()
        {
            Assert.AreEqual(7.IncreaseByHours(LeapYearCrossYearPivot), LeapYearCrossYearResult_Add7Hours);
            Assert.AreEqual(7.IncreaseByHours(LeapYearRoundupPivot), LeapYearRoundupResult_Add7Hours);
        }

        [Test]
        public void IncreaseByWeekOfDayBackwordsTest()
        {
            const int direction = -1;
            var pivot = DayOfWeek.Thursday;

            DayOfWeek[] ExpectedResults = { DayOfWeek.Thursday,
                                            DayOfWeek.Wednesday,
                                            DayOfWeek.Tuesday,
                                            DayOfWeek.Monday,
                                            DayOfWeek.Sunday,
                                            DayOfWeek.Saturday,
                                            DayOfWeek.Friday,
                                            DayOfWeek.Thursday,
                                            DayOfWeek.Wednesday,
                                            DayOfWeek.Tuesday,
                                            DayOfWeek.Monday,
                                            DayOfWeek.Sunday,
                                            DayOfWeek.Saturday,
                                            DayOfWeek.Friday,
                                            DayOfWeek.Thursday,
                                            DayOfWeek.Wednesday,
                                            DayOfWeek.Tuesday
                                        };
            for (int i = 0; i < ExpectedResults.Length; ++i)
                Assert.AreEqual(ExpectedResults[i], (direction * i).IncreaseByWeekOfDay(pivot));
        }

        [Test]
        public void IncreaseByWeekOfDayForwordsTest()
        {
            const int direction = +1;
            var pivot = DayOfWeek.Thursday;

            DayOfWeek[] ExpectedResults = { DayOfWeek.Thursday,
                                            DayOfWeek.Friday,
                                            DayOfWeek.Saturday,
                                            DayOfWeek.Sunday,
                                            DayOfWeek.Monday,
                                            DayOfWeek.Tuesday,
                                            DayOfWeek.Wednesday,
                                            DayOfWeek.Thursday,
                                            DayOfWeek.Friday,
                                            DayOfWeek.Saturday,
                                            DayOfWeek.Sunday,
                                            DayOfWeek.Monday,
                                            DayOfWeek.Tuesday,
                                            DayOfWeek.Wednesday,
                                            DayOfWeek.Thursday,
                                            DayOfWeek.Friday
                                        };
            for (int i = 0; i < ExpectedResults.Length; ++i)
                Assert.AreEqual(ExpectedResults[i], (direction * i).IncreaseByWeekOfDay(pivot));
        }
    }
}
