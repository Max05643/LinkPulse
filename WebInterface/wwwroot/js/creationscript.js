const actionButton = $("#actionbutton");
const inputUrl = $("#inputurl");
const output = $("#output");
const outputPanel = $("#outputpanel");
const outputInfo = $("#outputinfo");
const urlRegExp = new RegExp(String.raw`https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)`);

function SendRequestToShortenUrl() {

    const valueToSend = inputUrl.val();

    if (valueToSend.length > 2000 || !urlRegExp.test(valueToSend)) {
        outputPanel.show();
        outputInfo.html("The url provided should not be longer than 2000 characters and should be valid, including protocol");
        return;
    }

    const formData = {
        url: valueToSend
    };

    $.ajax({
        type: "POST",
        url: "/Api/TryAdd",
        data: formData,
        dataType: "json",
        error: function (xhr, status, error) {
            outputInfo.html("Something went wrong");
            outputPanel.show();
            output.val("");
        },
        success: function (response) {
            if (response.success) {
                outputInfo.html(`Success: ${response.shortenedUrl}; ${response.timeToExpireInSec}`);
                output.val(`${window.location.origin}/${response.shortenedUrl}`);
            }
            else {
                outputInfo.html("Something went wrong");
                output.val("");
            }
            outputPanel.show();
        }
    });
}

outputPanel.hide();
actionButton.click(SendRequestToShortenUrl);