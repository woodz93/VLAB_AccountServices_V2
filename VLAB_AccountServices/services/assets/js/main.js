

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
	prepSubmitBtn();
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

function SetInfo(q = false) {
	let obj = {
		"title": "Information",
		"content": q.getAttribute("data-content")
	};
	SetModal(obj);
}
function SetModal(obj = false) {
	if (obj !== false) {
		let elm_title = document.getElementById("modal_title");
		let elm_body = document.getElementById("modal_content");
		elm_title.innerHTML = "";
		elm_body.innerHTML = "";
		obj["content"] = Convert(obj["content"]);
		if (obj["title"]) {
			elm_title.innerHTML = obj["title"];
		}
		if (obj["content"]) {
			elm_body.innerHTML = obj["content"];
		}
	}
}

function Convert(q = false) {
	if (q !== false) {
		if (q.indexOf("\n") != -1) {
			q = q.replace(/[\n]/g, "<br>");
		}
		if (q.indexOf("\\n") != -1) {
			q = q.replace(/(\\n)/g, "<br>");
		}
	}
	return q;
}



function SubmitForm(q=false) {
	if (document.getElementById("submit_btn")) {
		let elm = document.getElementById("submit_btn");
		if (!elm.disabled) {
			elm.click();
			elm.disabled = true;
			let list = document.querySelectorAll("button.submit_button");
			let i = 0;
			while (i < list.length) {
				list[i].innerHTML = "<span class=\"spinner-grow spinner-grow-sm\"></span> " + list[i].innerHTML;
				i++;
			}
			/*
			setTimeout(function () {
				let list = document.querySelectorAll("*");
				let i = 0;
				while (i < list.length) {
					if (list[i].tagName == "INPUT" || list[i].tagName == "BUTTON") {
						list[i].disabled = true;
					}
					i++;
				}
			}, 100);
			*/
			if (q !== false) {
				try {
					q.disabled = true;
				} catch { }
			}
		}
	}
}


function prepSubmitBtn() {
	if (document.getElementById("submit_btn")) {
		/*
		document.getElementById("form_main").addEventListener("submit", function () {
			document.getElementById("submit_btn").disabled = true;
		});
		document.getElementById("submit_btn").addEventListener("click", function () {
			document.getElementById("submit_btn").disabled = true;
		});
		*/
		let list = document.querySelectorAll("table#GroupsElement span");
		let list0 = document.querySelectorAll("table#UserGroupsElement span");
		let i = 0;
		//let tmp = "<button type=\"button\" class=\"btn btn - info info\" data-bs-toggle=\"modal\" data-bs-target=\"#modal_panel\" onclick=\"SetInfo(this)\" data-content=\""+info[i]+"\"></button>";
		let elm = "";
		let info = {
			"Adobe":"Provides access to Adobe software tools such as Photoshop, Illustrator, Dreamweaver, and more."
		};
		let tmp = "";
		while (i < list.length) {
			tmp = "[" + list[i].textContent + "]";
			if (info[list[i].textContent]) {
				tmp = info[list[i].textContent];
			}
			elm = " <button type=\"button\" class=\"btn btn-info info\" data-bs-toggle=\"modal\" data-bs-target=\"#modal_panel\" onclick=\"SetInfo(this)\" data-content=\"" + tmp + "\"></button>";
			list[i].insertAdjacentHTML("beforeend", elm);
			if (list0[i]) {
				tmp = "[" + list0[i].textContent + "]";
				if (info[list0[i].textContent]) {
					tmp = info[list0[i].textContent];
				}
				elm = " <button type=\"button\" class=\"btn btn-info info\" data-bs-toggle=\"modal\" data-bs-target=\"#modal_panel\" onclick=\"SetInfo(this)\" data-content=\"" + tmp + "\"></button>";
				list0[i].insertAdjacentHTML("beforeend", elm);
			}
			i++;
		}
	} else {
		setTimeout(function () { prepSubmitBtn(); }, 100);
	}
}

function checkPassword() {
	if (document.getElementById("password") && document.getElementById("password_confirm") && document.getElementById("form_main") && document.getElementById("submit_btn")) {
		let fp = document.getElementById("password");
		let lp = document.getElementById("password_confirm");
		if (fp.value !== lp.value) {
			checkPasswordValue();
			//output("Passwords do not match!");
			if (!document.getElementById("submit_btn").disabled) {
				document.getElementById("submit_btn").disabled = true;
			}
			if (!fp.classList.contains("invalid")) {
				fp.classList.add("invalid");
			}
			if (!lp.classList.contains("invalid")) {
				lp.classList.add("invalid");
			}
		} else {
			checkPasswordValue();
			if (document.getElementById("submit_btn").disabled) {
				document.getElementById("submit_btn").disabled = false;
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
	let btn = document.getElementById("submit_btn");
	let res = false;
	if (user.value.length > 0) {
		if (pass.value.length > 4) {
			//if (pass.value.match(/[A-Z]{1,}[a-z]{1,}[0-9]{1,}([_!@#\$%\^&\*\(\)\-\{\}\[\]\.\,\`\~`\n\t:"'\?\<\>\|\/\\]{1,}|[\u200b]{1,})/g)) {
			if (pass.value.match(/[A-Z]{1,}/g) && pass.value.match(/[a-z]{1,}/g) && pass.value.match(/[0-9]{1,}/g) && pass.value.match(/([_!@#\$%\^&\*\(\)\-\{\}\[\]\.\,\`\~`\n\t:"'\?\<\>\|\/\\]{1,}|[\u200b]{1,})/g)) {
				if (pass.value === passc.value) {
					if (!pass.value.match(/^(apple[s]*[_\-\s]*[0-9]*|password[s]*[_\-\s]*[0-9]*|orange[s]*[_\-\s]*[0-9]*)/i)) {
						dismiss();
						res = true;
					}
				} else {
					output("Passwords do not match.");
				}
			} else {
				output("Password must contain at least one capital letter, one lowercase letter, one number, and one special character.");
			}
		} else {
			if (pass.value.length === 0) {
				dismiss();
			} else {
				output("Password must be longer than 4 characters long.");
			}
		}
		if (!res) {
			//output("Example: Password_000");
		}
	}
	return res;
}

function submitPasswordResetApplication() {
	let user = document.getElementById("username");
	let pass = document.getElementById("password");
	let passc = document.getElementById("password_confirm");
	let btn = document.getElementById("submit_btn");
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
	let btn = document.getElementById("submit_btn");
	user.disable = true;
	pass.disable = true;
	passc.disable = true;
	btn.disable = true;
}
function enableAllFields() {
	let user = document.getElementById("username");
	let pass = document.getElementById("password");
	let passc = document.getElementById("password_confirm");
	let btn = document.getElementById("submit_btn");
	user.disable = false;
	pass.disable = false;
	passc.disable = false;
	btn.disable = false;
}

function dismiss() {
	if (document.getElementById("status")) {
		let s = document.getElementById("status");
		if (s.classList.contains("alert-danger")) {
			s.classList.remove("alert-danger");
			s.innerHTML = "";
		}
		if (!s.classList.contains("alert-dark")) {
			s.classList.add("alert-dark");
		}
	}
}

function output(q = false) {
	if (q !== false) {
		if ((typeof q) === "string") {
			if (document.getElementById("status")) {
				let s = document.getElementById("status");
				s.innerHTML = q;
				if (!s.classList.contains("alert-danger")) {
					s.classList.add("alert-danger");
				}
				if (s.classList.contains("alert-dark")) {
					s.classList.remove("alert-dark");
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
				let elm0 = document.getElementById(tar);
				if (elm0.type === "password") {
					elm0.type = "text";
					if (!elm.classList.contains("crossout")) {
						elm.classList.add("crossout");
					}
				} else {
					if (elm.classList.contains("crossout")) {
						elm.classList.remove("crossout");
					}
					elm0.type = "password";
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