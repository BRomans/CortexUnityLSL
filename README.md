# CortexUnityLSL
An example on how to setup a client to interact with the CortexStreamer using LSL in Unity


## Requirements
- Unity version 2021.x or later
- LSL Unity plugin installed
- CortexStreamer from [CortexToolkit library](https://github.com/BRomans/CortexToolkit) 


## Usage
In the Tutorial scene there is a prefab called **BCI**, which needs to be in the scene. The **BCI** game object initializes a _NeuroDevice_, which in this case is an _LSLDevice_.

Two buttons are present in the scene:

- **SendMarker** : Will send a string containing "1" to the CortexStreamer as event marker.

- **StartStopEEG** : Will start/stop streaming the EEG data from the _CortexStreamer_ to the Unity scene. The data is pulled _sample by sample_

From this basic scene it is possible to build a complete BCI application by configuring commands into the _CortexStreamer_


#### Defined Commands
The following commands have been defined in the CortexStreamer.

- **98**: Start training a classifier (event markers need to be provided sequentially after the command)
- **99**: Stop training the classifier
- **100**: Start application mode (event markers need to be provided sequentially after the command)
- **101**: Stop application mode


## Credits
This project is freely available to anyone. If you use this project for academic purposes, please cite the original authors.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Authors
- [Michele Romani](https://bromans.github.io/)

Please make sure to update the [AUTHORS](AUTHORS) file if you are contributing to the project.


## Acknowledgments
- [LabStreamingLayer](https://labstreaminglayer.org/)
