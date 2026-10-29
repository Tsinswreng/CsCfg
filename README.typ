#import "@preview/tsinswreng-auto-heading:0.1.0": auto-heading
#let H = auto-heading;

= CfCfg
Configuration system for C\#

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

```bash
dotnet add package Tsinswreng.CsCfg --version 0.0.1-alpha
```

