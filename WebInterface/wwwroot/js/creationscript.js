const actionButton = $("#actionbutton");
const copyButton = $("#copybutton");
const inputUrl = $("#inputurl");
const output = $("#output");
const outputPanel = $("#outputpanel");
const outputInfo = $("#outputinfo");
const urlRegExp = new RegExp(String.raw`https?:\/\/.+`);


async function CopyUrl() {
    const text = document.getElementById('output').value;
    try {
        await navigator.clipboard.writeText(text);
    } catch (err) {
    }
}

function SendRequestToShortenUrl() {

    const valueToSend = inputUrl.val();

    if (valueToSend.length > 2000 || !urlRegExp.test(valueToSend)) {
        outputPanel.hide();
        outputInfo.html("The url provided should not be longer than 2000 characters and should be valid, including protocol");
        return;
    }
    else {
        outputInfo.html("");
    }

    let formData = new FormData();
    formData.append("url", valueToSend);

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
        },
        success: function (response) {
            if (response.success) {
                outputInfo.html(`Got it! The url will expire in ${response.timeToExpireInSec} seconds`);
                output.val(`${window.location.origin}/${response.shortenedUrl}`);
                outputPanel.show();
            }
            else {
                outputInfo.html("Something went wrong");
                outputPanel.hide();
                output.val("");
            }

        }
    });
}

outputPanel.hide();
actionButton.click(SendRequestToShortenUrl);
copyButton.click(CopyUrl);