
setTimeout(function () { ini(); }, 0);

function ini() {
	let t = (typeof Server);
	if (t !== "undefined") {
		let username = "";
		let a = {
			"src": "assets/svr/checkAccount.aspx",
			"args": {
				"cmd": "check-user",
				"username": username
			}
		};
		Server.send(a, "svrResponse");
	} else {
		setTimeout(function () { ini(); }, 100);
	}
}
// Processes the server response.
function svrResponse(q = false) {
	if (q !== false) {
		let t = (typeof q);
		if (t === "string") {
			try {
				q = JSON.parse(q);
			} catch (e) {
				q = false;
			}
		}
		t = (typeof q);
		if (t === "array" || t === "object") {
			t = (typeof q["status"]);
			if (t !== "undefined") {
				if (q["status"] === true) {
					getPasswordResetForm();
				} else {
					getNewUserForm();
				}
			}
		}
	}
}
// Gets a password reset form from the server.
function getPasswordResetForm() {
	let a = {
		"src": "assets/svr/getPRF.aspx",
		"args": {
			"cmd":"0"
		}
	};
	Server.send(a, "genForm");
}
// Gets a new user form from the server.
function getNewUserForm() {
	let a = {
		"src": "assets/svr/getNUF.aspx",
		"args": {
			"cmd": "0"
		}
	};
	Server.send(a, "genForm");
}
// Generates the form.
function genForm(q = false) {
	if (q !== false) {
		let t = (typeof q);
		if (t === "string") {
			try {
				q = JSON.parse(q);
			} catch (e) {
				q = false;
			}
			t = (typeof q);
			if (t === "array" || t === "object") {
				t = (typeof q["status"]);
				if (t !== "undefined") {
					if (q["status"] === true) {
						t = (typeof q["html"]);
						if (t === "string") {
							if (document.getElementById("form-main")) {
								document.getElementById("form-main").innerHTML = q["html"];
							}
						}
						t = (typeof q["js"]);
						if (t === "array") {
							let i = 0;
							let ty = false;
							let elm = false;
							let par = document.getElementById("js-assets");
							while (i < q["js"].length) {
								ty = (typeof q["js"]);
								if (ty === "string") {
									elm = document.createElement("script");
									elm.src = q["js"][i];
									elm.defer = true;
									par.appendChild(elm);
								}
								i++;
							}
						}
					}
				}
			}
		}
	}
}
