# Unity-Ocean-With-Job-System
Pretty simple example about Job System.
### Project Preview
![project overview](https://user-images.githubusercontent.com/26025564/220774109-7eed59cb-ccc8-489c-88f0-3462176b88ad.gif)


## Package Requirements
- Unity.Mathematics
- Unity.Burst
- Unity.Jobs

## Usage
- Create an empty GameObject.
- Add "WavesJobber.cs" script to GameObject.
- Add MeshRenderer Component to GameObject.
- Add Material to MeshRenderer.
- Set **ocean dimension**
- Add and edit new **noise layer** from Inspector. 
  - Example: 
  - ![example](https://user-images.githubusercontent.com/26025564/220772523-0bc32f3c-19f8-4ae9-bba7-ab8365d634c9.png)

That's all.
Now just **click to play button** and see the results. You can try to **disable UseJobSystem to see Job System & Burst Compiler difference.**




### Note: Make sure to enable **Use Job System** boolean for optimizing.

![usejobsystem](https://user-images.githubusercontent.com/26025564/220772755-8a163ccc-5356-4ec4-893a-36ffe8de7e8a.png)
