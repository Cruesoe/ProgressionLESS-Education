using System;

namespace ProgressionEducation;

// Marks types the HotSwap dev tool may reload while the game runs
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class HotSwappableAttribute : Attribute
{
}
