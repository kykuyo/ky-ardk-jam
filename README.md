## How to Build

This project was made with Unity, follow the steps below to compile. Firebase is used for the realtime database remotely, communication is done over REST APIs and thus needs no additional package installed.

1. Ensure you have Unity version [2022.3.10f1] installed
2. Install the https://github.com/Unity-Technologies/com.unity.xr-content.xr-sim-environments/releases via tarball to install v1.0.0
2. Go to the package manager in Unity and install from git URL: https://github.com/niantic-lightship/ardk-upm.git
3. Do the same for https://github.com/niantic-lightship/maps-upm.git (v0.4.0)
4. You will need to install the TMP essetnials (prompt should automatically come up)
5. Build the project for Android

The main scene to open is `/Assets/Scenes/WorldMap`