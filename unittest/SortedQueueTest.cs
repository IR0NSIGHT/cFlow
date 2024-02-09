using application.Maps.flowMap;

namespace unittest;
[TestFixture]
public class SortedQueueTest
{
    [Test]
    public void IsUnique()
    {
        var s = new SortedQueue();

        s.TryInsert((17,23),12);
        s.TryInsert((3, 5), 12);
        s.TryInsert((7, 9), 12);

        Assert.That(s.Get((17,23)).value, Is.EqualTo(12));

        //ignores higher values
        s.TryInsert((17, 23), 33);
        Assert.That(s.Get((17, 23)).value, Is.EqualTo(12));

        //accepts lower values
        s.TryInsert((17, 23), 4);
        Assert.That(s.Get((17, 23)).value, Is.EqualTo(4));

    }
}