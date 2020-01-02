/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Threading;

// ReSharper disable InconsistentNaming

namespace Sportradar.MTS.SDK.Test.Helpers
{
    public static class StaticRandom
    {
        private static int _seed;

        private static readonly ThreadLocal<Random> ThreadLocal = new ThreadLocal<Random>
            (() => new Random(Interlocked.Increment(ref _seed)));

        static StaticRandom()
        {
            _seed = Environment.TickCount;
        }

        public static Random Instance => ThreadLocal.Value;

        public static string SS(int limit = 0)
        {
            return limit > 1 ? Instance.Next(1, limit).ToString() : Instance.Next().ToString();
        }

        public static int II(int limit = 0)
        {
            return limit > 1 ? Instance.Next(1, limit) : Instance.Next();
        }

        public static string S => SS();

        public static int I => II();

        public static string S1000 => SS(1000);

        public static int I1000 => II(1000);

        public static int I1000P => II(100000) + 10000;

        public static string S1000P => (II(100000) + 10000).ToString();

        public static string S100 => SS(100);

        public static int I100 => II(100);

        public static bool B => I100 > 49;

        public static string SL(int length)
        {
            var result = string.Empty;
            if (length <= 0)
            {
                return result;
            }
            while (result.Length < length)
            {
                result += Guid.NewGuid().ToString().Replace("-", string.Empty);
            }
            return result.Substring(0, length);
        }
    }
}