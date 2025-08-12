using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rubedo.Graphics.Particles.Origins;

public class RectangleOrigin : Origin
{
    private readonly Interval x;
    private readonly Interval y;
    private readonly bool _edge;
    private readonly float _width;
    private readonly float _height;

    public override bool UseColorData => false;

    public RectangleOrigin(float width, float height, bool edge = false)
    {
        _edge = edge;
        _width = width;
        _height = height;
        x = new Interval(-width / 2, width / 2);
        y = new Interval(-height / 2, height / 2);
    }

    public override OriginData GetPosition(Emitter e)
    {
        if (_edge)
        {
            float n = Lib.Random.Next(_width + _height);

            if (n < _width)
            {

                if (Lib.Random.Flip)
                    return new OriginData(new Vector2((float)x.GetValue(), -_height / 2));
                else
                    return new OriginData(new Vector2((float)x.GetValue(), _height / 2));
            }
            else
            {
                if (Lib.Random.Flip)
                    return new OriginData(new Vector2(-_width / 2, (float)y.GetValue()));
                else
                    return new OriginData(new Vector2(_width / 2, (float)y.GetValue()));
            }
        }
        else
        {
            return new OriginData(new Vector2((float)x.GetValue(), (float)y.GetValue()));
        }
    }
}
