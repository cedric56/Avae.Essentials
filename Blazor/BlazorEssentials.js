// Browser Essentials interop module.
// Loaded by BrowserEssentials.InitializeAsync() via JSHost.ImportAsync and
// bound from C# with [JSImport]. All functions are plain ES module exports
// with no dependencies.

// ---------- Media Picker -----------

const IS_MOBILE = /Android|iPhone/i.test(navigator.userAgent);
const VIDEO_SIZE = IS_MOBILE ? { width: 320, height: 240 } : { width: 640, height: 480 };

const AUDIO_CONSTRAINTS = {
	echoCancellation: true,
	noiseSuppression: true,
	autoGainControl: true
};

const VIDEO_MIME_CANDIDATES = [
	"video/webm;codecs=vp8,opus",
	"video/webm",
	"video/mp4"
];

const OVERLAY_STYLE = {
	position: "fixed",
	top: "0",
	left: "0",
	width: "100%",
	height: "100%",
	background: "rgba(0,0,0,0.5)",
	zIndex: "9999",
	display: "flex",
	alignItems: "center",
	justifyContent: "center"
};

const MODAL_STYLE = {
	background: "#fff",
	padding: "10px",
	borderRadius: "8px",
	width: "90%",
	maxWidth: "480px",
	textAlign: "center"
};

/* ---------- helpers ---------- */

function stopStream(stream) {
	stream?.getTracks().forEach(track => track.stop());
}

function show(el, display = "block") {
	el.style.display = display;
}

function hide(el) {
	el.style.display = "none";
}

function pickVideoMimeType() {
	return VIDEO_MIME_CANDIDATES.find(type => MediaRecorder.isTypeSupported(type)) ?? "";
}

/**
 * Builds the overlay + modal, mounts it, and wires up the shared
 * "click outside / Escape to dismiss" behaviour.
 * Returns the modal element, a `close(result)` function and the promise.
 */
function openPopup(innerHtml, { onClose } = {}) {
	const overlay = document.createElement("div");
	Object.assign(overlay.style, OVERLAY_STYLE);

	const modal = document.createElement("div");
	Object.assign(modal.style, MODAL_STYLE);
	modal.innerHTML = innerHtml;

	overlay.appendChild(modal);
	document.body.appendChild(overlay);

	let resolvePromise;
	const promise = new Promise(resolve => (resolvePromise = resolve));
	let closed = false;

	const onKeyDown = e => { if (e.key === "Escape") close(null); };
	const onBeforeUnload = () => close(null);

	function close(result) {
		if (closed) return;
		closed = true;

		onClose?.();
		document.removeEventListener("keydown", onKeyDown);
		window.removeEventListener("beforeunload", onBeforeUnload);
		overlay.remove();
		resolvePromise(result);
	}

	overlay.addEventListener("click", e => { if (e.target === overlay) close(null); });
	document.addEventListener("keydown", onKeyDown);
	window.addEventListener("beforeunload", onBeforeUnload);

	const $ = selector => modal.querySelector(selector);
	return { $, close, promise };
}

/** Requests the camera; returns the stream or null if denied/unavailable. */
async function openCamera({ audio = false } = {}) {
	try {
		return await navigator.mediaDevices.getUserMedia({
			video: VIDEO_SIZE,
			...(audio ? { audio: AUDIO_CONSTRAINTS } : {})
		});
	} catch (err) {
		console.error("Camera access denied:", err);
		return null;
	}
}

/* ---------- photo ---------- */

export function capturePhotoInPopup() {
	let stream = null;

	const { $, close, promise } = openPopup(`
        <canvas id="photoCanvas" style="display:none;"></canvas>
        <div id="actionButtons" style="display:none; justify-content:center;">
            <button id="acceptPhoto">✅ Accept</button>
            <button id="rejectPhoto">❌ Reject</button>
        </div>
        <video id="videoPreview" autoplay playsinline style="width:100%; height:auto; border:1px solid #ccc;"></video>
        <button id="takePhoto">📸 Take Photo</button>
    `, { onClose: () => stopStream(stream) });

	const video = $("#videoPreview");
	const canvas = $("#photoCanvas");
	const actions = $("#actionButtons");
	const takeButton = $("#takePhoto");

	function showPreview() {
		hide(canvas);
		hide(actions);
		show(video);
		show(takeButton);
	}

	function showCaptured() {
		hide(video);
		hide(takeButton);
		show(canvas);
		show(actions, "flex");
	}

	function takePhoto() {
		if (!stream) return;

		canvas.width = video.videoWidth;
		canvas.height = video.videoHeight;
		canvas.style.width = "100%";
		canvas.style.height = "auto";
		canvas.getContext("2d").drawImage(video, 0, 0, canvas.width, canvas.height);

		showCaptured();
	}

	function acceptPhoto() {
		canvas.toBlob(blob => close(blob ? URL.createObjectURL(blob) : null), "image/png");
	}

	takeButton.addEventListener("click", takePhoto);
	$("#acceptPhoto").addEventListener("click", acceptPhoto);
	$("#rejectPhoto").addEventListener("click", showPreview);

	openCamera().then(s => {
		if (!s) return close(null);
		stream = s;
		video.srcObject = stream;
	});

	return promise;
}

/* ---------- video ---------- */

export function captureVideoInPopup() {
	let stream = null;
	let recorder = null;
	let chunks = [];
	let mimeType = "";

	const { $, close, promise } = openPopup(`
        <video id="videoPreview" autoplay playsinline muted style="width:100%;"></video>
        <video id="playbackPreview" style="display:none;width:100%;"></video>
        <div id="recordingControls" style="margin-top:10px;">
            <button id="startRecording">🎥 Start Recording</button>
            <button id="stopRecording" disabled>⏹ Stop</button>
        </div>
        <div id="actionButtons" style="display:none; justify-content:center; margin-top:10px;">
            <button id="acceptRecording">✅ Accept</button>
            <button id="rejectRecording">❌ Reject</button>
        </div>
    `, { onClose: () => stopStream(stream) });

	const preview = $("#videoPreview");
	const playback = $("#playbackPreview");
	const controls = $("#recordingControls");
	const actions = $("#actionButtons");
	const startButton = $("#startRecording");
	const stopButton = $("#stopRecording");

	const buildBlob = () => new Blob(chunks, { type: mimeType || "video/webm" });

	async function startCamera() {
		stream = await openCamera({ audio: true });
		if (!stream) return close(null);
		preview.srcObject = stream;
	}

	function startCapture() {
		if (!stream) return;

		startButton.disabled = true;
		stopButton.disabled = false;
		hide(actions);

		chunks = [];
		mimeType = pickVideoMimeType();
		recorder = new MediaRecorder(stream, mimeType ? { mimeType } : undefined);
		recorder.ondataavailable = e => { if (e.data.size > 0) chunks.push(e.data); };
		recorder.onstop = showPlayback;
		recorder.start();
	}

	function stopCapture() {
		if (recorder?.state !== "inactive") recorder?.stop();
		stopStream(stream);
		stopButton.disabled = true;
		hide(controls);
	}

	function showPlayback() {
		playback.src = URL.createObjectURL(buildBlob());
		playback.controls = true;
		playback.muted = false;
		playback.autoplay = false;

		hide(preview);
		show(playback);
		show(actions, "flex");
	}

	function acceptCapture() {
		close(URL.createObjectURL(buildBlob()));
	}

	function rejectCapture() {
		if (playback.src) URL.revokeObjectURL(playback.src);
		playback.removeAttribute("src");
		playback.muted = true;
		chunks = [];

		hide(playback);
		hide(actions);
		show(preview);
		show(controls, "flex");
		startButton.disabled = false;

		startCamera(); // stream was stopped when recording ended
	}

	startButton.addEventListener("click", startCapture);
	stopButton.addEventListener("click", stopCapture);
	$("#acceptRecording").addEventListener("click", acceptCapture);
	$("#rejectRecording").addEventListener("click", rejectCapture);

	startCamera();
	return promise;
}

/* ---------- .NET interop ---------- */

export async function sendBlobToDotNet(blobUrl, dotNetRef) {
	const response = await fetch(blobUrl);
	const buffer = await (await response.blob()).arrayBuffer();
	dotNetRef.ReceiveBlobData(Array.from(new Uint8Array(buffer)));
}

export const mediaCapture = {
	sendBlobToDotNet,
	capturePhotoInPopup,
	captureVideoInPopup
};

// ---------- Contacts -----------

export function getAllContactsAsync(multiple) {
	return new Promise(async (resolve, reject) => {
		if ('ContactsManager' in window) {

			const opts = { multiple: multiple };
			const contacts = await navigator.contacts.select(["name", "email", "tel", "address"], opts);
			const contactsJson = contacts.map(voice => ({
				name: voice.name,
				email: voice.email,
				tel: voice.tel,
				address: voice.address,
			}));
			resolve(JSON.stringify(contactsJson, null, 2));
			return;
		}

		resolve('');

	});
}

// ---------- Preferences (localStorage) ----------

export function prefsGetAll(prefix) {
	const result = {};
	for (let i = 0; i < localStorage.length; i++) {
		const k = localStorage.key(i);
		if (k && k.startsWith(prefix)) result[k] = localStorage.getItem(k);
	}
	return result;
}

export function prefsSet(key, value) { localStorage.setItem(key, value); }
export function prefsRemove(key) { localStorage.removeItem(key); }


function bytesToBase64(bytes) {
	let binary = '';
	const chunk = 0x8000;
	for (let i = 0; i < bytes.length; i += chunk)
		binary += String.fromCharCode.apply(null, bytes.subarray(i, i + chunk));
	return btoa(binary);
}

function base64ToBytes(base64) {
	const binary = atob(base64);
	const bytes = new Uint8Array(binary.length);
	for (let i = 0; i < binary.length; i++)
		bytes[i] = binary.charCodeAt(i);
	return bytes;
}

// ---------- Secure storage (AES-GCM via WebCrypto, key in IndexedDB) ----------

const SS_DB = "maui-securestorage";
const SS_STORE = "keys";
const SS_KEY_ID = "aes-key";

function ssOpenDb() {
	return new Promise((resolve, reject) => {
		const req = indexedDB.open(SS_DB, 1);
		req.onupgradeneeded = () => req.result.createObjectStore(SS_STORE);
		req.onsuccess = () => resolve(req.result);
		req.onerror = () => reject(req.error);
	});
}

async function ssGetOrCreateKey() {
	const db = await ssOpenDb();
	const existing = await new Promise((res, rej) => {
		const tx = db.transaction(SS_STORE, "readonly");
		const req = tx.objectStore(SS_STORE).get(SS_KEY_ID);
		req.onsuccess = () => res(req.result);
		req.onerror = () => rej(req.error);
	});
	if (existing) return existing;

	const key = await crypto.subtle.generateKey(
		{ name: "AES-GCM", length: 256 }, false, ["encrypt", "decrypt"]);

	await new Promise((res, rej) => {
		const tx = db.transaction(SS_STORE, "readwrite");
		tx.objectStore(SS_STORE).put(key, SS_KEY_ID);
		tx.oncomplete = () => res();
		tx.onerror = () => rej(tx.error);
	});
	return key;
}

export function secureKeys(prefix) {
	const keys = [];
	for (let i = 0; i < localStorage.length; i++) {
		const k = localStorage.key(i);
		if (k && k.startsWith(prefix)) keys.push(k);
	}
	return keys;
}

export async function secureGet(storageKey) {
	const raw = localStorage.getItem(storageKey);
	if (!raw) return null;

	try {
		const key = await ssGetOrCreateKey();
		const { iv, data } = JSON.parse(raw);
		const plainBuf = await crypto.subtle.decrypt(
			{ name: "AES-GCM", iv: new Uint8Array(iv) },
			key,
			new Uint8Array(data));
		return new TextDecoder().decode(plainBuf);
	} catch {
		return null; // key regenerated or corrupted value
	}
}

export async function secureSet(storageKey, value) {
	const key = await ssGetOrCreateKey();
	const iv = crypto.getRandomValues(new Uint8Array(12));
	const cipherBuf = await crypto.subtle.encrypt(
		{ name: "AES-GCM", iv },
		key,
		new TextEncoder().encode(value));

	localStorage.setItem(storageKey, JSON.stringify({
		iv: Array.from(iv),
		data: Array.from(new Uint8Array(cipherBuf))
	}));
}

export function secureRemove(storageKey) {
	localStorage.removeItem(storageKey);
}

// ---------- Clipboard ----------

export function clipboardWriteText(text) {
	return globalThis.navigator.clipboard.writeText(text);
}

export function clipboardReadText() {
	return globalThis.navigator.clipboard.readText();
}

// ---------- Device info ----------

export function getDeviceInfo() {
	const nav = globalThis.navigator;
	const uaData = nav.userAgentData;
	return {
		userAgent: nav.userAgent || '',
		vendor: nav.vendor || '',
		language: nav.language || '',
		platform: (uaData && uaData.platform) || nav.platform || '',
		mobile: uaData ? !!uaData.mobile : /Mobi|Android|iPhone|iPad/i.test(nav.userAgent || ''),
		brands: (uaData && uaData.brands) ? uaData.brands.map(b => ({ brand: b.brand, version: b.version })) : []
	};
}
// ---------- Display ----------
function ddGetInfo() {
	const o = screen.orientation;
	return {
		width: window.innerWidth,
		height: window.innerHeight,
		pixelRatio: window.devicePixelRatio || 1,
		orientationType: o?.type ?? ""
	};
}

let ddWakeLock = null;
let ddWakeLockRequested = false;

async function ddAcquireWakeLock() {
	if (!("wakeLock" in navigator)) return false;

	try {
		ddWakeLock = await navigator.wakeLock.request("screen");
		ddWakeLock.addEventListener("release", () => {
			ddWakeLock = null;
		});
		return true;
	} catch {
		return false;
	}
}

function ddReleaseWakeLock() {
	ddWakeLock?.release();
	ddWakeLock = null;
}

export function ddGetSnapshot() {
	return {
		...ddGetInfo(),
		wakeLockActive: ddWakeLock !== null
	};
}

export async function ddSetWakeLock(dotNetRef, on) {
	ddWakeLockRequested = on;

	if (on) {
		const ok = await ddAcquireWakeLock();
		await dotNetRef.invokeMethodAsync("OnWakeLockChanged", ok);
	} else {
		ddReleaseWakeLock();
		await dotNetRef.invokeMethodAsync("OnWakeLockChanged", false);
	}
}

// Keep the exact handlers so they can be removed later.
let ddSubscription = null;

export function ddSubscribe(dotNetRef) {
	// Prevent duplicate subscriptions.
	ddUnsubscribe();

	const notify = () => {
		dotNetRef
			.invokeMethodAsync("OnDisplayChanged", ddGetInfo())
			.catch(() => { });
	};

	const visibilityChanged = async () => {
		// Wake locks are released by the browser when the page is hidden;
		// re-request when the page becomes visible again.
		if (
			document.visibilityState === "visible" &&
			ddWakeLockRequested &&
			!ddWakeLock
		) {
			const ok = await ddAcquireWakeLock();

			try {
				await dotNetRef.invokeMethodAsync("OnWakeLockChanged", ok);
			} catch {
				// The .NET object may have been disposed while the request
				// was in progress.
			}
		}
	};

	window.addEventListener("resize", notify);
	screen.orientation?.addEventListener("change", notify);
	document.addEventListener("visibilitychange", visibilityChanged);

	ddSubscription = {
		notify,
		visibilityChanged
	};
}

export function ddUnsubscribe() {
	if (ddSubscription) {
		window.removeEventListener("resize", ddSubscription.notify);
		screen.orientation?.removeEventListener(
			"change",
			ddSubscription.notify
		);
		document.removeEventListener(
			"visibilitychange",
			ddSubscription.visibilityChanged
		);

		ddSubscription = null;
	}

	ddWakeLockRequested = false;
	ddReleaseWakeLock();
}
// ---------- Geolocation ----------

function positionToPayload(position) {
	const c = position.coords;

	return {
		latitude: c.latitude,
		longitude: c.longitude,
		accuracy: c.accuracy,
		altitude: c.altitude,
		altitudeAccuracy: c.altitudeAccuracy,
		heading: c.heading,
		speed: c.speed,
		timestamp: position.timestamp
	};
}

export function geoGetCurrentPosition(enableHighAccuracy, timeoutMs) {
	return new Promise((resolve, reject) => {
		if (!globalThis.navigator.geolocation) {
			reject(new Error("unsupported"));
			return;
		}

		globalThis.navigator.geolocation.getCurrentPosition(
			position => resolve(positionToPayload(position)),
			error => reject(
				new Error(error.code === 1 ? "permission" : error.message)
			),
			{
				enableHighAccuracy,
				timeout: timeoutMs > 0 ? timeoutMs : Infinity,
				maximumAge: 0
			});
	});
}

export function geoWatchStart(dotNetRef, enableHighAccuracy) {
	if (!globalThis.navigator.geolocation)
		return -1;

	return globalThis.navigator.geolocation.watchPosition(
		position => {
			dotNetRef
				.invokeMethodAsync(
					"OnGeolocationChanged",
					positionToPayload(position))
				.catch(() => { });
		},
		error => {
			dotNetRef
				.invokeMethodAsync(
					"OnGeolocationError",
					error.code === 1 ? "permission" : error.message)
				.catch(() => { });
		},
		{
			enableHighAccuracy
		});
}

export function geoWatchStop(watchId) {
	if (globalThis.navigator.geolocation && watchId >= 0)
		globalThis.navigator.geolocation.clearWatch(watchId);
}

// ---------- Battery ----------

let batteryManager = null;

const batterySubscriptions = new Map();

async function batGetManager() {
	if (!("getBattery" in navigator))
		return null;

	if (!batteryManager)
		batteryManager = await navigator.getBattery();

	return batteryManager;
}

function batSnapshotOf(b) {
	return {
		level: b.level,
		charging: b.charging
	};
}

export async function batGetSnapshot() {
	const b = await batGetManager();
	return b ? batSnapshotOf(b) : null;
}

export async function batSubscribe(dotNetRef) {
	const b = await batGetManager();

	if (!b)
		return null;

	// Prevent duplicate subscriptions for the same .NET reference.
	await batUnsubscribe(dotNetRef);

	const notify = () =>
		dotNetRef.invokeMethodAsync(
			"OnBatteryChanged",
			batSnapshotOf(b)
		).catch(() => {
			// The .NET object may already have been disposed.
		});

	b.addEventListener("levelchange", notify);
	b.addEventListener("chargingchange", notify);

	batterySubscriptions.set(dotNetRef, {
		battery: b,
		notify
	});

	return true;
}

export async function batUnsubscribe(dotNetRef) {
	const subscription = batterySubscriptions.get(dotNetRef);

	if (!subscription)
		return;

	const { battery, notify } = subscription;

	battery.removeEventListener("levelchange", notify);
	battery.removeEventListener("chargingchange", notify);

	batterySubscriptions.delete(dotNetRef);
}

// ---------- Vibration / haptics ----------

export function vibrationIsSupported() {
	return typeof globalThis.navigator.vibrate === 'function';
}

export function vibrate(durationMs) {
	if (typeof globalThis.navigator.vibrate === 'function')
		globalThis.navigator.vibrate(durationMs);
}

// ---------- Share ----------

export function shareIsSupported() {
	return typeof globalThis.navigator.share === 'function';
}

export function share(title, text, url) {
	const data = {};
	if (title) data.title = title;
	if (text) data.text = text;
	if (url) data.url = url;
	return globalThis.navigator.share(data);
}

export function shareFiles(title, namesJson, typesJson, base64Json) {
	const names = JSON.parse(namesJson);
	const types = JSON.parse(typesJson);
	const contents = JSON.parse(base64Json);
	const files = names.map((name, i) => new File([base64ToBytes(contents[i])], name, { type: types[i] || 'application/octet-stream' }));
	if (!globalThis.navigator.canShare || !globalThis.navigator.canShare({ files }))
		return Promise.reject(new Error('unsupported'));
	const data = { files };
	if (title) data.title = title;
	return globalThis.navigator.share(data);
}

// ---------- Launcher / browser ----------

export function openUrl(url) {
	// noopener so the opened page cannot script this app.
	return globalThis.window.open(url, '_blank', 'noopener') !== null;
}

export function navigateTo(url) {
	// Used for protocol-handler schemes (mailto:, tel:, sms:) — does not unload the app.
	globalThis.location.assign(url);
	return true;
}

export function openFileBlob(base64, contentType, name) {
	const blob = new Blob([base64ToBytes(base64)], { type: contentType || 'application/octet-stream' });
	const url = URL.createObjectURL(blob);
	const opened = globalThis.window.open(url, '_blank');
	// Give the new tab time to load before revoking.
	setTimeout(() => URL.revokeObjectURL(url), 60000);
	return opened !== null;
}


// ---------- Text to speech ----------

export function speechGetVoices() {
	const synth = globalThis.speechSynthesis;
	if (!synth)
		return JSON.stringify([]);
	return JSON.stringify(synth.getVoices().map(v => ({ name: v.name, lang: v.lang, isDefault: v.default })));
}

export function speak(text, lang, pitch, rate, volume) {
	return new Promise((resolve, reject) => {
		const synth = globalThis.speechSynthesis;
		if (!synth) {
			reject(new Error('unsupported'));
			return;
		}
		const utterance = new SpeechSynthesisUtterance(text);
		if (lang) utterance.lang = lang;
		if (pitch >= 0) utterance.pitch = pitch;
		if (rate >= 0) utterance.rate = rate;
		if (volume >= 0) utterance.volume = volume;
		utterance.onend = () => resolve();
		utterance.onerror = e => e.error === 'canceled' || e.error === 'interrupted' ? resolve() : reject(new Error(e.error));
		synth.speak(utterance);
	});
}

export function speechCancel() {
	if (globalThis.speechSynthesis)
		globalThis.speechSynthesis.cancel();
}

// ---------- Sensors (devicemotion / deviceorientation) ----------

const DEG_TO_RAD = Math.PI / 180;
const GRAVITY = 9.80665;
const sensorHandlers = {};

function orientationToQuaternion(e) {
	const x = (e.beta || 0) * DEG_TO_RAD / 2;
	const y = (e.gamma || 0) * DEG_TO_RAD / 2;
	const z = (e.alpha || 0) * DEG_TO_RAD / 2;

	const cX = Math.cos(x), cY = Math.cos(y), cZ = Math.cos(z);
	const sX = Math.sin(x), sY = Math.sin(y), sZ = Math.sin(z);

	return {
		x: sX * cY * cZ - cX * sY * sZ,
		y: cX * sY * cZ + sX * cY * sZ,
		z: cX * cY * sZ + sX * sY * cZ,
		w: cX * cY * cZ - sX * sY * sZ
	};
}

export function sensorIsSupported(kind) {
	switch (kind) {
		case "accelerometer":
		case "gyroscope":
			return "DeviceMotionEvent" in globalThis;

		case "orientation":
		case "compass":
			return "DeviceOrientationEvent" in globalThis;

		default:
			return false;
	}
}

export async function sensorStart(dotNetRef, kind, frequencyHz) {
	if (!sensorIsSupported(kind) || sensorHandlers[kind])
		return false;

	// iOS Safari requires an explicit permission request.
	const eventCtor =
		kind === "accelerometer" || kind === "gyroscope"
			? globalThis.DeviceMotionEvent
			: globalThis.DeviceOrientationEvent;

	if (typeof eventCtor.requestPermission === "function") {
		const state = await eventCtor.requestPermission();

		if (state !== "granted")
			return false;
	}

	let eventName;
	let handler;

	const notify = payload =>
		dotNetRef
			.invokeMethodAsync("OnSensorChanged", kind, payload)
			.catch(() => { });

	switch (kind) {
		case "accelerometer":
			eventName = "devicemotion";
			handler = e => {
				const a = e.accelerationIncludingGravity;

				if (a) {
					notify({
						x: (a.x || 0) / GRAVITY,
						y: (a.y || 0) / GRAVITY,
						z: (a.z || 0) / GRAVITY
					});
				}
			};
			break;

		case "gyroscope":
			eventName = "devicemotion";
			handler = e => {
				const r = e.rotationRate;

				if (r) {
					notify({
						x: (r.beta || 0) * DEG_TO_RAD,
						y: (r.gamma || 0) * DEG_TO_RAD,
						z: (r.alpha || 0) * DEG_TO_RAD
					});
				}
			};
			break;

		case "orientation":
			eventName = "deviceorientation";
			handler = e => notify(orientationToQuaternion(e));
			break;

		case "compass":
			eventName =
				"ondeviceorientationabsolute" in globalThis
					? "deviceorientationabsolute"
					: "deviceorientation";

			handler = e => {
				const heading =
					typeof e.webkitCompassHeading === "number"
						? e.webkitCompassHeading
						: (e.absolute && e.alpha !== null
							? (360 - e.alpha) % 360
							: null);

				if (heading !== null)
					notify({ heading });
			};
			break;

		default:
			return false;
	}

	sensorHandlers[kind] = {
		eventName,
		handler
	};

	globalThis.addEventListener(eventName, handler);

	return true;
}

export function sensorStop(kind) {
	const entry = sensorHandlers[kind];

	if (entry) {
		globalThis.removeEventListener(
			entry.eventName,
			entry.handler
		);

		delete sensorHandlers[kind];
	}
}

// ---------- App package files (fetch relative to base URL) ----------

export async function fetchAppFile(path) {
	const response = await globalThis.fetch(new URL(path, globalThis.document.baseURI), { method: 'GET' });
	if (!response.ok)
		return null;
	return bytesToBase64(new Uint8Array(await response.arrayBuffer()));
}

export async function appFileExists(path) {
	try {
		const response = await globalThis.fetch(new URL(path, globalThis.document.baseURI), { method: 'HEAD' });
		return response.ok;
	} catch {
		return false;
	}
}

// ---------- Screen reader announcements (aria-live) ----------

let ariaLiveRegion = null;

export function announce(text) {
	const doc = globalThis.document;
	if (!ariaLiveRegion) {
		ariaLiveRegion = doc.createElement('div');
		ariaLiveRegion.setAttribute('aria-live', 'polite');
		ariaLiveRegion.setAttribute('role', 'status');
		ariaLiveRegion.style.cssText = 'position:absolute;width:1px;height:1px;margin:-1px;padding:0;overflow:hidden;clip:rect(0 0 0 0);white-space:nowrap;border:0;';
		doc.body.appendChild(ariaLiveRegion);
	}
	// Clear then set so repeated identical announcements are re-read.
	ariaLiveRegion.textContent = '';
	globalThis.setTimeout(() => { ariaLiveRegion.textContent = text; }, 50);
}
















let connectivityHandler = null;

function readConnectivity() {
	const c = navigator.connection;
	return {
		online: navigator.onLine,
		type: c?.type ?? "unknown",       // Chromium only: wifi, cellular, ethernet, bluetooth...
		saveData: c?.saveData ?? false
	};
}

export function connectivityGetSnapshot() {
	return readConnectivity();
}

export function connectivitySubscribe(dotNetRef) {
	connectivityUnsubscribe();
	connectivityHandler = () =>
		dotNetRef.invokeMethodAsync("OnBrowserChanged", readConnectivity());
	window.addEventListener("online", connectivityHandler);
	window.addEventListener("offline", connectivityHandler);
	navigator.connection?.addEventListener("change", connectivityHandler);
}

export function connectivityUnsubscribe() {
	if (!connectivityHandler) return;
	window.removeEventListener("online", connectivityHandler);
	window.removeEventListener("offline", connectivityHandler);
	navigator.connection?.removeEventListener("change", connectivityHandler);
	connectivityHandler = null;
}

export function appInfoGet() {
	return {
		title: document.title,
		hostname: location.hostname,
		rtl: document.documentElement.dir === "rtl"
			|| getComputedStyle(document.documentElement).direction === "rtl",
		prefersDark: window.matchMedia("(prefers-color-scheme: dark)").matches
	};
}

let appInfoMql = null;
let appInfoHandler = null;

export function appInfoSubscribeTheme(dotNetRef) {
	appInfoUnsubscribeTheme();
	appInfoMql = window.matchMedia("(prefers-color-scheme: dark)");
	appInfoHandler = e => dotNetRef.invokeMethodAsync("OnThemeChanged", e.matches);
	appInfoMql.addEventListener("change", appInfoHandler);
}

export function appInfoUnsubscribeTheme() {
	if (appInfoMql && appInfoHandler) {
		appInfoMql.removeEventListener("change", appInfoHandler);
	}
	appInfoMql = null;
	appInfoHandler = null;
}

export function pickFiles(accept, multiple) {
	return new Promise(resolve => {
		const input = document.createElement("input");
		input.type = "file";
		input.multiple = !!multiple;
		if (accept) input.accept = accept;
		input.style.display = "none";

		input.addEventListener("change", async () => {
			const files = Array.from(input.files ?? []);
			const results = await Promise.all(files.map(f => new Promise((res, rej) => {
				const reader = new FileReader();
				reader.onload = async () => {
					// const base64 = reader.result.substring(reader.result.indexOf(",") + 1);
					// res({ name: f.name, type: f.type, dataBase64: base64 });
					const arrayBuffer = await f.arrayBuffer();
					const bytes = new Uint8Array(arrayBuffer);
					res({ name: f.name, type: f.type, data: bytes });
				};
				reader.onerror = () => rej(reader.error);
				reader.readAsDataURL(f);
			})));
			document.body.removeChild(input);
			resolve(results);
		});

		// Some browsers require the input to be in the DOM to show the dialog reliably.
		document.body.appendChild(input);
		input.click();
	});
}