// Browser Essentials interop module.
// Loaded by BrowserEssentials.InitializeAsync() via JSHost.ImportAsync and
// bound from C# with [JSImport]. All functions are plain ES module exports
// with no dependencies.

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

// const CRYPTO_DB = 'maui-essentials';
// const CRYPTO_STORE = 'crypto-keys';
// const CRYPTO_KEY_ID = 'securestorage-aes-gcm';
// let cachedCryptoKey = null;

// function openCryptoDb() {
// 	return new Promise((resolve, reject) => {
// 		const req = globalThis.indexedDB.open(CRYPTO_DB, 1);
// 		req.onupgradeneeded = () => req.result.createObjectStore(CRYPTO_STORE);
// 		req.onsuccess = () => resolve(req.result);
// 		req.onerror = () => reject(req.error);
// 	});
// }

// async function getCryptoKey() {
// 	if (cachedCryptoKey)
// 		return cachedCryptoKey;
// 	const db = await openCryptoDb();
// 	try {
// 		const existing = await new Promise((resolve, reject) => {
// 			const tx = db.transaction(CRYPTO_STORE, 'readonly');
// 			const rq = tx.objectStore(CRYPTO_STORE).get(CRYPTO_KEY_ID);
// 			rq.onsuccess = () => resolve(rq.result);
// 			rq.onerror = () => reject(rq.error);
// 		});
// 		if (existing) {
// 			cachedCryptoKey = existing;
// 			return existing;
// 		}
// 		// Non-extractable: the key material can never be read back out of the browser.
// 		const key = await globalThis.crypto.subtle.generateKey({ name: 'AES-GCM', length: 256 }, false, ['encrypt', 'decrypt']);
// 		await new Promise((resolve, reject) => {
// 			const tx = db.transaction(CRYPTO_STORE, 'readwrite');
// 			const rq = tx.objectStore(CRYPTO_STORE).put(key, CRYPTO_KEY_ID);
// 			rq.onsuccess = () => resolve();
// 			rq.onerror = () => reject(rq.error);
// 		});
// 		cachedCryptoKey = key;
// 		return key;
// 	} finally {
// 		db.close();
// 	}
// }

// function bytesToBase64(bytes) {
// 	let binary = '';
// 	const chunk = 0x8000;
// 	for (let i = 0; i < bytes.length; i += chunk)
// 		binary += String.fromCharCode.apply(null, bytes.subarray(i, i + chunk));
// 	return btoa(binary);
// }

// function base64ToBytes(base64) {
// 	const binary = atob(base64);
// 	const bytes = new Uint8Array(binary.length);
// 	for (let i = 0; i < binary.length; i++)
// 		bytes[i] = binary.charCodeAt(i);
// 	return bytes;
// }

// export async function secureSet(key, value) {
// 	const cryptoKey = await getCryptoKey();
// 	const iv = globalThis.crypto.getRandomValues(new Uint8Array(12));
// 	const ciphertext = await globalThis.crypto.subtle.encrypt({ name: 'AES-GCM', iv }, cryptoKey, new TextEncoder().encode(value));
// 	const buf = new Uint8Array(iv.length + ciphertext.byteLength);
// 	buf.set(iv);
// 	buf.set(new Uint8Array(ciphertext), iv.length);
// 	globalThis.localStorage.setItem(key, bytesToBase64(buf));
// }

// export async function secureGet(key) {
// 	const stored = globalThis.localStorage.getItem(key);
// 	if (stored === null)
// 		return null;
// 	try {
// 		const buf = base64ToBytes(stored);
// 		const iv = buf.subarray(0, 12);
// 		const ciphertext = buf.subarray(12);
// 		const cryptoKey = await getCryptoKey();
// 		const plaintext = await globalThis.crypto.subtle.decrypt({ name: 'AES-GCM', iv }, cryptoKey, ciphertext);
// 		return new TextDecoder().decode(plaintext);
// 	} catch {
// 		// Key lost (e.g. IndexedDB cleared) or value corrupt — treat as missing.
// 		return null;
// 	}
// }

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
	return JSON.stringify({
		userAgent: nav.userAgent || '',
		vendor: nav.vendor || '',
		language: nav.language || '',
		platform: (uaData && uaData.platform) || nav.platform || '',
		mobile: uaData ? !!uaData.mobile : /Mobi|Android|iPhone|iPad/i.test(nav.userAgent || ''),
		brands: (uaData && uaData.brands) ? uaData.brands.map(b => ({ brand: b.brand, version: b.version })) : []
	});
}

// ---------- Display ----------

export function getDisplayInfo() {
	const screen = globalThis.screen;
	const orientationType = (screen.orientation && screen.orientation.type) || 'landscape-primary';
	return JSON.stringify({
		width: screen.width,
		height: screen.height,
		pixelRatio: globalThis.devicePixelRatio || 1,
		orientation: orientationType
	});
}

export function watchDisplay(callback) {
	globalThis.addEventListener('resize', () => callback());
	if (globalThis.screen.orientation)
		globalThis.screen.orientation.addEventListener('change', () => callback());
}

let wakeLockSentinel = null;

export async function setWakeLock(enabled) {
	try {
		if (enabled) {
			if (!('wakeLock' in globalThis.navigator))
				return false;
			wakeLockSentinel = await globalThis.navigator.wakeLock.request('screen');
			return true;
		}
		if (wakeLockSentinel) {
			await wakeLockSentinel.release();
			wakeLockSentinel = null;
		}
		return true;
	} catch {
		return false;
	}
}

export function getWakeLock() {
	return wakeLockSentinel !== null && !wakeLockSentinel.released;
}

// ---------- Geolocation ----------

function positionToJson(position) {
	const c = position.coords;
	return JSON.stringify({
		latitude: c.latitude,
		longitude: c.longitude,
		accuracy: c.accuracy,
		altitude: c.altitude,
		altitudeAccuracy: c.altitudeAccuracy,
		heading: c.heading,
		speed: c.speed,
		timestamp: position.timestamp
	});
}

export function geoGetCurrentPosition(enableHighAccuracy, timeoutMs) {
	return new Promise((resolve, reject) => {
		if (!globalThis.navigator.geolocation) {
			reject(new Error('unsupported'));
			return;
		}
		globalThis.navigator.geolocation.getCurrentPosition(
			position => resolve(positionToJson(position)),
			error => reject(new Error(error.code === 1 ? 'permission' : error.message)),
			{ enableHighAccuracy: enableHighAccuracy, timeout: timeoutMs > 0 ? timeoutMs : Infinity, maximumAge: 0 });
	});
}

export function geoWatchStart(enableHighAccuracy, callback, errorCallback) {
	if (!globalThis.navigator.geolocation)
		return -1;
	return globalThis.navigator.geolocation.watchPosition(
		position => callback(positionToJson(position)),
		error => errorCallback(error.code === 1 ? 'permission' : error.message),
		{ enableHighAccuracy: enableHighAccuracy });
}

export function geoWatchStop(watchId) {
	if (globalThis.navigator.geolocation && watchId >= 0)
		globalThis.navigator.geolocation.clearWatch(watchId);
}

// ---------- Battery ----------

function batteryToJson(battery) {
	return JSON.stringify({
		level: battery.level,
		charging: battery.charging,
		chargingTime: battery.chargingTime,
		dischargingTime: battery.dischargingTime
	});
}

export async function batteryStart(callback) {
	if (!globalThis.navigator.getBattery)
		return null;
	const battery = await globalThis.navigator.getBattery();
	const notify = () => callback(batteryToJson(battery));
	battery.addEventListener('levelchange', notify);
	battery.addEventListener('chargingchange', notify);
	return batteryToJson(battery);
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

function orientationToQuaternionJson(e) {
	const x = (e.beta || 0) * DEG_TO_RAD / 2;
	const y = (e.gamma || 0) * DEG_TO_RAD / 2;
	const z = (e.alpha || 0) * DEG_TO_RAD / 2;
	const cX = Math.cos(x), cY = Math.cos(y), cZ = Math.cos(z);
	const sX = Math.sin(x), sY = Math.sin(y), sZ = Math.sin(z);
	return JSON.stringify({
		x: sX * cY * cZ - cX * sY * sZ,
		y: cX * sY * cZ + sX * cY * sZ,
		z: cX * cY * sZ + sX * sY * cZ,
		w: cX * cY * cZ - sX * sY * sZ
	});
}

export function sensorIsSupported(kind) {
	switch (kind) {
		case 'accelerometer':
		case 'gyroscope':
			return 'DeviceMotionEvent' in globalThis;
		case 'orientation':
		case 'compass':
			return 'DeviceOrientationEvent' in globalThis;
		default:
			return false;
	}
}

export async function sensorStart(kind, callback) {
	if (!sensorIsSupported(kind) || sensorHandlers[kind])
		return false;
	// iOS Safari requires an explicit permission request.
	const eventCtor = kind === 'accelerometer' || kind === 'gyroscope'
		? globalThis.DeviceMotionEvent : globalThis.DeviceOrientationEvent;
	if (typeof eventCtor.requestPermission === 'function') {
		const state = await eventCtor.requestPermission();
		if (state !== 'granted')
			return false;
	}
	let eventName, handler;
	switch (kind) {
		case 'accelerometer':
			eventName = 'devicemotion';
			handler = e => {
				const a = e.accelerationIncludingGravity;
				if (a)
					callback(JSON.stringify({ x: (a.x || 0) / GRAVITY, y: (a.y || 0) / GRAVITY, z: (a.z || 0) / GRAVITY }));
			};
			break;
		case 'gyroscope':
			eventName = 'devicemotion';
			handler = e => {
				const r = e.rotationRate;
				if (r)
					callback(JSON.stringify({ x: (r.beta || 0) * DEG_TO_RAD, y: (r.gamma || 0) * DEG_TO_RAD, z: (r.alpha || 0) * DEG_TO_RAD }));
			};
			break;
		case 'orientation':
			eventName = 'deviceorientation';
			handler = e => callback(orientationToQuaternionJson(e));
			break;
		case 'compass':
			eventName = 'ondeviceorientationabsolute' in globalThis ? 'deviceorientationabsolute' : 'deviceorientation';
			handler = e => {
				// webkitCompassHeading is iOS Safari; otherwise derive from absolute alpha.
				const heading = typeof e.webkitCompassHeading === 'number'
					? e.webkitCompassHeading
					: (e.absolute && e.alpha !== null ? (360 - e.alpha) % 360 : null);
				if (heading !== null)
					callback(JSON.stringify({ heading: heading }));
			};
			break;
	}
	sensorHandlers[kind] = { eventName, handler };
	globalThis.addEventListener(eventName, handler);
	return true;
}

export function sensorStop(kind) {
	const entry = sensorHandlers[kind];
	if (entry) {
		globalThis.removeEventListener(entry.eventName, entry.handler);
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