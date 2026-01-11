# Introduction

`px-swatch` generates a palette from a JPG or PNG image using Magick.NET
and Unicolour. It can output to PNG for visualization and sampling or GPL
format for importing into GIMP or Krita.

# How to Build

1. Install Magick.NET package
    - `dotnet add package Magick.NET-Q8-AnyCPU`
    - `dotnet add package Wacton.Unicolour`
2. Build Project
    - `dotnet publish -c Release -r <RID>`
    - Linux RID: `linux-x64`
    - Windows RID: `win-x64`

# How to Run
`./bin/Release/net8.0/<RID>/publish/px-swatch [Input File] [Flags]`

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

`px-swatch Ring.jpeg -o Palette.png`

<img width="512" height="128" alt="Palette" src="https://github.com/user-attachments/assets/cbc1c165-d344-4b68-bd87-f603e5e1ee3f" />

`px-swatch Flowers.jpg -p | chafa -f kitty`

<img width="512" height="128" alt="flowers" src="https://github.com/user-attachments/assets/d680aec2-b14d-45cd-b0ba-527195c5bf1a" />
