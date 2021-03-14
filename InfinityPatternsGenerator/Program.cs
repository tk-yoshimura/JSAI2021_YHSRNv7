using InfinityPatterns;
using System;
using System.IO;

namespace InfinityPatternsGenerator {
    class Program {
        const int max_images = 100000;

        static int Main(string[] args) {
            if(args.Length != 3) { 
                Console.WriteLine("Specify the arguments : \"[str]Output directory path\" \"[int]Number of images\"  \"[int]Generation seed\"");
                return -1;
            }

            string dirpath;
            int images, seed;

            try { 
                dirpath = Path.GetFullPath(args[0]);
                images = int.Parse(args[1]);
                seed = int.Parse(args[2]);

                if (dirpath.Contains("..")) {
                    throw new ArgumentException($"{nameof(dirpath)} contains invalid syntax.");
                }

                if(images < 1 || images > max_images) { 
                    throw new ArgumentOutOfRangeException($"{nameof(images)} <= {max_images}");
                }

                if (!Directory.Exists(dirpath)) { 
                    Directory.CreateDirectory(dirpath);
                }
            
                Random random = new Random(seed);

                int cursor_left = Console.CursorLeft, cursor_top = Console.CursorTop;

                for(int i = 0; i < images;) { 
                    if(i % 10 == 0) { 
                        Console.SetCursorPosition(cursor_left, cursor_top);
                        Console.Write($"Generating... {i * 100 / images} %");
                    }

                    try {
                        using Canvas canvas = ComplexPattern.Generate(random, size: 256, complexity: 12);

                        canvas.Save($"{dirpath}/img_{i}.png");
                    }
                    catch (Exception e) when (e is ArgumentException || e is OutOfMemoryException){ /* GDI+ often throws these exception. */
                        continue;
                    }

                    i++;
                }

                Console.SetCursorPosition(cursor_left, cursor_top);
                Console.Write($"Generating... 100 %");
            }
            catch (Exception e){ 
                Console.WriteLine($"\n{e.Message}");
                return -1;
            }

            return 0;
        }
    }
}
