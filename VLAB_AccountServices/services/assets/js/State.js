
class State {
	static Ready = false;
	// Performs an initialization.
	static ini() {
		if (!State.CheckReq()) {
			setTimeout(function () {
				State.ini();
			},100);
		} else {
			State.Ready = true;
			/*
			if (Store.Contains("responseMessage")) {
				if (document.getElementById("SMCElement")) {
					let elm = document.getElementById("SMCElement");
					State.Render(elm,Store.Get("SMCElement"));
				}
			}
			*/
		}
	}
	// Returns true if the required assets have already been loaded, false otherwise.
	static CheckReq() {
		let res = false;
		let t = (typeof Store);
		if (t !== "undefined") {
			t = (typeof Store.ClassId);
			if (t === "string") {
				res = true;
			}
		}
		return res;
	}
	// Saves the state of the provided element into storage.
	static Save(elm = null,name = null) {
		if (State.Ready) {
			if (name === "SMCElement") {
				//console.warn(elm);
			}
			let obj = State.Encode(elm);
			if (obj !== null) {
				if (name === null) {
					if (elm.id) {
						name = elm.id;
					} else {
						name = elm.tagName + "-" + document.getElementsByTagName(elm.tagName).length;
					}
				}
				//console.log(name);
				Store.Save(obj,name);
			} else {
				console.error("Object parameter is invalid.");
				console.warn(obj);
				console.warn(elm);
			}
		} else {
			setTimeout(function () {
				State.Save(elm,name);
			},100);
		}
	}
	static Get(name) {
		let res = null;
		if (Store.CheckValue(name)) {
			if (Store.Contains(name)) {
				res = Store.Get(name);
			}
		}
		return res;
	}
	// Loads a stored element's data into a given element.
	static Render(elm,obj) {
		let t = (typeof obj);
		if (t === "object" || t === "array") {
			if (State.IsElement(elm)) {
				let item = false;
				let value = false;
				//console.log(obj);
				for ([item,value] of Object.entries(obj)) {
					let tp = (typeof value);
					//console.log(tp);
					//console.log(item);
					if (tp === "array" || tp==="object") {
						//console.log("PASS");
						//console.log(item);
						if (item === "attributes") {
							let i = 0;
							let l = elm.getAttributeNames();
							while (i < l.length) {
								elm.removeAttribute(l[i]);
								i++;
							}
							let itm = false;
							let val = false;
							for ([itm,val] of Object.entries(value)) {
								elm.setAttribute(itm,val);
							}
						} else if (item === "classList") {
							let i = 0;
							while (i < elm.classList.length) {
								elm.classList.remove(elm.classList[i]);
								i++;
							}
							i = 0;
							//console.log(value);
							while (i < value.length) {
								elm.classList.add(value[i]);
								i++;
							}
						}
					}
					try {
						if (item !== "attributes" && item !== "classList" && item!=="tagName") {
							elm[item] = value;
						}
					} catch (e) {
						console.warn(e);
					}
				}
			}
		}
	}
	// Returns an object consisting of the element data, attributes, etc. upon success, null otherwise.
	static Encode(elm) {
		let res = null;
		if (State.IsElement(elm)) {
			let i = 0;
			let list = [
				"attributes",
				"classList"
			];
			let obj = {};
			while (i < list.length) {
				let buff = false;
				if (list[i]) {
					if (list[i] === "attributes") {
						let o = 0;
						let l = elm.getAttributeNames;
						let buff = {};
						while (o < l.length) {
							buff[l[o]] = elm.getAttribute(l[o]);
							o++;
						}
						obj[list[i]] = buff;
					} else {
						buff = elm[list[i]];
						obj[list[i]] = buff;
					}
				}
				i++;
			}
			list = [
				"textContent",
				"id",
				"innerHTML",
				"disabled",
				"hidden"
			];
			i = 0;
			while (i < list.length) {
				if (State.Contains(elm,list[i])) {
					obj[list[i]] = elm[list[i]];
				}
				i++;
			}
			res = obj;
		}
		return res;
	}
	// Returns true if the requested property exists, false otherwise.
	static Contains(elm,prop) {
		let res = false;
		let t = (typeof elm);
		if (t === "object") {
			t = (typeof prop);
			if (t === "string") {
				t = (typeof elm[prop]);
				if (t !== "undefined") {
					if (t === "string") {
						if (elm[prop].length > 0) {
							res = true;
						}
					} else {
						res = true;
					}
				}
			}
		}
		return res;
	}
	// Returns true if the value is an HTML object, false otherwise.
	static IsElement(elm) {
		let res = false;
		let t = (typeof elm);
		if (t === "object") {
			t = (typeof elm.tagName);
			if (t === "string") {
				res = true;
			}
		}
		return res;
	}

}
State.ini();