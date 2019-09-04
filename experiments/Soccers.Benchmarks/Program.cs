using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Soccers.Benchmarks
{
    public class Md5VsSha256
    {
        private const int N = 10000;
        private readonly byte[] data;

        private readonly SHA256 sha256 = SHA256.Create();
        private readonly MD5 md5 = MD5.Create();

        public Md5VsSha256()
        {
            data = new byte[N];
            new Random(42).NextBytes(data);
        }

        [Benchmark]
        public byte[] Sha256() => sha256.ComputeHash(data);

        //[Benchmark, Params, ParamsSource, [Arguments(200, 20)]]
        public byte[] Md5() => md5.ComputeHash(data);
    }

    public class IntroSetupCleanupGlobal
    {
        [Params(10, 100, 1000)]
        public int N;

        private int[] data;

        [GlobalSetup]
        public void GlobalSetup()
        {
            data = new int[N]; // executed once per each N value
        }

        [Benchmark]
        public int Logic()
        {
            int res = 0;
            for (int i = 0; i < N; i++)
                res += data[i];
            return res;
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            // Disposing logic
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ApiBenchmark>();
            //var benchmark = new ApiBenchmark();
            //benchmark.MakeRequest(new DateTime(2019, 08, 30, 0, 0, 0, DateTimeKind.Utc));
        }
    }
}
