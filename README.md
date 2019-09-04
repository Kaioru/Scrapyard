# Scrapyard
.. is a bundling tool to package folders and .json files into a NX PKG4.1 compliant file.

## Usage

### Running from executable
1. Download the executables from the [releases](https://github.com/Kaioru/Scrapyard/releases) page
2. Execute the program on your terminal like so:
  * Windows: ```Scrapyard.CLI.exe <inputDir> <outputDir>```
  * macOS: ```Scrapyard.CLI <inputDir> <outputDir>```
3. *change **inputDir** and **outputDir** without the `<>`'s to your desired paths*

### Running from source
1. ```git clone https://github.com/Kaioru/Scrapyard && cd Scrapyard```
2. ```dotnet run --project Scrapyard.CLI <inputDir> <outputDir>```
3. *change **inputDir** and **outputDir** without the `<>`'s to your desired paths*

## Projects using Scrapyard
* [Server.NX](https://github.com/Kaioru/Server.NX) - the unbundled server data of [Edelstein](https://github.com/Kaioru/Edelstein).
