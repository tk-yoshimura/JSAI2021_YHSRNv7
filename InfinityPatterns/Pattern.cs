using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace InfinityPatterns {
    public static class Pattern {

        public static void DrawParallelLines(Canvas canvas, Random random,
                                             float stroke_width,
                                             float stroke_shift, float loc_randrange,
                                             bool color_rand, bool stroke_width_rand) {

            int lines = RandomLines(random);

            stroke_shift *= random.NextUniform(0.4f, 1.2f) / canvas.Scale;

            PointF pt1 = RandomPointF(random), pt2 = RandomPointF(random);

            double dx = pt1.X - pt2.X, dy = pt1.Y - pt2.Y, norm = Math.Sqrt(dx * dx + dy * dy);
            double nx = dx / norm, ny = dy / norm;

            Color color = RandomColor(random);

            for (int i = 0; i < lines; i++) {
                float px1 = (float)(pt1.X + i * stroke_shift * ny + random.NextUniform(loc_randrange));
                float px2 = (float)(pt2.X + i * stroke_shift * ny + random.NextUniform(loc_randrange));
                float py1 = (float)(pt1.Y - i * stroke_shift * nx + random.NextUniform(loc_randrange));
                float py2 = (float)(pt2.Y - i * stroke_shift * nx + random.NextUniform(loc_randrange));

                canvas.DrawLine(
                    color,
                    stroke_width_rand ? stroke_width * random.NextUniform(0.75f, 1.25f) : stroke_width,
                    new PointF(px1, py1),
                    new PointF(px2, py2)
                );

                if (color_rand) {
                    color = RandomColor(random);
                }
            }
        }

        public static void DrawParallelArcs(Canvas canvas, Random random,
                                            float stroke_width,
                                            float stroke_shift, float loc_randrange, float curve_randrange,
                                            bool color_rand, bool stroke_width_rand) {

            int lines = RandomLines(random);

            stroke_shift *= random.NextUniform(0.4f, 1.2f) / canvas.Scale;

            PointF pt1 = RandomPointF(random), pt2 = RandomPointF(random);

            double dx = pt1.X - pt2.X, dy = pt1.Y - pt2.Y, norm = Math.Sqrt(dx * dx + dy * dy);
            double nx = dx / norm, ny = dy / norm;

            double curve = random.NextUniform(-0.5f, 0.5f);

            Color color = RandomColor(random);

            for (int i = 0; i < lines; i++) {
                float px1 = (float)(pt1.X + i * stroke_shift * ny + random.NextUniform(loc_randrange));
                float px2 = (float)(pt2.X + i * stroke_shift * ny + random.NextUniform(loc_randrange));
                float py1 = (float)(pt1.Y - i * stroke_shift * nx + random.NextUniform(loc_randrange));
                float py2 = (float)(pt2.Y - i * stroke_shift * nx + random.NextUniform(loc_randrange));

                canvas.DrawArc(
                    color,
                    stroke_width_rand ? stroke_width * random.NextUniform(0.75f, 1.25f) : stroke_width,
                    new PointF(px1, py1),
                    new PointF(px2, py2),
                    (float)curve
                );

                if (color_rand) {
                    color = RandomColor(random);
                }

                curve += random.NextUniform(-curve_randrange, curve_randrange);
            }
        }

        public static void DrawParallelZigzags(Canvas canvas, Random random,
                                               float stroke_width,
                                               float stroke_shift, float loc_randrange,
                                               bool color_rand, bool stroke_width_rand) {

            int lines = RandomLines(random);
            int g = random.Next(3, 12 + 1);

            stroke_shift *= random.NextUniform(0.4f, 1.2f) / canvas.Scale;

            PointF pt1 = RandomPointF(random), pt2 = RandomPointF(random);
            float dx = pt2.X - pt1.X, dy = pt2.Y - pt1.Y;
            float norm = (float)Math.Sqrt(dx * dx + dy * dy);
            float d = norm / g;
            float nx = dx / norm, ny = dy / norm;

            PointF[] pts = new PointF[g];

            for (int i = 0; i < g; i++) {
                PointF v = RandomPointF(random, d / 2);

                pts[i] = new PointF(pt1.X + dx * i / (g - 1) + v.X, pt1.Y + dy * i / (g - 1) + v.Y);
            }

            Color color = RandomColor(random);

            for (int i = 0; i < lines; i++) {
                PointF[] pts_shift = pts.Select(
                    (pt) => new PointF(pt.X + i * stroke_shift * ny + random.NextUniform(loc_randrange),
                                       pt.Y - i * stroke_shift * nx + random.NextUniform(loc_randrange))).ToArray();

                canvas.DrawLines(
                    color,
                    stroke_width_rand ? stroke_width * random.NextUniform(0.75f, 1.25f) : stroke_width,
                    pts_shift
                );

                if (color_rand) {
                    color = RandomColor(random);
                }
            }
        }

        public static void DrawParallelCurves(Canvas canvas, Random random,
                                              float stroke_width,
                                              float stroke_shift, float loc_randrange,
                                              bool color_rand, bool stroke_width_rand) {

            int lines = RandomLines(random);
            int g = random.Next(3, 12 + 1);

            stroke_shift *= random.NextUniform(0.4f, 1.2f) / canvas.Scale;

            PointF pt1 = RandomPointF(random), pt2 = RandomPointF(random);
            float dx = pt2.X - pt1.X, dy = pt2.Y - pt1.Y;
            float norm = (float)Math.Sqrt(dx * dx + dy * dy);
            float d = norm / g;
            float nx = dx / norm, ny = dy / norm;

            PointF[] pts = new PointF[g];

            for (int i = 0; i < g; i++) {
                PointF v = RandomPointF(random, d / 2);

                pts[i] = new PointF(pt1.X + dx * i / (g - 1) + v.X, pt1.Y + dy * i / (g - 1) + v.Y);
            }

            Color color = RandomColor(random);

            for (int i = 0; i < lines; i++) {
                PointF[] pts_shift = pts.Select(
                    (pt) => new PointF(pt.X + i * stroke_shift * ny + random.NextUniform(loc_randrange),
                                       pt.Y - i * stroke_shift * nx + random.NextUniform(loc_randrange))).ToArray();

                canvas.DrawCurve(
                    color,
                    stroke_width_rand ? stroke_width * random.NextUniform(0.75f, 1.25f) : stroke_width,
                    pts_shift
                );

                if (color_rand) {
                    color = RandomColor(random);
                }
            }
        }

        public static void DrawMonotoneLine(Canvas canvas, Random random) {

            int lines = random.Next(2, 5 + 1);

            float stroke_width = random.NextUniform(1.5f, 2.5f);
            float stroke_shift = stroke_width * random.NextUniform(1.75f, 2f) / canvas.Scale;

            PointF pt1 = RandomPointF(random), pt2 = RandomPointF(random);

            double dx = pt1.X - pt2.X, dy = pt1.Y - pt2.Y, norm = Math.Sqrt(dx * dx + dy * dy);
            double nx = dx / norm, ny = dy / norm;

            for (int i = 0; i < lines; i++) {
                float px1 = (float)(pt1.X + i * stroke_shift * ny);
                float px2 = (float)(pt2.X + i * stroke_shift * ny);
                float py1 = (float)(pt1.Y - i * stroke_shift * nx);
                float py2 = (float)(pt2.Y - i * stroke_shift * nx);

                canvas.DrawLine(
                    ((i & 1) == 0) ? RandomColor(random, 64, 128) : RandomColor(random, 192, 255),
                    stroke_width,
                    new PointF(px1, py1),
                    new PointF(px2, py2)
                );
            }
        }

        public static void DrawMonotoneArc(Canvas canvas, Random random) {

            int lines = random.Next(2, 5 + 1);

            float stroke_width = random.NextUniform(1.5f, 2.5f);
            float stroke_shift = stroke_width * random.NextUniform(1.75f, 2f) / canvas.Scale;

            PointF pt1 = RandomPointF(random), pt2 = RandomPointF(random);

            double dx = pt1.X - pt2.X, dy = pt1.Y - pt2.Y, norm = Math.Sqrt(dx * dx + dy * dy);
            double nx = dx / norm, ny = dy / norm;

            double curve = random.NextUniform(-0.5f, 0.5f);

            for (int i = 0; i < lines; i++) {
                float px1 = (float)(pt1.X + i * stroke_shift * ny);
                float px2 = (float)(pt2.X + i * stroke_shift * ny);
                float py1 = (float)(pt1.Y - i * stroke_shift * nx);
                float py2 = (float)(pt2.Y - i * stroke_shift * nx);

                canvas.DrawArc(
                    ((i & 1) == 0) ? RandomColor(random, 64, 128) : RandomColor(random, 192, 255),
                    stroke_width,
                    new PointF(px1, py1),
                    new PointF(px2, py2),
                    (float)curve
                );
            }
        }

        public static void DrawMonotoneZigzag(Canvas canvas, Random random) {

            int lines = random.Next(2, 5 + 1);
            int g = random.Next(3, 12 + 1);

            float stroke_width = random.NextUniform(1.5f, 2.5f);
            float stroke_shift = stroke_width * random.NextUniform(1.75f, 2f) / canvas.Scale;

            PointF pt1 = RandomPointF(random), pt2 = RandomPointF(random);
            float dx = pt2.X - pt1.X, dy = pt2.Y - pt1.Y;
            float norm = (float)Math.Sqrt(dx * dx + dy * dy);
            float d = norm / g;
            float nx = dx / norm, ny = dy / norm;

            PointF[] pts = new PointF[g];

            for (int i = 0; i < g; i++) {
                PointF v = RandomPointF(random, d / 2);

                pts[i] = new PointF(pt1.X + dx * i / (g - 1) + v.X, pt1.Y + dy * i / (g - 1) + v.Y);
            }

            for (int i = 0; i < lines; i++) {
                PointF[] pts_shift = pts.Select(
                    (pt) => new PointF(pt.X + i * stroke_shift * ny,
                                       pt.Y - i * stroke_shift * nx)).ToArray();

                canvas.DrawLines(
                    ((i & 1) == 0) ? RandomColor(random, 64, 128) : RandomColor(random, 192, 255),
                    stroke_width,
                    pts_shift
                );
            }
        }

        public static void DrawMonotoneCurve(Canvas canvas, Random random) {

            int lines = random.Next(2, 5 + 1);
            int g = random.Next(3, 12 + 1);

            float stroke_width = random.NextUniform(1.5f, 2.5f);
            float stroke_shift = stroke_width * random.NextUniform(1.75f, 2f) / canvas.Scale;

            PointF pt1 = RandomPointF(random), pt2 = RandomPointF(random);
            float dx = pt2.X - pt1.X, dy = pt2.Y - pt1.Y;
            float norm = (float)Math.Sqrt(dx * dx + dy * dy);
            float d = norm / g;
            float nx = dx / norm, ny = dy / norm;

            PointF[] pts = new PointF[g];

            for (int i = 0; i < g; i++) {
                PointF v = RandomPointF(random, d / 2);

                pts[i] = new PointF(pt1.X + dx * i / (g - 1) + v.X, pt1.Y + dy * i / (g - 1) + v.Y);
            }

            for (int i = 0; i < lines; i++) {
                PointF[] pts_shift = pts.Select(
                    (pt) => new PointF(pt.X + i * stroke_shift * ny,
                                       pt.Y - i * stroke_shift * nx)).ToArray();

                canvas.DrawCurve(
                    ((i & 1) == 0) ? RandomColor(random, 64, 128) : RandomColor(random, 192, 255),
                    stroke_width,
                    pts_shift
                );
            }
        }

        public static void DrawMonotoneParaLines(Canvas canvas, Random random) {

            int lines = random.Next(2, 5 + 1) * 2;

            float stroke_width = random.NextUniform(1, 1.5f);
            float stroke_shift = stroke_width * 2 / canvas.Scale;
            float stroke_pitch = stroke_shift * random.NextUniform(1.25f, 1.75f);

            float length = random.NextUniform(0.2f, 0.6f);
            double theta = random.Next(4) * Math.PI / 2 + random.NextUniform(0.15f);

            PointF pt1 = RandomPointF(random), pt2 = new PointF(pt1.X + (float)(length * Math.Cos(theta)), pt1.Y + (float)(length * Math.Sin(theta)));

            double dx = pt1.X - pt2.X, dy = pt1.Y - pt2.Y, norm = Math.Sqrt(dx * dx + dy * dy);
            double nx = dx / norm, ny = dy / norm;

            for (int i = 0; i < lines; i++) {
                float px1 = (float)(pt1.X + (i * stroke_shift + (i / 2) * stroke_pitch) * ny);
                float px2 = (float)(pt2.X + (i * stroke_shift + (i / 2) * stroke_pitch) * ny);
                float py1 = (float)(pt1.Y - (i * stroke_shift + (i / 2) * stroke_pitch) * nx);
                float py2 = (float)(pt2.Y - (i * stroke_shift + (i / 2) * stroke_pitch) * nx);

                canvas.DrawLine(
                    ((i & 1) == 0) ? RandomColor(random, 64, 128) : RandomColor(random, 192, 255),
                    stroke_width,
                    new PointF(px1, py1),
                    new PointF(px2, py2)
                );
            }
        }

        public static void DrawCrescent(Canvas canvas, Random random,
                                        float stroke_width,
                                        bool stroke_width_rand) {

            PointF pt1 = RandomPointF(random), pt2 = RandomPointF(random);

            float curve1 = random.NextUniform(0.5f);
            float curve2 = curve1 * random.NextUniform(-0.8f, -0.2f);

            if (random.NextDouble() < 0.5) {
                canvas.FillCrescent(RandomColor(random), RandomColor(random), pt1, pt2, curve1, curve2);
            }
            else {
                canvas.FillCrescent(RandomColor(random), pt1, pt2, curve1, curve2);
            }

            if (random.NextDouble() < 0.5) {
                using GraphicsPath path = new GraphicsPath();

                path.AddCrescent(pt1, pt2, curve1, curve2);

                PointF d = RandomPointF(random, Distance(pt1, pt2) * 0.1f);

                PointF pt3 = new PointF(pt1.X + d.X, pt1.Y + d.Y), pt4 = new PointF(pt2.X + d.X, pt2.Y + d.Y);

                using Region clip = canvas.Clip;

                canvas.SetClip(path, CombineMode.Intersect);

                if (random.NextDouble() < 0.5) {
                    canvas.FillCrescent(RandomColor(random), RandomColor(random), pt3, pt4, curve1, curve2);
                }
                else {
                    canvas.FillCrescent(RandomColor(random), pt3, pt4, curve1, curve2);
                }

                canvas.Clip = clip;
            }

            if (random.NextDouble() < 0.8) {
                canvas.DrawCrescent(
                    RandomColor(random),
                    stroke_width_rand ? stroke_width * random.NextUniform(0.75f, 1.25f) : stroke_width,
                    pt1, pt2, curve1, curve2
                );
            }
        }

        public static void DrawPolygon(Canvas canvas, Random random,
                                        float stroke_width,
                                        bool stroke_width_rand) {

            PointF ptc = RandomPointF(random);
            float r = random.NextUniform(0.05f, 0.5f);
            float arg = random.NextUniform((float)Math.PI);
            int g = random.Next(3, 12 + 1);

            PointF[] pts = new PointF[g];

            for (int i = 0; i < g; i++) {
                float radius = (float)(r * Math.Sqrt(random.NextDouble()));
                float theta = arg + 2 * (float)Math.PI * (float)i / (float)g;

                pts[i] = new PointF(ptc.X + radius * (float)Math.Cos(theta), ptc.Y + radius * (float)Math.Sin(theta));
            }

            if (random.NextDouble() < 0.5 || !IsSafeLinearGradientBrush(pts[0], pts[g / 2])) {
                canvas.FillPolygon(RandomColor(random), pts);
            }
            else if (random.NextDouble() < 0.5) {
                using LinearGradientBrush brush = new LinearGradientBrush(pts[0], pts[g / 2], RandomColor(random), RandomColor(random)) {
                    WrapMode = WrapMode.TileFlipXY
                };

                canvas.FillPolygon(brush, pts);
            }
            else {
                PointF d = RandomPointF(random, r);

                using GraphicsPath path = new GraphicsPath();
                path.AddPolygon(pts);

                try {
                    using PathGradientBrush brush = new PathGradientBrush(path) {
                        CenterColor = RandomColor(random),
                        SurroundColors = new Color[] { RandomColor(random) },
                        CenterPoint = new PointF(ptc.X + d.X, ptc.Y + d.Y)
                    };

                    canvas.FillPolygon(brush, pts);
                }
                catch (OutOfMemoryException) { }
            }

            if (random.NextDouble() < 0.5) {
                using GraphicsPath path = new GraphicsPath();

                path.AddPolygon(pts);

                PointF d = RandomPointF(random, r * 0.2f);

                PointF[] pts_shift = pts.Select((pt) => new PointF(pt.X + d.X, pt.Y + d.Y)).ToArray();

                using Region clip = canvas.Clip;

                canvas.SetClip(path, CombineMode.Intersect);

                if (random.NextDouble() < 0.5 || !IsSafeLinearGradientBrush(pts_shift[0], pts_shift[g / 2])) {
                    canvas.FillPolygon(RandomColor(random), pts_shift);
                }
                else if (random.NextDouble() < 0.5) {
                    using LinearGradientBrush brush = new LinearGradientBrush(pts_shift[0], pts_shift[g / 2], RandomColor(random), RandomColor(random)) {
                        WrapMode = WrapMode.TileFlipXY
                    };

                    canvas.FillPolygon(brush, pts_shift);
                }
                else {
                    PointF d_shift = RandomPointF(random, r * 0.5f);

                    using GraphicsPath path_shift = new GraphicsPath();
                    path_shift.AddPolygon(pts_shift);

                    try {
                        using PathGradientBrush brush = new PathGradientBrush(path_shift) {
                            CenterColor = RandomColor(random),
                            SurroundColors = new Color[] { RandomColor(random) },
                            CenterPoint = new PointF(ptc.X + d.X + d_shift.X, ptc.Y + d.Y + d_shift.Y)
                        };

                        canvas.FillPolygon(brush, pts_shift);
                    }
                    catch (OutOfMemoryException) { }
                }

                canvas.Clip = clip;
            }

            if (random.NextDouble() < 0.8) {
                canvas.DrawPolygon(
                    RandomColor(random),
                    stroke_width_rand ? stroke_width * random.NextUniform(0.75f, 1.25f) : stroke_width,
                    pts
                );
            }
        }

        public static void DrawCurvedPolygon(Canvas canvas, Random random,
                                             float stroke_width,
                                             bool stroke_width_rand) {

            PointF ptc = RandomPointF(random);
            float r = random.NextUniform(0.05f, 0.5f);
            float arg = random.NextUniform((float)Math.PI);
            int g = random.Next(3, 12 + 1);

            PointF[] pts = new PointF[g];
            float[] curves = new float[g];

            for (int i = 0; i < g; i++) {
                float radius = (float)(r * Math.Sqrt(random.NextDouble()));
                float theta = arg + 2 * (float)Math.PI * (float)i / (float)g;

                pts[i] = new PointF(ptc.X + radius * (float)Math.Cos(theta), ptc.Y + radius * (float)Math.Sin(theta));
                curves[i] = random.NextUniform(0.25f);
            }

            using GraphicsPath path = new GraphicsPath();
            for (int i = 0; i < g; i++) {
                path.AddArc(pts[i], pts[(i + 1) % g], curves[i]);
            }
            path.CloseFigure();

            if (random.NextDouble() < 0.5 || !IsSafeLinearGradientBrush(pts[0], pts[g / 2])) {
                canvas.FillPath(RandomColor(random), path);
            }
            else {
                using LinearGradientBrush brush = new LinearGradientBrush(pts[0], pts[g / 2], RandomColor(random), RandomColor(random)) {
                    WrapMode = WrapMode.TileFlipXY
                };

                canvas.FillPath(brush, path);
            }

            if (random.NextDouble() < 0.5) {
                PointF d = RandomPointF(random, r * 0.2f);

                PointF[] pts_shift = pts.Select((pt) => new PointF(pt.X + d.X, pt.Y + d.Y)).ToArray();

                using GraphicsPath path_shift = new GraphicsPath();
                for (int i = 0; i < g; i++) {
                    path_shift.AddArc(pts_shift[i], pts_shift[(i + 1) % g], curves[i]);
                }
                path_shift.CloseFigure();

                using Region clip = canvas.Clip;

                canvas.SetClip(path, CombineMode.Intersect);

                if (random.NextDouble() < 0.5 || !IsSafeLinearGradientBrush(pts_shift[0], pts_shift[g / 2])) {
                    canvas.FillPath(RandomColor(random), path_shift);
                }
                else {
                    using LinearGradientBrush brush = new LinearGradientBrush(pts_shift[0], pts_shift[g / 2], RandomColor(random), RandomColor(random)) {
                        WrapMode = WrapMode.TileFlipXY
                    };

                    canvas.FillPath(brush, path_shift);
                }

                canvas.Clip = clip;
            }

            if (random.NextDouble() < 0.8) {
                canvas.DrawPath(
                    RandomColor(random),
                    stroke_width_rand ? stroke_width * random.NextUniform(0.75f, 1.25f) : stroke_width,
                    path
                );
            }
        }

        public static void DrawSpear(Canvas canvas, Random random) {

            PointF ptc = RandomPointF(random);
            float r = random.NextUniform(0.05f, 0.5f);
            float arg = random.NextUniform((float)Math.PI);
            float darg = random.NextUniform(0.1f, 0.2f);
            const int g = 3;

            PointF[] pts = new PointF[3];
            float[] curves = new float[3];

            pts[0] = new PointF(ptc.X + r * (float)Math.Cos(arg), ptc.Y + r * (float)Math.Sin(arg));
            curves[0] = -random.NextUniform(0, 0.025f);

            pts[1] = new PointF(ptc.X - r * (float)Math.Cos(arg + darg), ptc.Y - r * (float)Math.Sin(arg + darg));
            curves[1] = 0;

            pts[2] = new PointF(ptc.X - r * (float)Math.Cos(arg - darg), ptc.Y - r * (float)Math.Sin(arg - darg));
            curves[2] = -random.NextUniform(0, 0.025f);

            using GraphicsPath path = new GraphicsPath();
            for (int i = 0; i < g; i++) {
                path.AddArc(pts[i], pts[(i + 1) % g], curves[i]);
            }
            path.CloseFigure();

            if (random.NextDouble() < 0.5 || !IsSafeLinearGradientBrush(pts[0], pts[g / 2])) {
                canvas.FillPath(RandomColor(random), path);
            }
            else {
                using LinearGradientBrush brush = new LinearGradientBrush(pts[0], pts[g / 2], RandomColor(random), RandomColor(random)) {
                    WrapMode = WrapMode.TileFlipXY
                };

                canvas.FillPath(brush, path);
            }
        }

        public static void DrawEclipse(Canvas canvas, Random random,
                                       float stroke_width,
                                       bool stroke_width_rand) {

            PointF ptc = RandomPointF(random);
            float r = random.NextUniform(0.1f, 0.2f);
            float rx = r * random.NextUniform(0.05f, 1f);
            float ry = r * random.NextUniform(0.05f, 1f);
            float arg = random.NextUniform(180);

            if (random.NextDouble() < 0.5) {
                canvas.FillEllipse(RandomColor(random), ptc, rx, ry, arg);
            }
            else if (random.NextDouble() < 0.5) {
                float theta = random.NextUniform((float)Math.PI);

                using LinearGradientBrush brush = new LinearGradientBrush(
                    new PointF(ptc.X + r * (float)Math.Cos(theta), ptc.Y + r * (float)Math.Sin(theta)),
                    new PointF(ptc.X - r * (float)Math.Cos(theta), ptc.Y - r * (float)Math.Sin(theta)),
                    RandomColor(random), RandomColor(random)
                );

                canvas.FillEllipse(brush, ptc, rx, ry, arg);
            }
            else {
                PointF d = RandomPointF(random, Math.Min(rx, ry) * 0.5f);

                using GraphicsPath path = new GraphicsPath();
                path.AddEllipse(ptc, rx, ry, 0);

                try {
                    using PathGradientBrush brush = new PathGradientBrush(path) {
                        CenterColor = RandomColor(random),
                        SurroundColors = new Color[] { RandomColor(random) },
                        CenterPoint = new PointF(ptc.X + d.X, ptc.Y + d.Y)
                    };

                    canvas.FillEllipse(brush, ptc, rx, ry, arg);
                }
                catch (OutOfMemoryException) { }
            }

            if (random.NextDouble() < 0.5) {
                using GraphicsPath path = new GraphicsPath();

                path.AddEllipse(ptc, rx, ry, arg);

                PointF d = RandomPointF(random, r * 0.2f);

                PointF ptc_shift = new PointF(ptc.X + d.X, ptc.Y + d.Y);

                using Region clip = canvas.Clip;

                canvas.SetClip(path, CombineMode.Intersect);

                if (random.NextDouble() < 0.5) {
                    canvas.FillEllipse(RandomColor(random), ptc_shift, rx, ry, arg);
                }
                else if (random.NextDouble() < 0.5) {
                    float theta = random.NextUniform((float)Math.PI);

                    using LinearGradientBrush brush = new LinearGradientBrush(
                        new PointF(ptc_shift.X + r * (float)Math.Cos(theta), ptc_shift.Y + r * (float)Math.Sin(theta)),
                        new PointF(ptc_shift.X - r * (float)Math.Cos(theta), ptc_shift.Y - r * (float)Math.Sin(theta)),
                        RandomColor(random), RandomColor(random)
                    );

                    canvas.FillEllipse(brush, ptc_shift, rx, ry, arg);
                }
                else {
                    PointF d_shift = RandomPointF(random, Math.Min(rx, ry) * 0.5f);

                    using GraphicsPath path_shift = new GraphicsPath();
                    path.AddEllipse(ptc_shift, rx, ry, 0);

                    try {
                        using PathGradientBrush brush = new PathGradientBrush(path_shift) {
                            CenterColor = RandomColor(random),
                            SurroundColors = new Color[] { RandomColor(random) },
                            CenterPoint = new PointF(ptc_shift.X + d_shift.X, ptc_shift.Y + d_shift.Y)
                        };

                        canvas.FillEllipse(brush, ptc_shift, rx, ry, arg);
                    }
                    catch (OutOfMemoryException) { }
                }

                canvas.Clip = clip;
            }

            if (random.NextDouble() < 0.8) {
                canvas.DrawEllipse(
                    RandomColor(random),
                    stroke_width_rand ? stroke_width * random.NextUniform(0.75f, 1.25f) : stroke_width,
                    ptc, rx, ry, arg
                );
            }
        }

        public static void DrawBranch(Canvas canvas, Random random,
                                      float stroke_width,
                                      bool stroke_width_rand) {

            int g = random.Next(3, 12 + 1);

            PointF pt1 = RandomPointF(random), pt2 = RandomPointF(random);
            float dx = pt2.X - pt1.X, dy = pt2.Y - pt1.Y;
            float d = (float)Math.Sqrt(dx * dx + dy * dy) / g;

            PointF[] pts = new PointF[g], pts2 = new PointF[g];

            for (int i = 0; i < g; i++) {
                PointF v = RandomPointF(random, d / 2), v2 = RandomPointF(random, d);

                pts[i] = new PointF(pt1.X + dx * i / (g - 1) + v.X, pt1.Y + dy * i / (g - 1) + v.Y);
                pts2[i] = new PointF(pts[i].X + v2.X, pts[i].Y + v2.Y);
            }

            using GraphicsPath path = new GraphicsPath();
            path.AddLines(pts);
            for (int i = 0; i < g; i++) {
                path.StartFigure();
                path.AddLine(pts[i], pts2[i]);
            }

            Color color = RandomColor(random);
            stroke_width = stroke_width_rand ? stroke_width * random.NextUniform(0.75f, 1.25f) : stroke_width;
            canvas.DrawPath(color, stroke_width, path);
        }

        public static void DrawCurvedBranch(Canvas canvas, Random random,
                                            float stroke_width,
                                            bool stroke_width_rand) {

            int g = random.Next(3, 12 + 1);

            PointF pt1 = RandomPointF(random), pt2 = RandomPointF(random);
            float dx = pt2.X - pt1.X, dy = pt2.Y - pt1.Y;
            float d = (float)Math.Sqrt(dx * dx + dy * dy) / g;

            PointF[] pts = new PointF[g], pts2 = new PointF[g];
            float[] curves = new float[g], curves2 = new float[g];

            for (int i = 0; i < g; i++) {
                PointF v = RandomPointF(random, d / 2), v2 = RandomPointF(random, d);

                pts[i] = new PointF(pt1.X + dx * i / (g - 1) + v.X, pt1.Y + dy * i / (g - 1) + v.Y);
                pts2[i] = new PointF(pts[i].X + v2.X, pts[i].Y + v2.Y);
                curves[i] = random.NextUniform(0.25f);
                curves2[i] = random.NextUniform(0.25f);
            }

            using GraphicsPath path = new GraphicsPath();
            for (int i = 0; i < g - 1; i++) {
                path.AddArc(pts[i], pts[i + 1], curves[i]);
            }
            for (int i = 0; i < g; i++) {
                path.StartFigure();
                path.AddArc(pts[i], pts2[i], curves2[i]);
            }

            Color color = RandomColor(random);
            stroke_width = stroke_width_rand ? stroke_width * random.NextUniform(0.75f, 1.25f) : stroke_width;
            canvas.DrawPath(color, stroke_width, path);
        }

        public static void DrawSnake(Canvas canvas, Random random,
                                     float stroke_shift) {

            int g = random.Next(12, 24 + 1);

            stroke_shift /= canvas.Scale;

            PointF pt1 = RandomPointF(random), pt2 = RandomPointF(random);
            float dx = pt2.X - pt1.X, dy = pt2.Y - pt1.Y;
            float d = (float)Math.Sqrt(dx * dx + dy * dy);
            float nx = dx / d, ny = dy / d;

            PointF[] pts = new PointF[g], pts2 = new PointF[g];

            for (int i = 0; i < g; i++) {
                PointF v = RandomPointF(random, d / g / 3), v2 = RandomPointF(random, d / g / 12);

                pts[i] = new PointF(pt1.X + dx * i / (g - 1) + v.X, pt1.Y + dy * i / (g - 1) + v.Y);
                pts2[i] = new PointF(pts[i].X + ny * stroke_shift + v2.X, pts[i].Y - nx * stroke_shift + v2.Y);
            }

            using GraphicsPath path = new GraphicsPath();
            path.AddCurve(pts);
            path.AddCurve(pts2.Reverse().ToArray());
            path.CloseFigure();

            Color color = RandomColor(random);

            if (random.NextDouble() < 0.75 || !IsSafeLinearGradientBrush(pt1, pt2)) {
                canvas.FillPath(color, path);
            }
            else {
                using LinearGradientBrush brush = new LinearGradientBrush(pt1, pt2, RandomColor(random), RandomColor(random)) {
                    WrapMode = WrapMode.TileFlipXY
                };

                canvas.FillPath(brush, path);
            }
        }

        public static void DrawRadiation(Canvas canvas, Random random,
                                         float stroke_width,
                                         bool stroke_width_rand) {

            int g = random.Next(3, 24 + 1);

            PointF ptc = RandomPointF(random);

            PointF[] pts = new PointF[g];
            float[] curves = new float[g];

            for (int i = 0; i < g; i++) {
                double theta = 2 * Math.PI * i / g;

                pts[i] = new PointF(ptc.X + 1.5f * (float)Math.Cos(theta), ptc.Y + 1.5f * (float)Math.Sin(theta));
                curves[i] = random.NextUniform(0.05f);
            }

            for (int i = 0; i < g; i++) {
                if (random.NextDouble() < 0.25) {
                    using GraphicsPath path = new GraphicsPath();

                    path.AddArc(ptc, pts[i], curves[i]);
                    path.AddLine(pts[i], pts[(i + 1) % g]);
                    path.AddArc(pts[(i + 1) % g], ptc, -curves[(i + 1) % g]);
                    path.CloseFigure();

                    if (random.NextDouble() < 0.5 || !IsSafeLinearGradientBrush(ptc, pts[i])) {
                        Color color = RandomColor(random);

                        canvas.DrawPath(color, 0.5f, path);
                        canvas.FillPath(color, path);
                    }
                    else {
                        Color color1 = RandomColor(random);
                        Color color2 = RandomColor(random);

                        using LinearGradientBrush brush = new LinearGradientBrush(
                            ptc, pts[i], color1, color2
                        );

                        canvas.DrawPath(CanvasUtil.AverageColor(new Color[] { color1, color2 }), 0.5f, path);
                        canvas.FillPath(brush, path);
                    }
                }
            }

            using (GraphicsPath path = new GraphicsPath()) {
                for (int i = 0; i < g; i++) {

                    if (random.NextDouble() < 0.5) {
                        path.StartFigure();
                        path.AddArc(
                            ptc, pts[i], curves[i]
                        );
                    }
                }

                canvas.DrawPath(
                    RandomColor(random),
                    stroke_width_rand ? stroke_width * random.NextUniform(0.75f, 1.25f) : stroke_width,
                    path
                );
            }
        }

        public static void DrawRectangle(Canvas canvas, Random random) {
            PointF ptc = RandomPointF(random);
            float w = random.NextUniform(0.1f, 0.2f);
            float h = random.NextUniform(0.1f, 0.2f);

            canvas.FillRectangle(RandomColor(random), new RectangleF(ptc.X - w / 2, ptc.Y - h / 2, w, h));
        }

        public static void DrawConcentric(Canvas canvas, Random random,
                                          float stroke_width,
                                          float stroke_shift,
                                          bool stroke_width_rand) {

            int lines = 1 + RandomLines(random, max: 3, add_line_prob: 0.25);

            stroke_shift *= random.NextUniform(0.8f, 1.2f) / canvas.Scale;

            PointF ptc = RandomPointF(random);
            float radius_x = random.NextUniform(0.2f, 0.4f);
            float radius_y = random.NextUniform(0.2f, 0.4f);
            float angle = random.NextUniform(0, 360);
            float start_angle = random.NextUniform(0, 360);
            float sweep_angle = random.NextUniform(30, 60);

            Color color = RandomColor(random, 128, 255);

            for (int i = 0; i < lines; i++) {
                canvas.DrawArc(
                    color,
                    stroke_width_rand ? stroke_width * random.NextUniform(0.75f, 1.25f) : stroke_width,
                    ptc, radius_x, radius_y, angle, start_angle, sweep_angle
                );
                radius_x -= stroke_shift;
                radius_y -= stroke_shift;

                start_angle += random.NextUniform(20);
                if (start_angle < 0) start_angle += 360;
                if (start_angle >= 360) start_angle -= 360;
            }
        }

        public static void DrawSlideLines(Canvas canvas, Random random,
                                          float stroke_width,
                                          float stroke_shift,
                                          bool stroke_width_rand) {

            int lines = 1 + RandomLines(random, max: 3, add_line_prob: 0.25);

            stroke_shift *= random.NextUniform(0.8f, 1.2f) / canvas.Scale;

            PointF pt1 = RandomPointF(random), pt2 = RandomPointF(random);

            double dx = pt1.X - pt2.X, dy = pt1.Y - pt2.Y, norm = Math.Sqrt(dx * dx + dy * dy);
            double nx = dx / norm, ny = dy / norm;

            Color color = RandomColor(random);

            for (int i = 0; i < lines; i++) {
                double s = random.NextUniform(0.2f);
                double sx = s * dx;
                double sy = s * dy;

                float px1 = (float)(pt1.X + i * stroke_shift * ny + sx);
                float px2 = (float)(pt2.X + i * stroke_shift * ny + sx);
                float py1 = (float)(pt1.Y - i * stroke_shift * nx + sy);
                float py2 = (float)(pt2.Y - i * stroke_shift * nx + sy);

                canvas.DrawLine(
                    color,
                    stroke_width_rand ? stroke_width * random.NextUniform(0.75f, 1.25f) : stroke_width,
                    new PointF(px1, py1),
                    new PointF(px2, py2)
                );
            }
        }

        public static void DrawMultiArcs(Canvas canvas, Random random) {

            PointF pt1 = RandomPointF(random), pt2 = RandomPointF(random);
            float d = Distance(pt1, pt2) * 0.2f;

            float curve1 = random.NextUniform(0.05f, 0.4f);
            float curve2 = -curve1 - random.NextUniform(0.01f, 0.02f);

            Color color = RandomColor(random);

            for (int i = random.Next(2) + 3; i > 0; i--) {
                PointF s = RandomPointF(random, d);

                canvas.FillCrescent(color, new PointF(pt1.X + s.X, pt1.Y + s.Y), new PointF(pt2.X + s.X, pt2.Y + s.Y), curve1, curve2);
            }
        }

        public static void DrawMesh(Canvas canvas, Random random,
                                    float stroke_width,
                                    float stroke_shift,
                                    bool stroke_width_rand) {

            int lines = RandomLines(random) + RandomLines(random);

            float stroke_shift1 = stroke_shift * random.NextUniform(0.8f, 1.2f) / canvas.Scale;
            float stroke_shift2 = stroke_shift * random.NextUniform(0.8f, 1.2f) / canvas.Scale;

            PointF ptc = new PointF(random.NextUniform(0.3f, 0.7f), random.NextUniform(0.3f, 0.7f));
            float theta1 = random.NextUniform(0, (float)(2 * Math.PI));
            float theta2 = theta1 + random.NextUniform((float)(0.3 * Math.PI), (float)(0.7 * Math.PI));
            float d = 0.40f;

            double dx1 = d * Math.Cos(theta1), dy1 = d * Math.Sin(theta1);
            double dx2 = d * Math.Cos(theta2), dy2 = d * Math.Sin(theta2);

            double nx1 = Math.Cos(theta1 + Math.PI / 2), ny1 = Math.Sin(theta1 + Math.PI / 2);
            double nx2 = Math.Cos(theta2 + Math.PI / 2), ny2 = Math.Sin(theta2 + Math.PI / 2);

            Color color = RandomColor(random, 128, 255);

            (PointF pt1, PointF pt2) line(PointF ptc, double index, double stroke_shift, double dx, double dy, double nx, double ny) { 
                float px1 = (float)(ptc.X + dx + index * stroke_shift * nx);
                float px2 = (float)(ptc.X - dx + index * stroke_shift * nx);
                float py1 = (float)(ptc.Y + dy + index * stroke_shift * ny);
                float py2 = (float)(ptc.Y - dy + index * stroke_shift * ny);

                return (new PointF(px1, py1), new PointF(px2, py2));
            }

            PointF ptqa = Intersection(
                line(ptc, -lines / 2, stroke_shift1, dx1, dy1, nx1, ny1),
                line(ptc, -lines / 2, stroke_shift2, dx2, dy2, nx2, ny2));

            PointF ptqb = Intersection(
                line(ptc, lines - lines / 2 - 1, stroke_shift1, dx1, dy1, nx1, ny1),
                line(ptc, -lines / 2, stroke_shift2, dx2, dy2, nx2, ny2));

            PointF ptqc = Intersection(
                line(ptc, lines - lines / 2 - 1, stroke_shift1, dx1, dy1, nx1, ny1),
                line(ptc, lines - lines / 2 - 1, stroke_shift2, dx2, dy2, nx2, ny2));

            PointF ptqd = Intersection(
                line(ptc, -lines / 2, stroke_shift1, dx1, dy1, nx1, ny1),
                line(ptc, lines - lines / 2 - 1, stroke_shift2, dx2, dy2, nx2, ny2));

            using GraphicsPath path = new GraphicsPath();
            path.AddLine(ptqa, ptqb);
            path.AddLine(ptqb, ptqc);
            path.AddLine(ptqc, ptqd);

            canvas.SetClip(path, CombineMode.Intersect);

            for (int i = 0, j = -lines / 2; i < lines; i++, j++) {
                (PointF pt1, PointF pt2) = line(ptc, j, stroke_shift1, dx1, dy1, nx1, ny1);

                canvas.DrawLine(
                    color,
                    stroke_width_rand ? stroke_width * random.NextUniform(0.75f, 1.25f) : stroke_width,
                    pt1, pt2
                );
            }

            for (int i = 0, j = -lines / 2; i < lines; i++, j++) {
                (PointF pt1, PointF pt2) = line(ptc, j, stroke_shift2, dx2, dy2, nx2, ny2);

                canvas.DrawLine(
                    color,
                    stroke_width_rand ? stroke_width * random.NextUniform(0.75f, 1.25f) : stroke_width,
                    pt1, pt2
                );
            }

            canvas.ResetClip();

            canvas.DrawPolygon(
                color,
                stroke_width_rand ? stroke_width * random.NextUniform(0.75f, 1.25f) : stroke_width,
                ptqa, ptqb, ptqc, ptqd
            );
        }

        private static float NextUniform(this Random random, float randrange) {
            return (float)(randrange * (random.NextDouble() * 2 - 1));
        }

        private static float NextUniform(this Random random, float min, float max) {
            return (float)(random.NextDouble() * (max - min) + min);
        }

        private static Color RandomColor(Random random) {
            int c = random.Next(256);
            int a = (random.NextDouble() < 0.9) ? 255 : random.Next(256);

            return Color.FromArgb(a, c, c, c);
        }

        private static Color RandomColor(Random random, int cmin, int cmax) {
            int c = random.Next(cmin, cmax + 1);

            return Color.FromArgb(255, c, c, c);
        }

        private static PointF RandomPointF(Random random) {
            return new PointF(NextUniform(random, -0.1f, 1.1f), NextUniform(random, -0.1f, 1.1f));
        }

        private static PointF RandomPointF(Random random, float radius) {
            double theta = random.NextUniform((float)Math.PI);
            double r = radius * Math.Sqrt(random.NextDouble());

            return new PointF((float)(r * Math.Cos(theta)), (float)(r * Math.Sin(theta)));
        }

        private static int RandomLines(Random random, int max = 8, double add_line_prob = 0.75) {
            for (int i = 1; i < max; i++) {
                if (random.NextDouble() > add_line_prob) {
                    return i;
                }
            }

            return max;
        }

        private static float Distance(PointF pt1, PointF pt2) {
            double dx = pt1.X - pt2.X, dy = pt1.Y - pt2.Y;

            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        private static PointF Intersection((PointF pt1, PointF pt2) line1, (PointF pt1, PointF pt2) line2){
            PointF dv1 = new PointF(line1.pt2.X - line1.pt1.X, line1.pt2.Y - line1.pt1.Y);
            PointF dv2 = new PointF(line2.pt2.X - line2.pt1.X, line2.pt2.Y - line2.pt1.Y);

            float vv1 = dv1.X * line1.pt1.Y - dv1.Y * line1.pt1.X;
            float vv2 = dv2.X * line2.pt1.Y - dv2.Y * line2.pt1.X;
            float vv12 = dv1.X * dv2.Y - dv1.Y * dv2.X;

            return new PointF((vv1 * dv2.X - vv2 * dv1.X) / vv12, (vv1 * dv2.Y - vv2 * dv1.Y) / vv12);
        }

        private static bool IsSafeLinearGradientBrush(PointF pt1, PointF pt2, float eps = 1e-2f) {
            return (Math.Abs(pt1.X - pt2.X) > eps) && (Math.Abs(pt1.Y - pt2.Y) > eps);
        }
    }
}
