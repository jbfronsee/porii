# Introduction

```
                  _ _ 
 _ __   ___  _ __(_|_)
| '_ \ / _ \| '__| | |
| |_) | (_) | |  | | |
| .__/ \___/|_|  |_|_|
|_| 
```

`porii` generates a palette from a JPG or PNG image using Magick.NET
and Unicolour. It can output to PNG for visualization and sampling or GPL
format for importing into GIMP or Krita. With the `map` subcommand it will also remap the palette from an image onto another image using dithering methods.

# How to Build and Run

1. Build Project
    - `dotnet publish -c Release -r <RID>`
    - Linux RID: `linux-x64`
    - Windows RID: `win-x64`
2. Run Project
    - `./bin/publish/<RID>/porii [Input File] [Flags]`

# Options

The standard behavior prints the palette to standard output as a list of 
hexadecimal color values. It is generated with two steps. First it builds a 
histogram of colors from the image. Afterwards it runs K-means clustering using 
the histogram as seed values.

|Argument|Description|Operand|
|--------|-----------|-------|
|`-f`|Sets the filter strength for the histogram based on number of pixels.|low, medium, or high e.g. `-f low`|
|`-g`|Outputs the palette as a GPL palette file.|No|
|`-h`|Only uses a histogram for generating the palette.|No|
|`-o`|Outputs the palette file as an image to a destination.|Destination File e.g. `-o out.png`|
|`-p`|Prints the palette as binary PNG image data to standard output.|No|
|`-r`|Resizes the image by a percentage before generating the palette.|Percentage e.g. `-r 75`|
|`-v`|Verbose printing.|No|

# Subcommands

`map` remaps palette onto an image using dithering.

`porii map palette.png image.png -o out.png`

# Examples

`porii Ring.jpeg -o Palette.png`

<img width="512" height="128" alt="Ring-Out" src="https://github.com/user-attachments/assets/c29ec258-4b58-4e08-900c-003e67ccc9f2" />

`porii Colors.jpeg -p | chafa -f kitty`

<img width="512" height="128" alt="colors-out" src="https://github.com/user-attachments/assets/dc2b9af4-0e7d-460a-91f8-f3361166ef27" />

`porii Portrait.png -o Palette.png`

<img width="512" height="128" alt="portrait-palette" src="https://github.com/user-attachments/assets/85e8c48c-b76b-4151-a8d9-b12f9bed774c" />
