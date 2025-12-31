# VR Spay App: First-Time Setup Guide

This document outlines the necessary steps to set up the **VR Spay** Unity project and deploy it to a Meta Quest headset.

## 1. Unity Installation & Environment Setup

### Install Unity Editor
* **Version:** Install **Unity 2022.3.4f1**.
* **Modules:** Ensure you include the **Android Build Support** modules (OpenJDK, Android SDK & NDK tools) during installation.
* **Note:** You may encounter a security risk warning regarding this specific version. You may ignore this for now.
    > *TODO: Update project to Unity 6.3 LTS in the future if possible.*

### Configure Project Settings
1.  **TextMesh Pro:**
    * Open the project and go to `Edit > Project Settings`.
    * Navigate to **TextMesh Pro** and import the "Essential Resources" if they are not already present.
2.  **XR Plug-in Management:**
    * In `Project Settings`, go to the **XR Plug-in Management** tab.
    * If not installed, click **Install XR Plug-in Management**.
    * Once installed, select the **Android** tab (robot icon).
    * Select the plug-in providers exactly as shown in the image below:
    
    <img width="1128" height="650" alt="xrpm" src="https://github.com/user-attachments/assets/ef4044fc-bc9c-42a1-8d30-9bdced67198c" />


## 2. Meta XR SDK Configuration

1.  **Install Meta XR Core SDK:**
    * Go to `Window > Package Manager`.
    * Check if **Meta XR Core SDK** is listed.
    * If it is missing, add it to your assets via the Unity Asset Store here: [Meta XR Core SDK](https://assetstore.unity.com/packages/tools/integration/meta-xr-core-sdk-269169).
    * Return to the Package Manager in Unity, select "My Assets" from the dropdown, find the SDK, and click **Download/Import**.
2.  **Apply Project Fixes:**
    * In the top menu bar, go to **Meta XR Tools > Project Setup Tool**.
    * Toggle to the **Android** tab (robot icon).
    * Click the **Fix All** button to automatically resolve configuration issues.

## 3. Connecting the Headset (Meta Quest)

We use the **Meta Quest Developer Hub (MQDH)** to manage device connections.

1.  **Install MQDH:**
    * Download and install the [Meta Quest Developer Hub](https://developers.meta.com/horizon/documentation/unity/ts-mqdh/).
    * *Note:* Follow the official instructions on the linked page to pair your headset with your computer. Meta updates these procedures frequently, so the official documentation is the most reliable source.
2.  **Connect Device:**
    * Connect your Meta Quest headset to your computer via USB.
    * Ensure the headset is detected in MQDH before proceeding.

## 4. Building to Quest

1.  **Switch Platform:**
    * In Unity, go to `File > Build Settings`.
    * Under **Platform**, select **Android**.
    * Click the **Switch Platform** button. *This process may take several minutes.*
2.  **Select Run Device:**
    * Under the **Run Device** dropdown, look for your connected headset (typically named "Oculus/Meta Quest <ID>").
    * **Troubleshooting:** If your device is not listed:
        * Check your USB connection.
        * Ensure Developer Mode is enabled on the headset.
        * Verify the device is recognized in the Meta Quest Developer Hub.
3.  **Build Settings:**
    * Check the box for **Development Build**.
4.  **Deploy:**
    * Click **Build and Run**.
    * Choose a save location for the `.apk` file.
    * Once the build finishes, the app should automatically launch on your headset.
