mergeInto(LibraryManager.library, {
    GP_Parity_Report: function (dataPtr) {
        if (typeof window === 'undefined') {
            return;
        }

        var raw = UTF8ToString(dataPtr);

        window.GPParity = window.GPParity || { events: [] };
        window.GPParity.events.push(raw);

        if (typeof window.dispatchEvent === 'function') {
            window.dispatchEvent(
                new CustomEvent('gp-parity', {
                    detail: raw
                })
            );
        }
    }
});
