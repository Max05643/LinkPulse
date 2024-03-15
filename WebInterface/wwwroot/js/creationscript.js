const actionButton = $("#actionbutton");
const copyButton = $("#copybutton");
const inputUrl = $("#inputurl");
const output = $("#output");
const outputPanel = $("#outputpanel");
const outputInfo = $("#outputinfo");
const urlRegExp = new RegExp(String.raw`https?:\/\/.+`);
const qrCodeDiv = $("#qrcode");

const qrCodeHandler = new QRCode(document.getElementById("qrcode"), {
    text: "",
    width: 256,
    height: 256,
});
function HideQrCode() {
    qrCodeHandler.clear();
    qrCodeDiv.hide();
}

function ShowQrCode(url) {
    qrCodeHandler.makeCode(url);
    qrCodeDiv.show();
}

function SetActionButtonStatus(status) {
    actionButton.prop("disabled", !status);
    if (status) {
        $("#loader").stop(true, true).animate({
            opacity: 0
        }, 500);
        $("#mainactionbuttontext").stop(true, true).animate({
            opacity: 1
        }, 500);
    }
    else {
        $("#loader").stop(true, true).animate({
            opacity: 1
        }, 500);
        $("#mainactionbuttontext").stop(true, true).animate({
            opacity: 0
        }, 500);
    }
}
function FormatDateDifference(startDate, endDate) {
    const difference = Math.max(0, endDate - startDate);
    const days = Math.floor(difference / (1000 * 60 * 60 * 24));
    const hours = Math.floor((difference % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const minutes = Math.floor((difference % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((difference % (1000 * 60)) / 1000);
    const result = `${days}:${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
    return result;
}

let lastDateTimeExpiration = null;

function UpdateTime() {
    if (lastDateTimeExpiration != null) {
        const differenceFormatted = FormatDateDifference(new Date(), lastDateTimeExpiration);
        outputInfo.html(`Got it! The shortened url will expire in ${differenceFormatted}`);
    }
}

async function CopyUrl() {
    const text = document.getElementById('output').value;
    try {
        await navigator.clipboard.writeText(text);
        copyButton.removeClass('btn-secondary').addClass('btn-success transition').text('Copied!');
        setTimeout(function () {
            copyButton.removeClass('btn-success transition').addClass('btn-secondary transition').text('Copy');
        }, 1000);
    }
    catch { }
}

function SendRequestToShortenUrl() {

    HideQrCode();

    const valueToSend = inputUrl.val();

    if (valueToSend.length > 2000 || !urlRegExp.test(valueToSend)) {
        outputPanel.hide();
        lastDateTimeExpiration = null
        outputInfo.html("The url provided should not be longer than 2000 characters and should be valid, including protocol");
        return;
    }
    else {
        outputInfo.html("Waiting...");
    }

    let formData = new FormData();
    formData.append("url", valueToSend);

    SetActionButtonStatus(false);

    $.ajax({
        type: "POST",
        url: "/Api/TryAdd",
        data: formData,
        processData: false,
        contentType: false,
        dataType: "json",
        error: function (xhr, status, error) {
            outputInfo.html("Something went wrong");
            outputPanel.hide();
            output.val("");
            lastDateTimeExpiration = null;
            SetActionButtonStatus(true);
        },
        success: function (response) {
            if (response.success) {
                let currentDate = new Date();
                lastDateTimeExpiration = currentDate.setSeconds(currentDate.getSeconds() + response.timeToExpireInSec);
                const newUrl = `${window.location.origin}/${response.shortenedUrl}`;
                output.val(newUrl);
                outputPanel.show();
                ShowQrCode(newUrl)
            }
            else {
                outputInfo.html("Something went wrong");
                outputPanel.hide();
                output.val("");
                lastDateTimeExpiration = null;
            }
            SetActionButtonStatus(true);
        }
    });
}

HideQrCode();
outputPanel.hide();
actionButton.click(SendRequestToShortenUrl);
copyButton.click(CopyUrl);
SetActionButtonStatus(true);
setInterval(UpdateTime, 500);
