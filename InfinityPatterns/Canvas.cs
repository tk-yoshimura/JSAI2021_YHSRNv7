using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace InfinityPatterns {
    public class Canvas : IDisposable {
        private const int scale = 2, margin = 1;

        private readonly Bitmap bitmap;
        private readonly Graphics graphics;
        public Size Size { private set; get; }
        public float Scale{ private set; get; }

        public Canvas(int size) {
            int size_with_margin = size + margin * 2;

            this.bitmap = new Bitmap(size_with_margin * scale, size_with_margin * scale, PixelFormat.Format32bppArgb);
            this.graphics = Graphics.FromImage(bitmap);
            this.Size = new Size(size_with_margin, size_with_margin);

            this.Scale = size_with_margin * scale;

            this.graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            this.graphics.SmoothingMode = SmoothingMode.HighQuality;
        }

        public void Save(string filename) {
            if (!Directory.Exists(Path.GetDirectoryName(filename))) {
                throw new DirectoryNotFoundException(Path.GetDirectoryName(filename));
            }

            using Bitmap bitmap_out = CanvasUtil.Reduction(bitmap, scale);

            using Bitmap bitmap_clip = bitmap_out.Clone(
                new Rectangle(margin, margin, Size.Width - margin * 2, Size.Height - margin * 2), bitmap_out.PixelFormat
            );

            bitmap_clip.Save(filename);
        }

        public void Clear(Color color) {
            this.graphics.Clear(color);
        }

        public void DrawLine(Color color, float stroke_width, PointF pt1, PointF pt2) {
            graphics.DrawLine(
                new Pen(color, stroke_width * scale) { LineJoin = LineJoin.Miter },
                new PointF(pt1.X * Scale, pt1.Y * Scale),
                new PointF(pt2.X * Scale, pt2.Y * Scale)
            );
        }

        public void DrawLines(Color color, float stroke_width, params PointF[] pts) {
            graphics.DrawLines(
                new Pen(color, stroke_width * scale) { LineJoin = LineJoin.Miter },
                pts.Select((pt) => new PointF(pt.X * Scale, pt.Y * Scale)).ToArray()
            );
        }

        public void DrawCurve(Color color, float stroke_width, params PointF[] pts) {
            graphics.DrawCurve(
                new Pen(color, stroke_width * scale) { LineJoin = LineJoin.Miter },
                pts.Select((pt) => new PointF(pt.X * Scale, pt.Y * Scale)).ToArray()
            );
        }

        public void DrawClosedCurve(Color color, float stroke_width, params PointF[] pts) {
            graphics.DrawClosedCurve(
                new Pen(color, stroke_width * scale) { LineJoin = LineJoin.Miter },
                pts.Select((pt) => new PointF(pt.X * Scale, pt.Y * Scale)).ToArray()
            );
        }

        public void DrawPolygon(Color color, float stroke_width, params PointF[] pts) {
            graphics.DrawPolygon(
                new Pen(color, stroke_width * scale) { LineJoin = LineJoin.Miter },
                pts.Select((pt) => new PointF(pt.X * Scale, pt.Y * Scale)).ToArray()
            );
        }

        public void DrawTriangle(Color color, float stroke_width, PointF pt1, PointF pt2, PointF pt3) {
            DrawPolygon(color, stroke_width, pt1, pt2, pt3);
        }

        public void FillClosedCurve(Color color, params PointF[] pts) {
            graphics.FillClosedCurve(
                new SolidBrush(color),
                pts.Select((pt) => new PointF(pt.X * Scale, pt.Y * Scale)).ToArray()
            );
        }

        public void FillPolygon(Color color, params PointF[] pts) {
            graphics.FillPolygon(
                new SolidBrush(color),
                pts.Select((pt) => new PointF(pt.X * Scale, pt.Y * Scale)).ToArray()
            );
        }

        public void FillPolygon(Color[] colors, params PointF[] pts) {
            if (colors.Length != pts.Length) {
                throw new ArgumentException("mismatch length");
            }

            PointF[] pts_scaled = pts.Select((pt) => new PointF(pt.X * Scale, pt.Y * Scale)).ToArray();

            using PathGradientBrush brush = new PathGradientBrush(pts_scaled) {
                SurroundColors = colors,
                CenterColor = CanvasUtil.AverageColor(colors)
            };

            graphics.FillPolygon(
                brush,
                pts_scaled
            );
        }

        public void FillPolygon(Brush brush, params PointF[] pts) {
            using Matrix matrix_origin = graphics.Transform.Clone();

            graphics.ScaleTransform(Scale, Scale);

            PointF[] pts_scaled = pts.Select((pt) => new PointF(pt.X, pt.Y)).ToArray();

            graphics.FillPolygon(
                brush,
                pts_scaled
            );

            graphics.Transform = matrix_origin;
        }

        public void FillTriangle(Color color, PointF pt1, PointF pt2, PointF pt3) {
            FillPolygon(color, pt1, pt2, pt3);
        }

        public void FillTriangle(Color color1, Color color2, Color color3, PointF pt1, PointF pt2, PointF pt3) {
            FillPolygon(new Color[]{ color1, color2, color3 }, pt1, pt2, pt3);
        }

        public void DrawCircle(Color color, float stroke_width, PointF pt, float radius) {
            graphics.DrawEllipse(
                new Pen(color, stroke_width * scale) { LineJoin = LineJoin.Miter },
                new RectangleF((pt.X - radius) * Scale, (pt.Y - radius) * Scale, radius * Scale * 2, radius * Scale * 2)
            );
        }

        public void FillCircle(Color color, PointF pt, float radius) {
            graphics.FillEllipse(
                new SolidBrush(color),
                new RectangleF((pt.X - radius) * Scale, (pt.Y - radius) * Scale, radius * Scale * 2, radius * Scale * 2)
            );
        }

        public void DrawEllipse(Color color, float stroke_width, PointF pt, float radius_x, float radius_y, float angle) {
            using Matrix matrix_origin = graphics.Transform.Clone();

            graphics.TranslateTransform(+pt.X * Scale, +pt.Y * Scale);
            graphics.RotateTransform(angle);
            graphics.TranslateTransform(-pt.X * Scale, -pt.Y * Scale);

            graphics.DrawEllipse(
                new Pen(color, stroke_width * scale) { LineJoin = LineJoin.Miter },
                new RectangleF((pt.X - radius_x) * Scale, (pt.Y - radius_y) * Scale, radius_x * Scale * 2, radius_y * Scale * 2)
            );

            graphics.Transform = matrix_origin;
        }

        public void FillEllipse(Color color, PointF pt, float radius_x, float radius_y, float angle) {
            using Matrix matrix_origin = graphics.Transform.Clone();

            graphics.TranslateTransform(+pt.X * Scale, +pt.Y * Scale);
            graphics.RotateTransform(angle);
            graphics.TranslateTransform(-pt.X * Scale, -pt.Y * Scale);

            graphics.FillEllipse(
                new SolidBrush(color),
                new RectangleF((pt.X - radius_x) * Scale, (pt.Y - radius_y) * Scale, radius_x * Scale * 2, radius_y * Scale * 2)
            );

            graphics.Transform = matrix_origin;
        }

        public void FillEllipse(Brush brush, PointF pt, float radius_x, float radius_y, float angle) {
            using Matrix matrix_origin = graphics.Transform.Clone();

            graphics.ScaleTransform(Scale, Scale);
            graphics.TranslateTransform(+pt.X, +pt.Y);
            graphics.RotateTransform(angle);
            graphics.TranslateTransform(-pt.X, -pt.Y );

            graphics.FillEllipse(
                brush,
                new RectangleF((pt.X - radius_x), (pt.Y - radius_y), radius_x * 2, radius_y * 2)
            );

            graphics.Transform = matrix_origin;
        }

        public void DrawArc(Color color, float stroke_width, PointF pt1, PointF pt2, float curvature) {

            (PointF ptc, float radius, float start_angle, float sweep_angle) = CanvasUtil.CircumArc(pt1, pt2, curvature);

            if (radius < 100f) {
                graphics.DrawArc(
                    new Pen(color, stroke_width * scale) { LineJoin = LineJoin.Miter },
                    new RectangleF((ptc.X - radius) * Scale, (ptc.Y - radius) * Scale, radius * Scale * 2, radius * Scale * 2),
                    start_angle, sweep_angle
                );
            }
            else {
                DrawLine(color, stroke_width, pt1, pt2);
            }
        }
                
        public void DrawArc(Color color, float stroke_width, PointF pt, float radius_x, float radius_y, float angle, float start_angle, float sweep_angle) {
            using Matrix matrix_origin = graphics.Transform.Clone();

            graphics.TranslateTransform(+pt.X * Scale, +pt.Y * Scale);
            graphics.RotateTransform(angle);
            graphics.TranslateTransform(-pt.X * Scale, -pt.Y * Scale);

            graphics.DrawArc(
                new Pen(color, stroke_width * scale) { LineJoin = LineJoin.Miter },
                new RectangleF((pt.X - radius_x) * Scale, (pt.Y - radius_y) * Scale, radius_x * Scale * 2, radius_y * Scale * 2),
                start_angle, sweep_angle 
            );

            graphics.Transform = matrix_origin;
        }

        public void DrawCrescent(Color color, float stroke_width, PointF pt1, PointF pt2, float curvature1, float curvature2) {

            using GraphicsPath path = new GraphicsPath();

            path.AddCrescent(pt1, pt2, curvature1, curvature2);

            DrawPath(color, stroke_width, path);
        }

        public void FillCrescent(Color color, PointF pt1, PointF pt2, float curvature1, float curvature2) {

            using GraphicsPath path = new GraphicsPath();

            path.AddCrescent(pt1, pt2, curvature1, curvature2);

            FillPath(color, path);
        }

        public void FillCrescent(Color color1, Color color2, PointF pt1, PointF pt2, float curvature1, float curvature2) {

            using LinearGradientBrush brush = new LinearGradientBrush(pt1, pt2, color1, color2) {
                WrapMode = WrapMode.TileFlipXY
            };

            using GraphicsPath path = new GraphicsPath();

            path.AddCrescent(pt1, pt2, curvature1, curvature2);

            FillPath(brush, path);
        }

        public void DrawRectangle(Color color, float stroke_width, RectangleF rectangle) {
            graphics.DrawRectangle(
                new Pen(color, stroke_width * scale) { LineJoin = LineJoin.Miter },
                rectangle.X * Scale, rectangle.Y * Scale,
                rectangle.Width * Scale, rectangle.Height * Scale
            );
        }

        public void FillRectangle(Color color, RectangleF rectangle) { 
            graphics.FillRectangle(
                new SolidBrush(color),
                rectangle.X * Scale, rectangle.Y * Scale,
                rectangle.Width * Scale, rectangle.Height * Scale
            );
        }

        public void DrawPath(Color color, float stroke_width, GraphicsPath path) {
            using Matrix matrix_origin = graphics.Transform.Clone();

            graphics.ScaleTransform(Scale, Scale);

            graphics.DrawPath(
                new Pen(color, stroke_width * scale / Scale) { LineJoin = LineJoin.Miter },
                path
            );

            graphics.Transform = matrix_origin;
        }

        public void FillPath(Color color, GraphicsPath path) {
            FillPath(new SolidBrush(color), path);
        }

        public void FillPath(Brush brush, GraphicsPath path) {
            using Matrix matrix_origin = graphics.Transform.Clone();

            graphics.ScaleTransform(Scale, Scale);

            graphics.FillPath(brush, path);

            graphics.Transform = matrix_origin;
        }

        public void SetClip(GraphicsPath path, CombineMode combine_mode = CombineMode.Replace) { 
            using Matrix matrix_origin = graphics.Transform.Clone();

            graphics.ScaleTransform(Scale, Scale);

            graphics.SetClip(path, combine_mode);

            graphics.Transform = matrix_origin;
        }

        public void SetClip(RectangleF rect, CombineMode combine_mode = CombineMode.Replace) { 
            using Matrix matrix_origin = graphics.Transform.Clone();

            graphics.ScaleTransform(Scale, Scale);

            graphics.SetClip(rect, combine_mode);

            graphics.Transform = matrix_origin;
        }

        public void ResetClip() { 
            graphics.ResetClip();
        }

        public Region Clip { 
            get {
                return graphics.Clip;
            }
            set {
                graphics.Clip = value;
            }
        }
        public void Dispose() {
            bitmap.Dispose();
            graphics.Dispose();
        }
    }

    public static class GraphicPathExtension {
        public static void AddArc(this GraphicsPath path, PointF pt1, PointF pt2, float curvature) {
            (PointF ptc, float radius, float start_angle, float sweep_angle) = CanvasUtil.CircumArc(pt1, pt2, curvature);

            if (radius < 100f) {
                path.AddArc(
                    new RectangleF(ptc.X - radius, ptc.Y - radius, radius * 2, radius * 2),
                    start_angle, sweep_angle
                );
            }
            else {
                path.AddLine(pt1, pt2);
            }
        }

        public static void AddTriangle(this GraphicsPath path, PointF pt1, PointF pt2, PointF pt3) {
            path.AddPolygon(new PointF[] { pt1, pt2, pt3 });
        }

        public static void AddCrescent(this GraphicsPath path, PointF pt1, PointF pt2, float curvature1, float curvature2) {
            path.AddArc(pt1, pt2, curvature1);
            path.AddArc(pt2, pt1, curvature2);
            path.CloseFigure();
        }

        public static void AddEllipse(this GraphicsPath path, PointF pt, float radius_x, float radius_y, float angle) {
            using Matrix matrix = new Matrix();
            matrix.Translate(+pt.X, +pt.Y);
            matrix.Rotate(angle);
            matrix.Translate(-pt.X, -pt.Y);

            path.AddEllipse(
                new RectangleF((pt.X - radius_x), (pt.Y - radius_y), radius_x * 2, radius_y * 2)
            );

            path.Transform(matrix);
        }
    }

    public static class CanvasUtil {
        public static Color AverageColor(Color[] colors) {
            return Color.FromArgb(
                colors.Select((cr) => (int)cr.A).Sum() / colors.Count(),
                colors.Select((cr) => (int)cr.R).Sum() / colors.Count(),
                colors.Select((cr) => (int)cr.G).Sum() / colors.Count(),
                colors.Select((cr) => (int)cr.B).Sum() / colors.Count()
            );
        }

        public static (PointF ptc, float radius, float start_angle, float sweep_angle) CircumArc(PointF pt1, PointF pt2, float curvature) {

            double x1 = pt1.X, y1 = pt1.Y, x2 = pt2.X, y2 = pt2.Y;
            double x3 = (x1 + x2) / 2 + (y1 - y2) * curvature, y3 = (y1 + y2) / 2 - (x1 - x2) * curvature;
            double d1 = x1 * x1 + y1 * y1;
            double d2 = x2 * x2 + y2 * y2;
            double d3 = x3 * x3 + y3 * y3;
            double u = 0.5 / (x1 * y2 - x2 * y1 + x2 * y3 - x3 * y2 + x3 * y1 - x1 * y3);

            if (!double.IsFinite(u)) {
                return (new PointF(float.NaN, float.NaN), float.NaN, float.NaN, float.NaN);
            }

            double xc = u * (d1 * y2 - d2 * y1 + d2 * y3 - d3 * y2 + d3 * y1 - d1 * y3);
            double yc = u * (x1 * d2 - x2 * d1 + x2 * d3 - x3 * d2 + x3 * d1 - x1 * d3);
            double r = Math.Sqrt((xc - x1) * (xc - x1) + (yc - y1) * (yc - y1));

            PointF ptc = new PointF((float)xc, (float)yc);
            float radius = (float)r;

            double arg1 = Math.Atan2(y1 - yc, x1 - xc);
            double arg2 = Math.Atan2(y2 - yc, x2 - xc);
            double arg3 = Math.Atan2(y3 - yc, x3 - xc);

            bool is_over = (arg1 < arg3) ^ (arg3 < arg2);
            
            float start_angle = (float)(arg1 / Math.PI * 180);
            float sweep_angle = (float)((is_over ? (arg2 - arg1 - Math.Sign(curvature) * Math.PI * 2) : (arg2 - arg1)) / Math.PI * 180);

            return (ptc, radius, start_angle, sweep_angle);
        }

        public unsafe static Bitmap Reduction(Bitmap src, int scale) {
            if (scale <= 1) {
                throw new ArgumentOutOfRangeException(nameof(scale));
            }

            int sq_scale = scale * scale;

            Bitmap dst = new Bitmap(src.Width / scale, src.Height / scale, PixelFormat.Format32bppArgb);
            
            BitmapData srcdata = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] srcarr = new byte[src.Width * src.Height * 4];
            byte[] dstarr = new byte[dst.Width * dst.Height * 4];

            Marshal.Copy(srcdata.Scan0, srcarr, 0, srcarr.Length);

            fixed (byte* s = srcarr, d = dstarr) {
                for (int oy = 0; oy < dst.Height; oy++) {
                    for (int ox = 0, o = (ox + oy * dst.Width) * 4; ox < dst.Width; ox++, o += 4) {
                        int c0 = 0, c1 = 0, c2 = 0, c3 = 0;

                        for (int ky = 0, iy = oy * scale; ky < scale; ky++, iy++) {
                            for (int kx = 0, ix = ox * scale, i = (ix + iy * src.Width) * 4; kx < scale; kx++, i += 4) {
                                c0 += s[i + 0]; c1 += s[i + 1]; c2 += s[i + 2]; c3 += s[i + 3];
                            }
                        }

                        c0 /= sq_scale; c1 /= sq_scale; c2 /= sq_scale; c3 /= sq_scale;

                        d[o + 0] = unchecked((byte)c0);
                        d[o + 1] = unchecked((byte)c1);
                        d[o + 2] = unchecked((byte)c2);
                        d[o + 3] = unchecked((byte)c3);
                    }
                }
            }

            src.UnlockBits(srcdata);

            BitmapData dstdata = dst.LockBits(new Rectangle(0, 0, dst.Width, dst.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(dstarr, 0, dstdata.Scan0, dstarr.Length);

            dst.UnlockBits(dstdata);

            return dst; 
        }
    }
}
