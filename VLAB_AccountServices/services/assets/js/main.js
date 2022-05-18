

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
			if (!document.getElementById("form_main").disabled) {
				document.getElementById("form_main").disabled = true;
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
			if (document.getElementById("form_main").disabled) {
				document.getElementById("form_main").disabled = false;
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
