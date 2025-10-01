# CortexUnityLSL
An example on how to setup a client to interact with OpenCortexBCI using LSL in Unity


## Requirements
- Unity version 2021.x or later
- LSL Unity plugin installed
- [OpenCortexBCI](https://github.com/BRomans/OpenCortexBCI) 


## Usage
In the Tutorial scene there is a prefab called **BCI**, which is required to receive data. The **BCI** game object internally initializes a _NeuroDevice_, which in this case is an _LSLDevice_.

Two buttons are present in the scene:

- **Send Marker** : Will send a string containing "1" to the CortexStreamer as event marker.

- **Receive Streams** : Will start/stop receiving raw EEG data or inference outputs from _OpenCortexBCI_ to the Unity scene. The data is pulled _sample by sample_

From this basic scene it is possible to build a complete BCI application by configuring pipelines and tasks into _OpenCortexBCI_ and, in Unity, subscribing to the event callbacks.


## Credits
This project is freely available to anyone. If you use this project for academic purposes, please credit the original authors.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Authors
- [Michele Romani](https://bromans.github.io/)
- Gregorio Andrea Giudici

See [AUTHORS.md](AUTHORS.md) for the list of contributors to this project.
Please make sure to update the file if you are contributing to the project.


## Acknowledgments
- [LabStreamingLayer](https://labstreaminglayer.org/)
