
class Loader {
	// Loads all resources.
	static LoadAll() {
		let list = [
			"assets/js/server.js",
			"assets/js/Storage.js",
			"assets/js/State.js",
			"assets/css/Bootstrap/Bootstrap.js",
			"assets/js/main.js"
		];
		Loader.Iterator(list);
	}
	static Iterator(list,i=0) {
		Loader.Load(list[i]);
		i++;
		if (i < list.length) {
			setTimeout(function () {
				Loader.Iterator(list,i);
			},100);
		} else {
			console.log("All assets have been loaded.");
		}
	}
	// Loads in a JS resource/asset from a given url source.
	static Load(src) {
		if (Loader.CheckValue(src)) {
			let ext = Loader.GetExtension(src);
			let elm = null;
			if (ext === "js") {
				elm = document.createElement("script");
				elm.src = src;
				elm.defer = true;
			} else if (ext === "css") {
				elm = document.createElement("link");
				elm.href = src;
				elm.rel = "stylesheet";
				elm.defer = true;
			}
			if (elm !== null) {
				document.getElementsByTagName("head")[0].appendChild(elm);
			} else {
				console.warn(src);
				console.log(elm);
				console.log(ext);
			}
		} else {
			console.error(src);
		}
	}
	// Returns the file extension from the url path provided.
	static GetExtension(src) {
		let res = null;
		if (Loader.CheckValue(src)) {
			let reg = /\.(js|html|css|json|text|javascript|aspx)$/i;
			let mp = src.match(reg);
			let t = (typeof mp);
			if (t==="array"||t==="object") {
				if (mp.length > 0) {
					res = mp[1].toLowerCase();
				}
			}
		}
		return res;
	}
	// Returns true if the string value is valid, false otherwise.
	static CheckValue(q) {
		let res = false;
		let t = (typeof q);
		if (t === "string") {
			if (q.length > 0) {
				res = true;
			}
		}
		return res;
	}
}
Loader.LoadAll();