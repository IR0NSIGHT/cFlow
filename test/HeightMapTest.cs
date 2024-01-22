namespace TestProject1
{
    [TestClass]
    public class HeightMapTest
    {
        [TestMethod]
        public void InitHeightMap()
        {
            var heightmap = new DummyDimension((8, 12), 17);
            Assert.AreEqual((8, 12), heightmap.Bounds());

            foreach (var point in heightmap.iterator().Points())
            {
                Assert.AreEqual(17, heightmap.GetHeight(point.x, point.y));
            }


        }
        [TestMethod]
        public void ChangeHeight()
        {
            var heightmap = new DummyDimension((8, 12), 17);
            heightmap.SetHeight(new Point(7, 5, 3));
            Assert.AreEqual(3, heightmap.GetHeight(7, 5));

            //only changed height of that single point
            foreach(var point in heightmap.iterator().Points())
            {
                if (!(point.x == 7 && point.y == 5))
                    Assert.AreEqual(17, heightmap.GetHeight(point.x, point.y));
            }
        }

        [TestMethod]
        public void IterateLines()
        {
            //iterates lines = increase y
            //start at zero
            //iterates row = increase x
            var heightmap = new DummyDimension((8, 12), 17);
            int row = 0;
            foreach(var iterateLine in heightmap.iterator().PointsByRow())
            {
                int column = 0;
                foreach (var point in iterateLine)
                {
                    Assert.AreEqual(row, point.y);
                    Assert.AreEqual(column, point.x);
                    column++; 
                }
                row ++;
            }
        }
    }
}