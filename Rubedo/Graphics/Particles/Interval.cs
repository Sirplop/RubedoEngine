using System;
using System.Collections.Generic;
using System.Text;

namespace Rubedo.Graphics.Particles;

public class Interval
{
    private static Random rand=new Random();
    private readonly double min;
    private readonly double max;
    public Interval(double a, double b)
    {
        min = a;
        max = b;
    }

    public double GetValue()
    {            
        return rand.NextDouble() * (max - min) + min;
    }

}
