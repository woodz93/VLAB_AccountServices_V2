

setTimeout(function () { ini(); }, 0);

function ini() {
	if (document.getElementById("password-confirm")) {
		document.getElementById("password-confirm").addEventListener("keypress", function (e) {
			checkPassword(document.getElementById("password-confirm"));
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



