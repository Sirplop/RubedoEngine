//Heavily based off of Apos.Camera: https://github.com/Apostolique/Apos.Camera

using Microsoft.Xna.Framework;
using Rubedo.Lib;
using Rubedo.Rendering.Viewports;
using System;

namespace Rubedo.Rendering;

/// <summary>
/// TODO: I am NeoCamera, and I don't have a summary yet.
/// </summary>
public class NeoCamera
{
    private Vector2 _xy = Vector2.Zero;
    private Vector3 _xyz = new Vector3(Vector2.Zero, 1f);
    private float _focalLength = 1f;
    private int _order;

    public float X
    {
        get => _xy.X;
        set
        {
            _xy.X = value;
            _xyz.X = value;
        }
    }
    public float Y
    {
        get => _xy.Y;
        set
        {
            _xy.Y = value;
            _xyz.Y = value;
        }
    }
    public float Z
    {
        get => _xyz.Z;
        set
        {
            _xyz.Z = value;
        }
    }

    public float FocalLength
    {
        get => _focalLength;
        set
        {
            _focalLength = value > 0.01f ? value : 0.01f;
        }
    }

    public float Rotation { get; set; } = 0f;
    public Vector2 Scale { get; set; } = Vector2.One;

    public Vector2 XY
    {
        get => _xy;
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }
    public Vector3 XYZ
    {
        get => _xyz;
        set
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
        }
    }

    public IVirtualViewport VirtualViewport
    {
        get;
        set;
    }

    public int Order => _order;

    public NeoCamera(IVirtualViewport virtualViewport, int order)
    {
        VirtualViewport = virtualViewport;
        _order = order;
    }

    public void SetViewport()
    {
        VirtualViewport.Set();
    }
    public void ResetViewport()
    {
        VirtualViewport.Reset();
    }

    public Matrix View => GetView(0);
    public Matrix ViewInvert => GetViewInvert(0);

    public Matrix GetView(float z = 0)
    {
        float scaleZ = ZToScale(_xyz.Z, z);
        return VirtualViewport.Transform(
            Matrix.CreateTranslation(new Vector3(-XY, 0f)) *
            Matrix.CreateRotationZ(Rotation) *
            Matrix.CreateScale(Scale.X, -Scale.Y, 1f) *
            Matrix.CreateScale(scaleZ, scaleZ, 1f) *
            Matrix.CreateTranslation(new Vector3(VirtualViewport.Origin, 0f)));
    }
    public Matrix GetView3D()
    {
        return
            Matrix.CreateLookAt(XYZ, new Vector3(XY, Z - 1), new Vector3((float)MathF.Sin(Rotation), (float)MathF.Cos(Rotation), 0)) *
            Matrix.CreateScale(Scale.X, -Scale.Y, 1f);
    }
    public Matrix GetViewInvert(float z = 0) => Matrix.Invert(GetView(z));

    public Matrix GetProjection()
    {
        return Matrix.CreateOrthographicOffCenter(0, VirtualViewport.Width, VirtualViewport.Height, 0, 0, 1);
    }
    public Matrix GetProjection3D(float nearPlaneDistance = 0.01f, float farPlaneDistance = 100f)
    {
        var aspect = VirtualViewport.VirtualWidth / (float)VirtualViewport.VirtualHeight;
        var fov = (float)MathF.Atan(VirtualViewport.VirtualHeight / 2f / FocalLength) * 2f;

        return Matrix.CreatePerspectiveFieldOfView(fov, aspect, nearPlaneDistance, farPlaneDistance);
    }

    public float ScaleToZ(float scale, float targetZ)
    {
        if (scale == 0)
        {
            return float.MaxValue;
        }
        return FocalLength / scale + targetZ;
    }
    public float ZToScale(float z, float targetZ)
    {
        if (z - targetZ == 0)
        {
            return float.MaxValue;
        }
        return FocalLength / (z - targetZ);
    }

    public float WorldToScreenScale(float z = 0f) => Vector2.Distance(WorldToScreen(0f, 0f, z), WorldToScreen(1f, 0f, z));
    public float ScreenToWorldScale(float z = 0f) => Vector2.Distance(ScreenToWorld(0f, 0f, z), ScreenToWorld(1f, 0f, z));

    public Vector2 WorldToScreen(float x, float y, float z = 0f) => WorldToScreen(new Vector2(x, y), z);
    public Vector2 WorldToScreen(Vector2 xy, float z = 0f)
    {
        return Vector2.Transform(xy, GetView(z)) + VirtualViewport.XY;
    }
    public Vector2 ScreenToWorld(float x, float y, float z = 0f) => ScreenToWorld(new Vector2(x, y), z);
    public Vector2 ScreenToWorld(Vector2 xy, float z = 0f)
    {
        return Vector2.Transform(xy - VirtualViewport.XY, GetViewInvert(z));
    }

    public bool IsZVisible(float z, float minDistance = 0.1f)
    {
        float scaleZ = ZToScale(Z, z);
        float maxScale = ZToScale(minDistance, 0f);

        return scaleZ > 0 && scaleZ < maxScale;
    }

    public RectF ViewRect => GetViewRect(0);
    public RectF GetViewRect(float z = 0)
    {
        BoundingFrustum frustum = GetBoundingFrustum(z);
        Vector3[] corners = frustum.GetCorners();
        Vector3 a = corners[0];
        Vector3 b = corners[1];
        Vector3 c = corners[2];
        Vector3 d = corners[3];

        float left = Lib.Math.Min(a.X, b.X, c.X, d.X);
        float right = Lib.Math.Max(a.X, b.X, c.X, d.X);
        float top = Lib.Math.Min(a.Y, b.Y, c.Y, d.Y);
        float bottom = Lib.Math.Max(a.Y, b.Y, c.Y, d.Y);

        float width = right - left;
        float height = bottom - top;

        return new RectF(left, top, width, height);
    }
    public BoundingFrustum GetBoundingFrustum(float z = 0)
    {
        // TODO: Use 3D view and projection?
        Matrix view = GetView(z);
        Matrix projection = GetProjection();
        return new BoundingFrustum(view * projection);
    }

    public void GetExtents(out float width, out float height)
    {
        width = VirtualViewport.Width / 24f;
        height = VirtualViewport.Height / 24f;
    }
}