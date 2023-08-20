using ConsoleApp1;

namespace TestProject2
{
    public class UnitTest1
    {
        [Fact]
        public void TestBoxCreating()
        {
            var b1 = new Box(1, 1, 1, 1, new DateOnly(2020, 1, 1), null);
            var b2 = new Box(1, 1, 1, 1, null, new DateOnly(2020, 9, 1));
            var b3 = new Box(1, 1, 1, 1, new DateOnly(2020, 1, 1), new DateOnly(2020, 7, 1));

            try
            {
                var b4 = new Box(1, 1, 1, 1, new DateOnly(2020, 9, 1), new DateOnly(2020, 7, 1));
                Assert.Fail("Expiration Date violation");
            }
            catch (Box.ExpirationDateViolationException e) { }

            Assert.True(b1.ExpirationDate == new DateOnly(2020, 4, 10));
            Assert.True(b2.ExpirationDate == new DateOnly(2020, 9, 1));
            Assert.True(b3.ExpirationDate == new DateOnly(2020, 7, 1));
        }

        [Theory]
        [InlineData(0,0,0,0)]
        [InlineData(1,1,1,-1)]
        [InlineData(int.MaxValue, int.MaxValue, int.MinValue, int.MaxValue)]
        public void TestBoxCreating2(int width, int length, int height, int weight)
        {
            try
            {
                var b4 = new Box(width, length, height, weight, null, null);
                Assert.Fail("args must be > 0");
            }
            catch (ArgumentException e) { }
        }


        [Theory]
        [InlineData(14, 1, 1, 4, 6, 2)]
        [InlineData(1, 1)]
        [InlineData(36, 5, 1,10,20)]
        public void TestPalletHeight(int expected, int pallet_h, params int[] height)
        {
            var p1 = new Pallet(1, 1, pallet_h);
            foreach (int h in height)
            {
                p1.AddBox(new Box(1, 1, h, 1, null, null));
            }
            Assert.True(expected == p1.Heigth);
        }

        public void Test2(int expected, int pallet_h, params int[] height)
        {
            var p1 = new Pallet(1, 1, pallet_h);
            foreach (int h in height)
            {
                p1.AddBox(new Box(1, 1, h, 1, null, null));
            }
            Assert.True(expected == p1.Heigth);
        }
    }
}