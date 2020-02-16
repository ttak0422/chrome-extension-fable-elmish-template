function setBadgeText(text) {
    chrome.browserAction.setBadgeText({text: text});
}

export { setBadgeText }