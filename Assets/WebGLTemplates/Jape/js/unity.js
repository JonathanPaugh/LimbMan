'use strict';

if (exports.supported({failIfMajorPerformanceCaveat: true})) {
	launchGame();
} else {
	hardwareError();
}

function launchGame() {
	let element = document.createElement("canvas");
	element.id = "game";
	document.body.prepend(element);
	
	updateGameHeight();

	let loader = document.createElement("img");
	loader.id = "loader";
	document.body.prepend(loader);

	createUnityInstance(document.querySelector("#game"), {
		dataUrl: "Build/{{{ DATA_FILENAME }}}",
		frameworkUrl: "Build/{{{ FRAMEWORK_FILENAME }}}",
		codeUrl: "Build/{{{ CODE_FILENAME }}}",

		#if MEMORY_FILENAME
		memoryUrl: "Build/{{{ MEMORY_FILENAME }}}",
		#endif

		#if SYMBOLS_FILENAME
		symbolsUrl: "Build/{{{ SYMBOLS_FILENAME }}}",
		#endif

		streamingAssetsUrl: "StreamingAssets",
		companyName: "{{{ COMPANY_NAME }}}",
		productName: "{{{ PRODUCT_NAME }}}",
		productVersion: "{{{ PRODUCT_VERSION }}}",
	}).then(instance => {
		loader.remove();
		onLaunch();
	});
}

function onLaunch() {
	window.addEventListener('resize', () => {
		updateGameHeight();
	});
}

function updateGameHeight() {
	document.querySelector("#game").style.height = `${window.innerHeight}px`;
}

function hardwareError() {
	let element = document.createElement("div");
	element.id = "info";
	element.innerHTML = 
		"⚠️ <b>Hardware Acceleration Required</b> ⚠️"
		+ "<br>"
		+ "<br>"
		+ "Turn on hardware acceleration in your browser settings"
		+ "<br>"
		+ "<br>"
		+ "Chrome:"
		+ "<br>"
		+ "Settings -> Advanced -> System -> \"Use hardware acceleration when available\" -> Relaunch"

	document.body.prepend(element);
}