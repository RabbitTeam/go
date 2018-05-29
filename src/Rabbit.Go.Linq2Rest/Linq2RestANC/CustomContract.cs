using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Linq2Rest
{
    public static class CustomContract
    {
        public static void Requires(bool cond)
        {
            if (!cond)
                throw new ArgumentException("Contract Requires failed");
        }

        public static void Assert(bool cond) => Assert(cond, "Contract Assert failed");
        public static void Assert(bool cond, string msg)
        {
            if (!cond)
                throw new ArgumentException(msg);
        }

        public static void Assume(bool cond) => Assume(cond, "Contract Assume failed");
        public static void Assume(bool cond, string msg)
        {
            if (!cond)
                throw new ArgumentException(msg);
        }

        public static T Result<T>()
        {
            return default(T);
        }

        public static void Ensures(bool cond)
        {

        }

        public static void Requires<TEx>(bool cond) where TEx : Exception, new()
        {
            if (!cond)
                throw new TEx();
        }

        public static void Invariant(bool cond)
        {
            if (!cond)
                throw new Exception();
        }

    }
}
