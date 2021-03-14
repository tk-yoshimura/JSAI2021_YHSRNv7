using System;
using System.Drawing;

namespace InfinityPatterns {
    public static class ComplexPattern {
        public static Canvas Generate(Random random, int size, int complexity) {
            Canvas canvas = new Canvas(size);

            canvas.Clear(Color.Black);

            Pattern.DrawRadiation(canvas, random, stroke_width: RandomStrokeWidth(random), stroke_width_rand: true);

            bool hasdrew_mesh = false;

            while (complexity > 0) {
                float stroke_width = RandomStrokeWidth(random);
                float stroke_shift = stroke_width * 5;

                int sel = random.Next(16);

                if (sel < 4) {
                    int sel2 = random.Next(11);

                    if (sel2 < 3) {
                        Pattern.DrawCrescent(canvas, random, stroke_width, stroke_width_rand: true);
                    }
                    if (sel2 < 5) {
                        Pattern.DrawEclipse(canvas, random, stroke_width, stroke_width_rand: true);
                    }
                    else if (sel2 < 7) {
                        Pattern.DrawPolygon(canvas, random, stroke_width, stroke_width_rand: true);
                    }
                    else if (sel2 < 9){
                        Pattern.DrawCurvedPolygon(canvas, random, stroke_width, stroke_width_rand: true);
                    }
                    else {
                        Pattern.DrawSpear(canvas, random);
                    }
                }
                else if (sel < 8) {
                    int sel2 = random.Next(11);

                    if (sel2 < 4) {
                        int sel3 = random.Next(10);

                        if (sel3 < 4) {
                            Pattern.DrawParallelLines(
                                canvas, random,
                                stroke_width, stroke_shift, loc_randrange: 0.01f,
                                color_rand: false, stroke_width_rand: false
                            );
                        }
                        else if (sel3 < 8) {
                            Pattern.DrawParallelLines(
                                canvas, random,
                                stroke_width, stroke_shift, loc_randrange: 0.01f,
                                color_rand: false, stroke_width_rand: true
                            );
                        }
                        else if (sel3 < 9) {
                            Pattern.DrawParallelLines(
                                canvas, random,
                                stroke_width, stroke_shift, loc_randrange: 0.01f,
                                color_rand: true, stroke_width_rand: false
                            );
                        }
                        else {
                            Pattern.DrawParallelLines(
                                canvas, random,
                                stroke_width, stroke_shift, loc_randrange: 0.01f,
                                color_rand: true, stroke_width_rand: true
                            );
                        }
                    }
                    else if (sel2 < 8) {
                        int sel3 = random.Next(10);

                        if (sel3 < 4) {
                            Pattern.DrawParallelArcs(
                                canvas, random,
                                stroke_width, stroke_shift, loc_randrange: 0.01f, curve_randrange: 0.01f,
                                color_rand: false, stroke_width_rand: false
                            );
                        }
                        else if (sel3 < 8) {
                            Pattern.DrawParallelArcs(
                                canvas, random,
                                stroke_width, stroke_shift, loc_randrange: 0.01f, curve_randrange: 0.01f,
                                color_rand: false, stroke_width_rand: true
                            );
                        }
                        else if (sel3 < 9) {
                            Pattern.DrawParallelArcs(
                                canvas, random,
                                stroke_width, stroke_shift, loc_randrange: 0.01f, curve_randrange: 0.01f,
                                color_rand: true, stroke_width_rand: false
                            );
                        }
                        else {
                            Pattern.DrawParallelArcs(
                                canvas, random,
                                stroke_width, stroke_shift, loc_randrange: 0.01f, curve_randrange: 0.01f,
                                color_rand: true, stroke_width_rand: true
                            );
                        }
                    }
                    else if (sel2 < 10) {
                        int sel3 = random.Next(10);

                        if (sel3 < 4) {
                            Pattern.DrawParallelCurves(
                                canvas, random,
                                stroke_width, stroke_shift, loc_randrange: 0.01f,
                                color_rand: false, stroke_width_rand: false
                            );
                        }
                        else if (sel3 < 8) {
                            Pattern.DrawParallelCurves(
                                canvas, random,
                                stroke_width, stroke_shift, loc_randrange: 0.01f,
                                color_rand: false, stroke_width_rand: true
                            );
                        }
                        else if (sel3 < 9) {
                            Pattern.DrawParallelCurves(
                                canvas, random,
                                stroke_width, stroke_shift, loc_randrange: 0.01f,
                                color_rand: true, stroke_width_rand: false
                            );
                        }
                        else {
                            Pattern.DrawParallelCurves(
                                canvas, random,
                                stroke_width, stroke_shift, loc_randrange: 0.01f,
                                color_rand: true, stroke_width_rand: true
                            );
                        }
                    }
                    else {
                        int sel3 = random.Next(10);

                        if (sel3 < 4) {
                            Pattern.DrawParallelZigzags(
                                canvas, random,
                                stroke_width, stroke_shift, loc_randrange: 0.01f,
                                color_rand: false, stroke_width_rand: false
                            );
                        }
                        else if (sel3 < 8) {
                            Pattern.DrawParallelZigzags(
                                canvas, random,
                                stroke_width, stroke_shift, loc_randrange: 0.01f,
                                color_rand: false, stroke_width_rand: true
                            );
                        }
                        else if (sel3 < 9) {
                            Pattern.DrawParallelZigzags(
                                canvas, random,
                                stroke_width, stroke_shift, loc_randrange: 0.01f,
                                color_rand: true, stroke_width_rand: false
                            );
                        }
                        else {
                            Pattern.DrawParallelZigzags(
                                canvas, random,
                                stroke_width, stroke_shift, loc_randrange: 0.01f,
                                color_rand: true, stroke_width_rand: true
                            );
                        }
                    }
                }
                else if (sel < 10) {
                    int sel2 = random.Next(3);

                    if (sel2 < 1) {
                        Pattern.DrawSnake(canvas, random, stroke_shift / 2);
                    }
                    else if (sel2 < 2) {
                        Pattern.DrawBranch(canvas, random, stroke_width, stroke_width_rand: true);
                    }
                    else if (sel2 < 3) {
                        Pattern.DrawCurvedBranch(canvas, random, stroke_width, stroke_width_rand: true);
                    }
                }
                else if (sel < 11) {
                    int sel2 = random.Next(12);

                    if (sel2 < 4) {
                        Pattern.DrawMonotoneLine(canvas, random);
                    }
                    else if (sel2 < 6) {
                        Pattern.DrawMonotoneArc(canvas, random);
                    }
                    else if (sel2 < 7) {
                        Pattern.DrawMonotoneCurve(canvas, random);
                    }
                    else if (sel2 < 8) {
                        Pattern.DrawMonotoneZigzag(canvas, random);
                    }
                    else {
                        Pattern.DrawMonotoneParaLines(canvas, random);
                    }
                }
                else if (sel < 12) {
                    Pattern.DrawRectangle(canvas, random);
                }
                else if (sel < 13) { 
                    Pattern.DrawConcentric(
                        canvas, random, 
                        stroke_width, stroke_shift: stroke_width * 4,
                        stroke_width_rand: true
                    );
                }
                else if (sel < 14) { 
                    Pattern.DrawSlideLines(
                        canvas, random, 
                        stroke_width, stroke_shift: stroke_width * 4,
                        stroke_width_rand: true
                    );
                }
                else if (sel < 15) { 
                    Pattern.DrawMultiArcs(
                        canvas, random
                    );
                }
                else {
                    int sel2 = random.Next(5);

                    if (sel2 < 3 || hasdrew_mesh) {
                        Pattern.DrawRadiation(canvas, random, stroke_width, stroke_width_rand: true);
                    }
                    else {
                        Pattern.DrawMesh(canvas, random, stroke_width: 2, stroke_shift: 12, stroke_width_rand: true);
                        hasdrew_mesh = true;
                    }
                }

                complexity--;
            }

            { 
                float stroke_width = 1.5f + (float)random.NextDouble();
                float stroke_shift = stroke_width * 5;
            
                int sel = random.Next(4);

                if (sel < 3) {
                    Pattern.DrawParallelLines(
                        canvas, random,
                        stroke_width, stroke_shift, loc_randrange: 0.01f,
                        color_rand: false, stroke_width_rand: true
                    );
                }
                else {
                    Pattern.DrawParallelArcs(
                        canvas, random,
                        stroke_width, stroke_shift, loc_randrange: 0.01f, curve_randrange: 0.01f,
                        color_rand: false, stroke_width_rand: true
                    );
                }
            }

            return canvas;
        }

        private static float RandomStrokeWidth(Random random) {
            float stroke_width = 1.5f + (float)random.NextDouble();

            while (random.Next(8) >= 5) {
                stroke_width++;
            }

            return stroke_width;
        } 
    }
}
