using System;
using System.Collections.Generic;
using System.Text;

namespace split_wav.Models
{
    public class Segment
    {
        public double OffsetInSeconds { get; set; }
        public double DurationInSeconds { get; set; }
    }

    public class RootObject
    {
        public List<Segment> Segments { get; set; }
    }
}
