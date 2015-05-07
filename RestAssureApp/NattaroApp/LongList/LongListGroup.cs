using System.Collections.Generic;

namespace RestAssure.LongList
{
    public class LongListGroup<T> : List<T>
    {
        public LongListGroup(string name, IEnumerable<T> items)
            : base(items)
        {
            Key = name;
        }

        public string Key { get; private set; }
    }
}