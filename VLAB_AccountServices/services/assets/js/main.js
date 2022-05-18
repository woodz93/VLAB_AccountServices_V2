

setTimeout(function () { ini(); }, 0);

function ini() {
	if (document.getElementById("password-confirm")) {
		document.getElementById("password-confirm").addEventListener("keypress", function (e) {
			checkPassword(document.getElementById("password-confirm"));
		});
		document.getElementById("status").addEventListener("click", function (e) {
			setTimeout(function () { dismiss(); }, 0);
		});
	} else {
		setTimeout(function () { ini(); }, 100);
	}
}

function checkPassword() {
	if (document.getElementById("password") && document.getElementById("confirm-password") && document.getElementById("form_main") && document.getElementById("submit")) {
		let fp = document.getElementById("password");
		let lp = document.getElementById("confirm-password");
		if (fp.value !== lp.value) {
			if (document.getElementById("status")) {
				document.getElementById("status").innerHTML = "Passwords do not match.";
			}
			if (!document.getElementById("submit").disabled) {
				document.getElementById("submit").disabled = true;
			}
			if (!fp.classList.contains("invalid")) {
				fp.classList.add("invalid");
			}
			if (!lp.classList.contains("invalid")) {
				lp.classList.add("invalid");
			}
		} else {
			if (document.getElementById("submit").disabled) {
				document.getElementById("submit").disabled = false;
			}
			if (fp.classList.contains("invalid")) {
				fp.classList.remove("invalid");
			}
			if (lp.classList.contains("invalid")) {
				lp.classList.remove("invalid");
			}
		}
	}
}

function submitPasswordResetApplication() {
	let user = document.getElementById("username");
	let pass = document.getElementById("password");
	let passc = document.getElementById("password-confirm");
	let btn = document.getElementById("submit");
	if (user.value.length > 0) {
		if (pass.value.length > 4) {
			if (pass.value.match(/[A-Z]{1,}[a-z]{1,}[0-9]{1,}([_!@#\$%\^&\*\(\)\-\{\}\[\]\.\,\`\~`\n\t:"'\?\<\>\|\/\\]{1,}|[\u200b]{1,})/g)) {
				if (pass.value === passc.value) {
					let a = {
						"src": "assets/svr/passwordReset.aspx",
						"args": {
							"cmd": "0",
							"username": user,
							"password": pass
						}
					};
					Server.send(a, "passwordResetSvrResponse");
				} else {
					output("Passwords do not match.");
				}
			} else {
				output("Password must contain at least one capital letter, one lowercase letter, one number, and one special character (Keyboard-based and some whitespace characters are valid... As well as the zero-width character).");
			}
		} else {
			output("Password must be longer than 4 characters long.");
		}
	}
}


function dismiss() {
	if (document.getElementById("status")) {
		let s = document.getElementById("status");
		if (s.classList.contains("error")) {
			s.classList.remove("error");
			s.innerHTML = "";
		}
	}
}

function output(q = false) {
	if (q !== false) {
		if ((typeof q) === "string") {
			if (document.getElementById("status")) {
				let s = document.getElementById("status");
				s.innerHTML = q;
				if (!s.classList.contains("error")) {
					s.classList.add("error");
				}
			}
		}
	}
}

function passwordResetSvrResponse(q = false) {
	console.log(q);
	if (q !== false) {
		let t = (typeof q);
		if (t === "string") {
			try {
				q = JSON.parse(q);
			} catch (e) {
				t = "string";
			}
		}
		if (t === "array" || t === "object") {
			t = (typeof q["status"]);
			if (t !== "undefined") {
				if (q["status"] === true) {
					if (q["msg"]) {
						output(q["msg"]);
					}
				} else {
					if (q["msg"]) {
						output(q["msg"]);
					}
				}
			}
		}
	}
}


