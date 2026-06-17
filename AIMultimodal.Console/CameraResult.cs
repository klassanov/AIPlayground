using System;
using System.Collections.Generic;
using System.Text;

namespace AIMultimodal.Console
{
    //2 ways of generating strucutred output in C# and .NET
    //The LLM shall support structured outputs
    //1. Use a class (current case)
    //2. Use a json schema

    internal record CameraResult(bool IsBroken, TrafficCongestionLevel CongestionLevel, string Analysis);

    public enum TrafficCongestionLevel
    {
        Light,
        Moderate,
        High,
        Unknown
    }

}
