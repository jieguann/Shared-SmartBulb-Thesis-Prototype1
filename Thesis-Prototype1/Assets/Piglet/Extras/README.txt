# URP Shaders

To use Piglet with a Universal Render Pipeline (URP) based project, unpack the shaders from the appropriate `.unitypackage` file in this directory. For Unity versions 2019.3.0f6 through 2020.1.x, use `URP-Shaders-2019.3.unitypackage`. For Unity 2020.2.0b14 or newer, use `URP-Shaders-2020.2.unitypackage`. The shader files will be unpacked into `Assets/Piglet/Resources/Shaders/URP`.

# Json.NET

If you see compile errors in the Unity console that say `error CS0246: The type or namespace name 'Newtonsoft' could not be found (are you missing a using directive or an assembly reference?)`, double-click the Json.NET-10.0.3.unitypackage in this directory. This will unpack the missing Newtonsoft.Json.dll into `Assets/Piglet/Dependencies/Json.NET`.
