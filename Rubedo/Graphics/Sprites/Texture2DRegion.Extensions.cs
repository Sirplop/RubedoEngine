using Microsoft.Xna.Framework;
using Rubedo.Lib;
using System;

namespace Rubedo.Graphics.Sprites;

/// <summary>
/// Extension methods for <see cref="TextureRegion2D"/>.
/// </summary>
public static class Texture2DRegionExtensions
{
    public static TextureRegion2D GetSubregion(this TextureRegion2D source, in Rectangle region)
    {
        return source.GetSubregion(region.X, region.Y, region.Width, region.Height);
    }
    public static TextureRegion2D GetSubregion(this TextureRegion2D source, int x, int y, int width, int height)
    {
        ArgumentNullException.ThrowIfNull(source);
        Rectangle region = source.Bounds.GetRelativeRectangle(x, y, width, height);
        return new TextureRegion2D(source.Texture, region);
    }

    public static TextureRegion2D GetSubregionFromUVs(this TextureRegion2D source, float leftUV, float topUV, float width, float height)
    {
        ArgumentNullException.ThrowIfNull(source);

        Rectangle region = source.Bounds.GetRelativeRectangle(Lib.Math.FloorToInt(source.Width * leftUV), Lib.Math.FloorToInt(source.Height * topUV),
            Lib.Math.FloorToInt(source.Width * width), Lib.Math.FloorToInt(source.Height * height));
        return new TextureRegion2D(source.Texture, region);
    }

    /// <summary>
    /// Constructs a new NineSlice from the given region, with the given pixel padding on all edges.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <param name="top"></param>
    /// <param name="bottom"></param>
    /// <returns></returns>
    public static NineSlice CreateNineSlice(this TextureRegion2D source, int left, int right, int top, int bottom)
    {
        ArgumentNullException.ThrowIfNull(source);
        int middleWidth = source.Width - left - right;
        int middleHeight = source.Height - top - bottom;
        int rightX = source.Width - right;
        int bottomY = source.Height - bottom;

        TextureRegion2D[] slices = new TextureRegion2D[9];

        slices[0] = source.GetSubregion(0, 0, left, top);
        slices[1] = source.GetSubregion(left, 0, middleWidth, top);
        slices[2] = source.GetSubregion(rightX, 0, right, top);

        slices[3] = source.GetSubregion(0, top, left, middleHeight);
        slices[4] = source.GetSubregion(left, top, middleWidth, middleHeight);
        slices[5] = source.GetSubregion(rightX, top, right, middleHeight);

        slices[6] = source.GetSubregion(0, bottomY, left, bottom);
        slices[7] = source.GetSubregion(left, bottomY, middleWidth, bottom);
        slices[8] = source.GetSubregion(rightX, bottomY, right, bottom);

        return new NineSlice(slices);
    }

    /// <summary>
    /// Constructs a new NineSlice from the texture region, with the given UV padding on all edges.
    /// </summary>
    /// <remarks>Slice lines are a percent of the texture region, from 0 to 1. Left less than Right, Top less than Bottom.</remarks>
    public static NineSlice CreateNineSliceFromUVs(this TextureRegion2D source, float allUV)
    {
        return source.CreateNineSliceFromUVs(allUV, 1 - allUV, allUV, 1 - allUV);
    }
    /// <summary>
    /// Constructs a new NineSlice from the texture region, using the given UV coordinate slice lines.
    /// </summary>
    /// <remarks>Slice lines are a percent of the texture region, from 0 to 1. Left less than Right, Top less than Bottom.</remarks>
    public static NineSlice CreateNineSliceFromUVs(this TextureRegion2D source, float leftUV, float rightUV, float topUV, float bottomUV)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (leftUV < 0 || leftUV > rightUV)
            throw new ArgumentOutOfRangeException(nameof(leftUV));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(rightUV, 1);
        if (topUV < 0 || topUV > bottomUV)
            throw new ArgumentOutOfRangeException(nameof(topUV));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(bottomUV, 1);

        float middleWidth = rightUV - leftUV;
        float middleHeight = bottomUV - topUV;
        float rightWidth = 1 - rightUV;
        float bottomHeight = 1 - bottomUV;

        //construct all 9 slice regions from the UV regions.
        TextureRegion2D[] slices = new TextureRegion2D[9];

        slices[0] = source.GetSubregionFromUVs(0, 0, leftUV, topUV);
        slices[1] = source.GetSubregionFromUVs(leftUV, 0, middleWidth, topUV);
        slices[2] = source.GetSubregionFromUVs(rightUV, 0, rightWidth, topUV);

        slices[3] = source.GetSubregionFromUVs(0, topUV, leftUV, middleHeight);
        slices[4] = source.GetSubregionFromUVs(leftUV, topUV, middleWidth, middleHeight);
        slices[5] = source.GetSubregionFromUVs(rightUV, topUV, rightWidth, middleHeight);

        slices[6] = source.GetSubregionFromUVs(0, bottomUV, leftUV, bottomHeight);
        slices[7] = source.GetSubregionFromUVs(leftUV, bottomUV, middleWidth, bottomHeight);
        slices[8] = source.GetSubregionFromUVs(rightUV, bottomUV, rightWidth, bottomHeight);

        return new NineSlice(slices);
    }
}