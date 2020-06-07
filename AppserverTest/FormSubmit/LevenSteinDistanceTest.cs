using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using AFO = AbstractFormObject;
namespace AppserverTest.FormSubmit
{
    [TestFixture]
    class LevenSteinDistanceTest
    {
        [TestCase(0, 0, 0, 0)]
        [TestCase(0, 1, 2, 0)]
        [TestCase(0, 1, 2, 0)]
        [TestCase(2, 1, 2, 1)]
        [TestCase(2, 1, 0, 0)]
        [TestCase(2, 1, -1, -1)]
        public void MinimumTest(int x, int y, int z, int min)
        {
            Assert.AreEqual(min, AFO.minimum(x, y, z));
        }

        [TestCase(new int[] { 0, 1, 2, 3, 4, 5 }, 0)]
        [TestCase(new int[] { 5, 4, 3, 2, 1, 0 }, 0)]
        [TestCase(new int[] { 5, 0, 3, 1, 2, 4 }, 0)]
        public void MinimumParamTest( int[] x, int min)
        {
            Assert.AreEqual(min, AFO.minimum(x));
        }

        [TestCase("SomeString")]
        [TestCase("short")]
        [TestCase("CAPS")]
        public void SameStringTest(string s)
        {
            Assert.AreEqual(0,AFO.LevenshteinDistance(s, s));
        }

        [TestCase("SomeString","SOMEstring")]
        public void OnlyCapsDifference(string s, string t)
        {
            Assert.AreEqual(s.ToLower(), t.ToLower());
            int diff = 0;
            for( int i = 0; i < s.Length; ++i)
            {
                if(s[i] != t[i])
                {
                    diff++;
                }
            }
            Assert.AreEqual(diff, AFO.LevenshteinDistance(s,t));
        }

        [TestCase("SomeString")]
        public void PrefixStrings(string s)
        {
            Assert.IsTrue(s.Length > 4);
            string t = s.Substring(0, 4);
            Assert.AreEqual(s.Length - 4,AFO.LevenshteinDistance(s, t));
        }

        [TestCase("sitting", "kitten", 3)]
        public void DifferentString( string s, string t, int diff)
        {
            Assert.AreEqual(diff, AFO.LevenshteinDistance(s, t));
        }

        [TestCase("SomeString")]
        [TestCase("short")]
        [TestCase("CAPS")]
        public void SameString_NGLDTest(string s)
        {
            Assert.AreEqual(0, AFO.NGLD(s,s));
        }

        [TestCase("SomeString", "SOMEstring")]
        [TestCase("Short","short")]
        [TestCase("SHort","short")]
        public void OnlyCapsDifference_NGLDTest(string s, string t)
        {
            Assert.AreEqual(s.ToLower(), t.ToLower());
            double diff = 0;
            for (int i = 0; i < s.Length; ++i)
            {
                if (s[i] != t[i])
                {
                    diff++;
                }
            }
            Assert.AreEqual(2*diff/(2*s.Length+diff), AFO.NGLD(s, t));
        }
        [TestCase("SomeString", "SOMEstring")]
        [TestCase("Short", "short")]
        [TestCase("SHort", "short")]
        [TestCase("Short", "Long")]
        [TestCase("SomeString", "short")]
        public void Metric_NGLDTest(string s, string t)
        {
            Assert.AreEqual(AFO.NGLD(s,t), AFO.NGLD(t,s));
        }

        [TestCase("Service Delivered On:", "ervice Delivered On")]
        [TestCase("Service Delivered On:", "ervice Delivered ")]
        public void ToleranceTest(string s, string t)
        {
            var dist = AFO.NGLD(s, t);
            Assert.IsTrue( dist < AFO.tolerance);
        }

        [TestCase("Service Delivered On:", "Service:")]
        [TestCase("Service Delivered On:", "Provider")]
        [TestCase("Service Delivered On:", "Provider Name")]
        [TestCase("Service Delivered On:", "service delivered.")]
        public void ExceedsToleranceTest(string s, string t)
        {

            Assert.IsTrue(AFO.NGLD(s, t) > AFO.tolerance);
        }

        [Test]
        public void MatchingSameStringsTest()
        {
            var input = new List<string>() {
                "Customer Name:",
                "Provider Name:",
                "CM Organization:",
                "Service:",
                "Prime:",
                "Provider Num:",
                "SC/PA Name:"
            };
            var expected = new List<KeyValuePair<string, string>>(input.Count);
            foreach( var s in input)
            {
                expected.Add(new KeyValuePair<string,string>(s, s));
            }

            Assert.AreEqual(expected, AFO.MatchKeyValuePairs(input, input));
        }

        [Test]
        public void MatchingStringsMissingFirstCharTest()
        {
            var input1 = new List<string>() {
                "Customer Name:",
                "Provider Name:",
                "CM Organization:",
                "Service:",
                "Prime:",
                "Provider Num:",
                "SC/PA Name:"
            };
            var input2 = new List<string>() {
                "ustomer Name:",
                "rovider Name:",
                "M Organization:",
                "ervice:",
                "rime:",
                "rovider Num:",
                "C/PA Name:"
            };
            var expected = new List<KeyValuePair<string, string>>(input1.Count);
            for( int i = 0; i < input1.Count; ++i)
            {
                expected.Add(new KeyValuePair<string, string>(input1[i], input2[i]));
            }

            Assert.AreEqual(expected, AFO.MatchKeyValuePairs(input1, input2));
        }

        [Test]
        public void MatchingStringExtraInputs()
        {
            var input1 = new List<string>() {
                "Customer Name:",
                "Provider Name:",
                "CM Organization:",
                "Service:",
                "Prime:",
                "Provider Num:",
                "SC/PA Name:"
            };
            var input2 = new List<string>() {
                "Customer Name:",
                "Provider Name:",
                "CM Organization:",
                "Service:",
                "Prime:",
                "Provider Num:",
                "SC/PA Name:",
                "Progress",
                "Service Delivered On:",
                "Date",
                "Start/Time IN",
                "End/Time OUT"
            };
            var expected = new List<KeyValuePair<string, string>>(input1.Count);
            foreach (var s in input1)
            {
                expected.Add(new KeyValuePair<string, string>(s, s));
            }
            Assert.AreEqual(expected, AFO.MatchKeyValuePairs(input1, input2));
        }

        [Test]
        public void MatchingStringMissingInputs()
        {
            var input1 = new List<string>() {
                "Customer Name:",
                "Provider Name:",
                "CM Organization:",
                "Service:",
                "Prime:",
                "Provider Num:",
                "SC/PA Name:"
            };
            var input2 = new List<string>() {
                "Customer Name:",
                "Provider Name:",
                "CM Organization:",
                "Provider Num:",
                "SC/PA Name:"
            };

            var output = new List<string>() {
                "Customer Name:",
                "Provider Name:",
                "CM Organization:",
                "",
                "",
                "Provider Num:",
                "SC/PA Name:"
            };
            var expected = new List<KeyValuePair<string, string>>(input1.Count);
            for (int i = 0; i < input1.Count; ++i)
            {
                expected.Add(new KeyValuePair<string, string>(input1[i], output[i]));
            }
            Assert.AreEqual(expected, AFO.MatchKeyValuePairs(input1, input2));
        }

        [TestCase("0123456789", "012345", 0, 6)]
        [TestCase("0123456789", "123456", 1, 6)]
        [TestCase("0123456789", "123456789", 1, 9)]
        [TestCase("0123456789", "89", 8, 10)]
        [TestCase("0123456789", "89", 8, 20)]
        [TestCase("0123456789", "", 8, 0)]
        public void SubString(string s, string t, int start, int end)
        {
            Assert.AreEqual(t, AFO.Substring(s, start, end));
        }
    } // Class
}
