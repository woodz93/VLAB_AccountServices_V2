

setTimeout(function () { ini(); }, 0);

function ini() {
	if ((typeof Server) !== "undefined") {
		setup();
	} else {
		setTimeout(function () {
			ini();
		}, 100);
	}
}
function setup() {
	if (document.getElementById("password_confirm")) {
		document.getElementById("password").addEventListener("keyup", function (e) {
			checkPassword();
		});
		document.getElementById("password_confirm").addEventListener("keyup", function (e) {
			checkPassword();
		});
		document.getElementById("status").addEventListener("click", function (e) {
			setTimeout(function () { dismiss(); }, 0);
		});
		window.addEventListener("keydown", function (event) {
			if (event.keyCode === 9) {
				if (event.srcElement.id === "password") {
					setTimeout(function () { document.getElementById("password_confirm").focus(); }, 0);
				}
				if (event.srcElement.id === "password_confirm") {
					setTimeout(function () {
						document.getElementById("submit").focus();
					}, 0);
				}
			}
		});
		console.log("All elements have successfully been setup!");
	} else {
		setTimeout(function () { ini(); }, 100);
	}
}

function checkPassword() {
	if (document.getElementById("password") && document.getElementById("password_confirm") && document.getElementById("form_main") && document.getElementById("submit")) {
		let fp = document.getElementById("password");
		let lp = document.getElementById("password_confirm");
		if (fp.value !== lp.value) {
			checkPasswordValue();
			//output("Passwords do not match!");
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
			checkPasswordValue();
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

function checkPasswordValue() {
	let user = document.getElementById("username");
	let pass = document.getElementById("password");
	let passc = document.getElementById("password_confirm");
	let btn = document.getElementById("submit");
	let res = false;
	if (user.value.length > 0) {
		if (pass.value.length > 4) {
			//if (pass.value.match(/[A-Z]{1,}[a-z]{1,}[0-9]{1,}([_!@#\$%\^&\*\(\)\-\{\}\[\]\.\,\`\~`\n\t:"'\?\<\>\|\/\\]{1,}|[\u200b]{1,})/g)) {
			if (pass.value.match(/[A-Z]{1,}/g) && pass.value.match(/[a-z]{1,}/g) && pass.value.match(/[0-9]{1,}/g) && pass.value.match(/([_!@#\$%\^&\*\(\)\-\{\}\[\]\.\,\`\~`\n\t:"'\?\<\>\|\/\\]{1,}|[\u200b]{1,})/g)) {
				if (pass.value === passc.value) {
					dismiss();
					res = true;
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
	return res;
}

function submitPasswordResetApplication() {
	let user = document.getElementById("username");
	let pass = document.getElementById("password");
	let passc = document.getElementById("password_confirm");
	let btn = document.getElementById("submit");
	if (checkPasswordValue()) {
		let a = {
			"src": "resetPassword.aspx",
			"args": {
				"cmd": "0",
				"username": user.value,
				"password": pass.value
			}
		};
		disableAllFields();
		Server.send(a, "passwordResetSvrResponse");
	}
}

function disableAllFields() {
	let user = document.getElementById("username");
	let pass = document.getElementById("password");
	let passc = document.getElementById("password_confirm");
	let btn = document.getElementById("submit");
	user.disable = true;
	pass.disable = true;
	passc.disable = true;
	btn.disable = true;
}
function enableAllFields() {
	let user = document.getElementById("username");
	let pass = document.getElementById("password");
	let passc = document.getElementById("password_confirm");
	let btn = document.getElementById("submit");
	user.disable = false;
	pass.disable = false;
	passc.disable = false;
	btn.disable = false;
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

function peak(elm = false) {
	if (elm !== false) {
		let t = (typeof elm);
		if (t === "element" || t === "object") {
			let tar=elm.getAttribute("data-ref");
			if (document.getElementById(tar)) {
				let elm = document.getElementById(tar);
				if (elm.type === "password") {
					elm.type = "text";
				} else {
					elm.type = "password";
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
					} else {
						output("Server didn't return a response message...<br>But it would appear that the process was succcessful.<br><br>If you encounter any issues, contact <a href='https://maui.hawaii.edu/helpdesk/#gform_7' target='_blank'>IT Help Desk</a> for further assistance.");
					}
				} else {
					enableAllFields();
					if (q["msg"]) {
						output(q["msg"]);
					} else {
						output("ERROR (500): An unknown server-side error has occurred.");
					}
				}
			}
		}
	}
}