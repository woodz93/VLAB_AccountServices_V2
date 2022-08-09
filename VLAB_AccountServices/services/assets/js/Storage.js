
class Store {
	static ClassId = "StorageClassObject";
	static Records = [];
	// Initializes the data.
	static ini() {
		if (!Store.CheckRL()) {
			Store.CreateRL();
		}
		Store.LoadRL();
	}
	// Returns true if the record list exists, false otherwise.
	static CheckRL() {
		let res = true;
		if (!localStorage.getItem("AS_RecordList")) {
			res = false;
		}
		return res;
	}
	// Updates the record list.
	static Update() {
		if (Store.CheckRL()) {
			Store.Save(Store.Records,"AS_RecordList");
		}
	}
	// Loads the record list.
	static LoadRL() {
		if (Store.CheckRL()) {
			Store.Records = Store.GetRL();
			Store.PurgeOld();
		}
	}
	// Purges all old records.
	static PurgeOld() {
		let i = 0;
		while (i < Store.Records.length) {
			let tmp = localStorage.getItem(Store.Records[i]);
			let t = (typeof tmp);
			if (t==="object" && t==="array") {
				t = (typeof tmp["timestamp"]);
				if (t !== "undefined") {
					let d = new Date();
					let s = d.getTime() / 1000;
					let c = s - tmp["timestamp"];
					if (c > 300) {
						Store.Remove(Store.Records[i]);
					}
				}
			}
			i++;
		}
	}
	// Returns the record list.
	static GetRL() {
		let res = null;
		if (Store.CheckRL()) {
			res = Store.GetObject(localStorage.getItem("AS_RecordList"));
		}
		return res;
	}
	// Creates a new record list.
	static CreateRL() {
		localStorage.setItem("AS_RecordList",Store.GetString(Store.Records));
	}
	// Clears the record list.
	static Clear() {
		//localStorage.setItem("AS_RecordList","[]");
		Store.Records = [];
		Store.Update();
	}
	// Saves an item to the local storage of the client.
	static Save(obj = null,name = null) {
		let t = (typeof obj);
		if (t === "string" && obj!==null) {
			let tmp = Store.GetObject(obj);
			if (tmp !== null) {
				obj = tmp;
			} else {
				obj = {
					"value": obj
				};
			}
		}
		if (name !== null && (typeof name) === "string") {
			let d = new Date();
			obj["timestamp"] = d.getTime()/1000;
			localStorage.setItem(name,Store.GetString(obj));
			if (name !== "AS_RecordList") {
				Store.Records.push(name);
				Store.Update();
				console.log("Updated records list");
			}
		}
	}
	// Removes a stored item.
	static Remove(name) {
		if (Store.CheckValue(name)) {
			if (Store.Records.indexOf(name) != -1) {
				let i = 0;
				let tmp = [];
				while (i < Store.Records.length) {
					if (Store.Records[i] !== name) {
						tmp.push(Store.Records[i]);
					}
					i++;
				}
				Store.Records = tmp;
				Store.Update();
			}
			if (Store.Exists(name)) {
				localStorage.removeItem(name);
			}
		}
	}
	// Returns the value of the stored item.
	static Get(name) {
		let res = null;
		if (Store.Contains(name)) {
			if (Store.Exists(name)) {
				res = Store.GetObject(localStorage.getItem(name));
				let t = (typeof res);
				if (t === "object" || t === "array") {
					t = (typeof res["value"]);
					if (t !== "undefined") {
						let item = false;
						let value = false;
						let i = 0;
						for ([item,value] of Object.entries(res)) {
							i++;
							if (i > 1) {
								break;
							}
						}
						if (!(i > 1)) {
							res = res["value"];
						}
					}
				}
			}
		}
		return res;
	}
	// Returns true if the name exists within the record storage, false otherwise.
	static Contains(name) {
		let res = false;
		if (Store.CheckValue(name)) {
			if (Store.Records.indexOf(name) != -1) {
				res = true;
			}
		}
		return res;
	}
	// Returns true if the name of a stored item is found, false otherwise.
	static Exists(name) {
		let res = false;
		if (Store.CheckValue(name)) {
			if (localStorage.getItem(name)) {
				res = true;
			}
		}
		return res;
	}
	// Returns true if the parameter value is an array/object.
	static CheckObject(q) {
		let res = false;
		let t = (typeof q);
		if (t === "array" || t === "object") {
			res = true;
		}
		return res;
	}
	// Returns a string representation of the JSON object.
	static GetString(obj) {
		let res = null;
		if (Store.CheckObject(obj)) {
			try {
				res = JSON.stringify(obj);
			} catch (e) {
				let err = {
					"param": value,
					"type": (typeof value),
					"msg": e
				};
				console.groupCollapsed("%cERROR:","color:rgb(255,0,0);font-weight:bolder;");
				console.error("Unable to parse string into JSON object.");
				console.table([err]);
				console.groupEnd();
			}
		}
		return res;
	}
	// Returns the object value from a string value.
	static GetObject(value) {
		let res = null;
		if (Store.CheckValue(value)) {
			let obj = null;
			try {
				obj = JSON.parse(value);
				if (obj !== null) {
					res = obj;
				}
			} catch(e) {
				let err = {
					"param": value,
					"type": (typeof value),
					"msg": e
				};
				console.groupCollapsed("%cERROR:","color:rgb(255,0,0);font-weight:bolder;");
				console.error("Unable to parse string into JSON object.");
				console.table([err]);
				console.groupEnd();
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
Store.ini();
