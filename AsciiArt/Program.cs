using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text;

namespace AsciiArt;

class Program
{
    // Different character sets for different rendering modes
    // Ordered from darkest (most dense) to lightest (least dense)
    private static readonly char[] DetailedChars = { '@', '#', 'M', 'N', 'W', 'Q', 'B', 'D', 'R', '$', '8', '&', '%', '0', 'H', 'U', 'A', 'G', 'X', 'h', 'q', 'K', 'P', 'd', 'b', 'p', 'k', 'w', 'a', 'o', 'g', 'Z', 'O', 'V', 'n', 'u', 's', 'x', 'z', 'f', 'c', 'r', '*', '{', '}', '[', ']', '(', ')', '/', '\\', '|', 'j', 't', 'l', 'i', '!', '<', '>', '+', '~', '?', ';', ':', '^', ',', '\'', '.', ' ' };
    private static readonly char[] SimpleChars = { '@', '#', 'S', '%', '?', '*', '+', ';', ':', ',', ' ' };
    private static readonly char[] EdgeChars = { '@', '#', '|', '/', '\\', '-', '+', '\'', '.', ' ' };
    // Unicode block characters - provide much better visual density and legibility
    private static readonly char[] BlockChars = { '█', '▓', '▒', '░', ' ' };
    private static readonly string[] ShadeChars = { "█", "▓", "▒", "░", " " };

    enum RenderMode
    {
        Simple,
        Detailed,
        Block,
        Edge,
        Enhanced
    }

    static void Main(string[] args)
    {
        if (args.Length == 0 || args[0] == "--help" || args[0] == "-h")
        {
            ShowHelp();
            return;
        }

        string imagePath = args[0];
        int width = 100;
        RenderMode mode = RenderMode.Simple;

        // Parse additional arguments
        for (int i = 1; i < args.Length; i++)
        {
            if (int.TryParse(args[i], out int w))
            {
                width = w;
            }
            else if (args[i].StartsWith("--mode=") || args[i].StartsWith("-m="))
            {
                string modeValue = args[i].Split('=')[1].ToLower();
                mode = modeValue switch
                {
                    "simple" => RenderMode.Simple,
                    "detailed" => RenderMode.Detailed,
                    "block" => RenderMode.Block,
                    "edge" => RenderMode.Edge,
                    "enhanced" => RenderMode.Enhanced,
                    _ => RenderMode.Simple
                };
            }
        }

        if (!File.Exists(imagePath))
        {
            Console.WriteLine($"Error: Image file '{imagePath}' not found.");
            return;
        }

        try
        {
            string asciiArt = ConvertImageToAscii(imagePath, width, mode);
            Console.WriteLine(asciiArt);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: Failed to convert image.");
            Console.WriteLine($"Details: {ex.Message}");
        }
    }

    static void ShowHelp()
    {
        Console.WriteLine("ASCII Art Converter - Enhanced Edition");
        Console.WriteLine();
        Console.WriteLine("Usage: dotnet run <image-path> [width] [--mode=<mode>]");
        Console.WriteLine();
        Console.WriteLine("Arguments:");
        Console.WriteLine("  image-path    Path to the image file to convert");
        Console.WriteLine("  width         Width in characters (default: 100)");
        Console.WriteLine("  --mode=       Rendering mode (default: simple)");
        Console.WriteLine();
        Console.WriteLine("Rendering Modes:");
        Console.WriteLine("  simple        Basic grayscale conversion (default)");
        Console.WriteLine("  detailed      High detail with larger character set");
        Console.WriteLine("  block         Unicode block characters (HIGHEST QUALITY)");
        Console.WriteLine("  edge          Edge detection focused (experimental)");
        Console.WriteLine("  enhanced      Dual-layer: edges + tone (experimental)");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run image.jpg");
        Console.WriteLine("  dotnet run photo.png 150 --mode=block");
        Console.WriteLine("  dotnet run portrait.jpg 120 --mode=detailed");
    }

    static string ConvertImageToAscii(string imagePath, int width, RenderMode mode)
    {
        using var image = Image.Load<Rgb24>(imagePath);

        // Calculate new height maintaining aspect ratio
        int height = (int)(width * image.Height / image.Width * 0.55);

        // Resize image
        image.Mutate(x => x.Resize(width, height));

        // Apply contrast enhancement only for edge/enhanced modes
        if (mode == RenderMode.Edge || mode == RenderMode.Enhanced)
        {
            image.Mutate(x => x.Contrast(1.15f));
        }

        return mode switch
        {
            RenderMode.Simple => RenderSimple(image),
            RenderMode.Detailed => RenderDetailed(image),
            RenderMode.Block => RenderBlock(image),
            RenderMode.Edge => RenderEdge(image),
            RenderMode.Enhanced => RenderEnhanced(image),
            _ => RenderSimple(image)
        };
    }

    static string RenderSimple(Image<Rgb24> image)
    {
        var asciiArt = new StringBuilder();

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                var pixel = image[x, y];
                int grayValue = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
                int charIndex = grayValue * (SimpleChars.Length - 1) / 255;
                asciiArt.Append(SimpleChars[charIndex]);
            }
            asciiArt.AppendLine();
        }

        return asciiArt.ToString();
    }

    static string RenderDetailed(Image<Rgb24> image)
    {
        var asciiArt = new StringBuilder();

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                var pixel = image[x, y];
                int grayValue = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
                int charIndex = grayValue * (DetailedChars.Length - 1) / 255;
                asciiArt.Append(DetailedChars[charIndex]);
            }
            asciiArt.AppendLine();
        }

        return asciiArt.ToString();
    }

    static string RenderBlock(Image<Rgb24> image)
    {
        var asciiArt = new StringBuilder();

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                var pixel = image[x, y];
                int grayValue = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);

                // Map to block characters (inverted because blocks are solid)
                // 0 (black) -> full block, 255 (white) -> space
                int charIndex = (255 - grayValue) * (BlockChars.Length - 1) / 255;
                asciiArt.Append(BlockChars[charIndex]);
            }
            asciiArt.AppendLine();
        }

        return asciiArt.ToString();
    }

    static string RenderEdge(Image<Rgb24> image)
    {
        var edges = DetectEdges(image);
        var asciiArt = new StringBuilder();

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                int edgeStrength = edges[x, y];
                int charIndex = edgeStrength * (EdgeChars.Length - 1) / 255;
                asciiArt.Append(EdgeChars[charIndex]);
            }
            asciiArt.AppendLine();
        }

        return asciiArt.ToString();
    }

    static string RenderEnhanced(Image<Rgb24> image)
    {
        var edges = DetectEdges(image);
        var asciiArt = new StringBuilder();

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                var pixel = image[x, y];
                int grayValue = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
                int edgeStrength = edges[x, y];

                // Combine edge detection with tone
                // If edge is very strong, use edge-oriented characters
                // Otherwise, blend edge information with tone
                char selectedChar;

                if (edgeStrength > 150)
                {
                    // Very strong edge - use darkest edge characters
                    selectedChar = EdgeChars[0];
                }
                else if (edgeStrength > 80)
                {
                    // Medium edge - darken the tone character
                    int darkenedValue = Math.Max(0, grayValue - edgeStrength / 2);
                    int charIndex = darkenedValue * (DetailedChars.Length - 1) / 255;
                    selectedChar = DetailedChars[charIndex];
                }
                else
                {
                    // Weak edge - use normal tone characters
                    selectedChar = DetailedChars[grayValue * (DetailedChars.Length - 1) / 255];
                }

                asciiArt.Append(selectedChar);
            }
            asciiArt.AppendLine();
        }

        return asciiArt.ToString();
    }

    static int[,] DetectEdges(Image<Rgb24> image)
    {
        int width = image.Width;
        int height = image.Height;
        int[,] edges = new int[width, height];

        // Sobel operator kernels
        int[,] sobelX = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
        int[,] sobelY = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

        // Convert to grayscale first
        int[,] gray = new int[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var pixel = image[x, y];
                gray[x, y] = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
            }
        }

        // Apply Sobel operator
        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                int gx = 0, gy = 0;

                for (int ky = -1; ky <= 1; ky++)
                {
                    for (int kx = -1; kx <= 1; kx++)
                    {
                        int pixelValue = gray[x + kx, y + ky];
                        gx += pixelValue * sobelX[ky + 1, kx + 1];
                        gy += pixelValue * sobelY[ky + 1, kx + 1];
                    }
                }

                // Calculate gradient magnitude
                int magnitude = (int)Math.Sqrt(gx * gx + gy * gy);
                edges[x, y] = Math.Min(255, magnitude);
            }
        }

        return edges;
    }
}
