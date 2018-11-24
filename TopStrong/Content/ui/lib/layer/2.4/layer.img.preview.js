//处理BUG
function imgPreview(imgDomId) {
	var _imgDom = document.getElementById(imgDomId);
	if (!(_imgDom["src"].length > 0)) {
		return;
	}
	var imgJson = {
		"title": "", "id": "", "start": "",
		"data": [{ "alt": _imgDom["alt"], "pid": "", "src": _imgDom["src"], "thumb": "" }
		]
	};

	layer.photos({
		photos: imgJson,
		closeBtn: 1
	});
}