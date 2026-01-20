# Introduction

`porii` generates a palette from a JPG or PNG image using Magick.NET
and Unicolour. It can output to PNG for visualization and sampling or GPL
format for importing into GIMP or Krita.

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

|Argument|Description|Destination|
|--------|-----------|-----------|
|`-g`|Outputs the palette as a GPL palette file.|Yes|
|`-h`|Only uses a histogram for generating the palette.|No|
|`-o`|Outputs the palette file as an image to a destination.|Yes|
|`-p`|Prints the palette as binary PNG image data to standard output.|No|
|`-r`|Resizes the image by a percentage before generating the palette.|No|
|`-v`|Verbose printing.|No|

# Examples

`porii Ring.jpeg -o Palette.png`

<img width="512" height="128" alt="Ring-Out" src="https://github.com/user-attachments/assets/c29ec258-4b58-4e08-900c-003e67ccc9f2" />

`porii Colors.jpeg -p | chafa -f kitty`

<img width="512" height="128" alt="colors-out" src="https://github.com/user-attachments/assets/dc2b9af4-0e7d-460a-91f8-f3361166ef27" />

`porii Portrait.png -o Palette.png`

<img width="512" height="128" alt="portrait-palette" src="https://github.com/user-attachments/assets/85e8c48c-b76b-4151-a8d9-b12f9bed774c" />
