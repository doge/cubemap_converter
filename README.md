# cubemap_converter
convert cubemaps to .dds 

## arguments

|  argument |             description           |     type      |
|:---------:|:---------------------------------:|:-------------:|
| hcross    | Path to horizontal cross cubemap. |     string    |
| vcross    | Path to vertical cross cubemap.   |     string    |
| hrow      | Path to horizontal row cubemap.   |     string    |
| vrow      | Path to vertical cross cubemap.   |     string    |

## diagrams

![cubemap diagram](https://docs.unity3d.com/uploads/Textures/CubeLayout6Faces.png)

## usage

compile cubemap_converter.exe and launch it with one of the launch parameters like so;

```powershell
cubemap_converter.exe -hcross <path to hcross>
```

the output is located at `<exe root folder>/bin/sky.dds`.

## goals
* [ ] support all types of cubemaps
	* [x] horizontal cross
	* [x] vertical cross
	* [ ] horizontal row
	* [ ] vertical row

[textconv & texassemble](https://github.com/microsoft)
