
function auto_focus_elements_in_modal(modal_id, restore_scroll_id) { restore_scroll_id = restore_scroll_id === undefined ? "" : restore_scroll_id; if (typeof is_mobile != undefined && is_mobile == 1) { var modal_element = "#" + modal_id; $(modal_element).on("shown.bs.modal", function (e) { goToByScroll(modal_id, "fast") }); $(modal_element).on("hide.bs.modal", function (e) { if (restore_scroll_id) goToByScroll(restore_scroll_id, "fast") }) } }; function output(target, value) { if (target == null) return; try { var t = document.getElementsByName(target)[0]; if (t instanceof HTMLInputElement) { t.value = value; return } if (t instanceof HTMLTextAreaElement) { t.value = value; return } t.innerHTML = value } catch (e) { console.error("output", target, value, e); return null } } function get(target) { var t = document.getElementsByName(target)[0]; if (t instanceof HTMLInputElement) return t.value; if (t instanceof HTMLTextAreaElement) return t.value; return t.innerHTML }
function chunkArray(array, size) { var start = array.byteOffset || 0; array = array.buffer || array; var index = 0; var result = []; while (index + size <= array.byteLength) { result.push(new Uint8Array(array, start + index, size)); index += size } if (index <= array.byteLength) result.push(new Uint8Array(array, start + index)); return result } function newSalt() { return window.crypto.getRandomValues(new Uint8Array(16)) }
var base64url = {
    _strmap: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefg" + "hijklmnopqrstuvwxyz0123456789-_", encode: function (data) { data = new Uint8Array(data); var len = Math.ceil(data.length * 4 / 3); return chunkArray(data, 3).map(function (chunk) { return [chunk[0] >>> 2, (chunk[0] & 3) << 4 | chunk[1] >>> 4, (chunk[1] & 15) << 2 | chunk[2] >>> 6, chunk[2] & 63].map(function (v) { return base64url._strmap[v] }).join("") }).join("").slice(0, len) }, _lookup: function (s, i) { return base64url._strmap.indexOf(s.charAt(i)) }, decode: function (str) {
        var v = new Uint8Array(Math.floor(str.length *
            3 / 4)); var vi = 0; for (var si = 0; si < str.length;) { var w = base64url._lookup(str, si++); var x = base64url._lookup(str, si++); var y = base64url._lookup(str, si++); var z = base64url._lookup(str, si++); v[vi++] = w << 2 | x >>> 4; v[vi++] = x << 4 | y >>> 2; v[vi++] = y << 6 | z } return v
    }
};
function hmac(key) { this.keyPromise = webCrypto.importKey("raw", key, { name: "HMAC", hash: "SHA-256" }, true, ["sign"]) } hmac.prototype.hash = function (input) { return this.keyPromise.then(function (k) { return webCrypto.sign("HMAC", k, input) }) };
function hkdf(salt, ikm) { this.prkhPromise = (new hmac(salt)).hash(ikm).then(function (prk) { return new hmac(prk) }) } hkdf.prototype.extract = function (info, len) { var input = concatArray([info, new Uint8Array([1])]); return this.prkhPromise.then(function (prkh) { return prkh.hash(input) }).then(function (h) { if (h.byteLength < len) throw new Error("Length is too long"); return h.slice(0, len) }) };
function concatArray(arrays) { var size = arrays.reduce(function (total, a) { return total + a.byteLength }, 0); var index = 0; return arrays.reduce(function (result, a) { result.set(new Uint8Array(a), index); index += a.byteLength; return result }, new Uint8Array(size)) } function be16(val) { return (val & 255) << 8 | val >> 8 & 255 } window.base64url = base64url; $(document).ready(function () {
    var sideNav = $("#mySidenav"); var sideNavWidth = sideNav.width(); var layer = $(".layer"); navbar_and_sidenav_handler(); var openNav = function () { sideNav.addClass("open"); $("html, body").css("overflow", "hidden"); layer.addClass("visible") }; var closeNav = function () { sideNav.removeClass("open"); $("html, body").css("overflow", ""); layer.removeClass("visible") }; $("#hamburger_menu_key").on("click", openNav); layer.on("click", closeNav); $("#ham_login_button").on("click", closeNav); var xDown = null;
    var yDown = null; document.addEventListener("touchstart", handleTouchStart, false); document.addEventListener("touchend", handleTouchMove, false); function getTouches(evt) { return evt.touches || evt.originalEvent.touches } function handleTouchStart(evt) { xDown = getTouches(evt)[0].clientX; yDown = getTouches(evt)[0].clientY } function handleTouchMove(evt) {
        if (!xDown || !yDown) return; if (evt.type === "touchend") { var xUp = evt.changedTouches[0].clientX; var yUp = evt.changedTouches[0].clientY } else {
            var xUp = evt.touches[0].clientX; var yUp =
                evt.touches[0].clientY
        } var xDiff = xDown - xUp; var yDiff = yDown - yUp; if (Math.abs(xDiff) > Math.abs(yDiff)) if (xDiff > 0) { if ($(".layer").is(":visible") && xDiff >= sideNavWidth / 4) closeNav() } else if (xDown < 20 && xUp >= sideNavWidth / 4) openNav(); xDown = null; yDown = null
    }
});
function navbar_and_sidenav_handler() {
    var header = $("#header"); header.find(".profile-hover").hover(function () { }, function () { header.find(".profile-dropdown ul.dropdown-submenu").hide(); header.find(".profile-dropdown .glyphicon.pull-right").removeClass("glyphicon-menu-up").addClass("glyphicon-menu-down"); header.find(".profile-dropdown .fa.pull-right").removeClass("fa-chevron-up").addClass("fa-chevron-down"); header.find(".profile-dropdown li").removeClass("open") }); $(".dropdown-hover").hover(function () {
        if ($(this).find(".is_icon_header").length ===
            0 || $(this).find(".is_icon_header").is(":visible")) { $(".dropdown-menu", this).stop(true, true).show(); $(this).addClass("open"); $(this).addClass("active"); $(this).find(".is_icon_header").addClass("is-icon-arrow-filled-up"); $(this).find(".is_icon_header").removeClass("is-icon-arrow-filled-down"); if ($(this).is(".dropdown_backdrop")) $(".nav_dropdown_backdrop").show(); if ($(this).is(".profile_container_hover")) $(this).addClass("profile_hovered") }
    }, function () {
        if ($(this).find(".is_icon_header").length === 0 ||
            $(this).find(".is_icon_header").is(":visible")) {
            $(".dropdown-menu", this).stop(true, true).hide(); $(this).removeClass("open"); $(this).removeClass("active"); $(this).find(".is_icon_header").addClass("is-icon-arrow-filled-down"); $(this).find(".is_icon_header").removeClass("is-icon-arrow-filled-up"); if ($(this).is(".dropdown_backdrop")) $(".nav_dropdown_backdrop").hide(); if ($(this).is(".profile_container_hover")) {
                $(this).removeClass("profile_hovered"); $(this).find(".profile-dropdown ul.dropdown-submenu").hide();
                $(this).find(".profile-dropdown li").removeClass("open"); $(this).find(".is-icon-chevron-up").toggleClass("is-icon-chevron-down is-icon-chevron-up")
            }
        }
    }); $(".dropdown-click").on("click", function (e) { if (!$(this).next().hasClass("show")) $(this).parents(".dropdown-menu").first().find(".show").removeClass("show"); var $subMenu = $(this).next(".dropdown-menu"); $subMenu.toggleClass("show"); $(this).find("i:first").toggleClass(" is-icon-chevron-down  is-icon-chevron-up"); return false }); $(".ham_title").click(function () {
        var ham_menu_items =
            $(this).parent(); ham_menu_items.toggleClass("expanded"); ham_menu_items.find(".ham_submenu_items").toggle(); if ($(this).hasClass("ham_title_more") && ham_menu_items.hasClass("expanded")) { var sideNavElm = $("#mySidenav")[0]; sideNavElm.scrollTop = sideNavElm.scrollHeight } if (!ham_menu_items.hasClass("expanded")) {
                var ham_menu_items1 = $(this).parent().find(".ham_inner_menu_items"); if (ham_menu_items1.hasClass("inner_expanded")) {
                    ham_menu_items1.removeClass("inner_expanded"); ham_menu_items1.find(".ham_inner_submenu_items").hide();
                    ham_menu_items1.find(".ham_inner_title i").removeClass("is-icon-chevron-up").addClass(" is-icon-chevron-down")
                }
            }
    });
    $(".ham_title").click(function () { $(this).find("i").toggleClass("is-icon-chevron-down is-icon-chevron-up") }); $(".ham_inner_title").click(function () { var ham_menu_items = $(this).parent(); ham_menu_items.toggleClass("inner_expanded"); ham_menu_items.find(".ham_inner_submenu_items").toggle(); $(this).find("i").toggleClass("is-icon-chevron-up is-icon-chevron-down") }); header.find(".profile-dropdown li:has(ul.dropdown-submenu) a:not([href])").click(function () {
        $(this).parent().find("ul.dropdown-submenu:first").toggle();
        $(this).find(".glyphicon.pull-right:first").toggleClass("glyphicon-menu-down glyphicon-menu-up"); $(this).find(".fa.pull-right:first").toggleClass("fa-chevron-down fa-chevron-up"); $(this).closest("li").toggleClass("open"); return false
    })
};



