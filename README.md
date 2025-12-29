# ASCII Art Converter - Enhanced Edition

A .NET console application that converts images to ASCII art with multiple rendering modes for optimal legibility.

## Features

- **5 Rendering Modes**: Simple, Detailed, Block (Unicode), Edge Detection, and Enhanced
- **Unicode Block Characters**: High-resolution mode using block/shade characters for maximum legibility
- **Edge Detection**: Sobel operator for detecting image structure
- **Contrast Enhancement**: Automatic contrast boosting for better definition
- **Multiple Character Sets**: Standard ASCII, detailed ASCII, or Unicode blocks
- **Dual-Layer Rendering**: Combines edge detection with tone mapping
- Adjustable output width
- Maintains image aspect ratio
- Supports all major image formats (JPEG, PNG, BMP, GIF, etc.)

## Requirements

- .NET 8.0 or later

## Installation

1. Clone this repository
2. Navigate to the AsciiArt directory:
   ```bash
   cd AsciiArt
   ```

## Usage

### Basic usage (uses simple mode by default):
```bash
dotnet run <image-path>
```

### Specify custom width and rendering mode:
```bash
dotnet run <image-path> <width> --mode=<mode>
```

### Examples:
```bash
# Simple mode (default)
dotnet run image.jpg

# Block mode - HIGHEST QUALITY/MOST LEGIBLE
dotnet run image.jpg 100 --mode=block

# Detailed mode with custom width (150 characters)
dotnet run photo.png 150 --mode=detailed

# Edge detection mode for outlines and sketches
dotnet run portrait.jpg 120 --mode=edge
```

## Rendering Modes

### Simple Mode (`--mode=simple`)
- Basic grayscale conversion
- Uses 11 ASCII characters
- Fastest processing
- Good for quick previews

### Detailed Mode (`--mode=detailed`)
- High detail with 70-character set
- Better tonal range
- More subtle gradients
- Best for images with smooth transitions

### Block Mode (`--mode=block`) - RECOMMENDED
- **Uses Unicode block characters**: █ ▓ ▒ ░
- **Highest visual quality and legibility**
- Each character has much more visual density than ASCII
- Works best for photos, portraits, and detailed images
- Requires terminal with Unicode support (most modern terminals)
- **Try this mode first for best results**

### Edge Mode (`--mode=edge`)
- Sobel edge detection algorithm
- Focuses on outlines and structure
- Best for sketches and line art
- Uses directional characters
- Experimental

### Enhanced Mode (`--mode=enhanced`)
- Dual-layer rendering: Combines edge detection with tone mapping
- Strong edges use structural characters
- Weak edges use detailed tonal characters
- Automatic contrast enhancement
- Experimental

## How It Works

### Enhanced Mode Algorithm:
1. Loads image using SixLabors.ImageSharp library
2. Resizes to specified width while maintaining aspect ratio
3. Applies automatic contrast enhancement (1.3x boost)
4. Runs Sobel edge detection to identify structural features
5. Converts pixels to grayscale using luminosity method
6. For each pixel:
   - If edge strength > 100: Use edge-oriented character
   - Otherwise: Use detailed tone-based character
7. Outputs combined structure + tone ASCII art

### Edge Detection:
- Uses Sobel operator with horizontal and vertical kernels
- Calculates gradient magnitude for each pixel
- Identifies edges, lines, and structural boundaries
- Preserves important visual features

## Supported Image Formats

- JPEG/JPG
- PNG
- BMP
- GIF
- TIFF
- And many more supported by ImageSharp

## Tips for Best Results

- **Block mode** provides the highest quality and most legible output - try this first!
- **Detailed mode** is ideal for ASCII-only terminals or images with subtle gradients
- **Simple mode** is fastest and works well for quick previews
- Smaller widths (50-80) work better for complex images
- Larger widths (120-200) work better for simple images or larger terminals
- Block mode works best at higher widths (100-150) due to character density
- Images with good contrast produce better results
- Make your terminal full screen for larger ASCII art
- Ensure your terminal supports Unicode for block mode

## License

See LICENSE file for details.
