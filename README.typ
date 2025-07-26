= CfCfg
Configuration system for C\#

*⚠️This project has no release version yet and everything may change. If you want to use it, we recommand you to clone the code and reference the source code directly.⚠️*


= Usage

+ prepare your config source file e.g: `App.dev.json`
```json
{
	"SqlitePath": "./App.dev.sqlite"
	,"Background": {
		"GalleryDirs": [
			"C:/Users/lenovo/Pictures/qq_image/20240913141225"
		]
		,"Order": "Random"
		,"Stretch": "UniformToFill"
		,"Brightness": 1.0
	}
	,"ServerBaseUrl": "http://localhost:5000/"
}
```
+ Create your config accessor class

