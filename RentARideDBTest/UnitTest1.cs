using System.Runtime.Intrinsics.X86;
using Xunit;

namespace RentARideDBTest
{
    public class TrueTest
    {
        [Fact]
        public void MustBeTrue()
        {
            Assert.True(true);
        }
    }
}