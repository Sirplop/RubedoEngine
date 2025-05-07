using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

namespace Rubedo.Rendering;

public sealed class Camera
{
    public readonly static float MIN_Z = 1f;
    public readonly static float MAX_Z = 2048;
    public readonly static int MIN_ZOOM = 1;
    public readonly static int MAX_ZOOM = 32;

    private Vector2 position;
    private float z;
    private float baseZ;

    private float aspectRatio;
    private float fov;
    private int zoom;

    private Matrix view;
    private Matrix proj;
    private Matrix posMatrix;

    public Matrix View { get
        {
            if (_needsProjMatrixUpdate)
                UpdateMatrices();
            return view;
        } 
    }
    public Matrix Projection
    {
        get
        {
            if (_needsProjMatrixUpdate)
                UpdateMatrices();
            return proj;
        }
    }
    public Matrix TransformMatrix
    {
        get
        {
            if (_needsPosMatrixUpdate)
                UpdateMatrices();
            return posMatrix;
        }
    }
    public float GetZ() => z;
    public int GetZoom() => zoom;

    private bool _needsPosMatrixUpdate = false;
    private bool _needsProjMatrixUpdate = false;

    public Camera(Screen screen)
    {
        if (screen is null)
            throw new ArgumentNullException("screen");

        position = Vector2.Zero;

        aspectRatio = (float)screen.Width / screen.Height;
        fov = MathHelper.PiOver2;

        baseZ = GetZFromHeight(screen.Height);
        z = baseZ;
        zoom = 1;

        _needsPosMatrixUpdate = true;
        _needsProjMatrixUpdate = true;
        UpdateMatrices();
    }

    public void UpdateMatrices()
    {
        if (_needsProjMatrixUpdate)
        {
            view = Matrix.CreateLookAt(new Vector3(0, 0, z), Vector3.Zero, Vector3.Up);
            proj = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, MIN_Z, MAX_Z);
            _needsProjMatrixUpdate = false;
        }
        if (_needsPosMatrixUpdate)
        {
            //negate position so that the camera is in a sane position and makes sense.
            posMatrix = Matrix.CreateTranslation(-position.X, -position.Y, 0f);
            _needsPosMatrixUpdate = false;
        }
    }

    public float GetZFromHeight(float height)
    {
        return 0.5f * height / MathF.Tan(0.5f * fov);
    }
    public float GetHeightFromZ()
    {
        return z * MathF.Tan(0.5f * fov) * 2;
    }

    public void MoveZ(float amount)
    {
        z = Lib.Math.Clamp(z + amount, MIN_Z, MAX_Z);
        _needsProjMatrixUpdate = true;
    }
    public void ResetZ()
    {
        z = baseZ;
        _needsProjMatrixUpdate = true;
    }
    public void Move(Vector2 amount)
    {
        position += amount;
        _needsPosMatrixUpdate = true;
    }
    public void MoveTo(Vector2 position)
    {
        this.position = position;
        _needsPosMatrixUpdate = true;
    }

    public void IncZoom()
    {
        zoom = Lib.Math.Clamp(--zoom, MIN_ZOOM, MAX_ZOOM);
        z = baseZ / zoom;
        _needsProjMatrixUpdate = true;
    }
    public void DecZoom()
    {
        zoom = Lib.Math.Clamp(++zoom, MIN_ZOOM, MAX_ZOOM);
        z = baseZ / zoom;
        _needsProjMatrixUpdate = true;
    }
    public void SetZoom(int zoom)
    {
        if (this.zoom == zoom)
            return;
        this.zoom = Lib.Math.Clamp(zoom, MIN_ZOOM, MAX_ZOOM);
        z = baseZ / zoom;
        _needsProjMatrixUpdate = true;
    }

    public void GetExtents(out float width, out float height)
    {
        height = GetHeightFromZ();
        width = height * aspectRatio;
    }
    public void GetExtents(out float left, out float right, out float top, out float bottom)
    {
        GetExtents(out float width, out float height);
        left = position.X - width * 0.5f;
        right = left + width;
        bottom = position.Y - height * 0.5f;
        top = bottom + height;
    }
    public void GetExtents(out Vector2 min, out Vector2 max)
    {
        GetExtents(out float left, out float right, out float top, out float bottom);
        min = new Vector2(left, bottom);
        max = new Vector2(right, top);
    }

    public Vector2 ScreenToWorldPoint(Vector2 screenPoint)
    {
        GetExtents(out Vector2 min, out Vector2 max);
        int viewWidth = RubedoEngine.Instance.Screen.Width;
        int viewHeight = RubedoEngine.Instance.Screen.Height;

        float posX = MathHelper.Lerp(min.X, max.X, screenPoint.X / viewWidth);
        float posY = MathHelper.Lerp(min.Y, max.Y, 1 - screenPoint.Y / viewHeight);

        return new Vector2(posX, posY);
    }

    public Vector2 WorldToScreenPoint(Vector2 worldPoint)
    {
        GetExtents(out Vector2 min, out Vector2 max);
        int viewWidth = RubedoEngine.Instance.Screen.Width;
        int viewHeight = RubedoEngine.Instance.Screen.Height;
        float worldWidth = max.X - min.X;
        float worldHeight = max.Y - min.Y;

        //need to account for letterboxing.
        int subWidth = (RubedoEngine.Instance.GraphicsDevice.Viewport.Width - viewWidth) / 2;
        int subHeight = (RubedoEngine.Instance.GraphicsDevice.Viewport.Height - viewHeight) / 2;

        float posX = MathHelper.Lerp(0, viewWidth, (worldPoint.X - min.X) / worldWidth) - subWidth;
        float posY = MathHelper.Lerp(0, viewHeight, 1 - (worldPoint.Y - min.Y) / worldHeight) - subHeight;

        return new Vector2(posX, posY);
    }
}