using ConsoleApp1;

namespace TestProject2
{
    public class UnitTest1
    {
        internal class BoxWrapper : Box 
        {
            public BoxWrapper() : base() {  }
            public int W_Width
            {
                get => Width;
                set => Width = Math.Max(1, value);
            }
            public int W_Length
            {
                get => Length;
                set => Length = Math.Max(1, value);
            }
            public int W_Height
            {
                get => Heigth; 
                set => Heigth = Math.Max(1, value);
            }
            public int W_Weight
            {
                get => Weight;
                set => Weight = Math.Max(1, value);
            }
            public DateOnly? W_ManufactDate
            {
                get => _manufactDate; 
                set => _manufactDate = value;
            }
            public DateOnly? W_ExpirationDate
            {
                get => ExpirationDate;
                set => _expirationDate = value;
            }

        }

        public static Box CreateBoxWrapper(int width = 1, int length = 1, int height = 1, int weight = 1)
            => new BoxWrapper 
            { 
                W_Width = width, 
                W_Length = length, 
                W_Height = height, 
                W_Weight = weight, 
                W_ManufactDate = DateOnly.MinValue, 
                W_ExpirationDate = DateOnly.MaxValue 
            };


        public static Pallet CreatePallet(int width = 1, int length = 1, int height = 1)
            => new Pallet(width, length, height);

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
            catch (Box.ExpirationDateViolationException) { }

            Assert.True(b1.ExpirationDate == new DateOnly(2020, 4, 10));
            Assert.True(b2.ExpirationDate == new DateOnly(2020, 9, 1));
            Assert.True(b3.ExpirationDate == new DateOnly(2020, 7, 1));
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 1, 1, -1)]
        [InlineData(int.MaxValue, int.MaxValue, int.MinValue, int.MaxValue)]
        public void TestBoxCreating2(int width, int length, int height, int weight)
        {
            try
            {
                var b4 = new Box(width, length, height, weight, DateOnly.MinValue, DateOnly.MaxValue);
                Assert.Fail("args must be > 0");
            }
            catch (ArgumentException) { }
        }

        [Theory]
        [InlineData(16, 3, 1, 4, 6, 2)]
        [InlineData(1, 1)]
        [InlineData(36, 5, 1, 10, 20)]
        public void TestPalletHeight(int expected, int pallet_h, params int[] height)
        {
            var p1 = new Pallet(1, 1, pallet_h);
            foreach (int h in height)
            {
                p1.AddBox(new Box(1, 1, h, 1, DateOnly.MinValue, DateOnly.MaxValue));
            }
            Assert.Equal(expected, p1.Heigth);
        }

        public static IEnumerable<object[]> DataAddRemoveBox => new List<object[]> { 
            new object[] {4, CreateBoxWrapper(), CreateBoxWrapper(), CreateBoxWrapper(), CreateBoxWrapper() } 
        };

        [Theory]
        [MemberData(nameof(DataAddRemoveBox))]
        public void TestAddRemoveBox(int box_cnt, params Box[] boxes)
        {
            var pallet = CreatePallet();

            foreach(Box b in boxes)
            {
                pallet.AddBox(b);
            }
            Assert.Equal(pallet.GetBoxes().Count, box_cnt);

            pallet.RemoveBox(boxes[0]);
            Assert.Equal(pallet.GetBoxes().Count, box_cnt - 1);

            Assert.False(pallet.RemoveBox(boxes[0]));
            Assert.Equal(pallet.GetBoxes().Count, box_cnt - 1);

            pallet.RemoveBoxByID(boxes[1].Id);
            Assert.Equal(pallet.GetBoxes().Count, box_cnt - 2);

            Assert.False( pallet.RemoveBox(boxes[1]) );
            Assert.Equal(pallet.GetBoxes().Count, box_cnt - 2);
        }

        public static IEnumerable<object[]> DataPalletFit => new List<object[]> {
            new object[] {2, CreatePallet(5,5), CreateBoxWrapper(5, 5), CreateBoxWrapper(4, 4) },
            new object[] {2, CreatePallet(5,5), CreateBoxWrapper(4, 5), CreateBoxWrapper(5, 4) },
            new object[] {0, CreatePallet(5,5), 
                CreateBoxWrapper(6, 5), 
                CreateBoxWrapper(6, 4),
                CreateBoxWrapper(5, 6),
                CreateBoxWrapper(4, 6),
            }
        };

        [Theory]
        [MemberData(nameof(DataPalletFit))]
        public void TestPalletFit(int box_cnt_res, Pallet pallet, params Box[] boxes)
        {
            foreach (Box b in boxes)
            {
                pallet.AddBox(b);
            }
            Assert.Equal(box_cnt_res, pallet.GetBoxes().Count);
        }
    }
}