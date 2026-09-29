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