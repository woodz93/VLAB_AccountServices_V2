

setTimeout(function () { Initial(); }, 0);

//prepDisplay();
prepUserData();

var GV_Debug = false;

function Initial() {
	ini();
}

function ini() {
	if ((typeof Server) !== "undefined" && (typeof bootstrap)!=="undefined") {
		setTimeout(function(){setup();},0);
	} else {
		setTimeout(function () {
			ini();
		}, 50);
	}
}

function prepRegLoad() {
	if (document.getElementById("SMCElement")) {
		if (Store.Contains("SMCElement")) {
			let elm = document.getElementById("SMCElement");
			let obj = Store.Get("SMCElement");
			let t = (typeof obj);
			if (t === "object" || t === "array") {
				State.Render(elm,obj);
			}
		}
	}
}

function prepPostBack() {
	if (document.getElementById("SMCElement")) {
		let elm = document.getElementById("SMCElement");
		if (elm.hasAttribute("show")) {
			if (elm.getAttribute("show") === "true") {
				console.warn(elm);
				if (elm.classList.contains("hidden")) {
					elm.classList.remove("hidden");
				}
				State.Save(elm,"SMCElement");
			}
		}
	}
}

function prepHelpForm() {
	let list = {
		"info_fname": "input_6_5_3",
		"info_lname": "input_6_5_6",
		"info_email":"input_6_2"
	};
	let item = false;
	let value = false;
	for ([item,value] of Object.entries(list)) {
		if (document.getElementById(item)) {
			if (document.getElementById(value)) {
				document.getElementById(value).value = document.getElementById(item).value;
			} else {
				console.warn(value + " was not found!");
			}
		} else {
			console.warn(item + " was not found!");
		}
	}
}

function prepUserData() {
	let elm = document.getElementById("server_data_element");
	let q = elm.value;
	let obj = false;
	try {
		obj = JSON.parse(q);
	} catch {
		console.warn(q);
	}
	if (obj !== false) {
		//obj["Exists"] = false;
		//console.log(obj);
		if (obj["Campus"] !== "mauicc") {
			let tobj = {
				"title": "NOTICE",
				"content": "You are not authorized to manage your domain account from here!"
			};
			SetModal(tobj);
			OpenModal(document.getElementById("modal_panel"));
		}
		if (obj["Exists"] === false) {
			let t = obj["Role"];
			let r = t === "student" ? "Student" : "Staff";
			document.getElementById("form-pwd-btn").textContent = "Create "+r+" Account";
			let e = document.querySelector("div.card button[data-bs-target=\"#password_mgr_container\"]");
			if (e !== undefined) {
				setTimeout(function () { OpenSection(e); },25);
				//OpenSection(e);
			}
		}
	}
}

function OpenModal(elm) {
	if (!elm.classList.contains("show")) {
		elm.classList.add("show");
	}
	if (elm.hasAttribute("aria-hidden")) {
		elm.setAttribute("aria-hidden","false");
	}
	if (!elm.hasAttribute("aria-modal")) {
		elm.setAttribute("aria-modal","true");
	}
	if (!elm.hasAttribute("role")) {
		elm.setAttribute("role","dialog");
	}
	elm.style.display = "block";
	document.getElementsByTagName("body")[0].insertAdjacentHTML("beforeend","<div class=\"modal-backdrop fade show\"></div>");
}

function OpenSection(q) {
	//console.warn(q);
	if (q.classList.contains("collapsed")) {
		q.classList.remove("collapsed");
	}
	if (q.hasAttribute("aria-expanded")) {
		if (q.getAttribute("aria-expanded") !== "true") {
			q.setAttribute("aria-expanded","true");
		}
	} else {
		q.setAttribute("aria-expanded","true");
	}
	if (q.hasAttribute("data-bs-target")) {
		let tmp = document.querySelector(q.getAttribute("data-bs-target"));
		if (tmp) {
			if (tmp.classList.contains("collapse")) {
				tmp.classList.remove("collapse");
				tmp.classList.add("collapsing");
				setTimeout(function () {
					tmp.classList.remove("collapsing");
					tmp.classList.add("collapse");
					tmp.classList.add("show");
				},500);
			}
		}
	}
}

function setup() {
	//document.getElementById("LogoutBtn").type = "button";
	//document.getElementById("LogoutBtn").setAttribute("type","button");
	setTimeout(function () { prepHelpForm(); },100);
	prepRegLoad();
	prepPostBack();
	//PrepOutput();
	prepSubmitBtn();
	if (document.getElementById("password_confirm")) {
		document.getElementById("password").addEventListener("keyup", function (e) {
			checkPassword();
		});
		document.getElementById("password").addEventListener("keypress",function (e) {
			if (e.keyCode === 13) {
				SetProcBtn("form-pwd-btn");
			}
		});
		document.getElementById("password_confirm").addEventListener("keyup", function (e) {
			checkPassword();
		});
		document.getElementById("password_confirm").addEventListener("keypress",function (e) {
			if (e.keyCode === 13) {
				SetProcBtn("form-pwd-btn");
			}
		});
		/*
		document.getElementById("status").addEventListener("click", function (e) {
			setTimeout(function () { dismiss(); }, 0);
		});
		*/
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
		SetupWindowListeners();
		SetupPopOvers();
		AdjustCheckboxes();
		
		setTimeout(function(){LoadState();},0);
		//console.log("All elements have successfully been setup!");
	} else {
		setTimeout(function () { ini(); }, 100);
	}
}

function prepDisplay() {
	let q = document.getElementById("user_type_element").value.toLowerCase();
	let elm = document.getElementById("vdi-mgr-container");
	elm.classList.remove("s-hide");
	elm.classList.add("s-show");
	elm.disabled = false;
	/*
	//console.log(q);
	if (q === "student") {
		if (elm.classList.contains("s-hide")) {
			elm.classList.remove("s-hide");
		}
		if (!elm.classList.contains("s-show")) {
			elm.classList.add("s-show");
		}
		//elm.style.display = "none";
	} else {
		elm.disabled = true;
		elm.style.display = "none";
	}
	*/
}

function SetProcBtn(q) {
	let elm = document.getElementById(q);
	elm.disabled = true;
	if (elm.classList.contains("btn-outline-primary")) {
		elm.classList.remove("btn-outline-primary");
	}
	if (!elm.classList.contains("btn-outline-secondary")) {
		elm.classList.add("btn-outline-secondary");
	}
	elm.innerHTML = "<span class=\"spinner-grow spinner-grow-sm\"></span>" + elm.innerHTML;
}

function CheckSession() {
	if (CheckCookie("ASP.NET_SessionId")) {
		console.log("%cSession is valid!","color:rgb(0,2550);font-weight:bolder;");
	} else {
		console.error("%cSession is invalid!","color:rgb(255,0,0);font-weight:bolder;");
	}
}

function GetSessionInfo() {
	let obj = [
		"check-session"
	];
	let cmd = JSON.stringify(obj);
	let a = {
		"src": "Controllers/Access.aspx",
		"args": {
			"cmd":cmd
		}
	};
	Server.Send(a,true,"ProcSession");
}

function ProcSession(q) {
	try {
		let obj = JSON.parse(q);
	} catch {
		console.error(q);
	}
}

function CheckCookie(key) {
	let tmp = GetCookie(key);
	let t = (typeof tmp);
	let res = false;
	if (t === "string") {
		if (tmp.length > 0) {
			res = true;
		}
	}
	return res;
}



function Logout() {
	let host = window.location.host;
	let proto = window.location.protocol;
	RemoveCookie(".DotNetCasClientAuth");
	RemoveCookie("ASP.NET_SessionId");
	//let url = "https://cas-test.its.hawaii.edu/cas/logout";
	//let url = "https://authn.hawaii.edu/cas/logout?service=https://vlab.accountservices.maui.hawaii.edu";
	//let url = "https://vlab.accountservices.maui.hawaii.edu/ClientLogout.aspx";
	let url = window.location.protocol + "//" + window.location.host + "/ClientLogout.aspx";
	window.location.href = url;
	//window.open(url,"blank");
	//setTimeout(function () {
	//	window.close();
	//},1000);
}

function RemoveCookie(name) {
	if (GetCookie(name)) {
		document.cookie = name + "=;expires=0;path=/";
	}
}

function GetCookie(name) {
	return document.cookie.split(";").some(c => {
		return c.trim().startsWith(name + "=");
	});
}

function SetCookie(key,value,dur=30000) {
	let d = new Date();
	d.setTime(d.getTime() + dur);
	let exp = "expires=" + d.toUTCString();
	document.cookie = key + "=" + value + ";" + exp + ";path=/";
}

function PrepOutput() {
	if (document.getElementById("SMCElement")) {
		let elm = document.getElementById("SMCElement");
		if (elm.parentElement.classList.contains("aspNetHidden")) {
			State.Render(elm,Store.Get("responseMessage"));
		} else {
			State.Save(elm,Store.Get("responseMessage"));
		}
	}
}
function AdjustCheckboxes() {
	let list=document.querySelectorAll("input[type=\"checkbox\"]");
	let i=0;
	let elm=false;
	let par=false;
	let tmp=false;
	while(i<list.length){
		list[i].classList.add("form-check-input");
		par=list[i].parentElement;
		elm=list[i];
		tmp=document.createElement("div");
		tmp.classList.add("form-check");
		tmp.classList.add("form-switch");

		par.removeChild(list[i]);
		tmp.appendChild(elm);
		par.insertAdjacentElement("afterbegin",tmp);
		i++;
	}
	if (document.getElementById("modal_support_panel")) {
		elm=document.getElementById("modal_support_panel");
		par=elm.parentElement;
		par.removeChild(elm);
		document.getElementsByTagName("body")[0].appendChild(elm);
	}
	if (document.getElementById("input_6_2")) {
		document.getElementById("input_6_2").value=document.getElementById("username").value+"@hawaii.edu";
	}
}
function SetupPopOvers() {
	let list = document.querySelectorAll('*[data-bs-toggle="popover"]');
	let i = 0;
	let p = false;
	while (i < list.length) {
		p = false;
		if (list[i]) {
			if (!list[i].hasAttribute("title")) {
				p = true;
			} else if (!list[i].getAttribute("title")) {
				p = true;
			}
		}
		if (list[i].parentElement.getElementsByClassName("aspNetDisabled").length>0) {
			p=false;
		}
		if (p) {
			list[i].setAttribute("title", "Information");
			list[i].title = "Information";
		}
		i++;
	}
	var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
	var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
		return new bootstrap.Popover(popoverTriggerEl);
	});
}
function SetupWindowListeners() {
	document.querySelector("form#form_main").addEventListener("submit",function(event){
		if (GV_Debug) {
			event.preventDefault();
		}
		SaveState();
	});
	window.addEventListener("beforeunload",function(event){
		SaveState();
	});
	window.addEventListener("click",function(event){
		WinEvt(event);
	});
	let list=document.querySelectorAll("[data-bs-toggle=\"popover\"]");
	let i=0;
	while(i<list.length){
		//list[i].setAttribute("data-bs-content",Convert(list[i].getAttribute("data-bs-content")));
		list[i].addEventListener("click",function(event){
			setTimeout(function(){
				let elm=document.querySelector("#"+event.srcElement.getAttribute("aria-describedby"));
				if (elm) {
					//if (!(elm.parentElement.getElementsByClassName("aspNetDisabled").length>0)) {
						//console.log(elm.parentElement.getElementsByClassName("aspNetDisabled"));
						elm=elm.querySelector(".popover-body");
						elm.innerHTML=Convert(elm.textContent);
					//}
				}
			},10);
			WinEvt(event);
		});
		i++;
	}
}
function WinEvt(event = false) {
	if (event.srcElement) {
		let tar = document.querySelector("div.popover.fade.show.bs-popover-end[role=\"tooltip\"]") || false;
		//console.log(document.querySelector("[data-bs-toggle=\"popover\"][aria-describeby]"));
		//console.log(event);
		let id="";
		if (event.srcElement.hasAttribute("aria-describedby")) {
			id=event.srcElement.getAttribute("aria-describedby");
		}
		if (event.srcElement !== tar && event.srcElement!==document.querySelector("[data-bs-toggle=\"popover\"][aria-describedby=\""+id+"\"]")) {
			ClosePopover(id);
		}
	}
}
function ClosePopover(exc=false) {
	let e=false;
	let list = document.querySelectorAll("div.popover.fade.show.bs-popover-end[role=\"tooltip\"]");
	let i = 0;
	let par = false;
	let id="";
	let eid=false;
	if (exc!==false) {
		if (exc.length!==undefined) {
			if (exc.length>0) {
				eid=exc;
				e=document.querySelector("#"+eid);
			}
		}
	}
	while (i < list.length) {
		if (list[i]) {
			//par = list[i].parentElement;
			//par.removeChild(list[i]);
			
			id="";
			if (list[i].id) {
				id=list[i].id;
			}
			if (id!==e.id) {
				document.querySelector("[data-bs-toggle=\"popover\"][aria-describedby=\""+id+"\"]").click();
			}
		}
		i++;
	}
	/*
	if (list.length>0) {
		let elm = document.querySelector("[data-bs-toggle=\"popover\"][aria-describedby]");
		if (elm) {
			elm.click();
		}
	}
	*/
}
function SetInfo(q = false) {
	/*
	let obj = {
		"title": "Information",
		"content": q.getAttribute("data-bs-content")
	};
	SetModal(obj);
	*/
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
		let p = true;
		let t = (typeof q);
		if (t === "object") {
			t = (typeof q.id);
			if (t === "string") {
				if (q.id === "form-pwd-btn") {
					if (!checkPasswordValue()) {
						p = false;
					}
				} else if (q.id === "form-vd-btn") {
					let tmp = document.querySelectorAll("table#GroupsElement input[type=\"checkbox\"]:checked");
					let y = (typeof tmp);
					if (y !== "undefined") {
						if (!(tmp.length > 0)) {
							p = false;
						}
					} else {
						p = false;
					}
				}
			}
		}


		if (!elm.disabled && p) {
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

		

		document.getElementById("form-vd-btn").addEventListener("click",function (e) {
			let tmp = document.querySelectorAll("table#GroupsElement input[type=\"checkbox\"]:checked");
			let t = (typeof tmp);
			let p = false;
			if (t !== "undefined") {
				t = (typeof tmp.length);
				if (t !== "undefined") {
					if (!(tmp.length > 0)) {
						p = true;
					}
				} else {
					p = true;
				}
			} else {
				p = true;
			}
			if (p) {
				let elm = document.getElementById("form-vd-btn");
				elm.disabled = true;
				if (!elm.classList.contains("btn-outline-danger")) {
					elm.classList.add("btn-outline-danger");
				}
				elm.innerHTML = "[ERROR]";
				setTimeout(function () {
					elm.disabled = false;
					if (elm.classList.contains("btn-outline-danger")) {
						elm.classList.remove("btn-outline-danger");
					}
					elm.innerHTML = "Add";
				},5000);
			}
		});

		document.getElementById("form-pwd-btn").addEventListener("click",function (e) {
			let p0 = document.getElementById("password").value;
			let p1 = document.getElementById("password_confirm").value;
			let p = false;
			if (!checkPasswordValue()) {
				p = true;
			}
			if (p) {
				e.preventDefault();
				//document.getElementById("form_main").disabled = true;
				let elm = document.getElementById("form-pwd-btn");
				if (!elm.classList.contains("btn-outline-danger")) {
					elm.classList.add("btn-outline-danger");
					elm.innerHTML = "[ERROR]";
					elm.disabled = true;
					setTimeout(function () {
						elm.disabled = false;
						if (elm.classList.contains("btn-outline-danger")) {
							elm.classList.remove("btn-outline-danger");
						}
						elm.innerHTML = "Save Password";
					},5000);
				}
				output("Passwords are invalid or do not match!");
			}
		});

		let l=document.querySelectorAll("button[data-type=\"submit\"]");
		let i=0;
		while(i<l.length){
			let e = l[i];
			e.addEventListener("click",function () {
				setTimeout(function () {
					if (!e.disabled) {
						if (e.classList.contains("btn-outline-primary")) {
							e.classList.remove("btn-outline-primary");
						}
						if (e.querySelector("span")) {
							e.removeChild(e.querySelector("span"));
						}
						if (!e.classList.contains("btn-outline-danger")) {
							e.classList.add("btn-outline-danger");
						}
						e.innerHTML = "[ERROR]";
						OpenNote("Failed to process your request!");
					}
				},4000);
			});
			i++;
		}
		

		/*
		document.getElementById("form_main").addEventListener("submit", function () {
			document.getElementById("submit_btn").disabled = true;
		});
		document.getElementById("submit_btn").addEventListener("click", function () {
			document.getElementById("submit_btn").disabled = true;
		});
		*/
		let list = document.querySelectorAll("table#GroupsElement td");
		let list0 = document.querySelectorAll("table#UserGroupsElement td");
		i = 0;
		//let tmp = "<button type=\"button\" class=\"btn btn - info info\" data-bs-toggle=\"modal\" data-bs-target=\"#modal_panel\" onclick=\"SetInfo(this)\" data-content=\""+info[i]+"\"></button>";
		let elm = "";
		let info = {
			"Adobe": "The Adobe CC Virtual Lab 20 Windows 10 Desktops featuring Adobe Creative Cloud and Office 2019 Professional Plus and other productivity software. Adobe Areo, After Effects, Animate, Audition, InCopy, InDesign, Lightroom, Media Encoder, Dimension, Photoshop, Premier Pro, Premier Rush, Substance 3D Designer, Substance 3D Modeler, Substance 3D Painter, Substance 3D Sampler, Substance 3D Stager, UXP Developer Tool, Dreamweaver, Illustrator, Word 2019, Excel 2019, Power Point 2019, Access 2019, Publisher 2019, Outlook 2019, Visual Studio Code, Google Chrome, Firefox,  VLC Player, Alice 3, Adobe Acrobat DC, Java JDK, and Zoom.",
			"Business Virtual Lab": "The Business Lab office 60 Windows 10 Desktops featuring Office 2021 Professional Plus and other productivity software. Word 2021, Excel 2021, Power Point 2021, Access 2021, Publisher 2021, Outlook 20021, Visual Studio Code, Google Chrome, Firefox, Apache Netbeans, VLC Player, Alice 3, Adobe Acrobat DC, Java JDK, and Zoom.",
			"Business Virtual Lab 2": "The Business Lab 2 office 60 Windows 10 Desktops featuring Office 2019 Professional Plus and other productivity software. Word 2019, Excel 2019, Power Point 2019, Access 2019, Publisher 2019, Outlook 2019, Visual Studio Code, Google Chrome, Firefox, Apache Netbeans, VLC Player, Alice 3, Adobe Acrobat DC, Java JDK, and Zoom.",
			"Math Virtual Lab": "The Math Virtual Lab offers 50 Windows 10 Desktops Featuring Python and other development tools: Python, Visual Studio code, Wireshark, VLC Player, Firefox, Google Chrome, and Office 2019."
		};
		let tmp = "";
		while (i < list.length) {
			let attr = "";
			if (list[i].textContent.toLowerCase() === "business virtual lab")
				attr = "checked";
			tmp = "[" + list[i].textContent + "]";
			if (info[list[i].textContent]) {
				tmp = info[list[i].textContent];
			}
			//console.log(list[i]);
			if (!(list[i].getElementsByClassName("aspNetDisabled").length>0)) {
				elm = " <button type=\"button\" class=\"btn btn-info info t-500\" data-bs-toggle=\"popover\" data-bs-target=\"#modal_panel\" onclick=\"SetInfo(this)\" data-bs-content=\"" + tmp + "\"></button>";
				list[i].insertAdjacentHTML("beforeend", elm);
			}
			if (list0[i]) {
				tmp = "[" + list0[i].textContent + "]";
				if (info[list0[i].textContent]) {
					tmp = info[list0[i].textContent];
				}
				elm = " <button type=\"button\" class=\"btn btn-info info t-500\" data-bs-toggle=\"popover\" data-bs-target=\"#modal_panel\" onclick=\"SetInfo(this)\" data-bs-content=\"" + tmp + "\"></button>";
				list0[i].insertAdjacentHTML("beforeend", elm);
			}
			if (attr !== "") {
				list[i].querySelector("input[type=\"checkbox\"]").setAttribute("checked","true");
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
				if (pass.value.length === passc.value.length) {
					if (pass.value === passc.value) {
						if (!pass.value.match(/^(apple[s]*[_\-\s]*[0-9]*|password[s]*[_\-\s]*[0-9]*|orange[s]*[_\-\s]*[0-9]*)\z/i)) {
							dismiss();
							res = true;
							//console.log(pass.value);
							//console.log(passc.value);
						} else {
							output("Password is too simple.");
							//console.log(pass.value);
							//console.log(passc.value);
						}
					} else {
						output("Passwords do not match.");
						//console.log(pass.value);
						//console.log(passc.value);
					}
				} else {
					output("Passwords do not match length.");
					//console.log(pass.value);
					//console.log(passc.value);
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
	//console.log(q);
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
function DisableHelpForm() {
	let elm=document.getElementById("submit_ticket_btn");
	elm.disabled=true;
	elm.insertAdjacentHTML("afterbegin","<span class=\"spinner-border spinner-border-sm\"></span>");
}
var GV_HelpPass=false;
var GV_OVR=true;
function SubmitHelpRequest() {

	DisableHelpForm();

	let form={
		//"elm":document.getElementById("gform_6"),
		"fname":document.getElementById("input_6_5_3").value,
		"lname":document.getElementById("input_6_5_6").value,
		"email":document.getElementById("input_6_2").value,
		"department":document.getElementById("input_6_7").value,
		"desc":document.getElementById("input_6_8").value,
		"message":document.getElementById("input_6_3").value
	};
	/*
	form["name"]=form.fname+" "+form.lname;
	form["alert"]=true;
	form["source"]="API";
	form["autorespond"]=true;
	*/
	let a={
		"src":"./resetPassword.aspx",
		"args":form
	};
	Server.send(a,true,"HelpResponse");
	GV_HelpPass=true;
	setTimeout(function(){CheckHelp();},5000);
	try{
		if (GV_OVR) {
			Server.send(a,true,"HelpResponse");
		}
	}catch{
		HelpError();
	}
}
function HelpResponse(q=false) {
	//console.log(q);
	GV_HelpPass=false;
}
function CheckHelp() {
	//console.log(GV_HelpPass);
	if (GV_HelpPass) {
		HelpError();
	}
}
// This is a testing message from the UHMC account services website.
function HelpError() {
	let elm=document.getElementById("submit_ticket_btn");
	if (elm.classList.contains("btn-outline-primary")) {
		elm.classList.remove("btn-outline-primary");
	}
	if (elm.querySelector("span")) {
		elm.removeChild(elm.querySelector("span"));
	}
	if (!elm.classList.contains("btn-outline-danger")) {
		elm.classList.add("btn-outline-danger");
	}
	elm.innerHTML="[ERROR]";
	OpenNote("Unable to submit help ticket!");
	GV_OVR=false;
}
function OpenNote(q=false) {
	if (q!==false) {
		let elm=document.getElementById("note");
		if (elm) {
			if (elm.classList.contains("hidden")) {
				elm.innerHTML=q;
				elm.classList.remove("hidden");
			}
		}
	}
}
function CloseNote() {
	let elm=document.getElementById("note");
	if (elm) {
		if (!elm.classList.contains("hidden")) {
			elm.classList.add("hidden");
		}
	}
}
var GV_SaveElms=[
	document.querySelector("[data-bs-target=\"#password_mgr_container\"]"),
	document.getElementById("password_mgr_container"),
	document.querySelector("button[data-bs-toggle=\"collapse\"][data-bs-target=\"#vdi_container\"]"),
	document.getElementById("vdi_container"),
	document.getElementById("input_6_3"),
	document.getElementById("input_6_5_3"),
	document.getElementById("input_6_5_6")
	//document.getElementById("status")
];
function SaveState() {
	let list=GV_SaveElms;
	//console.log(list);
	let i=0;
	let obj={};
	let tag="";
	while(i<list.length){
		if (list[i]) {
			tag=list[i].tagName.toLowerCase();
			if (tag==="textarea" || tag==="input") {
				obj[i]=list[i].value;
			} else if (tag==="span" && list[i].id==="status") {
				obj[i]=list[i].innerHTML;
			} else {
				obj[i]=list[i].classList;
			}

		}
		i++;
	}
	let str=JSON.stringify(obj);
	//console.log(str);
	localStorage.setItem("vdi-web-state-for-"+GetUsername(),str);
}
function LoadState() {
	let list=GV_SaveElms;
	let i=0;
	if (localStorage.getItem("vdi-web-state-for-"+GetUsername())) {
		let obj=false;
		try{
			obj=JSON.parse(localStorage.getItem("vdi-web-state-for-"+GetUsername()));
		}catch{
			SaveState();
		}
		//console.log(obj);
		if (obj!==false) {
			let item=false;
			let value=false;
			let tag="";
			for([item,value] of Object.entries(obj)){
				tag=list[item].tagName.toLowerCase();
				if (list[item]) {
					if (tag==="textarea" || tag==="input") {
						list[item].value=value;
					} else if (tag==="span" && list[item].id==="status") {
						if (value.length>0) {
							if (!(list[item].textContent.length>0)) {
								list[item].innerHTML=value;
							}
						}
					} else {
						list[item].setAttribute("aria-expanded","false");
						LoadClassList(list[item],value);
					}
				}
			}
		}
	}
}

function GetUsername() {
	return document.getElementById("username").value;
}

function LoadClassList(elm,items) {
	let item=false;
	let value=false;
	if (elm.tagName!=="TEXTAREA") {
		elm.class="";
		let i=0;
		while(i<elm.classList.length){
			elm.classList.remove(elm.classList[i]);
			i++;
		}
	}

	for([item,value] of Object.entries(items)){
		
		if (!elm.classList.contains(value)) {
			if (value==="show") {
				if (elm.classList.contains("collapse")) {
					elm.classList.remove("collapse");
				}
				elm.classList.add("collapsing");
				elm.style="height:406px;";
				//elm.setAttribute("aria-expanded","true");
				let tmp=elm;
				setTimeout(function(){
					tmp.classList.remove("collapsing");
					tmp.classList.add("collapse");
					tmp.classList.add("show");
					tmp.style="";
				},0);
			} else {
				if (value==="collapsed") {
					if (elm.tagName==="button") {
						elm.setAttribute("aria-expanded","true");
					}
				}
				elm.classList.add(value);
			}
		}
		
	}
}

function ProcInfo() {
	let list=document.querySelectorAll("[id~=\"info_\"]");
	let i=0;
	let name="";
	let elms={
		"fname":document.getElementById("input_6_5_3"),
		"lname":document.getElementById("input_6_5_6")
	};
	while(i<list.length){
		name=list[i].replace(/(info_)/,"");
		if (elms[name]) {
			elms[name].value=list[i].value;
		}
		i++;
	}
}

